Shader "masked_texture"
{
	Properties
	{
		[MainTexture] _BodyTex ("_MainTex", 2D) = "white" {}
		_InnardsTex ("_Innards", 2D) = "white" {}
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
                    half4 alpha = (
                        tex2D(_BodyMask, IN.texcoord)
                    );
                    half4 color = (
                        tex2D(_BodyTex, IN.texcoord)
                    );
					color.a *= alpha.a;
                    return color;
                }
				
			ENDCG
		}
	}
}

 