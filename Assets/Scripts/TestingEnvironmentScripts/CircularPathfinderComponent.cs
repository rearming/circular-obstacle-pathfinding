using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DebugDrawers;
using Pathfinding;
using Pathfinding.Algorithms;
using Pathfinding.CircularObstacleGraph;
using Pathfinding.Graph;
using UnityEditor.Rendering;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	public class CircularPathfinderComponent : MonoBehaviour
	{
		private AStar<Vector2> pathfinder;
		private CircularObsticleGraphGenerator graphGenerator;

		private (Transform, CapsuleCollider, NeutralComponent) [] circlesObstacles;
		private Circle[] circles;
		private List<NodeWithEdge<Vector2>> path;
		
		private Vector2 currentGoal;
		
		private Actor actor;
		private float baseActorRadius;
		
		private NeutralComponent neutral;
		private CapsuleCollider capsuleCollider;

		private int pathNodeIdx;
		private int PathNodeIdx
		{
			get => pathNodeIdx;
			set
			{
				bezierT = 0f; // reset bezier lerp time on current node change
				pathNodeIdx = value;
			}

		}
		private float bezierT;

		#region Debug Fields

		private PathfindingDebugDrawer debugDrawer;

		#endregion

		private void Awake()
		{
			neutral = GetComponent<NeutralComponent>();
			capsuleCollider = GetComponent<CapsuleCollider>();
			debugDrawer = GetComponent<PathfindingDebugDrawer>();
		}

		private void Start()
		{
			GetObstacles();
			baseActorRadius = capsuleCollider.radius * 1.5f;
			actor = new Actor(baseActorRadius);
			
			graphGenerator = new CircularObsticleGraphGenerator();
			graphGenerator.graph.SetContentEqualsComparer((v1, v2) => v1.AlmostEqual(v2, 0.1f));
			graphGenerator.SetActor(actor);
			pathfinder = new AStar<Vector2>(graphGenerator.graph, AStarHeuristic<Vector2>.EuclideanDistance);
			
			if (!debugDrawer.IsRealNull())
				debugDrawer.Setup(graphGenerator, path);
		}

		private void Update()
		{
			if (!debugDrawer.IsRealNull())
				debugDrawer.path = path;
		}

		public void StartPathfing()
		{
			PathNodeIdx = 1;
		}

		private bool FindPath()
		{
			GetCircles();

			var limit = 3;
			var pathIsFound = false;

			while (!pathIsFound)
			{
				limit--;
				if (limit < 0)
				{
					Debug.LogWarning("Stop pathfinding, tries limit reached.");
					return false;
				}
				
				// var start = neutral.transform.position.ToVec2();
				var start = neutral.StartPos;
				pathIsFound = true;

				graphGenerator.SetStart(start);
				graphGenerator.SetGoal(neutral.Goal.Value);
				graphGenerator.SetCircles(circles);
				graphGenerator.GenerateGraph();

				try
				{
					pathfinder.SetStart(start);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					pathIsFound = false;
					ResetStart();
				}

				try
				{
					pathfinder.SetGoal(neutral.Goal);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					ResetGoal();
					pathIsFound = false;
				}

				try
				{
					pathfinder.FindPath();
					path = pathfinder.GetPath();
				}
				catch (Exception e)
				{
					switch (e)
					{
						case IncompletePathException ipe:
							if (ipe.nodesNum == 0)
								ResetStart();
							break;
					}

					Debug.LogException(e);
					ResetGoal();
					pathIsFound = false;
				}

			}

			return true;
		}

		private void ResetStart()
		{
			// var closest = graphGenerator.graph.Closest(
			// 		neutral.transform.position.ToVec2(), Vector2.Distance)
			// 	.Content.ToVec3(transform.position.y);
			//
			// neutral.transform.position += closest.normalized * 0.05f;
			// neutral.StartPos = neutral.transform.position.ToVec2();
		}

		private void ResetGoal()
		{
			neutral.SetGoal(graphGenerator.graph.Closest(
				neutral.Goal.Value, Vector2.Distance)
				.Content);
			Debug.Log($"New goal: [{neutral.Goal.Value.ToString()}]");
		}
		
		public Vector2 GetNextPos()
		{
			if (!FindPath())
			{
				neutral.UnsetGoal();
				return DefaultPosition;
			}

			if (!GetNextNode())
			{
				neutral.UnsetGoal();
				return DefaultPosition;
			}
			
			return currentGoal;
		}

		private bool GetNextNode()
		{
			if (Vector2.Distance(neutral.transform.position.ToVec2(), neutral.Goal.Value) < 0.1f || path.Count < 2)
			{
				Debug.LogWarning($"[{neutral.gameObject.name}] Path ended. Unsetting Goal.");
				path = null;
				return false;
			}

			try
			{
				if (Vector2.Distance(neutral.transform.position.ToVec2(), path[PathNodeIdx].node.Content) < 0.1f)
					PathNodeIdx++;
				currentGoal = path[PathNodeIdx].node.Content;
			}
			catch
			{
				return true;
			}

			if (IsHuggingEdge())
				BezierMovement();

			return true;
		}

		private bool IsHuggingEdge()
		{
			if (PathNodeIdx > 1) 
				return path[PathNodeIdx - 1].graphEdge.info != null && path[PathNodeIdx].graphEdge.info != null;
			return false;
		}

		private void BezierMovement()
		{
			const float step = 0.1f;
			
			if (path[PathNodeIdx].node.Info == null)
				return;
			var circle = graphGenerator.Circles[(int) path[PathNodeIdx].node.Info];

			var start = path[PathNodeIdx - 1].node.Content;
			var end = path[PathNodeIdx].node.Content;
			var middle = (start + end) / 2;
			
			var dir = (middle - circle.center).normalized;
			if (dir.AlmostEqual(Vector2.zero, 0.01f))
				dir = Vector2.Perpendicular((start - end).normalized).normalized;
			
			middle += dir * (Vector2.Distance(start, end));
					
			currentGoal = GetBezier(start, end, middle, bezierT + step);
			
			bezierPointsGizmos = new List<Vector2> { start, end, middle };
			
			if (Vector2.Distance(neutral.transform.position.ToVec2(), GetBezier(start, end, middle, bezierT + step)) < 0.1f)
				bezierT += step;
		}

		Vector2 GetBezier(Vector2 start, Vector2 end, Vector2 middle, float t)
		{
			return Mathf.Pow(1 - t, 2) * start + 2 * t * (1 - t) * middle + Mathf.Pow(t, 2) * end;
		}

		private Vector2 DefaultPosition => neutral.transform.position.ToVec2();

		private void GetObstacles()
		{
			circlesObstacles = FindObjectsOfType<NeutralComponent>()
				.Select(n => (n.GetComponent<CapsuleCollider>(), n))
				.Where(tuple =>
					tuple.n.gameObject.activeSelf && tuple.n.gameObject != gameObject)
				.Select(tuple =>
					(tuple.n.gameObject.transform, tuple.Item1, tuple.n))
				.ToArray();
		}

		private void GetCircles()
		{
			circles = circlesObstacles
				.Select(tuple =>
					new Circle(tuple.Item2.ScaledRadius(), tuple.Item1.position.ToVec2())).ToArray();
		}

		#region Debug Drawing

		private List<Vector2> bezierPointsGizmos;
		
		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			if (bezierPointsGizmos == null) return;
			
			Gizmos.color = Color.cyan;
			const int segments = 10;
			for (var t = 0; t < segments; t++)
			{
				var p1 = GetBezier(bezierPointsGizmos[0], bezierPointsGizmos[1], bezierPointsGizmos[2], t / (float)segments);
				var p2 = GetBezier(bezierPointsGizmos[0], bezierPointsGizmos[1], bezierPointsGizmos[2], (t + 1) / (float)segments);
				Gizmos.DrawLine(p1.ToVec3(-3), p2.ToVec3(-3));
			}
			
			bezierPointsGizmos = null;
		}

		#endregion
	}
}