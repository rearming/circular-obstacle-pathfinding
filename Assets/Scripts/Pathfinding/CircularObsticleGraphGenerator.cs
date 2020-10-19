using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Pathfinding;

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

		public override bool Equals(object obj)
		{
			return obj is Bitangent bitangent && ((bitangent.a == a && bitangent.b == b) ||
			                                      (bitangent.a == b && bitangent.b == a));
		}

		public override int GetHashCode()
		{
			return unchecked(a.GetHashCode() + b.GetHashCode());
		}
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
		public HashSet<Bitangent> Bitangents { get; private set; } = new HashSet<Bitangent>();

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
		public void SetGoal(Vector2 goal) => Goal = goal;

		public void GenerateGraph()
		{
			foreach (var circle1 in circles)
			{
				foreach (var circle2 in circles)
				{
					if (circle1 == circle2)
						continue;
					Bitangents.UnionWith(GetBitangents(circle1, circle2));
				}
			}
		}

		private HashSet<Bitangent> GetBitangents(Circle circle1, Circle circle2)
		{
			HashSet<Bitangent> result = new HashSet<Bitangent>();

			result.UnionWith(GetInternal(circle1, circle2));
			result.UnionWith(GetExternal(circle1, circle2));
			
			return result;
		}

		private HashSet<Bitangent> GetInternal(Circle circle1, Circle circle2)
		{
			var theta = Mathf.Acos(
				(circle1.Radius - circle2.Radius) / Vector2.Distance(circle1.Center, circle2.Center));

			var dir1 = (circle2.Center - circle1.Center).normalized;
			var D = dir1.Rotate(theta);
			var C = dir1.Rotate(-theta);

			var dir2 = (circle1.Center - circle2.Center).normalized;
			var E = dir2.Rotate(theta);
			var F = dir2.Rotate(-theta);
			
			var CF = new Bitangent(C, F);
			var DE = new Bitangent(D, E);

			return new HashSet<Bitangent>{CF, DE};
		}

		private HashSet<Bitangent> GetExternal(Circle circle1, Circle circle2)
		{
			return new HashSet<Bitangent>(); // todo implement
		}

		private void GenerateSurfingEdges()
		{
			
		}
		
		
		
		
		private void GenerateHuggingEdges()
		{
			
		}
	}
}