using System;

namespace Pathfinding.Graph
{
	public class NodeWithEdge<T> where T : IEquatable<T>
	{
		public readonly GraphEdge graphEdge;
		public readonly Node<T> node;

		public NodeWithEdge(Node<T> node, float cost)
		{
			graphEdge = new GraphEdge(cost);
			this.node = node;
		}
	}
}