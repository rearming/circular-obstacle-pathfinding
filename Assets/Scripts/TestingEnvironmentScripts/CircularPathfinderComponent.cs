using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Debug_Drawers;
using Pathfinding.Algorithms;
using Pathfinding.Circular_Obstacle_Graph;
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
		private List<Node<Vector2>> path;
		
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
			actor = new Actor(capsuleCollider.radius);
			
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
			FindPath();
		}

		private bool FindPath()
		{
			GetCircles();
			
			graphGenerator.SetStart(neutral.StartPos);
			graphGenerator.SetGoal(neutral.Goal.Value);
			graphGenerator.SetCircles(circles);
			graphGenerator.GenerateGraph();

			try
			{
				pathfinder.SetStart(neutral.StartPos);
			}
			catch (Exception e)
			{
				neutral.StartPos = neutral.transform.position.ToVec2();
				Debug.LogWarning($"Resetting start position");
				Debug.LogException(e);
				return false;
			}

			try
			{
				pathfinder.SetGoal(neutral.Goal);
				pathfinder.FindPath();
				path = pathfinder.GetPath();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return false;
			}

			currentNode = path.GetEnumerator();
			currentNode.MoveNext();
			return true;
		}

		private IEnumerator<Node<Vector2>> currentNode;
		
		public Vector2 GetNextPos()
		{
			if (!GetNextNode())
				return DefaultPosition();
			return currentGoal;
		}

		private bool GetNextNode()
		{
			if (currentNode.Current == null)
				return false;
			
			if (Vector2.Distance(neutral.transform.position.ToVec2(), currentGoal) < 0.1f)
			{
				if (!currentNode.MoveNext())
				{
					Debug.LogWarning($"Path ended. Unsetting Goal.");
					neutral.UnsetGoal();
					return false;
				}
			}
			currentGoal = currentNode.Current.Content;
			return true;
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