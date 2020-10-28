using UnityEngine.EventSystems;
using Utils;

namespace Pathfinding.Graph
{
	public readonly struct GraphEdge
	{
		private readonly float cost;

		private readonly object info;
		
		public GraphEdge(float cost, object info = null)
		{
			this.cost = cost;
			this.info = info;
		}

		public static implicit operator GraphEdge(float cost) => new GraphEdge(cost);

		public float GetCost() => cost;

		public static bool operator ==(GraphEdge edge1, GraphEdge edge2)
		{
			return edge1.cost.AlmostEqual(edge2.cost, 0.05f);
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
			return cost.GetHashCode();
		}

	}
}