using System.Collections.Generic;
using System.Linq;
using Pathfinding.Graph;
using TestingEnvironmentScripts;
using UnityEngine;
using Utils;

namespace Pathfinding.CircularObstacleGraph
{
	public class CircularObsticleGraphGenerator
	{
		public readonly Graph<Vector2> graph = new Graph<Vector2>();

		public CircularObsticleGraphGenerator(IEnumerable<Circle> circles, Vector2 start, Vector2 goal)
		{
			SetStart(start);
			SetGoal(goal);

			SetCircles(circles);

			graph.SetContentEqualsComparer((v1, v2) => v1 == v2);
		}

		public CircularObsticleGraphGenerator()
		{
		}

		private Actor Actor { get; set; } = new Actor(0.01f);

		public Dictionary<int, Circle> Circles { get; private set; }
		// key - circle.GetHashCode()

		public Dictionary<int, List<Edge>> SurfingEdges { get; } = new Dictionary<int, List<Edge>>();
		// key - GetCirclesHash(circle1, circle2) 

		public Dictionary<int, List<Vector2>> PointsOnCircle { get; private set; } =
			new Dictionary<int, List<Vector2>>();

		// key - circle.GetHashCode()
		public Dictionary<int, List<Edge>> HuggingEdges { get; } = new Dictionary<int, List<Edge>>();
		// key - circle.GetHashCode()

		public Vector2 Start { get; private set; }
		public Vector2 Goal { get; private set; }

		/// <summary>
		///     Nodes closer than that distance will be considered as one same node
		/// </summary>
		public float DistanceTolerance { get; set; } = 0.05f;

		/// <summary>
		///     Must be called before SetCirlces()
		/// </summary>
		/// <param name="start"></param>
		public void SetStart(Vector2 start)
		{
			Start = start;
		}

		/// <summary>
		///     Must be called before SetCirlces()
		/// </summary>
		/// <param name="goal"></param>
		public void SetGoal(Vector2 goal)
		{
			Goal = goal;
		}

		public void SetActor(Actor a)
		{
			Actor = a;
		}

		public void SetCircles(IEnumerable<Circle> c)
		{
			Circles = c
				.Select(cr => new Circle(cr.radius + Actor.Radius, cr.center, cr.info)) // Minkowski Expansion by Actor.radius
				.ToDictionary(cr => cr.GetHashCode());
		}

		public void GenerateGraph()
		{
			SurfingEdges.Clear();
			foreach (var circle1 in Circles.Values)
			{
				foreach (var circle2 in Circles.Values)
				{
					if (circle1 == circle2)
						continue;
					GetSurfingEdges(circle1, circle2);
				}
			}

			GetHuggingEdges();
			GetGraph();
		}

		private void GetGraph()
		{
			graph.Clear();
			foreach (var surfingEdgeList in SurfingEdges.Values)
			{
				foreach (var surfingEdge in surfingEdgeList)
				{
					if (Vector2.Distance(surfingEdge.a, surfingEdge.b) <= DistanceTolerance)
						continue;
					var nodeA = new Node<Vector2>(surfingEdge.a, surfingEdge.circleAhash);
					var nodeB = new Node<Vector2>(surfingEdge.b, surfingEdge.circleBhash);
					graph.AddNode(nodeA);
					graph.AddNode(nodeB);
					graph.ConnectNodes(nodeA, nodeB, Vector2.Distance(surfingEdge.a, surfingEdge.b));
				}
			}

			foreach (var huggingEdgeList in HuggingEdges.Values)
			{
				foreach (var huggingEdge in huggingEdgeList)
				{
					if (Vector2.Distance(huggingEdge.a, huggingEdge.b) <= DistanceTolerance)
						continue;
					graph.ConnectNodes(
						huggingEdge.a, huggingEdge.b,
						Vector2.Distance(huggingEdge.a, huggingEdge.b),
						huggingEdge.edgeInfo);
				}
			}

			AddStartAndGoalToGraph();
		}

		#region Surging Edges Generation

		private void GetSurfingEdges(Circle circle1, Circle circle2)
		{
			if (SurfingEdges.ContainsKey(GetCirclesHash(circle1, circle2)))
				return;
			var bitangents = new List<Edge>();
			if (!circle1.Overlaps(circle2))
				bitangents.AddRange(GetInternalBitangents(circle1, circle2));
			if (!circle1.Contains(circle2))
				bitangents.AddRange(GetExternalBitangents(circle1, circle2));
			ThrowSurfingEdgesOut(circle1, circle2, bitangents);
			AddEdges(SurfingEdges, GetCirclesHash(circle1, circle2), bitangents);
		}

