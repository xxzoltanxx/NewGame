// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnlitTextureAmbient"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue"="Transparent" "RenderType"="Transparent" } //We're not rendering any transparent objects
		LOD 200 //Level of detail
		Pass
	{
		Tags { "LightMode" = "ForwardBase" }
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		// Use shader model 3.0 target, to get nicer looking lighting

		struct Input
		{
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};

		sampler2D _MainTex;
		float4 _Color;
		uniform float4 _LightColor0;

		struct VertOutput
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};


		float4 _MainTex_ST;

		VertOutput vert(Input i)
		{
			VertOutput o;
			o.pos = UnityObjectToClipPos(i.pos);
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			return o;
		}

		half4 frag(VertOutput i) : SV_TARGET
		{
			float a = tex2D(_MainTex, i.uv).a;
			return tex2D(_MainTex, i.uv) * fixed4(UNITY_LIGHTMODEL_AMBIENT.rgb,1.0f);
		}



		ENDCG
	}
	}
	FallBack "Diffuse"
}