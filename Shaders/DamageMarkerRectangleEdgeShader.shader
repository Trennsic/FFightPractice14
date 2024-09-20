Shader "F3P/RectEdgeHighlightShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _HighlightColor ("Highlight Color", Color) = (1, 1, 1, 1)
        _EffectStrength ("Effect Strength", Range(0, 1)) = 0.5
        _LightStrength ("Light Strength", Range(0, 2)) = 1.0
        _OverallAlpha ("Overall Alpha", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        Cull Off
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
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            float4 _MainColor;
            float4 _HighlightColor;
            float _EffectStrength;
            float _LightStrength;
            float _OverallAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Calculate the distance from the object's center
                float distance = length(i.worldPos);
                
                // Remap the distance to effect strength
                float edgeEffect = 1.0 - saturate(distance / _EffectStrength);
                
                // Calculate highlight intensity based on light strength
                float intensity = edgeEffect * _LightStrength;

                // Mix highlight color with the calculated intensity
                float4 highlight = _HighlightColor * intensity;

                // Blend the main color and highlight color
                float4 finalColor = _MainColor + highlight;

                // Apply the overall alpha to control transparency
                finalColor.a *= _OverallAlpha;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
