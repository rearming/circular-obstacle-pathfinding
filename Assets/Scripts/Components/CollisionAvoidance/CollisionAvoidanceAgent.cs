using Components.Interfaces;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Components.CollisionAvoidance
{
	public class CollisionAvoidanceAgent : MonoBehaviour
	{
		[SerializeField] private CollisionAvoidaceAgentSpec agentSpec;
		public CollisionAvoidaceAgentSpec AgentSpec => agentSpec;

		[Tooltip("If no Capsule Collider assigned, using this value.")]
		[SerializeField] private float radius;
		public float Radius => _capsuleCollider.IsRealNull() ? radius : _capsuleCollider.radius;
		
		public IMovementAgent MovementAgent { get; private set; }

		private CapsuleCollider _capsuleCollider;
		private Movement _movement;
		
		private void Awake()
		{
			_capsuleCollider = GetComponent<CapsuleCollider>();
			MovementAgent = GetComponent<IMovementAgent>();
			_movement = GetComponent<Movement>();
		}

		public void Move(Vector2 velocity)
		{
			_movement.MoveVelocity(velocity);
		}
	}
}