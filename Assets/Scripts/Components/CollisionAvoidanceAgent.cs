using Components.Interfaces;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Components
{
	public class CollisionAvoidanceAgent : MonoBehaviour
	{
		[SerializeField] private ObstacleAvoidaceAgentSpec agentSpec;
		public ObstacleAvoidaceAgentSpec AgentSpec => agentSpec;

		[Tooltip("If no capsule collider assigned, using this value.")]
		[SerializeField] private float radius;
		public float Radius => _capsuleCollider.IsRealNull() ? radius : _capsuleCollider.radius;

		public float Speed => _movementAgent.GetSpeed();

		public Vector2? Goal => _movementAgent.GetGoal();
		
		private CapsuleCollider _capsuleCollider;
		private IMovementAgent _movementAgent;
		private Movement _movement;
		
		private void Awake()
		{
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_movementAgent = GetComponent<IMovementAgent>();
			_movement = GetComponent<Movement>();
		}

		public void Move(Vector2 velocity)
		{
			_movement.MoveVelocity(velocity);
		}
	}
}