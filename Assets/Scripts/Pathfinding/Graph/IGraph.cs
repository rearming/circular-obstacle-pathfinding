using System;
using System.Collections.Generic;

namespace Pathfinding.Graph
{         
	public interface IGraph<T> : IEnumerable<Node<T>> where T : IEquatable<T>
	{
		void AddNode(Node<T> n);
		void RemoveNode(Node<T> n);
		void ConnectNodes(Node<T> node1, Node<T> node2, float cost = 1, object info = null);
		void ConnectAllNodes(Action<Node<T>> connectorFunc);
		/// <summary>
		/// Removes all nodes with 0 links to others.
		/// </summary>
		void CleanupDisconnectedNodes();

		Node<T> Closest(T place, Func<T, T, float> cmpFunc, Func<Node<T>, bool> ignoreNodeFunc = null);

		void SetContentEqualsComparer(Func<T, T, bool> comparer);
		void Clear();

		bool FindNode(Node<T> node, out Node<T> result);
		float Cost(Node<T> current, Node<T> next);
		List<Node<T>> Neighbors(Node<T> current);
	}
}