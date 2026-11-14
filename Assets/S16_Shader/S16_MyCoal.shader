Shader "CG/S16_MyCoal"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)                 // 1. 칸 만들기
        [HDR] _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)   // 1. 칸 만들기 (HDR: 1을 넘는 밝기)
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BaseColor;      // 2. 값 받기 (Properties와 같은 이름)
            float4 _EmissionColor;  // 2. 값 받기

            float4 vert(float3 positionOS : POSITION) : SV_POSITION
            {
                return TransformObjectToHClip(positionOS);
            }

            float4 frag() : SV_Target
            {
                // 3. 값 쓰기: 기본 색에 Emission의 밝은 값을 더함, 알파는 1
                return float4(_BaseColor.rgb + _EmissionColor.rgb, 1);
            }
            ENDHLSL
        }
    }
}
