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
		private Actor _actor;
		private float _baseActorRadius;
		private Circle[] _circles;

		private (Transform, CapsuleCollider, NeutralComponent)[] _circlesObstacles;

		private Vector2 _currentGoal;

		#region Debug Fields

		private PathfindingDebugDrawer _debugDrawer;

		#endregion

		private CircularObsticleGraphGenerator _graphGenerator;

		private NeutralComponent _neutral;
		private List<NodeWithEdge<Vector2>> _path;
		private AStar<Vector2> _pathfinder;

		private Vector2 DefaultPosition => _neutral.transform.position.ToVec2();

		private void Awake()
		{
			_neutral = GetComponent<NeutralComponent>();
			_debugDrawer = GetComponent<PathfindingDebugDrawer>();
		}

		private void Start()
		{
			GetObstacles();
			_baseActorRadius = GetComponent<CapsuleCollider>().radius * 1f;
			_actor = new Actor(_baseActorRadius);

			_graphGenerator = new CircularObsticleGraphGenerator();
			_graphGenerator.graph.SetContentEqualsComparer((v1, v2) => v1.AlmostEqual(v2, 0.1f));
			_graphGenerator.SetActor(_actor);
			_pathfinder = new AStar<Vector2>(_graphGenerator.graph, AStarHeuristic<Vector2>.EuclideanDistance);

			if (!_debugDrawer.IsRealNull())
				_debugDrawer.Setup(_graphGenerator, _path);
		}

		private void Update()
		{
			if (!_debugDrawer.IsRealNull())
				_debugDrawer.path = _path;
		}

		private bool FindPath()
		{
			GetCircles();

			// var start = neutral.transform.position.ToVec2();
			var start = _neutral.StartPos;

			_graphGenerator.SetStart(start);
			_graphGenerator.SetGoal(_neutral.Goal.Value);
			_graphGenerator.SetCircles(_circles);
			_graphGenerator.GenerateGraph();


			_pathfinder.SetStart(start);
			_pathfinder.SetGoal(_neutral.Goal);
			_pathfinder.FindPath();
			_path = _pathfinder.GetPath();

			return true;
		}

		public void StartPathfing()
		{
		}

		public Vector2 GetNextPos()
		{
			return DefaultPosition;
		}

		private void GetObstacles()
		{
			_circlesObstacles = FindObjectsOfType<NeutralComponent>()
				.Select(n => (n.GetComponent<CapsuleCollider>(), n))
				.Where(tuple =>
					tuple.n.gameObject.activeSelf && tuple.n.gameObject != gameObject)
				.Select(tuple =>
					(tuple.n.gameObject.transform, tuple.Item1, tuple.n))
				.ToArray();
		}

		private void GetCircles()
		{
			_circles = _circlesObstacles
				.Select(tuple =>
					new Circle(tuple.Item2.ScaledRadius(), tuple.Item1.position.ToVec2())).ToArray();
		}

		#region Debug Drawing

		private List<Vector2> _bezierPointsGizmos;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			if (_bezierPointsGizmos == null) return;

			Gizmos.color = Color.cyan;
			const int segments = 10;
			for (var t = 0; t < segments; t++)
			{
				var p1 = MathUtils.GetQuadBezier(_bezierPointsGizmos[0], _bezierPointsGizmos[1], _bezierPointsGizmos[2],
					t / (float) segments);
				var p2 = MathUtils.GetQuadBezier(_bezierPointsGizmos[0], _bezierPointsGizmos[1], _bezierPointsGizmos[2],
					(t + 1) / (float) segments);
				Gizmos.DrawLine(p1.ToVec3(-3), p2.ToVec3(-3));
			}

			_bezierPointsGizmos = null;
		}

		#endregion
	}
}