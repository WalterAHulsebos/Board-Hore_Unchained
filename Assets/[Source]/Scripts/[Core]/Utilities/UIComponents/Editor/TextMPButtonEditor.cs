using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Core.Utilities.UIComponents;
using TMPro;

//namespace Editor
[CustomEditor(typeof(TextMPButton))]
public class TextMPButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        TextMPButton targetButton = (TextMPButton)target;
        
        base.OnInspectorGUI();
        
        targetButton.text = EditorGUILayout.ObjectField(targetButton.text, typeof(TMP_Text), true) as TMP_Text;
    }
}