// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Grass/swaying"
{
    Properties
    {
        [MainTexture] _MainTex ("Main Texture", 2D) = "white" {}
        _SwayingTex ("Swaying Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        _SrcBlend("_SrcBlend", Float) = 5
        _DstBlend("_DstBlend", Float) = 10

       // _RegularNoMasking("", string) = "Transparent+30"
    }

    Category
    {
        SubShader
        {
            Tags {
                "Queue"="Transparent+30"}

            BlendOp Add
            Blend [_SrcBlend] [_DstBlend] 
            ZWrite On
            Cull Off


            Pass
            {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

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

                sampler2D _SwayingTex;
                float _DistortionDamper;


                fixed4 frag(v2f IN) : SV_Target
                {
                    float randomization = sin(_Time[0]);

                    
                    float4 swaying_offset_raw = tex2D(
                        _SwayingTex,
                        float2(IN.texcoord.x+randomization, IN.texcoord.y)
                    );

                    float noise_power = tex2D(
                        _SwayingTex,
                        IN.texcoord
                    ).a;
                    
                    
                    /* float2 swaying_offset = 
                        float2(
                            swaying_offset_raw.r * 0.2 - 0.1,
                            swaying_offset_raw.g * 0.2 - 0.1
                        ); */
                    float2 swaying_offset = 
                        1+ swaying_offset_raw * randomization * swaying_offset_raw.a/2;    
                        
                    //half2(0.1, 0.1);
                    
                    //noise /= 2;
      

                    half4 color = (
                        tex2D(_MainTex, IN.texcoord * swaying_offset/*  * noise_power */)
                    ) * _Color;
                    return color;
                }
                
                ENDCG
            }
        }
    }

    
}
