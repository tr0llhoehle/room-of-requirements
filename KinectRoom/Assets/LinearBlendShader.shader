// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "HorizontalBlendShader" {
   Properties {
      _DecalTex ("Top", 2D) = "white" {}
      _MainTex ("Base", 2D) = "white" {} 
	  _Balance ("Balance", Float) = 1.0 
   }
   SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardAdd" } 
            // pass for the first, directional light 
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         uniform sampler2D _MainTex;
         uniform sampler2D _DecalTex;
		 uniform float _Balance;
 
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

 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject;
 
            float3 normalDirection = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            float3 lightDirection = normalize(
               _WorldSpaceLightPos0.xyz);
 
            output.levelOfLighting = 
               max(0.0, dot(normalDirection, lightDirection));
            output.tex = input.texcoord;
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	// mirror base texture
         	float2 baseCoord = input.tex.xy;
         	baseCoord.x = 1 - baseCoord.x;
            float4 baseColor = 
               tex2D(_MainTex, baseCoord);    
            float4 topColor = 
               tex2D(_DecalTex, baseCoord);    
			float alpha = min(1.0f, _Balance * input.tex.x);
			return lerp(baseColor, topColor, alpha);
         }
 
         ENDCG
      }
   } 
   Fallback "Decal"
}