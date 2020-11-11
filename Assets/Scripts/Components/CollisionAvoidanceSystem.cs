using System.Collections.Generic;
using System.Linq;
using RVO;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Components
{
	public class CollisionAvoidanceSystem : MonoBehaviour
	{
		[SerializeField] private ObstacleAvoidanceSimulationSpec simulationSpec;
		[SerializeField] private List<CollisionAvoidanceAgent> avoidanceAgents;
		
		private void Start()
		{
			Simulator.Instance.setTimeStep(simulationSpec.TimeStep);

			foreach (var agent in avoidanceAgents)
				Simulator.Instance.AddUnityAgent(agent);
			
		}

		private void FixedUpdate()
		{
			
			Simulator.Instance.doStep();
		}

		#region Editor Extensions

		[ContextMenu(nameof(AutoFillAvoidanceAgents))]
		private void AutoFillAvoidanceAgents()
		{
			avoidanceAgents = FindObjectsOfType<CollisionAvoidanceAgent>()
				.OrderBy(caa => caa.name)
				.ToList();
		}

		#endregion
	}
}