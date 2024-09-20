Shader "F3P/DamageMarkerBaseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // Main texture (not necessarily used)
        [HDR] _Color ("Color", Color) = (1,1,1,1)  // Central color
        _FresnelPower("Fresnel Power", Range(0, 10)) = 3  // Controls the intensity of the Fresnel effect
        _Alpha ("Alpha", Range(0, 1)) = 1.0  // Transparency slider for overall control
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha  // Standard alpha blending
        LOD 100
        Cull Back
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fresnel : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _FresnelPower;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Calculate the view direction
                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));

                // Calculate Fresnel effect based on view direction and vertex normal
                o.fresnel = pow(1.0 - saturate(dot(viewDir, v.normal)), _FresnelPower);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Fresnel-based transparency
                fixed4 baseColor = _Color;
                baseColor.a *= _Alpha;

                // Interpolate between the central color and transparency using the Fresnel effect
                fixed4 outputColor = lerp(baseColor, float4(0, 0, 0, 0), i.fresnel);

                return outputColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}