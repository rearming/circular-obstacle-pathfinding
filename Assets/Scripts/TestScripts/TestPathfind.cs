using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Pathfinding.Algorithms;
using Pathfinding.Graph;
using UnityEngine;

namespace TestScripts
{
	public class TestPathfind : MonoBehaviour
	{
		[TextArea(10, 30)]
		[SerializeField] private string rawMap;

		[TextArea(10, 30)]
		[SerializeField] private string mapWithPathInspector;
		
		[SerializeField] private char start = 'S';
		[SerializeField] private char end = 'E';
		[SerializeField] private char empty = '*';
		[SerializeField] private char obstacle = '#';

		[SerializeField] private char pathChar = '$';

		private char[,] map;
		private char[][] mapWithPath;
		
		private Node<Vector2Int> endNode;
		private Node<Vector2Int> startNode;

		private Graph<Vector2Int> graph = new Graph<Vector2Int>();
		
		private void Start()
		{
			GetMap();
			GetNodes();
			
			// graph.ConnectAllNodes(ConnectGridNode8);
			graph.ConnectAllNodes(ConnectGridNode4);
			
			PathfindTest();
		}

		private void GetMap()
		{
			var splitMap = rawMap.Split('\n');
			var maxWidth = splitMap.Max(s => s.Length);
			var maxHeight = splitMap.Length;
			
			splitMap = splitMap.Select(s => s.PadRight(maxWidth, obstacle)).ToArray();
			rawMap = string.Join("\n", splitMap); // correct in inspector

			map = new char[maxHeight, maxWidth];
			for (int y = 0; y < maxHeight; y++)
			{
				for (int x = 0; x < maxWidth; x++)
					map[y, x] = splitMap[y][x];
			}
		}

		private void GetNodes()
		{
			for (int y = 0; y < map.GetLength(0); y++)
			{
				for (int x = 0; x < map.GetLength(1); x++)
				{
					if (map[y, x] == start)
						startNode = new Node<Vector2Int>(new Vector2Int(x, y));
					if (map[y, x] == end)
						endNode = new Node<Vector2Int>(new Vector2Int(x, y));
					if (map[y, x] != obstacle)
						graph.AddNode(new Vector2Int(x, y));
				}
			}
		}

		private void ConnectGridNode8(Node<Vector2Int> node)
		{
			for (int y = node.Content.y - 1; y <= node.Content.y + 1; y++)
			{
				for (int x = node.Content.x - 1; x <= node.Content.x + 1; x++)
				{
					if (x == node.Content.x && y == node.Content.y)
						continue;
					graph.ConnectNodes(node, new Vector2Int(x, y));
				}
			}
		}

		private void ConnectGridNode4(Node<Vector2Int> node)
		{
			var n = node.Content;
			graph.ConnectNodes(node, new Vector2Int(n.x - 1, n.y));
			graph.ConnectNodes(node, new Vector2Int(n.x + 1, n.y));
			graph.ConnectNodes(node, new Vector2Int(n.x, n.y - 1));
			graph.ConnectNodes(node, new Vector2Int(n.x, n.y + 1));
		}

		private void PathfindTest()
		{
			var AStar = new AStar<Vector2Int>(graph, AStarHeuristic<Vector2Int>.ManhattanDistanceInt);
			
			AStar.SetGoal(endNode);
			AStar.SetStart(startNode);
			AStar.FindPath();
			var path =	AStar.GetPath();
			
			PrintPath(path);
		}

		private void PrintPath(List<NodeWithEdge<Vector2Int>> path)
		{
			mapWithPath = new char[map.GetLength(0)][];

			for (int y = 0; y < map.GetLength(0); y++)
			{
				mapWithPath[y] = new char[map.GetLength(1)];
				for (int x = 0; x < map.GetLength(1); x++)
				{
					mapWithPath[y][x] = map[y, x];
				}
			}
			
			foreach (var node in path)
			{
				if (mapWithPath[node.node.Content.y][node.node.Content.x] == start 
				    || mapWithPath[node.node.Content.y][node.node.Content.x] == end)
					continue;
				
				mapWithPath[node.node.Content.y][node.node.Content.x] = pathChar;
			}

			mapWithPathInspector = string.Join("\n", mapWithPath.Select(s => new string(s)));
		}
	}
}