using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	public class EdgeInfo
	{
		public readonly float arcAngle;
		public readonly List<Vector2> arcPoints;
		public readonly Transform circleOwner;

		public EdgeInfo(float arcAngle, List<Vector2> arcPoints, Transform circleOwner, object otherInfo = null)
		{
			this.arcAngle = arcAngle;
			this.arcPoints = arcPoints;
			this.circleOwner = circleOwner;
		}
	}
}