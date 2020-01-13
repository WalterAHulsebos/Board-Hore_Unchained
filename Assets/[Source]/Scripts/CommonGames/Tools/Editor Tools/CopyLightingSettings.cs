﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;

// Latest version at https://bitbucket.org/snippets/pschraut/LeykeL
internal static class CopyLightingSettings
{
    //
    // Written by Peter Schraut
    //     http://www.console-dev.de
    //
    // This Unity editor extension allows yoyu to copy&paste lighting settings 
    // from one scene to another. Check the following video to see it in action
    // https://youtu.be/-TQzrVn1kWM 
    //
    // Save this file as "Assets/Editor/CopyLightingSettings.cs"
    //
    // Download most recent version from:
    //     https://bitbucket.org/snippets/pschraut/LeykeL
    //	

    private static SerializedObject s_SourceLightmapSettings;
    private static SerializedObject s_SourceRenderSettings;

#if UNITY_2018_2_OR_NEWER
private const string k_CopySettingsMenuPath = "Window/Rendering/Copy Lighting Settings";
private const string k_PasteSettingsMenuPath = "Window/Rendering/Paste Lighting Settings";
#else
    const string k_CopySettingsMenuPath = "Window/Lighting/Copy Settings";
    const string k_PasteSettingsMenuPath = "Window/Lighting/Paste Settings";
#endif

    [MenuItem(k_CopySettingsMenuPath, priority = 200)]
    private static void CopySettings()
    {
        UnityEngine.Object lightmapSettings;
        if (!TryGetSettings(typeof(LightmapEditorSettings), "GetLightmapSettings", out lightmapSettings))
            return;

        UnityEngine.Object renderSettings;
        if (!TryGetSettings(typeof(RenderSettings), "GetRenderSettings", out renderSettings))
            return;

        s_SourceLightmapSettings = new SerializedObject(lightmapSettings);
        s_SourceRenderSettings = new SerializedObject(renderSettings);
    }

    [MenuItem(k_PasteSettingsMenuPath, priority = 201)]
    private static void PasteSettings()
    {
        UnityEngine.Object lightmapSettings;
        if (!TryGetSettings(typeof(LightmapEditorSettings), "GetLightmapSettings", out lightmapSettings))
            return;

        UnityEngine.Object renderSettings;
        if (!TryGetSettings(typeof(RenderSettings), "GetRenderSettings", out renderSettings))
            return;

        CopyInternal(s_SourceLightmapSettings, new SerializedObject(lightmapSettings));
        CopyInternal(s_SourceRenderSettings, new SerializedObject(renderSettings));

        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    [MenuItem(k_PasteSettingsMenuPath, validate = true)]
    private static bool PasteValidate()
    {
        return s_SourceLightmapSettings != null && s_SourceRenderSettings != null;
    }

    private static void CopyInternal(SerializedObject source, SerializedObject dest)
    {
        SerializedProperty prop = source.GetIterator();
        while (prop.Next(true))
        {
            bool copyProperty = true;
            foreach (string propertyName in new[] { "m_Sun", "m_FileID", "m_PathID", "m_ObjectHideFlags" })
            {
                if (string.Equals(prop.name, propertyName, System.StringComparison.Ordinal))
                {
                    copyProperty = false;
                    break;
                }
            }

            if (copyProperty)
                dest.CopyFromSerializedProperty(prop);
        }

        dest.ApplyModifiedProperties();
    }

    private static bool TryGetSettings(System.Type type, string methodName, out UnityEngine.Object settings)
    {
        settings = null;

        MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        if (method == null)
        {
            Debug.LogErrorFormat("CopyLightingSettings: Could not find {0}.{1}", type.Name, methodName);
            return false;
        }

        Object value = method.Invoke(null, null) as UnityEngine.Object;
        if (value == null)
        {
            Debug.LogErrorFormat("CopyLightingSettings: Could get data from {0}.{1}", type.Name, methodName);
            return false;
        }

        settings = value;
        return true;
    }
}
#endif