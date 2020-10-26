using System;
using Pathfinding.Graph;
using UnityEngine;

namespace Pathfinding.Algorithms
{
	public class AStarHeuristic<T> where T : IEquatable<T>
	{
		public readonly Func<Node<T>, Node<T>, float> func;
		
		public AStarHeuristic(Func<Node<T>, Node<T>, float> heuristic)
		{
			func = heuristic;
		}
		
		public static AStarHeuristic<Vector2> DijkstraHeuristic => new AStarHeuristic<Vector2>((goal, next) => 0f);
		
		public static AStarHeuristic<Vector2> EuclideanDistance => new AStarHeuristic<Vector2>((goal, next) =>
		{
			const float D = 1f;
			var dx = Mathf.Abs(next.Content.x - goal.Content.x);
			var dy = Mathf.Abs(next.Content.y - goal.Content.y);
			return D * Mathf.Sqrt(dx * dx + dy * dy);
		});
		
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