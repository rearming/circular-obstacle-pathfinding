using Utils;

namespace Pathfinding.Graph
{
	public readonly struct GraphEdge
	{
		private readonly float _cost;

		public readonly object info;

		public GraphEdge(float cost, object info = null)
		{
			this._cost = cost;
			this.info = info;
		}

		public static implicit operator GraphEdge(float cost)
		{
			return new GraphEdge(cost);
		}

		public float GetCost()
		{
			return _cost;
		}

		public static bool operator ==(GraphEdge edge1, GraphEdge edge2)
		{
			return edge1._cost.AlmostEqual(edge2._cost, 0.05f);
		}

		public static bool operator !=(GraphEdge edge1, GraphEdge edge2)
		{
			return !(edge1 == edge2);
		}

		public bool Equals(GraphEdge other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return obj is GraphEdge other && Equals(other);
		}

		public override int GetHashCode()
		{
			return _cost.GetHashCode();
		}
	}
}