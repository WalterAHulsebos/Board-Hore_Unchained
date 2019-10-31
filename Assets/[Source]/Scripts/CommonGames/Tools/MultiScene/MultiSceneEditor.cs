//      
//   ^\.-
//  c====ɔ   Crafted with <3 by Nate Tessman
//   L__J    nate@madgvox.com
// 

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;
using UnityEditorInternal;

using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
#if ODIN_INSPECTOR
using Editor = Sirenix.OdinInspector.Editor.OdinEditor;
#endif
#endif

//using static MultiScene;

using Utilities.CGTK;

[CustomEditor(typeof(MultiScene))]
public class MultiSceneEditor : Editor
{
	private static class Styles
	{
		public static readonly GUIStyle dragInfoStyle;

		static Styles()
		{
			dragInfoStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel) {wordWrap = true};
		}
	}

	[MenuItem("Assets/Create/Multi-Scene", false, 201)]
	private static void CreateMultiScene()
	{
		MultiScene multi = CreateInstance<MultiScene>();
		multi.name = "New Multi-Scene";

		UnityEngine.Object parent = Selection.activeObject;

		string directory = "Assets";
		if(parent != null)
		{
			directory = AssetDatabase.GetAssetPath(parent.GetInstanceID());
			if(!Directory.Exists(directory))
			{
				directory = Path.GetDirectoryName(directory);
			}
		}

		ProjectWindowUtil.CreateAsset(multi, $"{directory}/{multi.name}.asset");
	}

	[MenuItem("Edit/Multi-Scene From Open Scenes %#&s", false, 0)]
	private static void CreatePresetFromOpenScenes()
	{
		MultiScene multi = CreateInstance<MultiScene>();
		multi.name = "New Multi-Scene";

		Scene activeScene = SceneManager.GetActiveScene();
		int sceneCount = SceneManager.sceneCount;

		for(int i = 0; i < sceneCount; i++)
		{
			Scene scene = SceneManager.GetSceneAt(i);

			SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
			
			if(activeScene == scene)
			{
				multi.activeScene = sceneAsset;
			}

			multi.sceneAssets.Add(new MultiScene.SceneInfo(sceneAsset, scene.isLoaded));
		}

		string directory = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
		bool isDirectory = Directory.Exists(directory);
		if(!isDirectory)
		{
			directory = Path.GetDirectoryName(directory);
		}

		ProjectWindowUtil.CreateAsset(multi, $"{directory}/{multi.name}.asset");
	}

	[OnOpenAsset(1)]
	private static bool OnOpenAsset(int id, int line)
	{
		UnityEngine.Object obj = EditorUtility.InstanceIDToObject(id);
		if(obj is MultiScene scene)
		{
			OpenMultiScene(scene, Event.current.alt);
			return true;
		}

		if(!(obj is SceneAsset)) { return false; }
		if(!Event.current.alt) { return false; }
		
		EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(obj.GetInstanceID()), OpenSceneMode.Additive);
		
		return true;

	}

	private new MultiScene target;
	private ScenePresetList list;

	protected override void OnEnable()
	{
		base.OnEnable();
		
		target = (MultiScene)base.target;
		list = new ScenePresetList(target, target.sceneAssets, typeof(SceneAsset));
	}

	private static void OpenMultiScene(MultiScene obj, bool additive)
	{
		Scene activeScene = default;
		
		if(additive || EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			List<string> firstUnloadedScenes = new List<string>();
			bool inFirstUnloadedScenes = true;
			Scene firstLoadedScene = default;
			
			foreach(MultiScene.SceneInfo info in obj.sceneAssets)
			{
				if(info.asset == null) { continue; }
				
				string path = AssetDatabase.GetAssetPath(((SceneAsset)info.asset).GetInstanceID());
				OpenSceneMode mode = OpenSceneMode.Single;
				bool isActiveScene =(SceneAsset)info.asset == obj.activeScene;

				bool exitedFirstUnloadedScenes = false;
				if(inFirstUnloadedScenes)
				{
					if(!isActiveScene && !info.loadScene)
					{
						firstUnloadedScenes.Add(path);
						continue;
					}

					inFirstUnloadedScenes = false;
					exitedFirstUnloadedScenes = true;
				}

				if((!exitedFirstUnloadedScenes) || additive)
				{
					mode = ((!additive && isActiveScene) || info.loadScene)
						? OpenSceneMode.Additive
						: OpenSceneMode.AdditiveWithoutLoading; 
				}

				Scene scene = EditorSceneManager.OpenScene(path, mode);

				if(isActiveScene) activeScene = scene;
				if(exitedFirstUnloadedScenes) firstLoadedScene = scene;
			}

			foreach(string path in firstUnloadedScenes)
			{
				Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.AdditiveWithoutLoading);
				
				if(firstLoadedScene.IsValid())
				{
					EditorSceneManager.MoveSceneBefore(scene, firstLoadedScene);
				}
			}
			
		}
		if(!additive && activeScene.IsValid())
		{
			SceneManager.SetActiveScene(activeScene);
		}
	}

	private static Scene SceneAssetToScene(UnityEngine.Object asset)
	{
		return SceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(asset));
	}

	protected override void OnHeaderGUI()
	{
		if(target.sceneAssets == null) { return; }
		base.OnHeaderGUI();	
	}

	public override void OnInspectorGUI()
	{
		if(target.sceneAssets == null) { return; }
		EditorGUI.BeginChangeCheck();
		list.DoLayoutList();

		Event evt = Event.current;

		switch(evt.type)
		{
			case EventType.DragPerform:
			case EventType.DragUpdated:
			{
				UnityEngine.Object[] objects = DragAndDrop.objectReferences;
				bool canDrag = false;
				foreach(UnityEngine.Object obj in objects)
				{
					if(!(obj is SceneAsset))
					{
						canDrag = false;
						break;
					}

					canDrag = true;
				}

				if(canDrag)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Move;

					if(evt.type == EventType.DragPerform)
					{
						Undo.RecordObject(target, "Add Scenes");
						GUI.changed = true;
						foreach(UnityEngine.Object obj in objects)
						{
							SceneAsset scene =(SceneAsset)obj;
							int index = target.sceneAssets.FindIndex(i => i.asset == scene);
							if(index > -1)
							{
								target.sceneAssets.RemoveAt(index);
							}
							
							target.sceneAssets.Add(new MultiScene.SceneInfo(scene));
						}
					}
				}

				if(canDrag)
				{
					DragAndDrop.AcceptDrag();
					evt.Use();
				}

				break;
			}
			
			case EventType.DragExited:
				Repaint();
				break;
			case EventType.MouseDown:
				break;
			case EventType.MouseUp:
				break;
			case EventType.MouseMove:
				break;
			case EventType.MouseDrag:
				break;
			case EventType.KeyDown:
				break;
			case EventType.KeyUp:
				break;
			case EventType.ScrollWheel:
				break;
			case EventType.Repaint:
				break;
			case EventType.Layout:
				break;
			case EventType.Ignore:
				break;
			case EventType.Used:
				break;
			case EventType.ValidateCommand:
				break;
			case EventType.ExecuteCommand:
				break;
			case EventType.ContextClick:
				break;
			case EventType.MouseEnterWindow:
				break;
			case EventType.MouseLeaveWindow:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if(EditorGUI.EndChangeCheck()){ EditorUtility.SetDirty(target); }

		//EditorGUILayout.Space();
		//GUILayout.Label("Drag and drop scenes into the inspector window to append them to the list.", Styles.dragInfoStyle);
	}

	private sealed class ScenePresetList : ReorderableList
	{
		private static readonly GUIContent loadSceneContent = new GUIContent(string.Empty, "Load Scene");
		private static readonly GUIContent activeSceneContent = new GUIContent(string.Empty, "Set Active Scene");

		private readonly MultiScene target;
		private new readonly List<MultiScene.SceneInfo> list;

		public ScenePresetList(MultiScene target, List<MultiScene.SceneInfo> elements, Type elementType) 
			: base(elements, elementType, true, false, true, true)
		{
			this.target = target;
			list = elements;

			drawElementCallback = DrawElement;
			drawHeaderCallback = DrawHeader;
			onRemoveCallback = OnRemove;
			onAddCallback = OnAdd;
		}

		private static void DrawHeader(Rect rect)
		{
			const int toggleWidth = 17;

			Rect loadSceneRect = new Rect(rect.x + rect.width - toggleWidth * 2, rect.y, toggleWidth, rect.height);
			Rect activeSceneRect = new Rect(rect.x + rect.width - toggleWidth, rect.y, toggleWidth, rect.height);
			Rect labelRect = new Rect(rect.x, rect.y, rect.width -(toggleWidth * 2) - 5, EditorGUIUtility.singleLineHeight);

			GUI.Label(labelRect, "Scenes");
			GUI.Label(loadSceneRect, "L");
			GUI.Label(activeSceneRect, "A");
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			rect.y += 2;

			const int toggleWidth = 17;

			Rect loadSceneRect = new Rect(rect.x + rect.width - toggleWidth * 2, rect.y, toggleWidth, rect.height);
			Rect activeSceneRect = new Rect(rect.x + rect.width - toggleWidth, rect.y, toggleWidth, rect.height);
			Rect labelRect = new Rect(rect.x, rect.y, rect.width -(toggleWidth * 2) - 5, EditorGUIUtility.singleLineHeight);

			MultiScene.SceneInfo info = list[index];
			 EditorGUI.BeginChangeCheck();
			SceneAsset scene =(SceneAsset)EditorGUI.ObjectField(labelRect, info.asset, typeof(SceneAsset), false);
			
			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Change Scene");
				info.asset = scene;
				target.sceneAssets[index] = info;
			}

			bool active = info.asset == target.activeScene;
			GUI.enabled = !active;
			EditorGUI.BeginChangeCheck();
			bool loadScene = GUI.Toggle(loadSceneRect, info.loadScene, loadSceneContent);
			
			if(EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Change Load Scene");
				info.loadScene = loadScene;
				target.sceneAssets[index] = info;
			}
			GUI.enabled = true;

			EditorGUI.BeginChangeCheck();
			bool setActive = GUI.Toggle(activeSceneRect, active, activeSceneContent);
			
			if(!EditorGUI.EndChangeCheck()) { return; }
			if(!setActive) { return; }
			
			Undo.RecordObject(target, "Change Active Scene");
			target.activeScene = info.asset;
		}

		private void OnRemove(ReorderableList l)
		{
			Undo.RecordObject(target, "Remove Scene");
			MultiScene.SceneInfo removed = target.sceneAssets[ index ];
			if(removed.asset == target.activeScene)
			{
				target.activeScene = null;
			}
			target.sceneAssets.RemoveAt(index);
		}

		private void OnAdd(ReorderableList l)
		{
			index = list.Count;
			Undo.RecordObject(target, "Add Scene");
			list.Add(default);
		}
	}
}

#endif