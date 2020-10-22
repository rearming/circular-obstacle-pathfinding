using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace Pathfinding
{
	public class AStar<T> where T : IEquatable<T>
	{
		private Node<T> start;
		private Node<T> goal;
		private readonly Func<Node<T>, Node<T>, int> heuristic;
		
		private readonly SimplePriorityQueue<Node<T>> frontier = new SimplePriorityQueue<Node<T>>();
		private readonly Dictionary<Node<T>, Node<T>> cameFrom = new Dictionary<Node<T>, Node<T>>();
		private readonly Dictionary<Node<T>, float> costSoFar = new Dictionary<Node<T>, float>();

		private readonly IGraph<T> graph;

		public AStar(IGraph<T> graph, Func<Node<T>, Node<T>, int> heuristic)
		{
			this.graph = graph;
			this.heuristic = heuristic;
		}

		public void SetStart(Node<T> s)
		{
			if (graph.FindNode(s, out var node))
				start = node;
			else
				Debug.LogError($"[{nameof(AStar<T>)}] There is no start [{s}] node in the graph!");
		}
		
		public void SetGoal(Node<T> g)
		{
			if (graph.FindNode(g, out var node))
				goal = node;
			else
			{
				Debug.LogError($"[{nameof(AStar<T>)}] There is no goal [{g}] node in the graph.");
			}
		}
		
		public void FindPath()
		{
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
						var priority = newCost + heuristic(goal, next);
						frontier.Enqueue(next, priority);
						cameFrom[next] = current;
					}
				}
			}
		}

		public List<Node<T>> GetPath()
		{
			var current = goal;
			var path = new List<Node<T>>();

			while (!Equals(current, start))
			{
				path.Add(current);
				if (!cameFrom.TryGetValue(current, out var newCurrent))
				{
					Debug.LogWarning($"[{nameof(AStar<T>)}] Path incomplete!");
					break;
				}
				current = newCurrent;
			}
			path.Add(start);
			path.Reverse();
			return path;
		}
	}
}