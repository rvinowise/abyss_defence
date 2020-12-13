// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "computations/mask"
{
    Properties
    {
        _Color ("_Color", Color) = (0,0,0,1)

    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Default"
        CGPROGRAM
            //#include "simple_shaders.cginc"
            #pragma vertex vert
            #pragma fragment frag


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
            };

            fixed4 _Color;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.vertex = vPosition;
                OUT.color = v.color * _Color;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                return IN.color;
            }
        ENDCG
        }
    }
}
