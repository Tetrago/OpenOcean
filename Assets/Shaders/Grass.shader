////////////////////
/// DISCLAIMER	 ///
/// NOT MINE	 ///
/// (Tutorial)	 ///
////////////////////

Shader "Unlit/Geometry"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1, 1, 1, 1)
		_BottomColor("Bottom Color", Color) = (1, 1, 1, 1)

		_BendRotationRandom("Bend Rotation Random", Range(0, 1)) = 0.2
		_TessellationUniform("Tessellation Uniform", Range(1, 64)) = 1

		_BladeWidth("Blade Width", Float) = 0.05
		_BladeWidthRandom("Blade Width Random", Float) = 0.02
		_BladeHeight("Blade Height", Float) = 0.5
		_BladeHeightRandom("Blade Height Random", Float) = 0.3

		_WindDistortionMap("Wind Distortion Map", 2D) = "white" {}
		_WindFrequency("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
		_WindStrength("Wind Strength", Float) = 1

		_BladeForward("Blade Forward Amount", Float) = 0.38
		_BladeCurve("Blade Curvature Amount", Range(1, 4)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

			#pragma vertex vert
            #pragma fragment frag
			#pragma geometry geo
			#pragma hull hull
			#pragma domain domain
            
            #pragma multi_compile_fog

			#pragma require geometry
			#pragma require tessell	ation tessHW

			#define BLADE_SEGMENTS 3

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct gOut
			{
				float4 pos_ : SV_POSITION;
				float2 uv_ : TEXTCOORD0;
				UNITY_FOG_COORDS(1)
			};

			#include "CustomTessellation.cginc"

            float4 _TopColor;
			float4 _BottomColor;

			float _BendRotationRandom;

			float _BladeHeight;
			float _BladeHeightRandom;
			float _BladeWidth;
			float _BladeWidthRandom;

			sampler2D _WindDistortionMap;
			float4 _WindDistortionMap_ST;
			float2 _WindFrequency;
			float _WindStrength;

			float _BladeForward;
			float _BladeCurve;

			////////////////////
			/// DISCLAIMER	 ///
			/// NOT MINE	 ///

			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			float3x3 AngleAxis3x3(float angle, float3 axis)
			{
				float c, s;
				sincos(angle, s, c);

				float t = 1 - c;
				float x = axis.x;
				float y = axis.y;
				float z = axis.z;

				return float3x3(
					t * x * x + c, t * x * y - s * z, t * x * z + s * y,
					t * x * y + s * z, t * y * y + c, t * y * z - s * x,
					t * x * z - s * y, t * y * z + s * x, t * z * z + c
					);
			}

			////////////////////

			gOut VertexOutput(float3 pos, float2 uv)
			{
				gOut o;
				o.pos_ = UnityObjectToClipPos(pos);
				o.uv_ = uv;
				UNITY_TRANSFER_FOG(o, o.pos_);
				return o;
			}

			gOut GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 modelMatrix)
			{
				float3 tanPoint = float3(width, forward, height);
				float3 localPosition = vertexPosition + mul(modelMatrix, tanPoint);
				return VertexOutput(localPosition, uv);
			}

			[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
			void geo(triangle v2f IN[3] : SV_POSITION, inout TriangleStream<gOut> tris)
			{
				float4 pos = IN[0].vertex;
				float3 normal = IN[0].normal;
				float4 tangent = IN[0].tangent;
				float3 binormal = cross(normal, tangent) * tangent.w;

				float3x3 tanToLocal = float3x3(tangent.x, binormal.x, normal.x, tangent.y, binormal.y, normal.y, tangent.z, binormal.z, normal.z);
				float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
				float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

				float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
				float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;
				float3 wind = normalize(float3(windSample.x, windSample.y, 0));
				float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);

				float3x3 modelMatrix = mul(mul(mul(tanToLocal, windRotation), facingRotationMatrix), bendRotationMatrix);
				float3x3 modelMatrixFacing = mul(tanToLocal, facingRotationMatrix);

				float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
				float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;

				float forward = rand(pos.yyz) * _BladeForward;

				for(int i = 0; i < BLADE_SEGMENTS; ++i)
				{
					float t = i / (float)BLADE_SEGMENTS;

					float segmentHeight = height * t;
					float segmentWidth = width * (1 - t);

					float segmentForward = pow(t, _BladeCurve) * forward;
					float3x3 transformMatrix = i == 0 ? modelMatrixFacing : modelMatrix;

					tris.Append(GenerateGrassVertex(pos, segmentWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
					tris.Append(GenerateGrassVertex(pos, -segmentWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));
				}

				tris.Append(GenerateGrassVertex(pos, 0, height, forward, float2(0.5, 1), modelMatrix));
			}

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
				o.normal = v.normal;
				o.tangent = v.tangent;
                return o;
            }

            fixed4 frag(gOut i) : SV_Target
            {
				fixed4 col = lerp(_BottomColor, _TopColor, i.uv_.y);
				UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDCG
        }
    }
}
