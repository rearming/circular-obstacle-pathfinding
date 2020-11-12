using System;
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
		[SerializeField] private CollisionAvoidanceSimulationSpec simulationSpec;
		[SerializeField] private List<CollisionAvoidanceAgent> avoidanceAgents;

		private List<(Movement, CapsuleCollider)> _players;
		
		#region Debug
		
		private enum MovementType
		{
			Position,
			Velocity
		}

		[Header("Debug Fields")]
		[SerializeField] private MovementType movementType;

		#endregion

		private void Awake()
		{
			_players = GameObject
				.FindGameObjectsWithTag("Player")
				.Select(go => (go.GetComponent<Movement>(), go.GetComponent<CapsuleCollider>()))
				.ToList();
		}

		private void Start()
		{
			SetupSimulation();
		}

		private void SetupSimulation()
		{
			Simulator.Instance.Clear();
			foreach (var agent in avoidanceAgents)
				Simulator.Instance.AddUnityAgent(agent);
			AddPlayers();
		}

		private void AddPlayers()
		{
			foreach (var (movement, capsuleCollider) in _players)
			{
				Simulator.Instance.AddUnityAgent(
					movement.transform.position, 
					capsuleCollider.radius, 
					movement.Speed,
					simulationSpec.DefaultPlayerSpec);
			}
		}

		private void FixedUpdate()
		{
			UpdatePositions();
			UpdatePlayersPositions();
			SetPreferredVelocities();
			UpdateGoals();
			Simulator.Instance.setTimeStep(Time.deltaTime);
			Simulator.Instance.doStep();
		}

		private void UpdatePositions()
		{
			ForEachAIAgent(i =>
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
					default:
						throw new ArgumentOutOfRangeException();
				}
			});
		}

		private void UpdatePlayersPositions()
		{
			ForEachPlayerAgent((simIdx, playerIdx) =>
			{
				Simulator.Instance.setAgentPosition(simIdx, _players[playerIdx].Item1.transform.position.ToRVOVec2());
			});
		}
		
		private void SetPreferredVelocities()
		{
			ForEachAIAgent(i =>
			{
				var dirToGoal = Vector2.zero;
				if (avoidanceAgents[i].MovementAgent.GetGoal() != null)
					dirToGoal = (avoidanceAgents[i].MovementAgent.GetGoal().Value - 
					             Simulator.Instance.getAgentPosition(i).ToUnityVec2()).normalized;
				Simulator.Instance.setAgentPrefVelocity(i,
					dirToGoal.ToRVOVec2() * avoidanceAgents[i].MovementAgent.GetSpeed());
			});
		}

		private void UpdateGoals()
		{
			ForEachAIAgent(i =>
			{
				if (avoidanceAgents[i].MovementAgent.GetGoal() == null)
					return;
				if (Vector2.Distance(
					    avoidanceAgents[i].MovementAgent.GetGoal().Value,
					    Simulator.Instance.getAgentPosition(i).ToUnityVec2()) <= simulationSpec.GoalReachDistance)
					avoidanceAgents[i].MovementAgent.UnsetGoal();
			});
		}

		private void ForEachAIAgent(Action<int> action)
		{
			for (var i = 0; i < avoidanceAgents.Count; i++)
				action(i);
		}

		private void ForEachPlayerAgent(Action<int, int> action)
		{
			for (var i = avoidanceAgents.Count; i < Simulator.Instance.getNumAgents(); i++)
				action(i, i - avoidanceAgents.Count);
		}
		
		#region Editor Extensions

		[ContextMenu(nameof(AutoFillAvoidanceAgents))]
		private void AutoFillAvoidanceAgents()
		{
			avoidanceAgents = FindObjectsOfType<CollisionAvoidanceAgent>()
				.OrderBy(caa => caa.name)
				.ToList();
		}
		
		[ContextMenu(nameof(ReassignAgentsSettings))]
		private void ReassignAgentsSettings()
		{
			SetupSimulation();
		}

		#endregion
	}
}