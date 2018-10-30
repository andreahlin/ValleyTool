Shader "Unlit/Hologramboi"
{

	Properties     // allows us to define public variables in unity 
	{
		_MainTex ("Albedo Texture", 2D) = "white" {} // no semicolon to this line
        _TintColor ("Tint Color", Color) = (1,1,1,1) // declare it here, which will show up in inspector
        // ("display name", 2d)
        _Transparency ("Transparency", Range(0.0, 0.5)) = 0.25
	}
	SubShader // the actual code contains instructions how to set up renderer, contains the pass
	{
    // sometimes SubShader will have multiple shaders 
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        // in order to render something transparent, it means other things need to be
        // rendered first 
        // Rendering order queue tag = background, 
		LOD 100 // level of detail
        
        ZWrite Off // DONT RENER TO DEPTH BUFFER  
        Blend SrcAlpha OneMinusSrcAlpha // page on blending - render these things in order and then blend them all together

		Pass // tells GPU "hey draw this" 
		{
			CGPROGRAM 
			#pragma vertex vert // saying we'll have a vertex function called vert 
			#pragma fragment frag
			
			#include "UnityCG.cginc" // shaders unlike C# don't use inheritance
            // has a whole bunch of helper funcitons for rendering in unity
            
			struct appdata // objects used to pass in info about vertices of 3d model 
			{ // passed in a packed array 
				float4 vertex : POSITION; // semantic binding : tells shader how it's going to be used in rendering
				float2 uv : TEXCOORD0;
			};

			struct v2f // vert to frag
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION; // specifying that it will be a screen space poisition  
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _TintColor; 
            float _Transparency; 
			
			v2f vert (appdata v) //vertex function 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex); 
                // UnityObjectToClipPos = i think NDC 
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // taking UV data from model, data from main texture, and transforming it
                // this is where tiling and offset parameters are being applied 
				return o; // return the struct 
			}
			
			fixed4 frag (v2f i) : SV_Target // frag function 
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) + _TintColor; // could have fixed4 which could be coordinates or RGBA 
                col.a = _Transparency; 
				return col;
			}
			ENDCG
		}
	}
}