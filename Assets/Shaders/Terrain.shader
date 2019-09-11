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

        #pragma surface surf Standard fullforwardshadows
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
