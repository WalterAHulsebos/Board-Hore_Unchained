using UnityEngine;

#if ODIN_INSPECTOR
using ScriptableObject = Sirenix.OdinInspector.SerializedScriptableObject;
#endif

#if UNITY_EDITOR

using System.Collections;
using UnityEditor;

#if ODIN_INSPECTOR
using Editor = Sirenix.OdinInspector.Editor.OdinEditor;
#endif

#endif

using JetBrains.Annotations;

public abstract class AudioEvent : ScriptableObject //MemorizedScriptableObject<AudioEvent>
{
	[PublicAPI]
	public abstract void Play(AudioSource source);
}


#if UNITY_EDITOR

[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor
{

	[SerializeField] private AudioSource _previewer;

	protected override void OnEnable()
	{
		base.OnEnable();
		
		_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		
		DestroyImmediate(_previewer.gameObject);
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Preview"))
		{
			((AudioEvent) target).Play(_previewer);
		}
		EditorGUI.EndDisabledGroup();
	}
}

#endif