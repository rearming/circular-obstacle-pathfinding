using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Utils;
using Object = System.Object;

namespace Pathfinding
{
	public class Graph<T> : IGraph<T> where T : IEquatable<T>
	{
		private List<Node<T>> Nodes = new List<Node<T>>();
		private Func<T, T, bool> contentEqualsComparerFunc;
		private ContentEqualityComparer comparer;

		public Graph()
		{
			comparer = new ContentEqualityComparer(contentEqualsComparerFunc);
		}

		public void SetContentEqualsComparer(Func<T, T, bool> c) => contentEqualsComparerFunc = c;

		public void AddNode(Node<T> n)
		{
			Nodes.Add(n);
		}

		public void RemoveNode(Node<T> n)
		{
			foreach (var node in Nodes) // remove all connections
			{
				node.links.RemoveAt(node.links.FindIndex(nwe => nwe.node == n));
			}
			Nodes.Remove(n);
		}

		public void ConnectNodes(Node<T> node1, Node<T> node2, float cost = 1)
		{
			if (!FindNode(node1, out node1) || !FindNode(node2, out node2))
				return;
			
			var nodeWithEdge1 = new NodeWithEdge<T>(node1, cost);
			var nodeWithEdge2 = new NodeWithEdge<T>(node2, cost);

			if (node1.links.Find(nwe => NodesWithEdgesEquals(nwe, nodeWithEdge2)) == null)
				node1.links.Add(nodeWithEdge2);
			if (node2.links.Find(nwe => NodesWithEdgesEquals(nwe, nodeWithEdge1)) == null)
				node2.links.Add(nodeWithEdge1);
		}

		public void ConnectAllNodes(Action<Node<T>> connectorFunc)
		{
			foreach (var node in Nodes)
				connectorFunc(node);
		}

		public void CleanupDisconnectedNodes()
		{
			Nodes.RemoveAll(n => n.links.Count == 0);
		}

		public void Clear()
		{
			Nodes.Clear();
		}

		public bool FindNode(Node<T> node, out Node<T> result)
		{
			var idx = Nodes.FindIndex(n => NodesEquals(n, node));
			result = idx != -1 ? Nodes[idx] : default;
			return idx != -1;
		}

		public List<Node<T>> Neighbors(Node<T> current)
		{
			return current.links.Select(nwe => nwe.node).ToList();
		}
		
		public float Cost(Node<T> current, Node<T> next)
		{
			return current.links.Find(edge => edge.node.Equals(next)).graphEdge.cost;
		}

		private bool NodesEquals(Node<T> node1, Node<T> node2)
		{
			if (contentEqualsComparerFunc == null)
				return node1.Content.Equals(node2.Content);
			return contentEqualsComparerFunc(node1.Content, node2.Content);
		}

		private bool NodesWithEdgesEquals(NodeWithEdge<T> nwe1, NodeWithEdge<T> nwe2)
		{
			return NodesEquals(nwe1.node.Content, nwe2.node.Content) && nwe1.graphEdge == nwe2.graphEdge;
		}
		
		public IEnumerator<Node<T>> GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public class ContentEqualityComparer : IEqualityComparer<Node<T>>
		{

			private Func<T, T, bool> comparer;
			public ContentEqualityComparer(Func<T, T, bool> comparerFunc)
			{
				comparer = comparerFunc;
			}
			
			public bool Equals(Node<T> x, Node<T> y)
			{
				return comparer(x.Content, y.Content);
			}

			public int GetHashCode(Node<T> obj)
			{
				return obj.Content.GetHashCode();
			}
		}
	}

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

	public readonly struct GraphEdge
	{
		public readonly float cost;

		public GraphEdge(float cost) => this.cost = cost;
		
		public static implicit operator GraphEdge(float cost) => new GraphEdge(cost);

		public static bool operator ==(GraphEdge edge1, GraphEdge edge2)
		{
			return edge1.cost.AlmostEqual(edge2.cost, 0.05f);
		}

		public static bool operator !=(GraphEdge edge1, GraphEdge edge2)
		{
			return !(edge1 == edge2);
		}
	}
}