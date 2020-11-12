using RVO;
using UnityEngine;
using Utils;

namespace DebugDrawers
{
	public class CollisionAvoidanceDrawer : MonoBehaviour
	{
		[SerializeField] private float gizmosHeight = -3.5f;
		[SerializeField] private GizmosDrawingProperty preferredVelocityVectors;
		[SerializeField] private GizmosDrawingProperty realVelocityVectors;

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			
			if (preferredVelocityVectors) DrawPreferredVelocityVectors();
			if (realVelocityVectors) DrawRealVelocityVectors();
		}

		private void DrawPreferredVelocityVectors()
		{
			Gizmos.color = preferredVelocityVectors.color;
			foreach (var agent in Simulator.Instance.agents_)
			{
				Gizmos.DrawLine(agent.position_.ToUnityVec3(gizmosHeight), (agent.prefVelocity_ + agent.position_).ToUnityVec3(gizmosHeight));
			}
		}
		
		private void DrawRealVelocityVectors()
		{
			Gizmos.color = realVelocityVectors.color;
			foreach (var agent in Simulator.Instance.agents_)
			{
				Gizmos.DrawLine(agent.position_.ToUnityVec3(gizmosHeight), (agent.velocity_ + agent.position_).ToUnityVec3(gizmosHeight));
			}
		}
	}
}