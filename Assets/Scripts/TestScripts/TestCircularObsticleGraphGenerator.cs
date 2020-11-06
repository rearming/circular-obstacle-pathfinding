using System.Collections.Generic;
using System.Linq;
using DebugDrawers;
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

		[SerializeField] private Transform start;
		[SerializeField] private Transform goal;
		[SerializeField] private Actor actor;

		[SerializeField] private bool hideObstacles;
		[SerializeField] private bool hideObstacleMeshRenderers;

		private Circle[] _circles;

		private (Transform, Renderer)[] _circlesDebug;
		private (Transform, CapsuleCollider)[] _circlesObstacles;
		private CircularObsticleGraphGenerator _circularGenerator;

		[Header("Debug Drawing")]
		private PathfindingDebugDrawer _debugDrawer;

		private List<NodeWithEdge<Vector2>> _path;
		private AStar<Vector2> _pathfinder;

		private void Awake()
		{
			_debugDrawer = GetComponent<PathfindingDebugDrawer>();
		}

		private void Start()
		{
			GetObstacles();
			GetCircles();

			_circularGenerator = new CircularObsticleGraphGenerator();
			_circularGenerator.graph.SetContentEqualsComparer((v1, v2) => v1.AlmostEqual(v2, 0.1f));
			_circularGenerator.SetActor(actor);
			_pathfinder = new AStar<Vector2>(_circularGenerator.graph, AStarHeuristic<Vector2>.EuclideanDistance);

			_debugDrawer.Setup(_circularGenerator, _path);
		}

		private void Update()
		{
			GetCircles();
			_circularGenerator.SetStart(start.position.ToVec2());
			_circularGenerator.SetGoal(goal.position.ToVec2());
			_circularGenerator.SetCircles(_circles);
			_circularGenerator.GenerateGraph();

			_pathfinder.SetStart(_circularGenerator.Start);
			_pathfinder.SetGoal(_circularGenerator.Goal);
			_pathfinder.FindPath();
			_path = _pathfinder.GetPath();

			_debugDrawer.path = _path;
			ToggleObsticlesVisibility();
		}

		private void GetObstacles()
		{
			_circlesObstacles = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => (cc.gameObject.transform, cc))
				.ToArray();

			_circlesDebug = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => (cc.gameObject.transform, cc.gameObject.GetComponent<Renderer>()))
				.ToArray();
		}

		private void GetCircles()
		{
			_circles = _circlesObstacles.Select(t => new Circle(t.Item2.ScaledRadius(), t.Item1.position.ToVec2()))
				.ToArray();
		}

		private void ToggleObsticlesVisibility()
		{
			_circlesDebug.ForEach(tcc => tcc.Item1.gameObject.SetActive(!hideObstacles));
			_circlesDebug.ForEach(tcc => tcc.Item2.enabled = !hideObstacleMeshRenderers);
		}
	}
}