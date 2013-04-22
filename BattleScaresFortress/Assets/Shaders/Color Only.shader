Shader "Custom/Color Only"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
			Cull back
			Blend One Zero
            Color [_Color]
		}
	} 
	FallBack "Diffuse"
}
