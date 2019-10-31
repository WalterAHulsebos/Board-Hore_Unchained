using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour
{
	private Button previousButton;

	[SerializeField]
	private float scaleAmount = 1.4f;

	[SerializeField]
	private GameObject defaultButton;

	public ButtonHighlighter()
	{
	}

	private void HighlightButton(Button butt)
	{
		butt.transform.localScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
	}

	private void OnDisable()
	{
		if (previousButton != null)
		{
			UnHighlightButton(previousButton);
		}
	}

	private void Start()
	{
		if (defaultButton != null)
		{
			EventSystem.current.SetSelectedGameObject(defaultButton);
		}
	}

	private void UnHighlightButton(Button butt)
	{
		butt.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void Update()
	{
		GameObject gameObject = EventSystem.current.currentSelectedGameObject;
		if (gameObject == null)
		{
			return;
		}
		Button component = gameObject.GetComponent<Button>();
		if (component != null && component != previousButton && component.transform.name != "PauseButton")
		{
			HighlightButton(component);
		}
		if (previousButton != null && previousButton != component)
		{
			UnHighlightButton(previousButton);
		}
		previousButton = component;
	}
}