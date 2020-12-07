Shader "combining_textures"
{
	Properties
	{
		_Texture1 ("_Texture1", 2D) = "white" {}
		_Texture2 ("_Texture2", 2D) = "white" {}
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

           
            sampler2D _Texture1;
			sampler2D _Texture2;



			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color_bottom = (
					tex2D(_Texture1, IN.texcoord)
				);
				half4 color_top = (
					tex2D(_Texture2, IN.texcoord)
				);
				if (color_top.a > 0.1) {
					return color_top;
				}
				return color_bottom * (1-color_top.a) + color_top;
			} 
			
           
        ENDCG
		}
	}
}

 
