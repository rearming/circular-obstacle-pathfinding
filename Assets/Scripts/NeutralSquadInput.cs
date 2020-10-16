using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;


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
	[SerializeField] private KeyCode stopMovement = KeyCode.Space;

	private Camera cam;

	private NeutralComponent[] allNeutrals;
	private readonly HashSet<NeutralComponent> selectedNeutrals = new HashSet<NeutralComponent>();

	private void Awake()
	{
		cam = Camera.main;
	}

	private void Start()
	{
		allNeutrals = FindObjectsOfType<NeutralComponent>();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown((int)selectButton))
			SelectNeutral();
		if (Input.GetMouseButtonDown((int)moveButton))
			MoveNeutrals();
		if (Input.GetKeyDown(stopMovement))
			StopNeutrals();
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
		selectedNeutrals.ForEach(n => n.MoveTowards(pos));
	}

	private void StopNeutrals()
	{
		selectedNeutrals.ForEach(n => n.SetMovement(Vector2.zero));
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