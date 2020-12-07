Shader "masked_texture"
{
	Properties
	{
		[MainTexture] _MainTex ("_MainTex", 2D) = "white" {}
		_Mask ("_Mask", 2D) = "white" {}
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


                sampler2D _MainTex;
                sampler2D _Mask;



				fixed4 frag(v2f IN) : SV_Target
                {
                    half4 alpha = (
                        tex2D(_Mask, IN.texcoord)
                    );
                    half4 color = (
                        tex2D(_MainTex, IN.texcoord)
                    );
					color.a *= alpha.a;
                    return color;
                }
				
			ENDCG
		}
	}
}

 