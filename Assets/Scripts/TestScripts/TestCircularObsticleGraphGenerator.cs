using System;
using System.Collections.Generic;
using System.Linq;
using DebugDrawers;
using Pathfinding;
using Pathfinding.Algorithms;
using Pathfinding.CircularObstacleGraph;
using Pathfinding.Graph;
using UnityEngine;
using Utils;

namespace TestScripts
{
	public class TestCircularObsticleGraphGenerator : MonoBehaviour
	{
		[SerializeField] private Transform circlesTParent;
		private (Transform, CapsuleCollider) [] circlesObstacles;

		[SerializeField] private Transform start;
		[SerializeField] private Transform goal;
		[SerializeField] private Actor actor;

		private Circle[] circles;
		private CircularObsticleGraphGenerator circularGenerator;
		private AStar<Vector2> pathfinder;

		private List<NodeWithEdge<Vector2>> path;

		[Header("Debug Drawing")]
		
		private PathfindingDebugDrawer debugDrawer;
		
		[SerializeField] private bool hideObstacles;
		[SerializeField] private bool hideObstacleMeshRenderers;
		
		private (Transform, Renderer)[] circlesDebug;

		private void Awake()
		{
			debugDrawer = GetComponent<PathfindingDebugDrawer>();
		}

		private void Start()
		{
			GetObstacles();
			GetCircles();
			
			circularGenerator = new CircularObsticleGraphGenerator();
			circularGenerator.graph.SetContentEqualsComparer((v1, v2) => v1.AlmostEqual(v2, 0.1f));
			circularGenerator.SetActor(actor);
			pathfinder = new AStar<Vector2>(circularGenerator.graph, AStarHeuristic<Vector2>.EuclideanDistance);
			
			debugDrawer.Setup(circularGenerator, path);
		}

		private void Update()
		{
			GetCircles();
			circularGenerator.SetStart(start.position.ToVec2());
			circularGenerator.SetGoal(goal.position.ToVec2());
			circularGenerator.SetCircles(circles);
			circularGenerator.GenerateGraph();
			
			pathfinder.SetStart(circularGenerator.Start);
			pathfinder.SetGoal(circularGenerator.Goal);
			pathfinder.FindPath();
			path = pathfinder.GetPath();

			debugDrawer.path = path;
			ToggleObsticlesVisibility();
		}

		private void GetObstacles()
		{
			circlesObstacles = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => (cc.gameObject.transform, cc))
				.ToArray();
			
			circlesDebug = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => (cc.gameObject.transform, cc.gameObject.GetComponent<Renderer>()))
				.ToArray();
		}
		
		private void GetCircles()
		{
			circles = circlesObstacles.Select(t => new Circle(t.Item2.ScaledRadius(), t.Item1.position.ToVec2())).ToArray();
		}
		
		private void ToggleObsticlesVisibility()
		{
			circlesDebug.ForEach(tcc => tcc.Item1.gameObject.SetActive(!hideObstacles));
			circlesDebug.ForEach(tcc => tcc.Item2.enabled = !hideObstacleMeshRenderers);
		}
	}
}