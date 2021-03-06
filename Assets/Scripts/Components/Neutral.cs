﻿using Components.Interfaces;
using UnityEngine;
using Utils;

namespace Components
{
	public class Neutral : MonoBehaviour, IMovementAgent
	{
		[SerializeField] private Color selectionColor = Color.green;

		private Color _baseColor;

		public Movement movement;
		public Material Material { get; private set; }

		public Vector2 StartPos { get; set; }
		public Vector2? Goal { get; private set; }
		
		public Vector2? GetGoal() => Goal;
		public float GetSpeed() => movement.Speed;

		private CircularPathfinder _pathfinder;
		
		private void Awake()
		{
			Material = GetComponentInChildren<Renderer>().material;
			movement = GetComponent<Movement>();
			_pathfinder = GetComponent<CircularPathfinder>();
		}

		private void Start()
		{
			_baseColor = Material.color;
		}

		private void Update()
		{
			if (Goal != null && !_pathfinder.IsRealNull())
				movement.MoveTowards(_pathfinder.GetNextPos());
		}
		
		public void OnSelect() => Material.color = selectionColor;

		public void OnDeselect() => Material.color = _baseColor;

		public void SetStart() => StartPos = transform.position.ToVec2();

		public void SetStart(Vector2 startPos) => StartPos = startPos;

		public void SetGoal(Vector3 target) => SetGoal(target.ToVec2());

		public void SetGoal(Vector2 target)
		{
			Goal = target;
			StartPos = transform.position.ToVec2();
			
			if (!_pathfinder.IsRealNull())
				_pathfinder.StartPathfing();
		}

		public void UnsetGoal()
		{
			Goal = null;
		}

		public void Stop()
		{
			movement.Stop();
		}
	}
}