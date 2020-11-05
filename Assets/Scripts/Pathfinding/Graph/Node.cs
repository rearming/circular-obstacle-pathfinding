using System;
using System.Collections.Generic;

namespace Pathfinding.Graph
{
	public class Node<T> where T : IEquatable<T>
	{
		public readonly List<NodeWithEdge<T>> links;

		public Node(T content)
		{
			links = new List<NodeWithEdge<T>>();
			Content = content;
		}

		public Node(T content, object info)
		{
			links = new List<NodeWithEdge<T>>();
			Content = content;
			Info = info;
		}

		public T Content { get; set; }

		public object Info { get; }

		public static implicit operator Node<T>(T cont)
		{
			return new Node<T>(cont);
		}

		public override string ToString()
		{
			return Content.ToString();
		}

		public override bool Equals(object obj)
		{
			return Content.Equals(((Node<T>) obj).Content);
		}

		public override int GetHashCode()
		{
			return Content.GetHashCode();
		}
	}
}