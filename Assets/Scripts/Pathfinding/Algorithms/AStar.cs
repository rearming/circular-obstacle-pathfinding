using System;
using System.Collections.Generic;
using Pathfinding.Graph;
using Plugins.Priority_Queue;

namespace Pathfinding.Algorithms
{
	public class AStar<T> where T : IEquatable<T>
	{
		public List<NodeWithEdge<T>> Path { get; private set; }
		
		private readonly Dictionary<Node<T>, NodeWithEdge<T>> _cameFrom = new Dictionary<Node<T>, NodeWithEdge<T>>();
		private readonly Dictionary<Node<T>, float> _costSoFar = new Dictionary<Node<T>, float>();

		private readonly SimplePriorityQueue<Node<T>> _frontier = new SimplePriorityQueue<Node<T>>();

		private readonly IGraph<T> _graph;
		private Node<T> _goal;
		private AStarHeuristic<T> _heuristic;
		private Node<T> _start;

		public AStar(IGraph<T> graph, AStarHeuristic<T> heuristic)
		{
			this._graph = graph;
			this._heuristic = heuristic;
		}

		public void SetStart(Node<T> s)
		{
			if (_graph.FindNode(s, out var node))
				_start = node;
			else
				throw new ArgumentException($"[{nameof(AStar<T>)}] There is no start [{s}] node in the graph!");
		}

		public void SetGoal(Node<T> g)
		{
			if (_graph.FindNode(g, out var node))
				_goal = node;
			else
				throw new ArgumentException($"[{nameof(AStar<T>)}] There is no goal [{g}] node in the graph.");
		}

		public void SetHeuristic(AStarHeuristic<T> h)
		{
			_heuristic = h;
		}

		private void Cleanup()
		{
			_frontier.Clear();
			_cameFrom.Clear();
			_costSoFar.Clear();
			_graph.CleanupDisconnectedNodes();
		}

		public void FindPath()
		{
			Cleanup();

			_frontier.Enqueue(_start, 0);
			_cameFrom[_start] = null;
			_costSoFar[_start] = 0;

			while (_frontier.Count > 0)
			{
				var current = _frontier.Dequeue();
				if (current.Content.Equals(_goal.Content))
					break;
				foreach (var next in _graph.Neighbors(current))
				{
					var newCost = _costSoFar[current] + _graph.Cost(current, next);
					if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
					{
						_costSoFar[next] = newCost;
						var priority = newCost + _heuristic.func(_goal, next);
						_frontier.Enqueue(next, priority);
						_cameFrom[next] = next.links.Find(nwe => Equals(nwe.node, current));
					}
				}
			}

			Path = GetPath();
		}

		public List<NodeWithEdge<T>> GetPath()
		{
			var current = new NodeWithEdge<T>(_goal, -1);
			var path = new List<NodeWithEdge<T>>();

			for (var i = 0;; i++)
			{
				if (Equals(current.node, _start))
					break;
				path.Add(current);
				if (!_cameFrom.TryGetValue(current.node, out var next))
					throw new IncompletePathException("Incomplete path. Goal wasn't reached.", i);
				current = next;
			}

			path.Add(current);
			path.Reverse();
			if (path.Count < 2)
				throw new SmallPathException("Too small path. Path contains less than 2 nodes.");
			return path;
		}
	}
}