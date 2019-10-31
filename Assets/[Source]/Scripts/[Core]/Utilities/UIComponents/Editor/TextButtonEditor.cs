using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Core.Utilities.UIComponents;

//namespace Editor
[CustomEditor(typeof(TextButton))]
public class TextButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        TextButton targetButton = (TextButton)target;
        
        base.OnInspectorGUI();
        
        targetButton.text = EditorGUILayout.ObjectField(targetButton.text, typeof(Text), true) as Text;
    }
}