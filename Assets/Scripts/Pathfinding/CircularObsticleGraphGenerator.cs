using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public struct Bitangent
	{
		public Vector2 a;
		public Vector2 b;

		// int circleID; - для дебага
		
		public Bitangent(Vector2 a, Vector2 b)
		{
			this.a = a;
			this.b = b;
		}

		public override string ToString() => $"a: [{a.ToString()}], b: [{b.ToString()}]";

		public override bool Equals(object obj) =>
			obj is Bitangent bitangent && ((bitangent.a == a && bitangent.b == b) ||
			                               (bitangent.a == b && bitangent.b == a));

		public override int GetHashCode() => unchecked(a.GetHashCode() + b.GetHashCode());
	}

	public struct Circle
	{
		public float Radius { get; private set; }
		public Vector2 Center { get; private set; }

		public Circle(float radius, Vector2 center)
		{
			Radius = radius;
			Center = center;
		}

		public bool Overlaps(Circle circle2)
		{
			var a = Radius + circle2.Radius;
			var dx = Center.x - circle2.Center.x;
			var dy = Center.y - circle2.Center.y;
			return a * a > dx * dx + dy * dy;
		}

		public bool Contains(Circle circle2)
		{
			var d = Mathf.Sqrt(
				circle2.Center.x - Center.x * circle2.Center.x - Center.x +
				circle2.Center.y - Center.y * circle2.Center.y - Center.y);
			return Radius > d + circle2.Radius;
		}

		public static bool operator ==(Circle c1, Circle c2) =>
			c1.Center == c2.Center && Math.Abs(c1.Radius - c2.Radius) < 0.001f;

		public static bool operator !=(Circle c1, Circle c2) => !(c1 == c2);

		public override string ToString()
		{
			return $"Radius: [{Radius.ToString()}], Center: [{Center.ToString()}]";
		}

		public override bool Equals(object obj) => 
			obj is Circle circle && circle.Center == Center && Math.Abs(circle.Radius - Radius) < 0.001f;

		public override int GetHashCode() => new {Center, Radius}.GetHashCode();
	}
	
	public class CircularObsticleGraphGenerator<T> where T : IEquatable<T>
	{
		private IGraph<T> graph;
		private Circle[] circles;
		public Dictionary<int, List<Bitangent>> Bitangents { get; private set; } = new Dictionary<int, List<Bitangent>>();

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
			Bitangents.Clear();
			foreach (var circle1 in circles)
			{
				foreach (var circle2 in circles)
				{
					if (circle1 == circle2)
						continue;
					GetBitangents(circle1, circle2);
				}
			}
		}

		private void GetBitangents(Circle circle1, Circle circle2)
		{
			if (Bitangents.ContainsKey(GetCirclesHash(circle1, circle2)))
				return;
			var bitangents = new List<Bitangent>();
			if (!circle1.Overlaps(circle2))
				bitangents.AddRange(GetInternal(circle1, circle2));
			if (!circle1.Contains(circle2))
				bitangents.AddRange(GetExternal(circle1, circle2));
			GenerateSurfingEdges(circle1, circle2, bitangents);
			AddBitangents(circle1, circle2, bitangents);
		}

		private List<Bitangent> GetInternal(Circle circle1, Circle circle2)
		{
			var theta = Mathf.Acos(
				(circle1.Radius + circle2.Radius) / Vector2.Distance(circle1.Center, circle2.Center));

			var dir1 = (circle2.Center - circle1.Center).normalized * circle1.Radius;
			var D = dir1.Rotate(theta) + circle1.Center;
			var C = dir1.Rotate(-theta) + circle1.Center;

			var dir2 = (circle1.Center - circle2.Center).normalized * circle2.Radius;
			var E = dir2.Rotate(theta) + circle2.Center;
			var F = dir2.Rotate(-theta) + circle2.Center;
			
			var CF = new Bitangent(C, F);
			var DE = new Bitangent(D, E);
			
			return new List<Bitangent>{ CF, DE };
		}
		

		private List<Bitangent> GetExternal(Circle circle1, Circle circle2)
		{
			var theta = Mathf.Acos(
				(circle1.Radius - circle2.Radius) / Vector2.Distance(circle1.Center, circle2.Center));

			var dir1 = (circle2.Center - circle1.Center).normalized * circle1.Radius;
			var D = dir1.Rotate(theta) + circle1.Center;
			var C = dir1.Rotate(-theta) + circle1.Center;

			var dir2 = -(circle1.Center - circle2.Center).normalized * circle2.Radius;
			var E = dir2.Rotate(theta) + circle2.Center;
			var F = dir2.Rotate(-theta) + circle2.Center;
			
			var CF = new Bitangent(C, F);
			var DE = new Bitangent(D, E);
			
			return new List<Bitangent>{ CF, DE };
		}

		private void GenerateSurfingEdges(Circle circle1, Circle circle2, List<Bitangent> bitangents)
		{
			foreach (var circle in circles)
			{
				if (circle == circle1 || circle == circle2)
					continue;
				bitangents.RemoveAll(bt =>
				{
					var u = Vector2.Dot(circle.Center - bt.a, bt.b - bt.a) / Vector2.Dot(bt.b - bt.a, bt.b - bt.a);
					var e = bt.a + Mathf.Clamp01(u) * (bt.b - bt.a);
					var d = (e - circle.Center).magnitude;
					return d < circle.Radius; // remove if (d < radius)
				});
			}
		}
		
		private void AddBitangents(Circle circle1, Circle circle2, List<Bitangent> bitangents)
		{
			if (!Bitangents.ContainsKey(GetCirclesHash(circle1, circle2)))
				Bitangents.Add(GetCirclesHash(circle1, circle2), bitangents);
		}

		private bool TryGetBitangents(Circle circle1, Circle circle2, out List<Bitangent> bitangents)
		{
			return Bitangents.TryGetValue(GetCirclesHash(circle1, circle2), out bitangents);
		}

		private int GetCirclesHash(Circle circle1, Circle circle2)
		{
			return unchecked(circle1.GetHashCode() + circle2.GetHashCode());
		}

		public void ForEachBitangent(Action<Bitangent> action)
		{
			foreach (var bitangents in Bitangents.Values)
			{
				foreach (var bitangent in bitangents)
				{
					action(bitangent);
				}
			}
		}

		public void RemoveBitangentIf(Func<Bitangent, bool> removeIf)
		{
			var removals = new List<(int, int)>(); // key, index
			foreach (var btList in Bitangents)
			{
				for (var i = 0; i < btList.Value.Count; i++)
				{
					var bitangent = btList.Value[i];
					if (removeIf(bitangent))
						removals.Add((btList.Key, i));
				}
			}

			foreach (var (key, bitangentIdx) in removals)
			{
				Bitangents[key].RemoveAt(bitangentIdx);
			}
		}
		
		
		private void GenerateHuggingEdges()
		{
			
		}
	}
}