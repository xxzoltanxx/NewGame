// Unity screen grayscale grabpass overlay shader for Unity's default quad mesh that can be put onto objects.
// Due to frustum culling it is only visible if the quad is in camera's view.
// If you want it to be visible more you can scale the the quad transform up to make the bounding box/sphere bigger.
// If you need this grayscale effect all the time, it would make more sense to instead use post processing shader/stack.
// license: Unlicense

Shader "FX/Screen Gray"
{
	Properties
	{
		_Magnitude("Blur Amount", Range(0,1)) = 0.005
	}
	SubShader
	{
		Tags {
			"Queue" = "Overlay+10"
			"RenderType" = "Overlay"
			"ForceNoShadowCasting" = "True"
			"IgnoreProjector" = "True"
			"DisableBatching" = "True"
		}
		LOD 300
		GrabPass{ "_BackgroundTexture" }
		ZWrite Off
		ZTest LEqual
		Cull Off
		Blend One Zero

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"



			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _Magnitude;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			sampler2D _BackgroundTexture;

			fixed4 frag(v2f i) : SV_Target
			{
			float4 bis = i.uv;
			float4 uv = UNITY_PROJ_COORD(bis);
			float4 col = tex2Dproj(_BackgroundTexture, UNITY_PROJ_COORD(bis)); // center pixel color.

			// top row.
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y - (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y - (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x                   , uv.y - (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y - (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y - (_Magnitude * 2), uv.z, uv.w));

			// 2nd row.
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y - (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y - (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x                   , uv.y - (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y - (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y - (_Magnitude * 1), uv.z, uv.w));

			// middle row (note that we occluded middle pixel because it's handled above.
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y, uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y, uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y, uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y, uv.z, uv.w));


			// 4th row.
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y + (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y + (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x                   , uv.y + (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y + (_Magnitude * 1), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y + (_Magnitude * 1), uv.z, uv.w));

			// bottom row.
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y + (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y + (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x                   , uv.y + (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 1), uv.y + (_Magnitude * 2), uv.z, uv.w));
			col += tex2Dproj(_BackgroundTexture, float4(uv.x + (_Magnitude * 2), uv.y + (_Magnitude * 2), uv.z, uv.w));

			col /= 25; // normalize values
			//fixed grey = float3((col.r* .393) + (col.g *.769) + (col.b * .189), (col.r * .349) + (col.g *.686) + (col.b * .168), (col.r * .272) + (col.g *.534) + (col.b * .131));
			//fixed grey = dot(col.rgb, float3(0.3, 0.59, 0.11));
			fixed4 grey = fixed4(col.r * 0.5f, col.g * 0.5f, col.b * 0.5f, col.a);
			return grey;
			}
			ENDCG
		}
	}
}