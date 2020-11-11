using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Obstacle Avoidance Agent_1", menuName = "Specs/Obstacle Avoidance/Agent", order = 0)]
	public class ObstacleAvoidaceAgentSpec : ScriptableObject
	{
		[Tooltip("The maximum distance (center point to center point) to other agents this agent takes into account in the navigation. " +
		         "The larger this number, the longer the running time of the simulation. " +
		         "If the number is too low, the simulation will not be safe. " +
		         "Must be non-negative.")]
		[SerializeField] private float neighborsDistance = 15f;
		public float NeighborsDistance => neighborsDistance;

		[Tooltip("The maximum number of other agents this agent takes into account in the navigation. " +
		         "The larger this number, the longer the running time of the simulation. " +
		         "If the number is too low, the simulation will not be safe.")]
		[SerializeField] private int maxNeighbors = 10;
		public int MaxNeighbors => maxNeighbors;

		[Tooltip("The minimal amount of time for which this agent's velocities that are computed " +
		         "by the simulation are safe with respect to other agents. " +
		         "The larger this number, the sooner this agent will respond to the presence of other agents, " +
		         "but the less freedom this agent has in choosing its velocities. Must be positive.")]
		[SerializeField] private float timeHorizon = 10f;
		public float TimeHorizon => timeHorizon;

		[Tooltip("The minimal amount of time for which this agent's velocities that are computed by the " +
		         "simulation are safe with respect to obstacles. " +
		         "The larger this number, the sooner this agent will respond to the presence of obstacles, " +
		         "but the less freedom this agent has in choosing its velocities. Must be positive.")]
		[SerializeField] private float timeHorizonObstacles = 10f;
		public float TimeHorizonObstacles => timeHorizonObstacles;

		[Tooltip("Agent can accelerate up to speed * maxSpeedMultiplier to avoid collision.")]
		[SerializeField] private float maxSpeedMultiplier = 1.5f;
		public float MaxSpeedMultiplier => maxSpeedMultiplier;

		[SerializeField] private Vector2 initialVelocity = Vector2.zero;
		public Vector2 InitialVelocity => initialVelocity;
	}
}