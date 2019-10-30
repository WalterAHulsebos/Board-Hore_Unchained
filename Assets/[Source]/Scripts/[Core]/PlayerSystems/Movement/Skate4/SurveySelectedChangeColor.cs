using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SurveySelectedChangeColor : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
{
	public Text text;

	private Color startColor;

	public Color newColor;

	private Texture startTexture;

	public Texture selectedArrow;

	public SurveySelectedChangeColor()
	{
	}

	public void OnDeselect(BaseEventData eventData)
	{
		text.color = startColor;
		eventData.selectedObject.GetComponent<RawImage>().texture = startTexture;
	}

	void UnityEngine.EventSystems.ISelectHandler.OnSelect(BaseEventData eventData)
	{
		startTexture = eventData.selectedObject.GetComponent<RawImage>().texture;
		eventData.selectedObject.GetComponent<RawImage>().texture = selectedArrow;
		startColor = text.color;
		text.color = newColor;
	}
}