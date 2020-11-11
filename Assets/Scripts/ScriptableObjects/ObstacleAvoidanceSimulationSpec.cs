using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "ObstacleAvoidanceSimulationSpec_1", menuName = "Specs/Obstacle Avoidance/Simulation", order = 0)]
	public class ObstacleAvoidanceSimulationSpec : ScriptableObject
	{
		[SerializeField] private float goalReachDistance = 0.1f;
		public float GoalReachDistance => goalReachDistance;
	}
}