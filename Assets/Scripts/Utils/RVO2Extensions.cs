using Components;

namespace Utils
{
	public static class RVO2Extensions
	{
		public static void AddUnityAgent(this RVO.Simulator simulator, CollisionAvoidanceAgent agent)
		{
			simulator.addAgent(agent.transform.position.ToRVOVec2(), agent.AgentSpec.NeighborsDistance,
				agent.AgentSpec.MaxNeighbors, agent.AgentSpec.TimeHorizon, agent.AgentSpec.TimeHorizonObstacles,
				agent.Radius, agent.Speed * agent.AgentSpec.MaxSpeedMultiplier, 
				agent.AgentSpec.InitialVelocity.ToRVOVec2());
		}

		public static RVO.Vector2 ToRVOVec2(this UnityEngine.Vector2 vec) => new RVO.Vector2(vec.x, vec.y);

		public static RVO.Vector2 ToRVOVec2(this UnityEngine.Vector3 vec) => new RVO.Vector2(vec.x, vec.y);

		public static UnityEngine.Vector2 ToUnityVec2(this RVO.Vector2 vec) => new UnityEngine.Vector2(vec.x(), vec.y());
	}
}