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

		public float Speed => _movement.Speed;
		
		private CapsuleCollider _capsuleCollider;
		private Movement _movement;
		
		private void Awake()
		{
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_movement = GetComponent<Movement>();
		}

		private void Start()
		{
			
		}
	}
}