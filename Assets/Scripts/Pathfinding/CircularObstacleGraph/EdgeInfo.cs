using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	public class EdgeInfo
	{
		public readonly float arcAngle;
		public readonly List<Vector2> arcPoints;

		public EdgeInfo(float arcAngle, List<Vector2> arcPoints)
		{
			this.arcAngle = arcAngle;
			this.arcPoints = arcPoints;
		}
	}
}