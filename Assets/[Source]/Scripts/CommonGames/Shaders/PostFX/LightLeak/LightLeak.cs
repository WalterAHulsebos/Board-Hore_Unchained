using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#if UNITY_EDITOR
using UnityEditor.Rendering.PostProcessing;
#endif

[Serializable]
[PostProcess(typeof(LightLeakRenderer), PostProcessEvent.AfterStack, "CommonGames/LightLeak")]
public sealed class LightLeak : PostProcessEffectSettings
{
    //[Tooltip("LightLeak effect intensity.")]
    public BoolParameter screenSpace = new BoolParameter { value = false};
    
    [Range(0, 1f)] public FloatParameter intensity = new FloatParameter { value = 1f};

    [Range(0, 1f)] public FloatParameter redContribution = new FloatParameter { value = 1f};
    [Range(0, 1f)] public FloatParameter yellowContribution = new FloatParameter { value = 1f};
    [Range(0, 1f)] public FloatParameter blueContribution = new FloatParameter { value = 1f};

    [Range(0, 4f)] public FloatParameter moveSpeed = new FloatParameter { value = 1.5f};

}

public sealed class LightLeakRenderer : PostProcessEffectRenderer<LightLeak>
{
    private static readonly int
        Intensity = Shader.PropertyToID(name: "_Intensity"),

        Red = Shader.PropertyToID(name: "_Red"),
        Yellow = Shader.PropertyToID(name: "_Yellow"),
        Blue = Shader.PropertyToID(name: "_Blue"),
        
        MoveSpeed = Shader.PropertyToID(name: "_MoveSpeed");

    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet __sheet = context.propertySheets.Get(Shader.Find(name: "Hidden/CommonGames/PostFX/LightLeak"));
        
        __sheet.properties.SetFloat(Intensity, settings.intensity);
        
        __sheet.properties.SetFloat(Red, settings.redContribution);
        __sheet.properties.SetFloat(Yellow, settings.yellowContribution);
        __sheet.properties.SetFloat(Blue, settings.blueContribution);
        
        __sheet.properties.SetFloat(MoveSpeed, settings.moveSpeed);
        
        if(settings.screenSpace)
        {
            __sheet.EnableKeyword("SCREEN_BLENDMODE");
        }
        else
        {
            __sheet.DisableKeyword("SCREEN_BLENDMODE");
        }

        context.command.BlitFullscreenTriangle(context.source, context.destination, __sheet, 0);
    }
        
    /*
        _material.SetFloat("_Intensity", Instensity);
        _material.SetFloat("_Red", RedContribution);
        _material.SetFloat("_Yellow", YellowContribution);
        _material.SetFloat("_Blue", BlueContribution);
        _material.SetFloat("_MoveSpeed", MoveSpeed);

        if (useScreenBlendmode)
        {
            _material.EnableKeyword("SCREEN_BLENDMODE");
        }
        else
        {
            _material.DisableKeyword("SCREEN_BLENDMODE");
        }
     */
}

#if UNITY_EDITOR

[PostProcessEditor(typeof(LightLeak))]
public sealed class LightLeakEditor : PostProcessEffectEditor<LightLeak>
{
    SerializedParameterOverride
        _screenSpace,
        
        _intensity,
        
        _redContribution,
        _yellowContribution,
        _blueContribution,
        
        _moveSpeed;

    public override void OnEnable()
    {
        _screenSpace = FindParameterOverride(x => x.screenSpace);
        
        _intensity = FindParameterOverride(x => x.intensity);
        
        _redContribution = FindParameterOverride(x => x.redContribution);
        _yellowContribution = FindParameterOverride(x => x.yellowContribution);
        _blueContribution = FindParameterOverride(x => x.blueContribution);
        
        _moveSpeed = FindParameterOverride(x => x.moveSpeed);
    }

    public override void OnInspectorGUI()
    {
        PropertyField(_screenSpace);
        
        PropertyField(_intensity);
        
        PropertyField(_redContribution);
        PropertyField(_yellowContribution);
        PropertyField(_blueContribution);
        
        PropertyField(_moveSpeed);
    }
}

#endif