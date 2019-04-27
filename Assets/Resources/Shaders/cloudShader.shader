Shader "cloudShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SunDir("SunDirection", Vector) = (0,0,0,0)
	}
		SubShader
		{
			// No culling or depth
			//Cull Off ZWrite Off ZTest Always
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Pass
			{
				CGPROGRAM
				#pragma vertex vert alpha
				#pragma fragment frag alpha

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				float4 _SunDir;
				float getHeightAt(float2 texCoord, float distance, sampler2D heightMap, float4 lightDir)
				{
					float2 newTexCoord = float2(texCoord.x - lightDir.x * distance, texCoord.y - lightDir.y * distance);
					return tex2D(heightMap, newTexCoord).a;
				}

				bool isInShadow(float2 texCoord, float4 lightDir, sampler2D heightMap, float step)
				{
					float height = tex2D(_MainTex, texCoord).a;
					float otherHeight;
					float distance;

					step *= (1 - lightDir.z);
					for (int i = 0; i < 20; ++i)
					{
						distance = step * float(i);
						otherHeight = getHeightAt(texCoord, distance, heightMap, lightDir);

						if (otherHeight > 0)
						{
							return true;
						}
					}

					return false;

				}
				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = fixed4(0,0,0,0);
					float2 lightDirNormalized = _SunDir.xy;
					lightDirNormalized = normalize(lightDirNormalized);
					_SunDir.xy = lightDirNormalized;
					if (tex2D(_MainTex, i.uv).a != 0)
					{
						col.a = 0.5 - ((_SunDir.z - 0.90f) / (0.10f * 2.0f)) ;
					}
					return col;
				}

				ENDCG
			}
		}
}
