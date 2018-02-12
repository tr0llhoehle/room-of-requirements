// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "HorizontalBlendShader" {
   Properties {
	 [Toggle(MIRROR_LEFT)]
	 _MirrorLeft("Mirror Left Texture", Float) = 0
	 [Toggle(MIRROR_RIGHT)]
	 _MirrorRight("Mirror Right Texture", Float) = 0
	[Toggle(LEFT_TO_RIGHT)]
	 _MirrorRight("Blend from left to right", Float) = 1

      _LeftTex ("Left", 2D) = "white" {}
      _RightTex ("Right", 2D) = "white" {} 
	  _Balance ("Balance", Float) = 0.5 
	  _Saturation("Saturation", Float) = 1.0
	  _Lightness("Lightness", Float) = 1.0

   }
   SubShader {
      Pass {	
         Tags { "LightMode" = "Always" } 
            // pass for the first, directional light 
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
		 #pragma shader_feature MIRROR_LEFT
		 #pragma shader_feature MIRROR_RIGHT
		#pragma shader_feature LEFT_TO_RIGHT
 
         #include "UnityCG.cginc"
 
         uniform sampler2D _LeftTex;
         uniform sampler2D _RightTex;
		 uniform float _Balance;
		 uniform float _Saturation;
		 uniform float _Lightness;

	

         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
            float levelOfLighting : TEXCOORD1;
               // level of diffuse lighting computed in vertex shader
         };

		 float3 rgb2hsv(float3 c)
		 {
			 float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			 float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
			 float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

			 float d = q.x - min(q.w, q.y);
			 float e = 1.0e-10;
			 return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		 }

		 float3 hsv2rgb(float3 c)
		 {
			 float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
			 float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
			 return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
		 }

 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject;
 
            float3 normalDirection = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            float3 lightDirection = normalize(
               _WorldSpaceLightPos0.xyz);
 
            output.levelOfLighting = 1.0;
            output.tex = input.texcoord;
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	// mirror base texture
         	float2 coord = input.tex.xy;
#ifdef MIRROR_LEFT
            float4 leftColor = tex2D(_LeftTex, coord);    
#else
			float4 leftColor = tex2D(_LeftTex, float2(1 - coord.x, coord.y));
#endif

#ifdef MIRROR_RIGHT
			float4 rightColor = tex2D(_RightTex, coord);
#else
			float4 rightColor = tex2D(_RightTex, float2(1 - coord.x, coord.y));
#endif
			float min_alpha = _Balance - 0.2;
			float max_alpha = _Balance + 0.2;
			float alpha = min(1, max(0, (coord.x - min_alpha) / (max_alpha - min_alpha)));

#ifdef LEFT_TO_RIGHT
			float4 color = lerp(leftColor, rightColor, alpha);
#else
			float4 color = lerp(rightColor, leftColor, alpha);
#endif

			float3 hsv = rgb2hsv(color.rgb);
			hsv.g = hsv.g * _Saturation;
			hsv.b = hsv.b * _Lightness;
			color.rgb = hsv2rgb(hsv);
			return color;
         }
 
         ENDCG
      }
   } 
   Fallback "Decal"
}