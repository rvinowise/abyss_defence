Shader "computations/splitting"
{
	Properties
	{
		[MainTexture] _BodyTex ("_BodyTex", 2D) = "white" {}
		_InnardsTex ("_InnardsTex", 2D) = "white" {}
		_BodyMask ("_BodyMask", 2D) = "white" {}
		_InnardsMask ("_InnardsMask", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent"}
		Pass
		{
            CGPROGRAM
                #include "simple_shaders.cginc"
                #pragma vertex simple_vert
                #pragma fragment frag


                sampler2D _BodyTex;
                sampler2D _InnardsTex;
                sampler2D _BodyMask;
                sampler2D _InnardsMask;



				fixed4 frag(v2f IN) : SV_Target
                {
                    half4 body_alpha = (
                        tex2D(_BodyMask, IN.texcoord)
                    );
                    half4 body_color = (
                        tex2D(_BodyTex, IN.texcoord)
                    );
                    half4 innards_alpha = (
                        tex2D(_InnardsMask, IN.texcoord)
                    );
                    half4 innards_color = (
                        tex2D(_InnardsTex, IN.texcoord)
                    );
					innards_color.a *= innards_alpha.a;
                    body_color.a *= body_alpha.a;
                    
                    half4 final_color = innards_color * (1-body_color.a) +  body_color;
                    return final_color;
                }
				
			ENDCG
		}
	}
}

 