Shader "Masks/Depth_mask_inverse"
{
	Properties
    {
        
        //_Render_quere("Render Queue", Int) = 0.5
	}


    SubShader {
		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.
 
		Tags {"Queue" = "Geometry+110" }
 
		// Don't draw in the RGBA channels; just the depth buffer
 
		ColorMask 0
		ZWrite On
		ZTest Greater
 
		// Do nothing specific in the pass:
 
		Pass {}
	}
}
