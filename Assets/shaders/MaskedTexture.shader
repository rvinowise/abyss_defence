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
 /*
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
           
            void main()
            {
                gl_FragColor = mix(
					texture2D(_MainTex,gl_TexCoord[0].xy),
					texture2D(_Mask,gl_TexCoord[0].xy),
					texture2D(_Mask,gl_TexCoord[0].xy).a/2
				);
				
            }

#endif
           
            ENDGLSL
      }
   }
}*/