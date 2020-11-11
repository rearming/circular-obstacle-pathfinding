using System.Collections.Generic;
using System.Linq;
using RVO;
using ScriptableObjects;
using UnityEngine;
using Utils;
using Vector2 = UnityEngine.Vector2;

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
			UpdatePositions();
			SetPreferredVelocities();
			Simulator.Instance.doStep();
		}

		private void SetPreferredVelocities()
		{
			for (var i = 0; i < Simulator.Instance.getNumAgents(); i++)
			{
				var dirToGoal = Vector2.zero;
				if (avoidanceAgents[i].Goal != null)
					dirToGoal = (Vector2.zero - Simulator.Instance.getAgentPosition(i).ToUnityVec2()).normalized;
				
				Simulator.Instance.setAgentPrefVelocity(i, dirToGoal.ToRVOVec2() * avoidanceAgents[i].Speed);
			}
		}

		private void UpdatePositions()
		{
			for (var i = 0; i < Simulator.Instance.getNumAgents(); i++)
			{
				avoidanceAgents[i].Move(Simulator.Instance.getAgentVelocity(i).ToUnityVec2());
			}
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