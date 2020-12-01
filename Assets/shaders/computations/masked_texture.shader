Shader "masked_texture"
{
	Properties
	{
		_MainTex ("_MainTex", 2D) = "white" {}
		_Mask ("_Mask", 2D) = "white" {}
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
            
			uniform sampler2D _MainTex;      
            uniform sampler2D _Mask;

			out vec4 colorOut;
			void main(){ 

				vec4 color = texture(_MainTex,gl_TexCoord[0].xy);
				vec4 mask = texture(_Mask,gl_TexCoord[0].xy);

				colorOut = vec4(color.rgb,color.a * mask.r);
			}
#endif
           
            ENDGLSL
		}
	}
}

 