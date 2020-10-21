using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Pathfinding
{
	public class CircularObsticleGraphGenerator<T> where T : IEquatable<T>
	{
		private IGraph<T> graph;

		public Dictionary<int, Circle> Circles { get; private set; }
		
		public Dictionary<int, List<Edge>> SurfingEdges { get; private set; } = new Dictionary<int, List<Edge>>();
		public Dictionary<int, List<Vector2>> PointsOnCircle { get; private set; } = new Dictionary<int, List<Vector2>>();
		public Dictionary<int, List<Edge>> HuggingEdges { get; private set; } = new Dictionary<int, List<Edge>>();
		// key - circle.GetHashCode()

		public Vector2 Start { get; private set; }
		public Vector2 Goal { get; private set; }
		
		public CircularObsticleGraphGenerator(IEnumerable<Circle> circles, Vector2 start, Vector2 goal)
		{
			SetCircles(circles);
			Start = start;
			Goal = goal;
		}
		
		private CircularObsticleGraphGenerator() { }

		public void SetStart(Vector2 start) => Start = start;
		public void SetCircles(IEnumerable<Circle> c) => Circles = c.ToDictionary(cr => cr.GetHashCode());
		public void SetGoal(Vector2 goal) => Goal = goal;

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

		private void GetHuggingEdges()
		{
			GetPointsOnCircle();
			
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

		Comparer<Vector2> VectorPolarComparer(int circleKey)
		{
			var circle = Circles[circleKey];
			
			return Comparer<Vector2>.Create((a, b) =>
			{
				a -= circle.center;
				b -= circle.center;
				
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
			});
		}
		
		
	}

	#region Helper Classes
	
	public readonly struct Edge
	{
		public readonly Vector2 a;
		public readonly Vector2 b;

		public readonly int circleAhash;
		public readonly int circleBhash;

		public Edge(Vector2 a, Vector2 b, int circleAhash, int circleBhash)
		{
			this.a = a;
			this.b = b;
			this.circleAhash = circleAhash;
			this.circleBhash = circleBhash;
		}
		
		public Edge(Vector2 a, Vector2 b)
		{
			this.a = a;
			this.b = b;
			circleAhash = -1;
			circleBhash = -1;
		}

		public override string ToString() => $"a: [{a.ToString()}], b: [{b.ToString()}]";

		public override bool Equals(object obj) =>
			obj is Edge bitangent && ((bitangent.a == a && bitangent.b == b) ||
			                          (bitangent.a == b && bitangent.b == a));

		public override int GetHashCode() => unchecked(a.GetHashCode() + b.GetHashCode());
	}

	public readonly struct Circle
	{
		public readonly float radius;
		public readonly Vector2 center;

		public Circle(float radius, Vector2 center)
		{
			this.radius = radius;
			this.center = center;
		}

		public bool Overlaps(Circle circle2)
		{
			var a = radius + circle2.radius;
			var dx = center.x - circle2.center.x;
			var dy = center.y - circle2.center.y;
			return a * a > dx * dx + dy * dy;
		}

		public bool Contains(Circle circle2)
		{
			var d = Mathf.Sqrt(
				circle2.center.x - center.x * circle2.center.x - center.x +
				circle2.center.y - center.y * circle2.center.y - center.y);
			return radius > d + circle2.radius;
		}

		public static bool operator ==(Circle c1, Circle c2) =>
			c1.center == c2.center && Math.Abs(c1.radius - c2.radius) < 0.001f;

		public static bool operator !=(Circle c1, Circle c2) => !(c1 == c2);

		public override string ToString()
		{
			return $"Radius: [{radius.ToString()}], Center: [{center.ToString()}]";
		}

		public override bool Equals(object obj) => 
			obj is Circle circle && circle.center == center && Math.Abs(circle.radius - radius) < 0.001f;

		public override int GetHashCode() => new {Center = center, Radius = radius}.GetHashCode();
	}
	
	#endregion
	
}