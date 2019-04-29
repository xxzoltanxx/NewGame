Shader "enemyFOVshader"
{
	Properties
	{
		_PlayerPosition("Player Position", Vector) = (0,0,0,0)
		_Distance("Distance", Float) = 5.0
		_Threshold("Threshold", Float) = 0.5
		_Frequency("Frequency", Float) = 100.0
		_Scale("Scale", Float) = 0.01
		_DistAlphaFade("AlphaFadeCutout", Float) = 0.5
		_Color("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
		Blend One One
		Tags { "Queue" = "Opaque" "RenderType" = "Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
				float4 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 object : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.object = v.vertex;
                return o;
            }

			fixed4 _PlayerPosition;
			float _Threshold;
			float _Distance;
			float _Frequency;
			float _Scale;
			float _DistAlphaFade;
			float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
				float4 center = float4(0,0,0,1);
				center = mul(unity_ObjectToWorld, center);
				float dist = distance(center, i.worldPos) * _Scale;;
				float distObject = distance(i.object.xy, float2(0,0));
				float4 col = _Color;
				float height = sin(dist * 180.0 * 2.0 - _Time * _Frequency);
				if (height < _Threshold)
				{
					col = float4(0, 0, 0, 0);
				}
				else
				{
					col = col * (1 - distObject);
				}
				float playerVertexDist = distance(_PlayerPosition.xy, i.worldPos.xy);
				if (_Distance < playerVertexDist)
				{
					if (_Distance + _DistAlphaFade > playerVertexDist && col.a != 0)
					{
						float factor = 0.5 - (playerVertexDist - _Distance) / (_DistAlphaFade * 2.0f);
						col.a = col.a * factor;
						col.r = factor * col.r;
						col.g = factor * col.g;
						col.b = factor * col.b;
					}
					else
					{
						col.a = 0.0f;
						col = float4(0, 0, 0, 0);
					}
				}


                return col;
            }
            ENDCG
        }
    }
}
