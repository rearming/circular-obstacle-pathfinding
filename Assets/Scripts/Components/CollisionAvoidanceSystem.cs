using System.Collections.Generic;
using System.Linq;
using RVO;
using ScriptableObjects;
using UnityEngine;
using Utils;
using Vector2 = UnityEngine.Vector2;
// ReSharper disable PossibleInvalidOperationException

namespace Components
{
	public class CollisionAvoidanceSystem : MonoBehaviour
	{
		[SerializeField] private ObstacleAvoidanceSimulationSpec simulationSpec;
		[SerializeField] private List<CollisionAvoidanceAgent> avoidanceAgents;

		#region Debug
		
		private enum MovementType
		{
			Position,
			Velocity
		}

		[Header("Debug Fields")]
		[SerializeField] private MovementType movementType;

		#endregion

		private void Start()
		{
			foreach (var agent in avoidanceAgents)
				Simulator.Instance.AddUnityAgent(agent);
		}

		private void FixedUpdate()
		{
			UpdatePositions();
			SetPreferredVelocities();
			UpdateGoals();
			Simulator.Instance.setTimeStep(Time.deltaTime);
			Simulator.Instance.doStep();
		}

		private void SetPreferredVelocities()
		{
			for (var i = 0; i < Simulator.Instance.getNumAgents(); i++)
			{
				var dirToGoal = Vector2.zero;
				if (avoidanceAgents[i].MovementAgent.GetGoal() != null)
					dirToGoal = (avoidanceAgents[i].MovementAgent.GetGoal().Value - Simulator.Instance.getAgentPosition(i).ToUnityVec2()).normalized;
				
				Simulator.Instance.setAgentPrefVelocity(i, dirToGoal.ToRVOVec2() * avoidanceAgents[i].MovementAgent.GetSpeed());
			}
		}

		private void UpdatePositions()
		{
			for (var i = 0; i < Simulator.Instance.getNumAgents(); i++)
			{
				switch (movementType)
				{
					case MovementType.Velocity:
						avoidanceAgents[i].Move(Simulator.Instance.getAgentVelocity(i).ToUnityVec2());
						break;
					case MovementType.Position:
						var p = avoidanceAgents[i].transform.position;
						avoidanceAgents[i].transform.position = Simulator.Instance.getAgentPosition(i).ToUnityVec3(p.y);
						break;
				}
			}
		}

		private void UpdateGoals()
		{
			for (var i = 0; i < Simulator.Instance.getNumAgents(); i++)
			{
				if (avoidanceAgents[i].MovementAgent.GetGoal() == null)
					continue;
				if (Vector2.Distance(avoidanceAgents[i].MovementAgent.GetGoal().Value, Simulator.Instance.getAgentPosition(i).ToUnityVec2()) <=
				    simulationSpec.GoalReachDistance)
					avoidanceAgents[i].MovementAgent.UnsetGoal();
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