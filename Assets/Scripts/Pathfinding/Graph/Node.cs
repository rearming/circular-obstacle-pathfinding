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
		
		// public static bool operator ==(Node<T> node1, Node<T> node2)
		// {
		// 	return true;
		// }
		//
		// public static bool operator !=(Node<T> node1, Node<T> node2)
		// {
		// 	return !(node1 == node2);
		// }
		//
		public override bool Equals(object obj) => Content.Equals(((Node<T>) obj).Content);
		
		public override int GetHashCode() => Content.GetHashCode();
	}
}