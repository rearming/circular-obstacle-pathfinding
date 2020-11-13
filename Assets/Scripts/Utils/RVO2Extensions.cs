using Components;
using Components.CollisionAvoidance;
using ScriptableObjects;
using UnityEngine;

namespace Utils
{
	public static class RVO2Extensions
	{
		public static void AddUnityAgent(this RVO.Simulator simulator, CollisionAvoidanceAgent agent)
		{
			var i = simulator.addAgent(agent.transform.position.ToRVOVec2(), agent.AgentSpec.NeighborsDistance,
				agent.AgentSpec.MaxNeighbors, agent.AgentSpec.TimeHorizon, agent.AgentSpec.TimeHorizonObstacles,
				agent.Radius, agent.AgentSpec.MaxSpeedMultiplier * agent.MovementAgent.GetSpeed(), 
				agent.AgentSpec.InitialVelocity.ToRVOVec2());
		}
		
		public static void AddUnityAgent(this RVO.Simulator simulator, Vector3 position, float radius, float speed, CollisionAvoidaceAgentSpec spec)
		{
			var i = simulator.addAgent(position.ToRVOVec2(), spec.NeighborsDistance,
				spec.MaxNeighbors, spec.TimeHorizon, spec.TimeHorizonObstacles,
				radius, speed * spec.MaxSpeedMultiplier, 
				spec.InitialVelocity.ToRVOVec2());
		}

		public static RVO.Vector2 ToRVOVec2(this UnityEngine.Vector2 vec) => new RVO.Vector2(vec.x, vec.y);

		public static RVO.Vector2 ToRVOVec2(this UnityEngine.Vector3 vec) => new RVO.Vector2(vec.x, vec.z);

		public static UnityEngine.Vector2 ToUnityVec2(this RVO.Vector2 vec) => new UnityEngine.Vector2(vec.x(), vec.y());
		
		public static UnityEngine.Vector3 ToUnityVec3(this RVO.Vector2 vec, float height) => new UnityEngine.Vector3(vec.x(), height, vec.y());
	}
}