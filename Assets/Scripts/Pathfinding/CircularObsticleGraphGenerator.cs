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
		private Circle[] circles;
		
		public readonly Dictionary<(Circle, Circle), List<Edge>> surfingEdges = new Dictionary<(Circle, Circle), List<Edge>>();
		public readonly Dictionary<int, List<Vector2>> pointsOnCircle = new Dictionary<int, List<Vector2>>();
		public readonly Dictionary<int, List<Edge>> huggingEdges = new Dictionary<int, List<Edge>>();
		// key - circle.GetHashCode()

		public Vector2 Start { get; private set; }
		public Vector2 Goal { get; private set; }
		
		public CircularObsticleGraphGenerator(Circle[] circles, Vector2 start, Vector2 goal)
		{
			this.circles = circles;
			Start = start;
			Goal = goal;
		}
		
		private CircularObsticleGraphGenerator() { }

		public void SetStart(Vector2 start) => Start = start;
		public void SetCircles(Circle[] c) => circles = c;
		public void SetGoal(Vector2 goal) => Goal = goal;

		public void GenerateGraph()
		{
			surfingEdges.Clear();
			foreach (var circle1 in circles)
			{
				foreach (var circle2 in circles)
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
			if (surfingEdges.ContainsKey((circle1, circle2)))
				return;
			var bitangents = new List<Edge>();
			if (!circle1.Overlaps(circle2))
				bitangents.AddRange(GetInternalBitangents(circle1, circle2));
			if (!circle1.Contains(circle2))
				bitangents.AddRange(GetExternalBitangents(circle1, circle2));
			GenerateSurfingEdges(circle1, circle2, bitangents);
			AddEdges(surfingEdges, (circle1, circle2), bitangents);
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
			foreach (var circle in circles)
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

		private void GetPointsOnCircle()
		{
			
		}

		private void GetHuggingEdges()
		{
			foreach (var surfingEdgeList in surfingEdges.Values)
			{
				foreach (var surfingEdge in surfingEdgeList)
				{
					pointsOnCircle.AddToDictList(surfingEdge.circleAhash, surfingEdge.a);
					pointsOnCircle.AddToDictList(surfingEdge.circleBhash, surfingEdge.b);
				}
			}
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