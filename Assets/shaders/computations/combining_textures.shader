Shader "combining_textures"
{
	Properties
	{
		/*[MainTexture]*/
		//_MainTex ("MainTex", 2D) = "white" {}
		_Bottom ("Bottom", 2D) = "white" {}
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

           
            sampler2D _Top;
			sampler2D _Bottom;



			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color_bottom = (
					tex2D(_Bottom, IN.texcoord)
				);
				half4 color_top = (
					tex2D(_Top, IN.texcoord)
				);
				//color_top.a=1; //test
				
				// if (color_top.a < 0.7) {
				// 	if (color_top.a > 0.1) {
				// 		return half4(0,0,0,1);
				// 	}
				// }
				//return color_bottom + half4(0,color_top.a,0,color_top.a);// * (1/*-color_top.a*/);//
				return color_bottom * (1-color_top.a) + color_top*color_top.a;
			} 
			
           
        ENDCG
		}
	}
}

 
