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

    Category
    {
        SubShader
        {
            Tags {
                "Queue"="Transparent+1"}

            BlendOp [_BlendOp]
            Blend [_SrcBlend] [_DstBlend] 
            ZWrite On
            Cull [_Cull]
            ColorMask RGB

            GrabPass
            {
                Tags { "LightMode" = "Always" }
                "_GrabTexture"
            }

            Pass
            {
                Tags { "LightMode"="ForwardBase" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                /* #include "../UnityStandardParticles_copy.cginc"
                #pragma vertex vertParticleUnlit
                #pragma fragment fragParticleUnlit */

                #pragma target 2.0

                

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    float2 texcoord  : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _Color;
                float _Alpha;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    float4 vPosition = UnityObjectToClipPos(v.vertex);
                    OUT.vertex = vPosition;
                    OUT.texcoord = v.texcoord;

                    return OUT;
                }

                sampler2D _Noise_tex;
                sampler2D _Snake_offset_tex;
                float _DistortionDamper;
                float _Start_time;

                float time() {
                    return _Time[1] - _Start_time;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    float parabolic_progress = pow (time(), 0.5) *2;
                    float randomization = (_Start_time);

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
                    snakelike_offset *= parabolic_progress;

                    half4 color = (
                        tex2D(_MainTex, IN.texcoord  + snakelike_offset * _DistortionDamper )
                    );
                    color.a *= _Alpha;
                    return color;
                }
                
                ENDCG
            }
        }
    }

    
}
