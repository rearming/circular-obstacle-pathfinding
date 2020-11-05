using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	public class EdgeInfo
	{
		public readonly float arcAngle;
		public readonly IEnumerable<Vector2> middlePoints;

		public EdgeInfo(float arcAngle, IEnumerable<Vector2> middlePoints)
		{
			this.arcAngle = arcAngle;
			this.middlePoints = middlePoints;
		}
	}
}