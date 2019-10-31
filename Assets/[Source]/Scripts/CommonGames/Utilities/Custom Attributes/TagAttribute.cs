#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class TagAttribute : Attribute
{
	public string Tag { get; }

	public TagAttribute(string myTag) => Tag = myTag;
}

// Place the drawer script file in an Editor folder or wrap it in a #if UNITY_EDITOR condition.
public class TagAttributeDrawer : OdinAttributeDrawer<TagAttribute, string>  
{
	protected override void DrawPropertyLayout(GUIContent label)
	{
		Debug.Log($"Name:{Property.Name}");
		
		//Debug.Log($"Value:{Property.}");
	}
}

#endif