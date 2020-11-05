using System.Collections.Generic;
using System.Linq;
using Pathfinding.Algorithms;
using Pathfinding.Graph;
using UnityEngine;

namespace TestScripts
{
	public class TestPathfind : MonoBehaviour
	{
		[TextArea(10, 30)] [SerializeField] private string rawMap;

		[TextArea(10, 30)] [SerializeField] private string mapWithPathInspector;

		[SerializeField] private char start = 'S';
		[SerializeField] private char end = 'E';
		[SerializeField] private char empty = '*';
		[SerializeField] private char obstacle = '#';

		[SerializeField] private char pathChar = '$';

		private Node<Vector2Int> _endNode;

		private readonly Graph<Vector2Int> _graph = new Graph<Vector2Int>();

		private char[,] _map;
		private char[][] _mapWithPath;
		private Node<Vector2Int> _startNode;

		private void Start()
		{
			GetMap();
			GetNodes();

			// graph.ConnectAllNodes(ConnectGridNode8);
			_graph.ConnectAllNodes(ConnectGridNode4);

			PathfindTest();
		}

		private void GetMap()
		{
			var splitMap = rawMap.Split('\n');
			var maxWidth = splitMap.Max(s => s.Length);
			var maxHeight = splitMap.Length;

			splitMap = splitMap.Select(s => s.PadRight(maxWidth, obstacle)).ToArray();
			rawMap = string.Join("\n", splitMap); // correct in inspector

			_map = new char[maxHeight, maxWidth];
			for (var y = 0; y < maxHeight; y++)
			for (var x = 0; x < maxWidth; x++)
				_map[y, x] = splitMap[y][x];
		}

		private void GetNodes()
		{
			for (var y = 0; y < _map.GetLength(0); y++)
			for (var x = 0; x < _map.GetLength(1); x++)
			{
				if (_map[y, x] == start)
					_startNode = new Node<Vector2Int>(new Vector2Int(x, y));
				if (_map[y, x] == end)
					_endNode = new Node<Vector2Int>(new Vector2Int(x, y));
				if (_map[y, x] != obstacle)
					_graph.AddNode(new Vector2Int(x, y));
			}
		}

		private void ConnectGridNode8(Node<Vector2Int> node)
		{
			for (var y = node.Content.y - 1; y <= node.Content.y + 1; y++)
			for (var x = node.Content.x - 1; x <= node.Content.x + 1; x++)
			{
				if (x == node.Content.x && y == node.Content.y)
					continue;
				_graph.ConnectNodes(node, new Vector2Int(x, y));
			}
		}

		private void ConnectGridNode4(Node<Vector2Int> node)
		{
			var n = node.Content;
			_graph.ConnectNodes(node, new Vector2Int(n.x - 1, n.y));
			_graph.ConnectNodes(node, new Vector2Int(n.x + 1, n.y));
			_graph.ConnectNodes(node, new Vector2Int(n.x, n.y - 1));
			_graph.ConnectNodes(node, new Vector2Int(n.x, n.y + 1));
		}

		private void PathfindTest()
		{
			var aStar = new AStar<Vector2Int>(_graph, AStarHeuristic<Vector2Int>.ManhattanDistanceInt);

			aStar.SetGoal(_endNode);
			aStar.SetStart(_startNode);
			aStar.FindPath();
			var path = aStar.GetPath();

			PrintPath(path);
		}

		private void PrintPath(List<NodeWithEdge<Vector2Int>> path)
		{
			_mapWithPath = new char[_map.GetLength(0)][];

			for (var y = 0; y < _map.GetLength(0); y++)
			{
				_mapWithPath[y] = new char[_map.GetLength(1)];
				for (var x = 0; x < _map.GetLength(1); x++) _mapWithPath[y][x] = _map[y, x];
			}

			foreach (var node in path)
			{
				if (_mapWithPath[node.node.Content.y][node.node.Content.x] == start
				    || _mapWithPath[node.node.Content.y][node.node.Content.x] == end)
					continue;

				_mapWithPath[node.node.Content.y][node.node.Content.x] = pathChar;
			}

			mapWithPathInspector = string.Join("\n", _mapWithPath.Select(s => new string(s)));
		}
	}
}