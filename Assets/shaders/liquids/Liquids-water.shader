// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Liquids/water"
{
    Properties
    {
        [MainTexture] _MainTex ("Main Texture", 2D) = "white" {}
        _Noise_tex ("Noise Texture", 2D) = "white" {}
        _DistortionDamper ("Distortion Damper", Float) = 1

        _Color ("Tint", Color) = (1,1,1,1)
        _Alpha ("Alpha", float) = 1

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.1

        _SrcBlend("_SrcBlend", Float) = 5
        _DstBlend("_DstBlend", Float) = 10
    }

    Category
    {
        SubShader
        {
            Tags {
                "Queue"="Transparent+1"}

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

                sampler2D _Noise_tex;
                float _DistortionDamper;


                fixed4 frag(v2f IN) : SV_Target
                {
                    float randomization = _Time[1];

                    
                    float4 noise_raw = tex2D(
                        _Noise_tex,
                        float2(IN.texcoord.x+randomization, IN.texcoord.y)
                    );

                    float noise_power = tex2D(
                        _Noise_tex,
                        IN.texcoord
                    ).a;
                    
                    noise_power *= randomization;
                    
                    float2 noise_offset = 
                        float2(
                            noise_raw.r * 0.2 - 0.1,
                            noise_raw.g * 0.2 - 0.1
                        );
                        
                        
                    //half2(0.1, 0.1);
                    
                    //noise /= 2;
      

                    half4 color = (
                        tex2D(_MainTex, IN.texcoord + noise_offset * noise_power)
                    );
                    color.a *= _Alpha;
                    return color;
                }
                
                ENDCG
            }
        }
    }

    
}
