using System.Collections.Generic;
using System.Linq;
using Pathfinding.Graph;
using UnityEngine;
using Utils;

namespace Pathfinding.Circular_Obstacle_Graph
{
	public class CircularObsticleGraphGenerator
	{
		public readonly Graph<Vector2> graph = new Graph<Vector2>();
		private Actor Actor { get; set; } = new Actor(0.01f);

		public Dictionary<int, Circle> Circles { get; private set; }
		// key - circle.GetHashCode()
		
		public Dictionary<int, List<Edge>> SurfingEdges { get; } = new Dictionary<int, List<Edge>>();
		// key - GetCirclesHash(circle1, circle2) 
		
		public Dictionary<int, List<Vector2>> PointsOnCircle { get; private set; } = new Dictionary<int, List<Vector2>>();
		// key - circle.GetHashCode()
		public Dictionary<int, List<Edge>> HuggingEdges { get; } = new Dictionary<int, List<Edge>>();
		// key - circle.GetHashCode()

		public Vector2 Start { get; private set; }
		public Vector2 Goal { get; private set; }

		/// <summary>
		/// Nodes closer than that distance will be considered as one same node 
		/// </summary>
		private float DistanceTolerance { get; set; } = 0.05f;
		
		public CircularObsticleGraphGenerator(IEnumerable<Circle> circles, Vector2 start, Vector2 goal)
		{
			SetStart(start);
			SetGoal(goal);
			
			SetCircles(circles);
			
			graph.SetContentEqualsComparer((v1, v2) => v1 == v2);
		}

		public void SetStart(Vector2 start) => Start = start;

		public void SetGoal(Vector2 goal) => Goal = goal;

		public void SetActor(Actor a) => Actor = a;

		private void AddPointToCircles(Vector2 point)
		{
			var circle = new Circle(DistanceTolerance / 3f, point);
			Circles.Add(circle.GetHashCode(), circle);
		}

		public void SetCircles(IEnumerable<Circle> c)
		{
			Circles = c
				.Select(cr => new Circle(cr.radius + Actor.Radius, cr.center)) // Minkowski Expansion by Actor.radius
				.ToDictionary(cr => cr.GetHashCode());
			AddPointToCircles(Start);
			AddPointToCircles(Goal);
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
					graph.AddNode(surfingEdge.a);
					graph.AddNode(surfingEdge.b);
					graph.ConnectNodes(surfingEdge.a, surfingEdge.b, Vector2.Distance(surfingEdge.a, surfingEdge.b));
				}
			}
			foreach (var huggingEdgeList in HuggingEdges.Values)
			{
				foreach (var huggingEdge in huggingEdgeList)
				{
					if (Vector2.Distance(huggingEdge.a, huggingEdge.b) <= DistanceTolerance)
						continue;
					graph.ConnectNodes(huggingEdge.a, huggingEdge.b, Vector2.Distance(huggingEdge.a, huggingEdge.b));
				}
			}
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
			GenerateSurfingEdges(circle1, circle2, bitangents);
			AddEdges(SurfingEdges, GetCirclesHash(circle1, circle2), bitangents);
		}

		private List<Edge> GetInternalBitangents(Circle circle1, Circle circle2)
		{
			var theta = Mathf.Acos(
				(circle1.radius + circle2.radius) / Vector2.Distance(circle1.center, circle2.center));

			var dir1 = (circle2.center - circle1.center).normalized * circle1.radius;
			var D = dir1.Rotate(theta) + circle1.center;
			var C = dir1.Rotate(-theta) + circle1.center;

			var dir2 = (circle1.center - circle2.center).normalized * circle2.radius;
			var E = dir2.Rotate(theta) + circle2.center;
			var F = dir2.Rotate(-theta) + circle2.center;
			
			var CF = new Edge(C, F, circle1.GetHashCode(), circle2.GetHashCode());
			var DE = new Edge(D, E, circle1.GetHashCode(), circle2.GetHashCode());
			
			return new List<Edge>{ CF, DE };
		}

		private List<Edge> GetExternalBitangents(Circle circle1, Circle circle2)
		{
			var theta = Mathf.Acos(
				(circle1.radius - circle2.radius) / Vector2.Distance(circle1.center, circle2.center));

			var dir1 = (circle2.center - circle1.center).normalized * circle1.radius;
			var D = dir1.Rotate(theta) + circle1.center;
			var C = dir1.Rotate(-theta) + circle1.center;

			var dir2 = -(circle1.center - circle2.center).normalized * circle2.radius;
			var E = dir2.Rotate(theta) + circle2.center;
			var F = dir2.Rotate(-theta) + circle2.center;
			
			var CF = new Edge(C, F, circle1.GetHashCode(), circle2.GetHashCode());
			var DE = new Edge(D, E, circle1.GetHashCode(), circle2.GetHashCode());
			
			return new List<Edge>{ CF, DE };
		}

		private void GenerateSurfingEdges(Circle circle1, Circle circle2, List<Edge> bitangents)
		{
			foreach (var circle in Circles.Values)
			{
				if (circle == circle1 || circle == circle2)
					continue;
				bitangents.RemoveAll(bt =>
				{
					var u = Vector2.Dot(circle.center - bt.a, bt.b - bt.a) / Vector2.Dot(bt.b - bt.a, bt.b - bt.a);
					var e = bt.a + Mathf.Clamp01(u) * (bt.b - bt.a);
					var d = (e - circle.center).magnitude;
					return d < circle.radius; // remove if (d < radius)
				});
			}
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
			foreach (var surfingEdgeList in SurfingEdges.Values)
			{
				foreach (var surfingEdge in surfingEdgeList)
				{
					PointsOnCircle.AddToDictList(surfingEdge.circleAhash, surfingEdge.a);
					PointsOnCircle.AddToDictList(surfingEdge.circleBhash, surfingEdge.b);
				}
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
					HuggingEdges.AddToDictList(pointsList.Key,
						new Edge(pointsList.Value[i], pointsList.Value[(i + 1) % pointsList.Value.Count]));
					// add point and next point as hugging edge
				}
			}
		}

		private void ThrowHuggingEdgesOut()
		{
			foreach (var huggingEdge in HuggingEdges)
			{
				huggingEdge.Value.RemoveAll(edge => ThrowHuggingEdgeOut(huggingEdge.Key, edge));
			}
		}

		private bool ThrowHuggingEdgeOut(int circleHash, Edge edge)
		{
			foreach (var circle in Circles)
			{
				if (circle.Key == circleHash) // if same circle - continue
					continue;
				var circle1 = Circles[circleHash];
				var circle2 = Circles[circleHash];
				var d = Vector2.Distance(circle1.center, circle2.center);
				var a = circle1.radius * circle1.radius - circle2.radius * circle2.radius + d;
				var thetaAngle = Mathf.Acos(a / circle1.radius);


				var dir = (circle2.center - circle1.center).normalized * circle1.radius;

				var E = dir.Rotate(thetaAngle);
				var D = dir.Rotate(-thetaAngle);

				E -= circle1.center;
				D -= circle1.center;
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

		Comparer<Vector2> VectorPolarComparer(int circleKey)
		{
			var circle = Circles[circleKey];
			
			return Comparer<Vector2>.Create((a, b) =>
			{
				a -= circle.center;
				b -= circle.center;
				
				return VectorPolarCompare(a, b);
			});
		}

		int VectorPolarCompare(Vector2 a, Vector2 b)
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
		
		public CircularObsticleGraphGenerator() { }
	}

}