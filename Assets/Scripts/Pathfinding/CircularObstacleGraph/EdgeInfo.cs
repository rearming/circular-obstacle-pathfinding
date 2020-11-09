using System.Collections.Generic;
using TestingEnvironmentScripts;
using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	public class EdgeInfo
	{
		public readonly float arcAngle;
		public readonly List<Vector2> arcPoints;
		public readonly Neutral obstacleInfo;

		public EdgeInfo(float arcAngle, List<Vector2> arcPoints, Neutral obstacleInfo)
		{
			this.arcAngle = arcAngle;
			this.arcPoints = arcPoints;
			this.obstacleInfo = obstacleInfo;
		}
	}
}