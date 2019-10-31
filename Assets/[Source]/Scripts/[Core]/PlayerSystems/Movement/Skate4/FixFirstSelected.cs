using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixFirstSelected : MonoBehaviour
{
	public GameObject selected;

	private EventSystem es;

	private bool doSet;

	public FixFirstSelected()
	{
	}

	private void OnDisable()
	{
		if (es.currentSelectedGameObject != null)
		{
			selected = es.currentSelectedGameObject;
		}
	}

	private void OnEnable()
	{
		es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		es.SetSelectedGameObject(null);
		if (selected == null)
		{
			selected = es.firstSelectedGameObject;
		}
		doSet = true;
	}

	private void Update()
	{
		if (doSet)
		{
			es.SetSelectedGameObject(selected);
			Debug.Log(selected.name);
		}
		doSet = false;
	}
}