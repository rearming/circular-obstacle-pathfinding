using System.Collections.Generic;
using Pathfinding.Graph;
using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	public class CircularGraphExpander
	{
		/// <summary>
		///     All nodes except Start and Goal will be expanded from their circle's center by that value.
		/// </summary>
		public float ExpansionLength { get; set; }
		
		private readonly CircularObsticleGraphGenerator _graphGenerator;
		
		public CircularGraphExpander(CircularObsticleGraphGenerator graphGenerator, float expansionLength = 0.5f)
		{
			_graphGenerator = graphGenerator;
			ExpansionLength = expansionLength;
		}
		
		/// <summary>
		///     Expansion is needed to prevent inability of creating graph in case of small actor/obstacle overlap.
		///     For example, overlap can happen during hugging edge movement.
		/// </summary>
		public void ExpandPathPoints(List<NodeWithEdge<Vector2>> path)
		{
			for (var i = 0; i < path.Count; i++)
			{
				var nodeWithEdge = path[i];
				var node = nodeWithEdge.node;
				if (!(node.Info is int circleHash) || !_graphGenerator.Circles.TryGetValue(circleHash, out var circle))
					continue; // do not expand Start or Goal node

				var expansionDir = (node.Content - circle.center).normalized;
				node.Content += expansionDir * ExpansionLength; // expand all points a little from a circle

				if (nodeWithEdge.graphEdge.info == null || !(nodeWithEdge.graphEdge.info is EdgeInfo info)) 
					continue; // if node edge isn't hugging edge, continue

				for (var j = 0; j < info.arcPoints.Count; j++)
				{
					var p = info.arcPoints[j];
					p += (p - circle.center).normalized * ExpansionLength;
					info.arcPoints[j] = p;
				}
			}
		}
	}
}