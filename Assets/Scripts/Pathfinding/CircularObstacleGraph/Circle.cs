using System;
using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	public readonly struct Circle
	{
		public readonly float radius;
		public readonly Vector2 center;
		public readonly object info;

		private const float PI2 = Mathf.PI * 2;

		public Circle(float radius, Vector2 center, object info = null)
		{
			this.radius = radius;
			this.center = center;
			this.info = info;
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

		public float CircumferenceLength => PI2 * radius;

		/// <param name="arcAngle">degrees</param>
		/// <returns></returns>
		public float ArcLength(float arcAngle)
		{
			var circumferenceFraction = arcAngle / 360f;
			return CircumferenceLength * circumferenceFraction;
		}

		public bool TryGetInfo(out object oInfo)
		{
			oInfo = info;
			return oInfo != null;
		}

		public static bool operator ==(Circle c1, Circle c2)
		{
			return c1.center == c2.center && Math.Abs(c1.radius - c2.radius) < 0.001f;
		}

		public static bool operator !=(Circle c1, Circle c2)
		{
			return !(c1 == c2);
		}

		public override string ToString()
		{
			return $"Radius: [{radius.ToString()}], Center: [{center.ToString()}]";
		}

		public override bool Equals(object obj)
		{
			return obj is Circle circle && circle.center == center && Math.Abs(circle.radius - radius) < 0.001f;
		}

		public override int GetHashCode()
		{
			return new {Center = center, Radius = radius}.GetHashCode();
		}
	}
}