		private List<Edge> GetInternalBitangents(Circle circle1, Circle circle2)
		{
			var theta = Mathf.Acos(
				(circle1.radius + circle2.radius) / Vector2.Distance(circle1.center, circle2.center));

			var dir1 = (circle2.center - circle1.center).normalized * circle1.radius;
			var d = dir1.Rotate(theta) + circle1.center;
			var c = dir1.Rotate(-theta) + circle1.center;

			var dir2 = (circle1.center - circle2.center).normalized * circle2.radius;
			var e = dir2.Rotate(theta) + circle2.center;
			var f = dir2.Rotate(-theta) + circle2.center;

			var cf = new Edge(c, f, circle1.GetHashCode(), circle2.GetHashCode());
			var de = new Edge(d, e, circle1.GetHashCode(), circle2.GetHashCode());

			return new List<Edge> {cf, de};
		}

		private List<Edge> GetExternalBitangents(Circle circle1, Circle circle2)
		{
			var theta = Mathf.Acos(
				(circle1.radius - circle2.radius) / Vector2.Distance(circle1.center, circle2.center));

			var dir1 = (circle2.center - circle1.center).normalized * circle1.radius;
			var d = dir1.Rotate(theta) + circle1.center;
			var c = dir1.Rotate(-theta) + circle1.center;

			var dir2 = -(circle1.center - circle2.center).normalized * circle2.radius;
			var e = dir2.Rotate(theta) + circle2.center;
			var f = dir2.Rotate(-theta) + circle2.center;

			var cf = new Edge(c, f, circle1.GetHashCode(), circle2.GetHashCode());
			var de = new Edge(d, e, circle1.GetHashCode(), circle2.GetHashCode());

			return new List<Edge> {cf, de};
		}

		private void ThrowSurfingEdgesOut(Circle circle1, Circle circle2, List<Edge> bitangents)
		{
			foreach (var circle in Circles.Values) // test all circles
			{
				if (circle == circle1 || circle == circle2) // don't test circles to which edges belong
					continue;
				bitangents.RemoveAll(edge => ThrowSurfingEdgeOut(edge, circle));
				// if circle is between edge.a and edge.b, throw the edge out
			}
		}

		private bool ThrowSurfingEdgeOut(in Edge edge, Circle circle)
		{
			var u = Vector2.Dot(circle.center - edge.a, edge.b - edge.a) /
			        Vector2.Dot(edge.b - edge.a, edge.b - edge.a);
			var e = edge.a + Mathf.Clamp01(u) * (edge.b - edge.a);
			var d = (e - circle.center).magnitude;
			return d < circle.radius; // remove if (d < radius)
		}

		#endregion

		#region Hugging Edges Generation

		private void GetHuggingEdges()
		{
			GetPointsOnCircle();
			GenerateHuggingEdges();
			// ThrowHuggingEdgesOut(); since in our case obstacles don't overlap, we can skip this for now
		}

		private void GetPointsOnCircle()
		{
			PointsOnCircle.Clear();
			foreach (var surfingEdge in SurfingEdges.Values.SelectMany(surfingEdgeList => surfingEdgeList))
			{
				PointsOnCircle.AddToDictList(surfingEdge.circleAhash, surfingEdge.a);
				PointsOnCircle.AddToDictList(surfingEdge.circleBhash, surfingEdge.b);
			}

			var sortedPoints = new Dictionary<int, List<Vector2>>();
			foreach (var pointsList in PointsOnCircle)
			{
				sortedPoints[pointsList.Key] = pointsList.Value
					.OrderBy(v => v, VectorPolarComparer(pointsList.Key))
					.ToList();
			}

			PointsOnCircle = sortedPoints;
		}

		private void GenerateHuggingEdges()
		{
			HuggingEdges.Clear();
			foreach (var pointsList in PointsOnCircle)
			{
				for (var i = 0; i < pointsList.Value.Count; i++)
				{
					var p1 = pointsList.Value[i];
					var p2 = pointsList.Value[(i + 1) % pointsList.Value.Count];
					var circle = Circles[pointsList.Key];
					HuggingEdges.AddToDictList(pointsList.Key,
						new Edge(p1, p2, GetHuggingEdgeInfo(p1, p2, circle)));
					// add point and next point as hugging edge
				}
			}
		}

