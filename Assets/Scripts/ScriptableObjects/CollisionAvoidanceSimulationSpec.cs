using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "ObstacleAvoidanceSimulationSpec_1", menuName = "Specs/Obstacle Avoidance/Simulation", order = 0)]
	public class CollisionAvoidanceSimulationSpec : ScriptableObject
	{
		[SerializeField] private float goalReachDistance = 0.1f;
		public float GoalReachDistance => goalReachDistance;

		[SerializeField] private CollisionAvoidaceAgentSpec defaultPlayerSpec;
		public CollisionAvoidaceAgentSpec DefaultPlayerSpec => defaultPlayerSpec;
	}
}