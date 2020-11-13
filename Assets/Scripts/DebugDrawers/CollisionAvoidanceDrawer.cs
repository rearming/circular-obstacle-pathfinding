using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Components;
using Components.CollisionAvoidance;
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
		[SerializeField] private GizmosDrawingProperty staticObstacles;

		private CollisionAvoidanceStaticObstacles _staticObstacles;

		private void Awake()
		{
			_staticObstacles = GetComponent<CollisionAvoidanceStaticObstacles>();
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			
			if (preferredVelocityVectors) DrawPreferredVelocityVectors();
			if (realVelocityVectors) DrawRealVelocityVectors();
			if (staticObstacles) DrawStaticObstacles();
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

		private void DrawStaticObstacles()
		{
			if (Simulator.Instance.getNumObstacleVertices() == 0)
				return;
			Gizmos.color = staticObstacles.color;
			var obs = Simulator.Instance.obstacles_.First();
			var counter = 0;
			while (counter < _staticObstacles.Obstacles.Count)
			{
				var maxIdx = 0;
				var obsFirst = obs;
				while (true)
				{
					maxIdx = Math.Max(maxIdx, obs.id_);
					Gizmos.DrawLine(obs.point_.ToUnityVec3(gizmosHeight), obs.next_.point_.ToUnityVec3(gizmosHeight));
					obs = obs.next_;
					if (obs == obsFirst)
						break;
				}
				if (maxIdx + 1 >= Simulator.Instance.obstacles_.Count)
					break;
				obs = Simulator.Instance.obstacles_[maxIdx + 1];
				counter++;
			}
			
		}
	}
}