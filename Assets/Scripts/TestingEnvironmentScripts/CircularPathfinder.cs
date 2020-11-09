using System.Collections.Generic;
using System.Linq;
using DebugDrawers;
using Pathfinding;
using Pathfinding.Algorithms;
using Pathfinding.CircularObstacleGraph;
using Pathfinding.Graph;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	public class CircularPathfinder : MonoBehaviour
	{
		private Actor _actor;
		private float _baseActorRadius;
		private Circle[] _circles;

		private (Transform, CapsuleCollider, Neutral)[] _circlesObstacles;

		#region Debug Fields

		private PathfindingDebugDrawer _debugDrawer;

		#endregion

		private CircularObsticleGraphGenerator _graphGenerator;

		private Neutral _neutral;
		private List<NodeWithEdge<Vector2>> _path;
		private List<NodeWithEdge<Vector2>> _prevPath;
		private AStar<Vector2> _pathfinder;
		private CircularGraphExpander _expander;

		private Vector2 DefaultPosition => _neutral.transform.position.ToVec2();

		private void Awake()
		{
			_neutral = GetComponent<Neutral>();
			_debugDrawer = GetComponent<PathfindingDebugDrawer>();
		}

		private void Start()
		{
			GetObstacles();
			_baseActorRadius = GetComponent<CapsuleCollider>().radius;
			_actor = new Actor(_baseActorRadius);

			_graphGenerator = new CircularObsticleGraphGenerator();
			_graphGenerator.graph.SetContentEqualsComparer((v1, v2) => v1.AlmostEqual(v2, 0.05f));
			_graphGenerator.SetActor(_actor);
			_pathfinder = new AStar<Vector2>(_graphGenerator.graph, AStarHeuristic<Vector2>.EuclideanDistance);
			
			_expander = new CircularGraphExpander(_graphGenerator, 0.1f);

			if (!_debugDrawer.IsRealNull())
				_debugDrawer.Setup(_graphGenerator, _path);
		}

		private void Update()
		{
			if (!_debugDrawer.IsRealNull())
				_debugDrawer.path = _path;
		}

		public void StartPathfing()
		{
			_huggingEdgeMovement = false;
		}

		private bool FindPath()
		{ 
			GetCircles();

			var start = _neutral.transform.position.ToVec2();
			// var start = _neutral.StartPos;

			_graphGenerator.SetStart(start);
			_graphGenerator.SetGoal(_neutral.Goal.Value);
			_graphGenerator.SetCircles(_circles);
			_graphGenerator.GenerateGraph();
			
			_pathfinder.SetStart(start);
			_pathfinder.SetGoal(_neutral.Goal);

			try
			{
				_pathfinder.FindPath();
				_path = _pathfinder.Path;
				_expander.ExpandPathPoints(_path);

			}
			catch (SmallPathException e)
			{
				Debug.LogWarning(e.Message);
				return false;
			}
			catch (IncompletePathException e)
			{
				Debug.LogWarning(e.Message);
				// return false;
			}

			return true;
		}

		public Vector2 GetNextPos()
		{
			if (_huggingEdgeMovement)
			{
				return HuggingEdgeMovement();
			}
			
			if (FindPath())
			{
				if (_path[1].graphEdge.info == null)
					return _path[1].node.Content;
				return HuggingEdgeMovement();
			}

			_neutral.UnsetGoal();
			return DefaultPosition;
		}

		private int _huggingPointIdx;
		private bool _huggingEdgeMovement;
		private List<Vector2> _huggingEdgePath;
		
		private EdgeInfo _huggingEdgeInfo;

		private Vector2 HuggingEdgeMovement()
		{
			if (!_huggingEdgeMovement)
				PrepareHuggingEdgeMovement();

			var currentPoint = _huggingEdgePath[_huggingPointIdx];
			if (Vector2.Distance(_neutral.transform.position.ToVec2(), currentPoint) < 0.1f)
				_huggingPointIdx++;
			if (_huggingPointIdx >= _huggingEdgePath.Count)
			{
				_huggingEdgeMovement = false;
				_huggingPointIdx--;
			}
			
			return _huggingEdgePath[_huggingPointIdx];
		}
		
		private void PrepareHuggingEdgeMovement()
		{
			_huggingPointIdx = 0;
			_huggingEdgePath = ((EdgeInfo) _path[1].graphEdge.info).arcPoints;

			var np = _neutral.transform.position.ToVec2();
			if (Vector2.Distance(np, _huggingEdgePath[0]) > Vector2.Distance(np, _huggingEdgePath.Last()))
				_huggingEdgePath.Reverse();
			_huggingEdgeMovement = true;
		}

		#region Obstacles Getting

		private void GetObstacles()
		{
			_circlesObstacles = FindObjectsOfType<Neutral>()
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
					new Circle(tuple.Item2.ScaledRadius(), tuple.Item1.position.ToVec2(), tuple.Item3))
				.ToArray();
		}

		#endregion

		#region Debug Drawing
		
		private List<Vector2> _bezierPointsGizmos;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			
			if (_bezierPointsGizmos != null) DrawGizmosBezier();
		}

		private void DrawGizmosBezier()
		{
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