Shader "RadarShader"
{
    Properties
    {
		_ScanDistance("ScanDistance", Range(0,100)) = 0
		_ScanPosition("ScanPosition", Vector) = (0, 0, 0, 0)
		_BorderWidth("WidthBorder", Range(0,7)) = 2
		_LineWidth("LineWidth", Range(0,1)) = 0.1
		_GridScale("Scale", Range(0,3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
					Blend One One
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
			float2 uv2 : TEXCOORD1;
		};

		struct VertOutput
		{
			float2 objectUv : TEXCOORD2;
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 worldPos : TEXCOORD1;
		};

		float _ScanDistance;
		float4 _ScanPosition;
		float _BorderWidth;
		float _LineWidth;
		float _GridScale;

		VertOutput vert(Input i)
		{
			VertOutput o;
			o.pos = UnityObjectToClipPos(i.pos);
			o.worldPos = mul(unity_ObjectToWorld,i.pos);
			o.uv = i.uv;
			return o;
		}

		float getAngle(float3 first, float3 second)
		{
			float angle = acos(dot(normalize(first), normalize(second))) * 180 / 3.14;
			float3 crossP = cross(normalize(first), normalize(second));
			if (crossP.z < 0)
			{
				return -angle;
			}
			else
			{
				return angle;
			}
		}

		half4 frag(VertOutput i) : SV_TARGET
		{
			float angle = getAngle(float3(1,0,0),float3(i.worldPos.xy,0) - float3(_ScanPosition.xy,0));
			if (angle < 0)
			{
			angle = 360 + angle;
			}
			float currentScanAngle = (_Time.x * 2000.0f) % 360;
			bool isAngleInDeadZone;
			float deltaDZoneAngle = 0;
			if (currentScanAngle - 120 < 0 && angle > 360 - abs(currentScanAngle - 120))
			{
				float opositeDelta = currentScanAngle - 120;
				isAngleInDeadZone = angle > currentScanAngle;
				deltaDZoneAngle = abs(currentScanAngle - 120) - (360 - angle);
			}
			else
			{
				isAngleInDeadZone = angle < currentScanAngle && angle > currentScanAngle - 30;
				if (angle > currentScanAngle - 120)
					deltaDZoneAngle = angle - (currentScanAngle - 120);
				else
					deltaDZoneAngle = -1;
			}
			float deadZoneStart = currentScanAngle - 120;
			if (deadZoneStart < 0)
			{
				deadZoneStart = 360 + deadZoneStart;
			}
			//bool isInsideSectorX = i.worldPos.x > _ScanSector.x && i.worldPos.x < (_ScanSector.x + _ScanSector.z);
			//bool isInsideSectorY = i.worldPos.y > _ScanSector.y && i.worldPos.y < (_ScanSector.y + _ScanSector.w);
			//bool isInsideSector = isInsideSectorX && isInsideSectorY;
			bool isInsideDistance = false;
			if (distance(i.worldPos.xy, _ScanPosition.xy) < _ScanDistance)
			{
				isInsideDistance = true;
			}
			bool isInside = isInsideDistance;
			float4 col;
			if (isInside &&  distance(i.worldPos.xy, _ScanPosition.xy) > (_ScanDistance - _BorderWidth))
			{
				col = float4(0, 0, 0.05 + sin(_Time.x * 100.0)/ 4.0 + 0.25, 0.5f);
			}
			else if (isInside && (frac(i.worldPos.x * _GridScale + _Time.x * 30.0f) < _LineWidth || frac(i.worldPos.y * _GridScale + _Time.x * 30.0f) < _LineWidth))
			{
				if (deltaDZoneAngle < 120 && deltaDZoneAngle > 0)
				{
					float ss = smoothstep(0, 1.0, deltaDZoneAngle / 120.0);
					col = float4(0, 1.0 * ss, 1.0 * ss, 1.0 * ss);
				}
				else
				{
					col = float4(0, 0, 0, 0);
				}
				//col = tex2D(_GridText, i.objectUv);
			}
			else if (isInside)
			{
				if (deltaDZoneAngle < 120 && deltaDZoneAngle > 0)
				{
					float ss = smoothstep(0.0, 1.0, deltaDZoneAngle / 120.0);
					col = float4(0, 0.05 + 0.25 * ss, 0, 0.5f * ss);
				}
				else
				{
					col = float4(0, 0.05, 0, 0);
				}
			}
			/*else if (isInside && (frac(i.worldPos.x * _GridScale + _Time.x * 30.0f) < _LineWidth + 0.02))
			{
				float frakk = frac(i.worldPos.x * _GridScale + _Time.x * 30.0f) - 0.1;
				float al = 1 - smoothstep(0, 1.0, frakk / 0.1);
				col = float4(0, 0.50 * al, 0.50 * al, al);
			}
			else if (isInside  && (frac(i.worldPos.y * _GridScale + _Time.x * 30.0f) < _LineWidth + 0.02))
			{
				float frakk = frac(i.worldPos.y * _GridScale + _Time.x * 30.0f) - 0.1;
				float al = 1 - smoothstep(0, 1.0, frakk / 0.1);
				col = float4(0, 0.50 * al, 0.50 * al, al);
			}
			else if (isInside  && (frac(i.worldPos.x * _GridScale + _Time.x * 30.0f) > 1 - _LineWidth - 0.02))
			{
				float frakk = frac(i.worldPos.x * _GridScale + _Time.x * 30.0f) + 0.1;
				float al = 1 - smoothstep(0, 1.0, frakk / 0.1);
				col = float4(0, 0.50 * al, 0.50 * al, al);
			}
			else if (isInside  && (frac(i.worldPos.y * _GridScale + _Time.x * 30.0f) > 1 - _LineWidth - 0.02))
			{
				float frakk = frac(i.worldPos.y * _GridScale + _Time.x * 30.0f) + 0.1;
				float al = 1 - smoothstep(0, 1.0, frakk / 0.1);
				col = float4(0, 0.50 * al, 0.50 * al, al);
			}*/
			else
			{
				col = float4(0, 0.0, 0, 0);
			}
			return col;
		}
			float smoothStep(float edge0, float edge1, float x)
		{
			float t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
			return t * t * (3.0 - 2.0 * t);
		}
ENDCG
        }
    }
}
