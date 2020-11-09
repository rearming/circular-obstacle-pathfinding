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
			_graphGenerator.graph.SetContentEqualsComparer((v1, v2) => v1.AlmostEqual(v2, 0.1f));
			_graphGenerator.SetActor(_actor);
			_pathfinder = new AStar<Vector2>(_graphGenerator.graph, AStarHeuristic<Vector2>.EuclideanDistance);
			
			_expander = new CircularGraphExpander(_graphGenerator, 0.25f);

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
		private Vector2 _huggingEdgeOwnerBasePos;
		private Transform _huggingEdgeOwner;

		private Vector2 HuggingEdgeMovement()
		{
			if (!_huggingEdgeMovement)
				PrepareHuggingEdgeMovement();

			return GetHuggingEdgePoint();
		}

		private Vector2 GetHuggingEdgePoint()
		{
			var currentPoint = CorrectHuggingEdgePoint(_huggingEdgePath[_huggingPointIdx]);

			if (Vector2.Distance(_neutral.transform.position.ToVec2(), currentPoint) < 0.1f)
				_huggingPointIdx++;
			if (_huggingPointIdx >= _huggingEdgePath.Count)
			{
				_huggingEdgeMovement = false;
				_huggingPointIdx--;
			}

			return CorrectHuggingEdgePoint(_huggingEdgePath[_huggingPointIdx]);
		}

		private Vector2 CorrectHuggingEdgePoint(Vector2 point)
		{
			return point - (_huggingEdgeOwnerBasePos - _huggingEdgeOwner.position.ToVec2());
		}

		private void PrepareHuggingEdgeMovement()
		{
			_huggingPointIdx = 0;
			var edgeInfo = (EdgeInfo) _path[1].graphEdge.info;
			_huggingEdgePath = edgeInfo.arcPoints;
			_huggingEdgeOwner = edgeInfo.circleOwner;
			_huggingEdgeOwnerBasePos = _huggingEdgeOwner.position.ToVec2();
			
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
					new Circle(tuple.Item2.ScaledRadius(), tuple.Item1.position.ToVec2(), tuple.Item1))
				.ToArray();
		}

		#endregion

		#region Debug Drawing

		private List<Vector2> _bezierPointsGizmos;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			
			if (_bezierPointsGizmos != null) DrawGizmosBezier();
			if (_huggingEdgeMovement) DrawCorrectHuggingEdge();
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

		private void DrawCorrectHuggingEdge()
		{
			Gizmos.color = Color.red;
			var correctPoints = _huggingEdgePath.Select(CorrectHuggingEdgePoint);
			foreach (var correctPoint in correctPoints)
			{
				Gizmos.DrawSphere(correctPoint.ToVec3(-3), 0.04f);
			}
		}

		#endregion
	}
}