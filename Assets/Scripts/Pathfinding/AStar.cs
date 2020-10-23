using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Priority_Queue;
using UnityEngine;

namespace Pathfinding
{
	public class AStar<T> where T : IEquatable<T>
	{
		private Node<T> start;
		private Node<T> goal;
		private AStarHeuristic<T> heuristic;
		
		private readonly SimplePriorityQueue<Node<T>> frontier = new SimplePriorityQueue<Node<T>>();
		private readonly Dictionary<Node<T>, Node<T>> cameFrom = new Dictionary<Node<T>, Node<T>>();
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

	public class AStarHeuristic<T> where T : IEquatable<T>
	{

		public readonly Func<Node<T>, Node<T>, float> func;
		
		public AStarHeuristic(Func<Node<T>, Node<T>, float> heuristic)
		{
			func = heuristic;
		}

		public static AStarHeuristic<Vector2> ManhattanDistance => new AStarHeuristic<Vector2>((goal, next) =>
		{
			const float D = 1f;
			var dx = Mathf.Abs(next.Content.x - goal.Content.x);
			var dy = Mathf.Abs(next.Content.y - goal.Content.y);
			return D * dx * dy;
		});
		
		public static AStarHeuristic<Vector2Int> ManhattanDistanceInt => new AStarHeuristic<Vector2Int>((goal, next) =>
		{
			const float D = 1f;
			var dx = Mathf.Abs(next.Content.x - goal.Content.x);
			var dy = Mathf.Abs(next.Content.y - goal.Content.y);
			return D * dx * dy;
		});
		
		public static implicit operator Func<Node<T>, Node<T>, float> (AStarHeuristic<T> heuristic) => heuristic.func;
		
		private AStarHeuristic() { }
	}
}