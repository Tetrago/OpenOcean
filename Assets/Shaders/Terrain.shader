Shader "Custom/Terrain"
{
    Properties
    {
		_Dim("Dim", Float) = 0.4
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0

        struct Input
        {
			float3 worldPos;
        };

		float _Dim;
		
		float3 size_;
		
		float3 Hue(float h)
		{
			float r = abs(h * 6 - 3) - 1;
			float g = 2 - abs(h * 6 - 2);
			float b = 2 - abs(h * 6 - 4);
			return saturate(float3(r, g, b));
		}

		float rand(float2 co)
		{
			return 0.5 + (frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453)) * 0.5;
		}

		void vert(inout appdata_full v)
		{
			float3 pos = v.vertex;

			float blend = 1;
			for(uint i = 4u; i > 0u; --i)
			{
				if(pos.x < i || pos.x >= size_.x - i || pos.y < i || pos.y >= size_.y - i || pos.z < i || pos.z >= size_.z - i)
					blend = 1.0 - (1.0 / i);
			}

			v.vertex.x += rand(v.vertex.yz) * blend;
			v.vertex.y += rand(v.vertex.xz) * blend;
			v.vertex.z += rand(v.vertex.yx) * blend;
		}
		
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
			o.Albedo = Hue(IN.worldPos.y / 360) * _Dim;
			o.Metallic = 0;
			o.Smoothness = 0;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
