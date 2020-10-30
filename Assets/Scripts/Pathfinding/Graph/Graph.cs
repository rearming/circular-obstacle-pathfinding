using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.Graph
{
	public class Graph<T> : IGraph<T> where T : IEquatable<T>
	{
		private List<Node<T>> Nodes = new List<Node<T>>();

		private Func<T, T, bool> contentEqualsComparerFunc;
		private Func<T, T, bool> ContentEqualsComparerFunc
		{
			get => contentEqualsComparerFunc;
			set
			{
				comparer = new ContentEqualityComparer(contentEqualsComparerFunc);
				contentEqualsComparerFunc = value;
			}
		}

		private ContentEqualityComparer comparer; // for collections that require IEqualityComparer (for ex. HashSet)

		public Graph() { }

		public void SetContentEqualsComparer(Func<T, T, bool> c) => ContentEqualsComparerFunc = c;

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
			Nodes.Remove(n); // then remove node itself
		}

		public void ConnectNodes(Node<T> node1, Node<T> node2, float cost = 1, object info = null)
		{
			if (!FindNode(node1, out node1) || !FindNode(node2, out node2))
				return;
			
			var nodeWithEdge1 = new NodeWithEdge<T>(node1, cost, info);
			var nodeWithEdge2 = new NodeWithEdge<T>(node2, cost, info);

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
			return current.links.Find(edge => edge.node.Equals(next)).graphEdge.GetCost();
		}

		private bool NodesEquals(Node<T> node1, Node<T> node2)
		{
			if (ContentEqualsComparerFunc == null)
				return node1.Content.Equals(node2.Content);
			return ContentEqualsComparerFunc(node1.Content, node2.Content);
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
}