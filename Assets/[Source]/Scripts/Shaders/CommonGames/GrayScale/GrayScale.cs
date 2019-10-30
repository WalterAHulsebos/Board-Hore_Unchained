using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#if UNITY_EDITOR
using UnityEditor.Rendering.PostProcessing;
#endif

[Serializable]
[PostProcess(typeof(GrayscaleRenderer), PostProcessEvent.AfterStack, "CommonGames/Grayscale")]
public sealed class Grayscale : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class GrayscaleRenderer : PostProcessEffectRenderer<Grayscale>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Grayscale"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}

#if UNITY_EDITOR

[PostProcessEditor(typeof(Grayscale))]
public sealed class GrayscaleEditor : PostProcessEffectEditor<Grayscale>
{
    SerializedParameterOverride m_Blend;

    public override void OnEnable()
    {
        m_Blend = FindParameterOverride(x => x.blend);
    }

    public override void OnInspectorGUI()
    {
        PropertyField(m_Blend);
    }
}

#endif