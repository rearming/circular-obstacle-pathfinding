using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
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
			obj is Edge bitangent && (bitangent.a == a && bitangent.b == b ||
			                          bitangent.a == b && bitangent.b == a);

		public override int GetHashCode() => unchecked(a.GetHashCode() + b.GetHashCode());
	}
}