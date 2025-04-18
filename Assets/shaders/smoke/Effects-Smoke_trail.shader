// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Effects/Smoke_trail"
{
    Properties
    {
        [MainTexture] _MainTex ("Main Texture", 2D) = "white" {}
        _Noise_tex ("Noise Texture", 2D) = "white" {}
        _DistortionDamper ("Distortion Damper", Float) = 1
        _Snake_offset_tex ("Snake Offset Texture", 2D) = "white" {}

        _Start_time ("Start time", float) = 0    
        _Dumpen_with_time ("Dumpen with time", float)= 0.5
        _Spread_speed  ("Spread speed", float)= 2
        
        
        _Color ("Tint", Color) = (1,1,1,1)
        _Alpha ("Alpha", float) = 1

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DistortionStrength("Strength", Float) = 1.0
        _DistortionBlend("Blend", Range(0.0, 1.0)) = 0.5

        _SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
        _SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
        _CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
        _CameraFarFadeDistance("Camera Far Fade", Float) = 2.0

        // Hidden properties
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _ColorMode ("__colormode", Float) = 0.0
        [HideInInspector] _FlipbookMode ("__flipbookmode", Float) = 0.0
        [HideInInspector] _LightingEnabled ("__lightingenabled", Float) = 0.0
        [HideInInspector] _DistortionEnabled ("__distortionenabled", Float) = 0.0
        [HideInInspector] _EmissionEnabled ("__emissionenabled", Float) = 0.0
         _BlendOp ("__blendop", Float) = 0.0
         _SrcBlend ("__src", Float) = 1.0
         _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        _Cull ("Cull", Float) = 0
        [HideInInspector] _SoftParticlesEnabled ("__softparticlesenabled", Float) = 0.0
        [HideInInspector] _CameraFadingEnabled ("__camerafadingenabled", Float) = 0.0
        [HideInInspector] _SoftParticleFadeParams ("__softparticlefadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _CameraFadeParams ("__camerafadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _ColorAddSubDiff ("__coloraddsubdiff", Vector) = (0,0,0,0)
        [HideInInspector] _DistortionStrengthScaled ("__distortionstrengthscaled", Float) = 0.0

        
    }
    
    /* HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL */

    Category
    {
        SubShader
        {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

            BlendOp [_BlendOp]
            Blend [_SrcBlend] [_DstBlend] 
            ZWrite Off
            Cull Off
            ColorMask RGB

            Pass
            {
                Tags { "LightMode" = "Universal2D" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
                #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

                #if USE_SHAPE_LIGHT_TYPE_0
                SHAPE_LIGHT(0)
                #endif

                #if USE_SHAPE_LIGHT_TYPE_1
                SHAPE_LIGHT(1)
                #endif

                #if USE_SHAPE_LIGHT_TYPE_2
                SHAPE_LIGHT(2)
                #endif

                #if USE_SHAPE_LIGHT_TYPE_3
                SHAPE_LIGHT(3)
                #endif

                struct appdata_t
                {
                    float3 vertex   : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    float2 texcoord  : TEXCOORD0;
                    float2 lightingUV  : TEXCOORD1;
                    float parabolic_progress : BLENDWEIGHT0;
                };

                sampler2D _MainTex;
                float4 _Color;
                float _Alpha;
                float _Start_time;
                float _Dumpen_with_time;
                float _Spread_speed;
                float time() {
                    return _Time[1] - _Start_time;
                }

                //#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
                //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    float4 vPosition = TransformObjectToHClip(v.vertex);
                    OUT.vertex = vPosition;
                    OUT.texcoord = v.texcoord;
                    OUT.lightingUV = ComputeScreenPos(vPosition / vPosition.w).xy;
                    OUT.parabolic_progress = pow (time(), _Dumpen_with_time) * _Spread_speed;

                    return OUT;
                }

                sampler2D _Noise_tex;
                sampler2D _Snake_offset_tex;
                float _DistortionDamper;
                


                #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

                half4 frag(v2f IN) : SV_Target
                {
                    float randomization = _Start_time;

                    float2 snakelike_offset = float2(
                        0.0,
                        0.5 - tex2D(
                            _Snake_offset_tex,
                            float2(
                                IN.texcoord.x + randomization,
                                IN.texcoord.y
                            )
                        ).r
                    );
                    
                    half2 snakelike_offset_noise = 
                    tex2D(
                        _Noise_tex,
                        IN.texcoord + randomization
                    ).rg - 
                    half2(0.5, 0.5);
                    snakelike_offset_noise *= 4;
                    
                    snakelike_offset = snakelike_offset + snakelike_offset_noise;
                    snakelike_offset *= IN.parabolic_progress;

                    half4 color = (
                        tex2D(_MainTex, IN.texcoord  + snakelike_offset * _DistortionDamper )
                    );
                    color.a *= _Alpha;
                    
                    color =  CombinedShapeLightShared(color, float4(1,1,1,1), IN.lightingUV);

                    return color;
                }
                
                ENDHLSL
            }
        }
    }

    
}
