/*Shader "MaskedTexture"
{
   Properties
   {
      _MainTex ("_MainTex", 2D) = "white" {}
      _Mask ("_Mask", 2D) = "white" {}
      _Cutoff ("_Cutoff", Range (0,1)) = 0.1
   }
   SubShader
   {
      Tags {"Queue"="Transparent"}
      Lighting Off
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
	  //Blend One One
	  //Blend SrcColor DstColor
      AlphaTest GEqual [_Cutoff]
      Pass
      {
         SetTexture [_Mask] {combine texture, texture}
         SetTexture [_MainTex] {combine texture, previous * texture}
      }
   }
}
*/
Shader "MaskedTexture"
 {
    Properties {
		_Blend ("Blend", Range (0, 1) ) = 0.1 
		_MainTex ("_MainTex", 2D) = "" 
		_Mask ("_Mask", 2D) = ""
	}

	SubShader { 
		Pass {
			SetTexture[_MainTex]
			SetTexture[_Mask] { 
				ConstantColor (0,0,0, [_Blend]) 
				Combine texture Lerp(constant) previous
			}       
		}
	} 
 }