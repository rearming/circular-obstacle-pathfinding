using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		private (Transform, CapsuleCollider) [] circlesObstacles;
		private Circle[] circles;
		private List<Node<Vector2>> path;

		private int currentPathNodeIdx;
		private int prevPathNodeIdx = -1;
		private Vector2 currentFinalGoal;
		
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
			if (neutral.Goal == null)
				return;

			if (neutral.Goal.Value != currentFinalGoal)
				FindPath();

			Move();
			
			if (!debugDrawer.IsRealNull())
				debugDrawer.path = path;
		}

		private void Move()
		{
			if (prevPathNodeIdx == currentPathNodeIdx)
				return;
			prevPathNodeIdx = currentPathNodeIdx;
			neutral.MoveTowards(path[currentPathNodeIdx].Content.ToVec3(transform.position.y), () =>
			{
				currentPathNodeIdx++;
				if (currentPathNodeIdx == path.Count)
					neutral.UnsetGoal();
			});
		}

		private void FindPath()
		{
			currentFinalGoal = neutral.Goal.Value;
			currentPathNodeIdx = 0;
			prevPathNodeIdx = -1;
			
			actor.Radius = capsuleCollider.radius;
			
			GetCircles();
			graphGenerator.SetStart(transform.position.ToVec2());
			graphGenerator.SetGoal(currentFinalGoal);
			graphGenerator.SetCircles(circles);
			graphGenerator.GenerateGraph();
			
			pathfinder.SetStart(graphGenerator.Start);
			pathfinder.SetGoal(graphGenerator.Goal);
			pathfinder.FindPath();
			path = pathfinder.GetPath();
		}

		private void GetObstacles()
		{
			circlesObstacles = FindObjectsOfType<NeutralComponent>()
				.Select(cc => cc.GetComponent<CapsuleCollider>())
				.Where(cc => cc.gameObject.activeSelf && cc.gameObject != gameObject)
				.Select(cc => (cc.gameObject.transform, cc))
				.ToArray();
		}
		
		private void GetCircles()
		{
			circles = circlesObstacles
				.Select(t =>
					new Circle(t.Item2.ScaledRadius(), t.Item1.position.ToVec2())).ToArray();
		}
	}
}