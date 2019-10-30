﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Beans.Unity.ETE
{
	[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
	public class ExtendedTransformEditor : Editor
	{
		private class Content
		{
			public static Texture2D ResetTexture = Resources.Load<Texture2D> (EditorGUIUtility.isProSkin ? "Textures/ETE_Pro_Reset" : "Textures/ETE_Personal_Reset");

			public static readonly GUIContent Position	= new GUIContent ("Position", "The local position of this GameObject relative to the parent.");
			public static readonly GUIContent Rotation	= new GUIContent ("Rotation", "The local rotation of this Game Object relative to the parent.");
			public static readonly GUIContent Scale		= new GUIContent ("Scale", "The local scaling of this GameObject relative to the parent.");
			public static readonly GUIContent ResetPosition = new GUIContent (ResetTexture, "Reset the position.");
			public static readonly GUIContent ResetRotation = new GUIContent (ResetTexture, "Reset the rotation.");
			public static readonly GUIContent ResetScale = new GUIContent (ResetTexture, "Reset the scale.");

			public const string FloatingPointWarning = "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.";
		}

		private class Styles
		{
			public static GUISkin Skin;
			public static GUIStyle ResetButton;

			static Styles ()
			{
				Skin = Resources.Load<GUISkin> ("ETE");
				ResetButton = Skin.GetStyle ("button");
			}
		}

		private class Properties
		{
			public SerializedProperty Position;
			public SerializedProperty Rotation;
			public SerializedProperty Scale;

			public Properties (SerializedObject obj)
			{
				Position	= obj.FindProperty ("m_LocalPosition");
				Rotation	= obj.FindProperty ("m_LocalRotation");
				Scale		= obj.FindProperty ("m_LocalScale");
			}
		}

		private const int MaxDistanceFromOrigin = 100000;
		private const int ContentWidth = 60;

		private float xyRatio, xzRatio;

		private Properties properties;
		private TransformRotationGUI rotationGUI;

		private void OnEnable ()
		{
			properties = new Properties (serializedObject);

			if (rotationGUI == null)
				rotationGUI = new TransformRotationGUI ();
			rotationGUI.Initialize (properties.Rotation, Content.Rotation);
		}

		public override void OnInspectorGUI ()
		{
			if (!EditorGUIUtility.wideMode)
			{
				EditorGUIUtility.wideMode = true;
				EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
			}

			serializedObject.UpdateIfRequiredOrScript ();

			using (new EditorGUILayout.HorizontalScope ())
			{
				EditorGUILayout.PropertyField (properties.Position, Content.Position);
				if (GUILayout.Button (Content.ResetPosition, Styles.ResetButton))
					properties.Position.vector3Value = Vector3.zero;
			}
			using (new EditorGUILayout.HorizontalScope ())
			{
				rotationGUI.Draw ();
				if (GUILayout.Button (Content.ResetRotation, Styles.ResetButton))
				{
					rotationGUI.Reset ();
					if (Tools.current == Tool.Rotate)
					{
						if (Tools.pivotRotation == PivotRotation.Global)
						{
							Tools.handleRotation = Quaternion.identity;
							SceneView.RepaintAll ();
						}
					}
				}
			}
			using (new EditorGUILayout.HorizontalScope ())
			{
				EditorGUILayout.PropertyField (properties.Scale, Content.Scale);
				if (GUILayout.Button (Content.ResetScale, Styles.ResetButton))
					properties.Scale.vector3Value = Vector3.one;
			}

			// I can hard code this b/c the transform inspector is always drawn in the same spot lmao
			Rect dragRect = new Rect (16, 105, EditorGUIUtility.labelWidth - 10, 10);

			Event e = Event.current;
			if (dragRect.Contains (e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
			{
				Vector3 currentScale = properties.Scale.vector3Value;
				xyRatio = currentScale.y / currentScale.x;
				xzRatio = currentScale.z / currentScale.x;
			}

			using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope ())
			{
				Color c = GUI.color;
				GUI.color = Color.clear;
				float newScaleX = CustomFloatField.Draw (new Rect (), dragRect, properties.Scale.vector3Value.x, EditorStyles.numberField);

				if (check.changed)
				{
					Vector3 currentScale = properties.Scale.vector3Value;

					float delta = newScaleX - properties.Scale.vector3Value.x;

					currentScale.x += delta;
					currentScale.y += delta * xyRatio;
					currentScale.z += delta * xzRatio;
					
					properties.Scale.vector3Value = currentScale;
				}

				GUI.color = c;
			}

			serializedObject.ApplyModifiedProperties ();

			EditorGUIUtility.labelWidth = 0;

			Transform transform = target as Transform;
			Vector3 position = transform.position;

			if
			(
				Mathf.Abs (position.x) > MaxDistanceFromOrigin ||
				Mathf.Abs (position.y) > MaxDistanceFromOrigin ||
				Mathf.Abs (position.z) > MaxDistanceFromOrigin
			)
				EditorGUILayout.HelpBox (Content.FloatingPointWarning, MessageType.Warning);
		}

		[MenuItem ("CONTEXT/Transform/Set Random Rotation")]
		private static void RandomRotation (MenuCommand command)
		{
			Transform transform = command.context as Transform;

			Undo.RecordObject (transform, "Set Random Rotation");
			transform.rotation = Random.rotation;
		}

		[MenuItem ("CONTEXT/Transform/Snap to Ground")]
		private static void SnapToGround (MenuCommand command)
		{
			Transform transform = command.context as Transform;

			RaycastHit hit;
			if (Physics.Raycast (transform.position, Vector3.down, out hit))
			{
				Undo.RecordObject (transform, "Snapped To Ground");
				transform.position = hit.point;
			}
		}

		[MenuItem ("CONTEXT/Transform/Snap to Ground (Physics)", true)]
		private static bool ValidateSnapToGroundPhysics (MenuCommand command)
		{
			return ((Transform)command.context).GetComponent<Collider> () != null;
		}

		private static void DrawDebugPoint (Vector3 point)
		{
			Debug.DrawRay (point, Vector3.forward * 0.05f, Color.blue, 5f, true);
			Debug.DrawRay (point, Vector3.right * 0.05f, Color.red, 5f, true);
			Debug.DrawRay (point, Vector3.up * 0.05f, Color.green, 5f, true);
		}
	}
}