using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding.Circular_Obstacle_Graph;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace TestingEnvironmentScripts
{
	public class NeutralSquadInput : MonoBehaviour
	{
		[Serializable]
		public enum MouseButton
		{
			LeftButton = 0,
			RightButton = 1,
		}

		[SerializeField] private MouseButton selectButton;
		[SerializeField] private MouseButton moveButton;
		[SerializeField] private KeyCode toggleStopMovement = KeyCode.Space;

		private Camera cam;
		
		private (NeutralComponent, CircularPathfinderComponent) [] allNeutrals;
		private readonly HashSet<NeutralComponent> selectedNeutrals = new HashSet<NeutralComponent>();

		private void Awake()
		{
			cam = Camera.main;
		}

		private void Start()
		{
			allNeutrals = FindObjectsOfType<NeutralComponent>()
				.Select(nc => (nc, nc.GetComponent<CircularPathfinderComponent>()))
				.ToArray();
			StartCoroutine(ToggleStopNeutrals());
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown((int)selectButton))
				SelectNeutral();
			if (Input.GetMouseButtonDown((int)moveButton))
				MoveNeutrals();
		}

		private void SelectNeutral()
		{
			if (!RaycastMousePos(out var hit, out _))
				return;
			if (!hit.collider.gameObject.TryGetComponent<NeutralComponent>(out var neutral))
			{
				DeselectAll();
				return;
			}
			if (!selectedNeutrals.AddWithAction(neutral, n => n.OnSelect()))
				selectedNeutrals.RemoveWithAction(neutral, n => n.OnDeselect());
		}

		private void DeselectAll()
		{
			foreach (var neutral in selectedNeutrals) 
				neutral.OnDeselect();
			selectedNeutrals.Clear();
		}

		private void MoveNeutrals()
		{
			if (!RaycastMousePos(out _, out var pos))
				return;
			// selectedNeutrals.ForEach(n => n.MoveTowards(pos, null));
			selectedNeutrals.ForEach(n => n.SetGoal(pos));
		}

		private IEnumerator ToggleStopNeutrals()
		{
			var stopped = false;
			while (true)
			{
				if (Input.GetKeyDown(toggleStopMovement))
					stopped = !stopped;
				if (stopped)
					selectedNeutrals.ForEach(n => n.SetMovement(Vector2.zero));
				yield return null;
			}
		}

		private bool RaycastMousePos(out RaycastHit hit, out Vector3 pos)
		{
			pos = Vector3.zero;
			var ray = cam.ScreenPointToRay(Input.mousePosition);
			if (!Physics.Raycast(ray, out hit))
				return false;
			pos = hit.point;
			return true;
		}
	}
}