using System;
using System.Collections.Generic;
using Pathfinding.Graph;
using Plugins.Priority_Queue;

namespace Pathfinding.Algorithms
{
	public class AStar<T> where T : IEquatable<T>
	{
		private Node<T> start;
		private Node<T> goal;
		private AStarHeuristic<T> heuristic;
		
		private readonly SimplePriorityQueue<Node<T>> frontier = new SimplePriorityQueue<Node<T>>();
		private readonly Dictionary<Node<T>, NodeWithEdge<T>> cameFrom = new Dictionary<Node<T>, NodeWithEdge<T>>();
		private readonly Dictionary<Node<T>, float> costSoFar = new Dictionary<Node<T>, float>();

		private readonly IGraph<T> graph;

		public AStar(IGraph<T> graph, AStarHeuristic<T> heuristic)
		{
			this.graph = graph;
			this.heuristic = heuristic;
		}

		public void SetStart(Node<T> s)
		{
			if (graph.FindNode(s, out var node))
				start = node;
			else
				throw new ArgumentException($"[{nameof(AStar<T>)}] There is no start [{s}] node in the graph!");
		}
		
		public void SetGoal(Node<T> g)
		{
			if (graph.FindNode(g, out var node))
				goal = node;
			else
			{
				throw new ArgumentException($"[{nameof(AStar<T>)}] There is no goal [{g}] node in the graph.");
			}
		}

		public void SetHeuristic(AStarHeuristic<T> h) => heuristic = h;

		private void Cleanup()
		{
			frontier.Clear();
			cameFrom.Clear();
			costSoFar.Clear();
			graph.CleanupDisconnectedNodes();
		}
		
		public void FindPath()
		{
			Cleanup();
			
			frontier.Enqueue(start, 0);
			cameFrom[start] = null;
			costSoFar[start] = 0;

			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();
				if (current.Content.Equals(goal.Content))
					break;
				var neighbors = graph.Neighbors(current);
				foreach (var next in neighbors)
				{
					var newCost = costSoFar[current] + graph.Cost(current, next);
					if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
					{
						costSoFar[next] = newCost;
						var priority = newCost + heuristic.func(goal, next);
						frontier.Enqueue(next, priority);
						cameFrom[next] = next.links.Find(nwe => Equals(nwe.node, current));
					}
				}
			}
		}

		public List<NodeWithEdge<T>> GetPath()
		{
			var current = new NodeWithEdge<T>(goal, -1);
			var path = new List<NodeWithEdge<T>>();

			for (var i = 0; ; i++)
			{
				if (Equals(current.node, start))
					break;
				path.Add(current);
				if (!cameFrom.TryGetValue(current.node, out var next))
					throw new IncompletePathException($"Incomplete path. Goal wasn't reached.", i);
				current = next;
			}
			path.Add(current);
			path.Reverse();
			if (path.Count < 2)
				throw new SmallPathException($"Too small path. Path contains less than 2 nodes.");
			return path;
		}
	}
}