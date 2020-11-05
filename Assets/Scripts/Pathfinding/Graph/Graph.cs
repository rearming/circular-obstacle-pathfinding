using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.Graph
{
	public class Graph<T> : IGraph<T> where T : IEquatable<T>
	{
		private ContentEqualityComparer _comparer; // for collections that require IEqualityComparer (for ex. HashSet)

		private Func<T, T, bool> _contentEqualsComparerFunc;
		private readonly List<Node<T>> _nodes = new List<Node<T>>();

		private Func<T, T, bool> ContentEqualsComparerFunc
		{
			get => _contentEqualsComparerFunc;
			set
			{
				_comparer = new ContentEqualityComparer(_contentEqualsComparerFunc);
				_contentEqualsComparerFunc = value;
			}
		}

		public void SetContentEqualsComparer(Func<T, T, bool> c)
		{
			ContentEqualsComparerFunc = c;
		}

		public void AddNode(Node<T> n)
		{
			_nodes.Add(n);
		}

		public void RemoveNode(Node<T> n)
		{
			foreach (var node in _nodes) // remove all connections
				node.links.RemoveAt(node.links.FindIndex(nwe => nwe.node == n));
			_nodes.Remove(n); // then remove node itself
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
			foreach (var node in _nodes)
				connectorFunc(node);
		}

		public void CleanupDisconnectedNodes()
		{
			_nodes.RemoveAll(n => n.links.Count == 0);
		}

		public void Clear()
		{
			_nodes.Clear();
		}

		public bool FindNode(Node<T> node, out Node<T> result)
		{
			var idx = _nodes.FindIndex(n => NodesEquals(n, node));
			result = idx != -1 ? _nodes[idx] : default;
			return idx != -1;
		}

		public IEnumerable<Node<T>> Neighbors(Node<T> current)
		{
			return current.links.Select(nwe => nwe.node).ToList();
		}

		/// <summary>
		///     Finds closest to specified place node in the graph.
		/// </summary>
		/// <param name="place">Target place.</param>
		/// <param name="cmpFunc">Distance comparer.</param>
		/// <param name="ignoreNodeFunc">If this function returns true, node will be ignored.</param>
		/// <returns>Closest to specified place node in the graph.</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if Nodes.Count is less than one.</exception>
		public Node<T> Closest(T place, Func<T, T, float> cmpFunc, Func<Node<T>, bool> ignoreNodeFunc = null)
		{
			if (_nodes.Count < 1)
				throw new IndexOutOfRangeException("Can't find closest node. Nodes.Count < 1.");

			var closest = _nodes[0];
			for (var i = 1; i < _nodes.Count; i++)
			{
				if (ignoreNodeFunc != null && ignoreNodeFunc(_nodes[i]))
					continue;
				if (cmpFunc(place, closest.Content) > cmpFunc(place, _nodes[i].Content))
					closest = _nodes[i];
			}

			return closest;
		}

		public float Cost(Node<T> current, Node<T> next)
		{
			return current.links.Find(edge => edge.node.Equals(next)).graphEdge.GetCost();
		}

		public IEnumerator<Node<T>> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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

		public class ContentEqualityComparer : IEqualityComparer<Node<T>>
		{
			private readonly Func<T, T, bool> _comparer;

			public ContentEqualityComparer(Func<T, T, bool> comparerFunc)
			{
				_comparer = comparerFunc;
			}

			public bool Equals(Node<T> x, Node<T> y)
			{
				return _comparer(x.Content, y.Content);
			}

			public int GetHashCode(Node<T> obj)
			{
				return obj.Content.GetHashCode();
			}
		}
	}
}