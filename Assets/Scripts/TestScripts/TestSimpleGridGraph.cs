using Pathfinding.Graph;
using UnityEngine;

namespace TestScripts
{
	public class TestSimpleGridGraph : MonoBehaviour
	{
		[SerializeField] private int width;
		[SerializeField] private int height;

		private readonly Graph<Vector2Int> _graph = new Graph<Vector2Int>();

		private void Start()
		{
			GenerateGridGraph();
			PrintGraph();
		}

		private void PrintGraph()
		{
			foreach (var node in _graph)
			{
				Debug.Log($"Node: [{node.Content.ToString()}]");
				foreach (var neighbor in _graph.Neighbors(node))
					Debug.Log($"Neighbor: [{neighbor.Content.ToString()}]]");
				Debug.Log("-------------------------------------------");
			}
		}

		private void GenerateGridGraph()
		{
			for (var y = 0; y < height; y++)
			for (var x = 0; x < width; x++)
			{
				var newNode = new Node<Vector2Int>(new Vector2Int(x, y));
				_graph.AddNode(newNode);
				ConnectGridNode(newNode.Content);
			}
		}

		private void ConnectGridNode(Vector2Int nodeContent)
		{
			for (var y = nodeContent.y - 1; y <= nodeContent.y + 1; y++)
			for (var x = nodeContent.x - 1; x <= nodeContent.x + 1; x++)
			{
				if (x < 0 || y < 0 || x >= width || y >= height)
					continue;
				if (x == nodeContent.x && y == nodeContent.y)
					continue;
				_graph.ConnectNodes(nodeContent, new Vector2Int(x, y));
			}
		}
	}
}