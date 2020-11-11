using System;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	[RequireComponent(typeof(CapsuleCollider))]
	public class CircularObstacle : MonoBehaviour
	{
		private CapsuleCollider _capsuleCollider;
		public float Radius => _capsuleCollider.ScaledRadius();

		private void Awake()
		{
			_capsuleCollider = GetComponent<CapsuleCollider>();
		}
	}
}