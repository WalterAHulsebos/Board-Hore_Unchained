using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectedChangeColor : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
{
	public Text text;

	private Color startColor;

	public UISelectedChangeColor()
	{
	}

	public void OnDeselect(BaseEventData eventData)
	{
		text.color = new Color(0.04705882f, 0.04705882f, 0.04705882f, 0.9215686f);
	}

	void UnityEngine.EventSystems.ISelectHandler.OnSelect(BaseEventData eventData)
	{
		startColor = text.color;
		text.color = new Color(0.007843138f, 0.4509804f, 1f, 1f);
		Debug.Log(string.Concat("text:", text.text, "  thisName:", gameObject.name));
	}
}