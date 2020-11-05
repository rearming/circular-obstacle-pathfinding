using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	public readonly struct Edge
	{
		public readonly Vector2 a;
		public readonly Vector2 b;

		public readonly int circleAhash;
		public readonly int circleBhash;

		public readonly EdgeInfo edgeInfo;

		public Edge(Vector2 a, Vector2 b, int circleAhash, int circleBhash, EdgeInfo edgeInfo = null)
		{
			this.a = a;
			this.b = b;
			this.circleAhash = circleAhash;
			this.circleBhash = circleBhash;
			this.edgeInfo = edgeInfo;
		}

		public Edge(Vector2 a, Vector2 b, EdgeInfo edgeInfo = null)
		{
			this.a = a;
			this.b = b;
			this.edgeInfo = edgeInfo;
			circleAhash = -1;
			circleBhash = -1;
		}

		public override string ToString()
		{
			return $"a: [{a.ToString()}], b: [{b.ToString()}]";
		}

		public override bool Equals(object obj)
		{
			return obj is Edge bitangent && (bitangent.a == a && bitangent.b == b ||
			                                 bitangent.a == b && bitangent.b == a);
		}

		public override int GetHashCode()
		{
			return unchecked(a.GetHashCode() + b.GetHashCode());
		}
	}
}