		private EdgeInfo GetHuggingEdgeInfo(in Vector2 point1, in Vector2 point2, in Circle circle)
		{
			var point1Origin = point1 - circle.center;
			var point2Origin = point2 - circle.center;

			var radSq = circle.radius * 2 * (circle.radius * 2);
			var arcAngle = Mathf.Acos((radSq - Vector2.SqrMagnitude(point1Origin - point2Origin)) / radSq);

			var center = circle.center;
			var splits = circle.ArcLength(arcAngle * Mathf.Rad2Deg) * 3;
			var arcPoints = MathUtils.SplitArc(point1Origin, point2Origin, splits <= 1 ? 1 : (int)splits)
				.Select(p => p + center).ToList(); // move arc points from origin to their original position

			return new EdgeInfo(arcAngle, arcPoints);
		}

		private void ThrowHuggingEdgesOut()
		{
			foreach (var huggingEdge in HuggingEdges)
				huggingEdge.Value.RemoveAll(edge => ThrowHuggingEdgeOut(huggingEdge.Key, edge));
		}

		private bool ThrowHuggingEdgeOut(int circleHash, Edge edge)
		{
			foreach (var circle in Circles)
			{
				if (circle.Key == circleHash) // if same circle - continue
					continue;
				var circle1 = Circles[circleHash];
				var circle2 = Circles[circleHash];
				var distance = Vector2.Distance(circle1.center, circle2.center);
				var a = circle1.radius * circle1.radius - circle2.radius * circle2.radius + distance;
				var thetaAngle = Mathf.Acos(a / circle1.radius);

				var dir = (circle2.center - circle1.center).normalized * circle1.radius;

				var e = dir.Rotate(thetaAngle);
				var d = dir.Rotate(-thetaAngle);

				e -= circle1.center;
				d -= circle1.center;
				var edgePoint1 = edge.a - circle1.center;
				var edgePoint2 = edge.b - circle1.center;

				// if (VectorPolarCompare(E, )) 
				// {
				// 	
				// }
				//

				// compare vectors, if hugging edge vector within - return true
			}

			return false;
		}

		private Comparer<Vector2> VectorPolarComparer(int circleKey)
		{
			var circle = Circles[circleKey];

			return Comparer<Vector2>.Create((a, b) =>
			{
				a -= circle.center;
				b -= circle.center;

				return VectorPolarCompare(a, b);
			});
		}

		private int VectorPolarCompare(Vector2 a, Vector2 b)
		{
			int Quad(Vector2 p)
			{
				if (p.x < 0 && p.y < 0) return 0;
				if (p.x >= 0 && p.y < 0) return 1;
				if (p.x >= 0 && p.y >= 0) return 2;
				if (p.x < 0 && p.y >= 0) return 3;
				return -1;
			}

			if (Quad(a) == Quad(b))
				return a.x * b.y > a.y * b.x ? -1 : 1;
			return Quad(a) < Quad(b) ? -1 : 1;
		}

		#endregion

		#region Points Graph Connection

		private void AddStartAndGoalToGraph()
		{
			var startNode = new Node<Vector2>(Start);
			var goalNode = new Node<Vector2>(Goal);
			graph.AddNode(startNode);
			graph.AddNode(goalNode);
			foreach (var node in graph)
			{
				if (ReferenceEquals(node, startNode) || ReferenceEquals(node, goalNode))
					continue;
				if (ConnectPointToNode(startNode, node))
					graph.ConnectNodes(node, startNode, Vector2.Distance(node.Content, startNode.Content));
				if (ConnectPointToNode(goalNode, node))
					graph.ConnectNodes(node, goalNode, Vector2.Distance(node.Content, goalNode.Content));
			}

			if (CanConnectNodes(new Edge(Start, Goal)))
				graph.ConnectNodes(startNode, goalNode);
		}

		private bool ConnectPointToNode(Node<Vector2> pointNode, Node<Vector2> node)
		{
			var selfCircle = Circles[(int) node.Info];
			var shiftDir = node.Content - selfCircle.center;
			// expand all points from circle's center to avoid throwing out points by it's circle overlap  
			var shiftedCircleNode = node.Content + shiftDir * 0.1f;
			var edge = new Edge(pointNode.Content, shiftedCircleNode);
			return CanConnectNodes(edge);
		}

		private bool CanConnectNodes(in Edge edge)
		{
			foreach (var circle in Circles.Values)
			{
				if (ThrowSurfingEdgeOut(edge, circle))
					return false;
			}

			return true;
		}

		#endregion

		#region Edge Utils

		private void AddEdges<TK>(IDictionary<TK, List<Edge>> edges, TK key, List<Edge> newEdges)
		{
			if (!edges.ContainsKey(key))
				edges.Add(key, newEdges);
		}

		private int GetCirclesHash(Circle circle1, Circle circle2)
		{
			return unchecked(circle1.GetHashCode() + circle2.GetHashCode());
		}

		#endregion
	}
}