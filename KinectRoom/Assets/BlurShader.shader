// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "BlurShader" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Blur("Blur", Float) = 1.0
	}
	SubShader{
	Pass{
	Tags{ "LightMode" = "ForwardAdd" }
	// pass for the first, directional light 

	CGPROGRAM

#pragma vertex vert  
#pragma fragment frag 

#include "UnityCG.cginc"
	uniform float4 _LightColor0;
	// color of light source (from "Lighting.cginc")

	uniform sampler2D _MainTex;
	uniform float _Blur;

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


	//normpdf function gives us a Guassian distribution for each blur iteration; 
	//this is equivalent of multiplying by hard #s 0.16,0.15,0.12,0.09, etc. in code above
	float normpdf(float x, float sigma)
	{
		return 0.39894*exp(-0.5*x*x / (sigma*sigma)) / sigma;
	}
	//this is the blur function... pass in standard col derived from tex2d(_MainTex,i.uv)
	half4 blur(sampler2D tex, float2 uv, float blurAmount) {
		//get our base color...
		half4 col = tex2D(tex, uv);
		//total width/height of our blur "grid":
		const int mSize = 11;
		//this gives the number of times we'll iterate our blur on each side 
		//(up,down,left,right) of our uv coordinate;
		//NOTE that this needs to be a const or you'll get errors about unrolling for loops
		const int iter = (mSize - 1) / 2;
		//run loops to do the equivalent of what's written out line by line above
		//(number of blur iterations can be easily sized up and down this way)
		for (int i = -iter; i <= iter; ++i) {
			for (int j = -iter; j <= iter; ++j) {
				col += tex2D(tex, float2(uv.x + i * blurAmount, uv.y + j * blurAmount)) * normpdf(float(i), 7);
			}
		}
		//return blurred color
		return col / mSize;
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

		output.levelOfLighting =
			max(0.0, dot(normalDirection, lightDirection));
		output.tex = input.texcoord;
		output.pos = UnityObjectToClipPos(input.vertex);
		return output;
	}

	float4 frag(vertexOutput input) : SV_TARGET
	{
		fixed4 c = tex2D(_MainTex, input.tex.xy);
		return c;

		//float4 color = blur(_MainTex, input.tex.xy, _Blur);
		//color.a = 1.0f;
		//	return color;
	}

		ENDCG
	}
	}
		Fallback "Decal"
}