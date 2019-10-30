using System;
using UnityEngine;
using UnityEngine.UI;

public class TutTrickTextRotator : MonoBehaviour
{
	public string[] labels;

	public Text label;

	public int i;

	public TutTrickTextRotator()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			i++;
			if (i == (int)labels.Length)
			{
				i = 0;
				Debug.Log("reset");
			}
			label.text = labels[i];
		}
	}
}