using System;
using System.Collections.Generic;
using Priority_Queue;

namespace Pathfinding
{
	// frontier = PriorityQueue()
	// frontier.put(start, 0)
	// came_from = dict()
	// cost_so_far = dict()
	// came_from[start] = None
	// cost_so_far[start] = 0
	//
	// while not frontier.empty():
	// current = frontier.get()
	//
	// if current == goal:
	// break
 //   
	// for next in graph.neighbors(current):
	// new_cost = cost_so_far[current] + graph.cost(current, next)
	// if next not in cost_so_far or new_cost < cost_so_far[next]:
	// cost_so_far[next] = new_cost
	// priority = new_cost + heuristic(goal, next)
	// frontier.put(next, priority)
	// came_from[next] = current
	
	public class AStar<T>
	{
		private Node<T> start;
		private Node<T> goal;
		private Func<Node<T>, Node<T>, int> heuristic;
		
		private SimplePriorityQueue<Node<T>> frontier = new SimplePriorityQueue<Node<T>>();
		private Dictionary<Node<T>, Node<T>> cameFrom = new Dictionary<Node<T>, Node<T>>();
		private Dictionary<Node<T>, int> costSoFar = new Dictionary<Node<T>, int>();

		private Graph<T> graph;

		public AStar(Graph<T> graph, Func<Node<T>, Node<T>, int> heuristic)
		{
			this.graph = graph;
			this.heuristic = heuristic;
		}

		public void SetStart(Node<T> start)
		{
			this.start = start;
		}
		
		public void SetGoal(Node<T> goal)
		{
			this.goal = goal;
		}
		
		public void FindPath()
		{
			frontier.Enqueue(start, 0);
			cameFrom[start] = null;
			costSoFar[start] = 0;

			while (frontier.Count > 0)
			{
				var current = frontier.Dequeue();
				if (current == goal)
					break;
				foreach (var next in graph.Neighbors(current))
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

			while (current != start)
			{
				path.Add(current);
				current = cameFrom[current];
			}
			path.Add(start);
			path.Reverse();
			return path;
		}
	}
}