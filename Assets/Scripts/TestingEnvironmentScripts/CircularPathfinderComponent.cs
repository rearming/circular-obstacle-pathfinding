using System;
using System.Collections.Generic;
using System.Linq;
using DebugDrawers;
using Pathfinding.Algorithms;
using Pathfinding.CircularObstacleGraph;
using Pathfinding.Graph;
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
		private NeutralComponent neutral;
		private CapsuleCollider capsuleCollider;

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
			actor = new Actor(capsuleCollider.radius * 1.2f);
			
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
			// InvokeRepeating(nameof(FindPath), 0f, 0.2f);
		}

		private bool FindPath()
		{
			GetCircles();
			
			graphGenerator.SetStart(neutral.transform.position.ToVec2());
			graphGenerator.SetGoal(neutral.Goal.Value);
			graphGenerator.SetCircles(circles);
			graphGenerator.GenerateGraph();

			try
			{
				pathfinder.SetStart(neutral.transform.position.ToVec2());
			}
			catch (Exception e)
			{
				neutral.StartPos = neutral.transform.position.ToVec2();
				Debug.LogWarning($"Start position was overlapped, resetting to current.");
				Debug.LogException(e);
				return false;
			}

			try
			{
				pathfinder.SetGoal(neutral.Goal);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return false;
			}

			try
			{
				pathfinder.FindPath();
				path = pathfinder.GetPath();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return false;
			}

			return true;
		}

		public Vector2 GetNextPos()
		{
			if (!FindPath())
			{
				return DefaultPosition();
			}

			if (!GetNextNode())
			{
				neutral.UnsetGoal();
				return DefaultPosition();
			}
			
			return currentGoal;
		}

		private bool GetNextNode()
		{
			if (Vector2.Distance(neutral.transform.position.ToVec2(), neutral.Goal.Value) < 0.1f || path.Count < 2)
			{
				Debug.LogWarning($"Path ended. Unsetting Goal.");
				return false;
			}

			try
			{
				currentGoal = path[1].node.Content;
				bezierPointsGizmos = null;
				if (path[1].graphEdge.info != null && path[2].graphEdge.info == null)
				{
					var circle = graphGenerator.Circles[(int) path[1].node.Info];

					var start = neutral.transform.position.ToVec2();
					var end = path[1].node.Content;
					var middle = (start + end) / 2;
					var dir = (middle - circle.center).normalized;
					middle += dir * (Vector2.Distance(start, end) * 2);

					currentGoal = GetBezier(start, end, middle, 0.5f);
					
					bezierPointsGizmos = new List<Vector2> { start, end, middle };
				}
				
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			
			return true;
		}

		Vector2 GetBezier(Vector2 start, Vector2 end, Vector2 middle, float t)
		{
			return Mathf.Pow(1 - t, 2) * start + 2 * t * (1 - t) * middle + Mathf.Pow(t, 2) * end;
		}

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
		}

		private Vector2 DefaultPosition() => neutral.transform.position.ToVec2();

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
	}
}