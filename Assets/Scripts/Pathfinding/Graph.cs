using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
	public class Graph<T> : IGraph<T> where T : IEquatable<T>
	{
		private List<Node<T>> Nodes = new List<Node<T>>();
		private Func<T, T, bool> contentEqualsComparer;

		public Graph(List<Node<T>> nodes)
		{
			Nodes = nodes;
		}
		public Graph() { }

		public void SetContentEqualsComparer(Func<T, T, bool> c) => contentEqualsComparer = c;

		public void AddNode(Node<T> n)
		{
			Nodes.Add(n);
		}

		public void RemoveNode(Node<T> n)
		{
			foreach (var node in Nodes) // remove all connections
			{
				node.Links.RemoveAt(node.Links.FindIndex(nwe => nwe.Node == n));
			}
			Nodes.Remove(n);
		}

		public void ConnectNodes(Node<T> node1, Node<T> node2, float cost = 1)
		{
			var graphHode1 = Nodes.Find(n => NodesEquals(n, node1));
			var graphNode2 = Nodes.Find(n => NodesEquals(n, node2));
			
			if (graphHode1 == null || graphNode2 == null)
				return;

			graphHode1.Links.Add(new NodeWithEdge<T>(graphNode2, cost));
			graphNode2.Links.Add(new NodeWithEdge<T>(graphHode1, cost));
		}

		public void ConnectAllNodes(Action<Node<T>> connectorFunc)
		{
			foreach (var node in Nodes)
				connectorFunc(node);
		}

		public void Clear()
		{
			Nodes.Clear();
		}

		public bool FindNode(Node<T> node, out Node<T> result)
		{
			result = Nodes.Find(n => NodesEquals(n, node));
			return result != null;
		}

		public List<Node<T>> Neighbors(Node<T> current)
		{
			return current.Links.Select(nwe => nwe.Node).ToList();
		}
		
		public float Cost(Node<T> current, Node<T> next)
		{
			return current.Links.Find(node => node.Node == next).GraphEdge.Cost;
		}

		public bool NodesEquals(Node<T> node1, Node<T> node2)
		{
			if (contentEqualsComparer == null)
				return node1.Content.Equals(node2.Content);
			return contentEqualsComparer(node1.Content, node2.Content);
		}
		
		public IEnumerator<Node<T>> GetEnumerator()
		{
			return Nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class Node<T> where T : IEquatable<T>
	{
		public T Content { get; }
		
		public List<NodeWithEdge<T>> Links { get; private set; } = new List<NodeWithEdge<T>>();

		public Node(T content) => this.Content = content;

		private Node() { }
		
		public static implicit operator Node<T>(T cont) => new Node<T>(cont);

		public override string ToString() => Content.ToString();
		
		public override bool Equals(object obj) => Content.Equals(((Node<T>) obj).Content);
		
		public override int GetHashCode() => Content.GetHashCode();
	}

	public class NodeWithEdge<T> where T : IEquatable<T>
	{
		public GraphEdge GraphEdge { get; private set; }
		public Node<T> Node { get; private set; }

		public NodeWithEdge(Node<T> node, float cost)
		{
			GraphEdge = new GraphEdge(cost);
			Node = node;
		}
	}

	public class GraphEdge
	{
		public float Cost { get; set; }

		public GraphEdge(float cost) => Cost = cost;
		
		public static implicit operator GraphEdge(float cost) => new GraphEdge(cost);
	}
}