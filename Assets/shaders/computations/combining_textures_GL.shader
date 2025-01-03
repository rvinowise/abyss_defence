﻿Shader "combining_textures"
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
            GLSLPROGRAM
           
#ifdef VERTEX
           
            void main()
            {
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
				gl_TexCoord[0].xy = gl_MultiTexCoord0.xy;
            }
#endif
           
#ifdef FRAGMENT
            
			uniform sampler2D _Texture1;      
            uniform sampler2D _Texture2;

			out vec4 colorOut;
			//in smooth vec2 texCoords;
			void main(){ 

				vec4 color1 = texture(_Texture1, gl_TexCoord[0].xy);
				vec4 color2 = texture(_Texture2, gl_TexCoord[0].xy);

				//alpha value can be in any channel, depends on texture format.
				if (color2.a > 0.1) {
					colorOut = vec4(
						color2.rgba
					);
				} else {
					colorOut = vec4(
						color1.rgba
					);
				}
			}
#endif
           
            ENDGLSL
		}
	}
}

 
