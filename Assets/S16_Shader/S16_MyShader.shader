Shader "CG/S16_MyShader"
{
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // vert: 정점마다 실행, Object Space 위치에 P x V x M을 곱해 Clip Space 좌표를 돌려줌
            float4 vert(float3 positionOS : POSITION) : SV_POSITION
            {
                return TransformObjectToHClip(positionOS);
            }

            // frag: 픽셀마다 실행, 그 픽셀의 색(R, G, B, A)을 돌려줌
            // - 실습 1 미션: 1을 넘는 값(번짐)을 쓰지 않아 Bloom이 생기지 않는 빨강
            float4 frag() : SV_Target
            {
                return float4(0.8, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}
