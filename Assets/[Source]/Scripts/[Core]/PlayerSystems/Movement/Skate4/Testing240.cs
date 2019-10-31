using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Testing240 : MonoBehaviour
{
	public GameObject cube1;

	public GameObject cube2;

	public RectTransform rect1;

	public GameObject sprite2;

	public Testing240()
	{
	}

	public static void ScaleGroundColor(Color to)
	{
		RenderSettings.ambientGroundColor = to;
	}

	private void Start()
	{
		LeanTween.moveY(cube1, cube1.transform.position.y - 15f, 10f).setEase(LeanTweenType.easeInQuad).setDestroyOnComplete(false).setOnComplete(() => Debug.Log("Done"));
		Vector3 vector3 = cube1.transform.position;
		LeanTween.rotateAround(cube1, Vector3.forward, 360f, 10f).setOnComplete(() => Debug.Log(string.Concat(new object[] { "before:", vector3, " after :", cube1.transform.position })));
		LeanTween.@value(gameObject, new Vector3(1f, 1f, 1f), new Vector3(10f, 10f, 10f), 1f).setOnUpdate((Vector3 val) => {
		}, null);
		LeanTween.@value(gameObject, new Action<Color>(ScaleGroundColor), new Color(1f, 0f, 0f, 0.2f), Color.blue, 2f).setEaseInOutBounce();
		LeanTween.scale(cube2, Vector3.one * 2f, 1f).setEasePunch().setScale(5f);
		LeanTween.scale(rect1, Vector3.one * 2f, 1f).setEasePunch().setScale(-1f);
		Vector3[] vector2 = new Vector3[] { Vector2.zero, Vector2.zero, new Vector2(1f, -0.5f), new Vector2(1.4f, 0f), new Vector2(1f, 0.5f), Vector2.zero, new Vector2(-1f, -0.5f), new Vector2(-1.4f, 0f), new Vector2(-1f, 0.5f), Vector2.zero, Vector2.zero };
		LeanTween.moveSplineLocal(sprite2, vector2, 4f).setOrientToPath2d(true).setRepeat(-1);
	}

	private void Update()
	{
	}
}