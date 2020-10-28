using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DebugDrawers;
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
				pathfinder.FindPath();
				path = pathfinder.GetPath();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return false;
			}

			currentNode = path.GetEnumerator();
			currentNode.MoveNext(); // skip null path node
			// currentNode.MoveNext(); // skip first path node (start position), move right away to the next
			currentGoal = currentNode.Current.Content;
			return true;
		}

		private IEnumerator<Node<Vector2>> currentNode;
		
		public Vector2 GetNextPos()
		{
			FindPath();
			if (!GetNextNode())
				return DefaultPosition();
			Debug.Log($"distance between goal and pos: [{Vector2.Distance(currentGoal, transform.position.ToVec2()).ToString()}]");
			Debug.Log($"current goal: [{currentGoal.ToString()}]");
			return currentGoal;
		}

		private bool GetNextNode()
		{
			// if (currentNode.Current == null)
			// 	return false;
			//
			if (Vector2.Distance(neutral.transform.position.ToVec2(), neutral.Goal.Value) < 0.1f)
			{
				Debug.LogWarning($"Path ended. Unsetting Goal.");
				neutral.UnsetGoal();
				return false;
			}

			try
			{
				currentGoal = path[1].Content;

			}
			catch (Exception e)
			{
				Debug.LogError($"exc, path count: [{path.Count.ToString()}]");
			}
			// currentGoal = currentNode.Current.Content;
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