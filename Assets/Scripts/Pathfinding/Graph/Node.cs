using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

namespace Pathfinding.Graph
{
	public class Node<T> where T : IEquatable<T>
	{
		public T Content { get; }
		
		public object Info { get; }

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
		
		public static implicit operator Node<T>(T cont) => new Node<T>(cont);

		public override string ToString() => Content.ToString();
		
		public override bool Equals(object obj) => Content.Equals(((Node<T>) obj).Content);
		
		public override int GetHashCode() => Content.GetHashCode();
	}
}