// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Effects/Smoke_trail_copy"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Noise_tex ("Noise Texture", 2D) = "white" {}
        _DistortionDamper ("Distortion Damper", Float) = 1
        _Snake_offset_tex ("Snake Offset Texture", 2D) = "white" {}

        _Start_time ("Start time", float) = 0    

        
        _Color ("Tint", Color) = (1,1,1,1)
        _Alpha ("Alpha", float) = 1


        [HideInInspector] _BlendOp ("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }


        Cull Off
        Lighting Off
        ZWrite On
        //Blend One OneMinusSrcAlpha
        Blend One OneMinusSrcAlpha
        ColorMask RGB

        GrabPass
        {
            Tags { "LightMode" = "Always" }
            "_GrabTexture"
        }

        Pass
        {
            Name "Default"

            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            //#include "UnityStandardParticles_copy.cginc"
            //#pragma vertex vertParticleUnlit
            //#pragma fragment fragParticleUnlit

            #pragma multi_compile_instancing
            #pragma instancing_options procedural:vertInstancingSetup
            #pragma multi_compile __ SOFTPARTICLES_ON
            #pragma multi_compile_fog
            #pragma target 2.5

            #pragma shader_feature_local_fragment _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
            #pragma shader_feature_local_fragment _ _COLOROVERLAY_ON _COLORCOLOR_ON _COLORADDSUBDIFF_ON
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_fragment _EMISSION
            #pragma shader_feature_local _FADING_ON
            #pragma shader_feature_local _REQUIRE_UV2
            #pragma shader_feature_local EFFECT_BUMP

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            

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
                color.a = _Alpha;
                return color;
            }
        ENDCG
        }
    }
}
