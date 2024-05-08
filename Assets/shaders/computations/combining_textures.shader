Shader "combining_textures"
{
	Properties
	{
		[MainTexture] _Bottom ("Bottom", 2D) = "white" {}
		_Top ("Top", 2D) = "white" {}
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

           
            sampler2D _Bottom;
			sampler2D _Top;



			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color_bottom = (
					tex2D(_Bottom, IN.texcoord)
				);
				half4 color_top = (
					tex2D(_Top, IN.texcoord)
				);
				//color_top.a=1; //test
				// if (color_top.a > 0.1) {
				// 	return color_top;
				// }
				return color_bottom * (1-color_top.a) + color_top*color_top.a;
			} 
			
           
        ENDCG
		}
	}
}

 
