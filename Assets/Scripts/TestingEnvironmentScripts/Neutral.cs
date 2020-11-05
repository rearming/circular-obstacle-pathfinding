﻿using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	public class Neutral : MonoBehaviour
	{
		[SerializeField] private Color selectionColor = Color.green;

		private Color _baseColor;

		private Movement _movement;
		private CircularPathfinder _pathfinder;
		public Material Material { get; private set; }

		public Vector2 StartPos { get; set; }
		public Vector2? Goal { get; private set; }

		private void Awake()
		{
			Material = GetComponent<Renderer>().material;
			_movement = GetComponent<Movement>();
			_pathfinder = GetComponent<CircularPathfinder>();
		}

		private void Start()
		{
			_baseColor = Material.color;
		}

		private void Update()
		{
			// if (Goal != null)
				// MoveTowards(_pathfinder.GetNextPos().ToVec3(transform.position.y));
		}

		public void OnSelect()
		{
			Material.color = selectionColor;
		}

		public void OnDeselect()
		{
			Material.color = _baseColor;
		}

		public void SetStart()
		{
			StartPos = transform.position.ToVec2();
		}

		public void SetStart(Vector2 startPos)
		{
			StartPos = startPos;
		}

		public void SetGoal(Vector3 target)
		{
			SetGoal(target.ToVec2());
		}

		public void SetGoal(Vector2 target)
		{
			Goal = target;
			StartPos = transform.position.ToVec2();
			_pathfinder.StartPathfing();
		}

		public void UnsetGoal()
		{
			Goal = null;
		}

		public void Stop()
		{
			_movement.Stop();
		}
	}
}