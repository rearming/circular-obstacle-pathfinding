using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components.CollisionAvoidance
{
	public class CollisionAvoidanceOverlapper : MonoBehaviour
	{
		[SerializeField] private string obstacleTag = "Avoidance Obstacle";

		private BoxCollider _boxCollider;

		private int _collisionsExpected;
		private int _collisionsHappened;
		private readonly List<List<Vector3>> _contactPoints = new List<List<Vector3>>();
		private Action<List<List<Vector3>>> _contactPointsGetter;

		public void SetupCallbacks(Action<List<List<Vector3>>> contactPointsGetter)
		{
			_contactPointsGetter = contactPointsGetter;
		}
		
		public void Setup()
		{
			StartCoroutine(SetupRoutine());
		}
		
		private IEnumerator SetupRoutine()
		{
			_boxCollider = GetComponent<BoxCollider>();
			_boxCollider.enabled = false;
			yield return null;
			
			var rb = gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeAll;
			rb.useGravity = false;
			
			_contactPoints.Clear();
			_collisionsExpected = GameObject.FindGameObjectsWithTag(obstacleTag).Length;
			_collisionsHappened = 0;
			_boxCollider.enabled = true;
			
			yield return new WaitUntil(() => _collisionsHappened == _collisionsExpected);
			_contactPointsGetter.Invoke(_contactPoints);

			Destroy(GetComponent<Rigidbody>());
			_boxCollider.enabled = false;
		}

		private void OnCollisionEnter(Collision other)
		{
			if (!other.gameObject.CompareTag(obstacleTag))
				return;
			_contactPoints.Add(other.contacts.Select(c => c.point).ToList());
			_collisionsHappened++;
		}
	}
}