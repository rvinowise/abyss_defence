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
         SetTexture [_MainTex] {combine texture, previous}
      }
   }
}*/

/*Shader "MaskedTexture"
 {
    Properties {
		_Blend ("Blend", Range (0, 1) ) = 0.5
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
 }*/
 
Shader "MaskedTexture"
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
      Pass
      {
            GLSLPROGRAM
           
#ifdef VERTEX
           
            
           
            void main()
            {
                //gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                //TextureCoordinate = gl_MultiTexCoord0.xy;
				
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
				gl_TexCoord[0].xy = gl_MultiTexCoord0.xy;
 
            }
           
#endif
           
#ifdef FRAGMENT
            
			uniform sampler2D _MainTex;      
            uniform sampler2D _Mask;

			out vec4 colorOut;
			//in smooth vec2 texCoords;
			void main(){ 

				 vec4 color = texture(_MainTex,gl_TexCoord[0].xy);
				 vec4 mask  = texture(_Mask,gl_TexCoord[0].xy);

				 colorOut =vec4(color.rgb,color.a * mask.r);//alpha value can be in any channel ,depends on texture format.
			}
#endif
           
            ENDGLSL
      }
   }
}