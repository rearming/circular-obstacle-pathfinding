using System;
using UnityEngine;

namespace Pathfinding.Circular_Obstacle_Graph
{
	[Serializable]
	public class Actor
	{
		public Actor(float radius) => this.radius = radius;

		[Range(0f, 5f)] [SerializeField]
		private float radius;
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

		private Actor() { }
	}
}