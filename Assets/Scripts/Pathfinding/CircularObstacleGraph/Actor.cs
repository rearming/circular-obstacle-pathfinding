using System;
using UnityEngine;

namespace Pathfinding.CircularObstacleGraph
{
	[Serializable]
	public class Actor
	{
		[Range(0f, 5f)] [SerializeField] private float radius;

		public Actor(float radius)
		{
			this.radius = radius;
		}

		private Actor()
		{
		}

		public float Radius
		{
			get => radius;
			set
			{
				if (value <= 0f)
					Debug.LogError("Actor radius cannot be less then zero!");
				radius = value;
			}
		}
	}
}