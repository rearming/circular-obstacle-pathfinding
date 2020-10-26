using System;
using System.Collections.Generic;

namespace Pathfinding.Graph
{
	public class Node<T> where T : IEquatable<T>
	{
		public T Content { get; }
		
		public readonly List<NodeWithEdge<T>> links;

		public Node(T content)
		{
			links = new List<NodeWithEdge<T>>();
			this.Content = content;
		}

		public static implicit operator Node<T>(T cont) => new Node<T>(cont);

		public override string ToString() => Content.ToString();
		
		public override bool Equals(object obj) => Content.Equals(((Node<T>) obj).Content);
		
		public override int GetHashCode() => Content.GetHashCode();
	}
}