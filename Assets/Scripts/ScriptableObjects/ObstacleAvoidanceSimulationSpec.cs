using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "ObstacleAvoidanceSimulationSpec_1", menuName = "Specs/Obstacle Avoidance/Simulation", order = 0)]
	public class ObstacleAvoidanceSimulationSpec : ScriptableObject
	{
		[SerializeField] private float timeStep = 0.25f;
		public float TimeStep => timeStep;
		
		
	}
}