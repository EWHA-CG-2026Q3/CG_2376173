Shader "CG/S17_MyTexture"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _BaseMap;

            // vert: 정점의 UV 좌표(uv)를 그대로 uvOut으로 넘기고, 위치는 Clip Space로 바꿈
            void vert(float3 positionOS : POSITION,
                      float2 uv : TEXCOORD0,
                      out float2 uvOut : TEXCOORD0,
                      out float4 positionCS : SV_POSITION)
            {
                uvOut = uv;
                positionCS = TransformObjectToHClip(positionOS);
            }

            // frag: GPU가 보간한 UV 좌표로 텍스처에서 읽을 위치 p를 정하고, 그 texel 색을 돌려줌
            float4 frag(float2 uv : TEXCOORD0) : SV_Target
            {
                // 실습 기본: 그림 그대로
                // float2 p = uv;

                // 미션 1: B2 한 칸만 표면 전체에 (B2: u 0.25~0.5, v 0.5~0.75)
                // float2 p = uv * 0.25 + float2(0.25, 0.5);

                // 미션 2: 그림 전체를 표면의 왼쪽 아래 1/4에, 나머지는 회색
                // float2 p = uv * 2;
                // if (p.x > 1 || p.y > 1) return float4(0.5, 0.5, 0.5, 1);

                // 과제: D3 한 칸만 표면 전체에
                // - D3: 3번째 열 -> u 0.5~0.75, 맨 아래 D행 -> v 0~0.25
                // - x 0.25: 0~1 범위를 한 칸 크기로 줄임, + (0.5, 0): D3 칸의 왼쪽 아래로 옮김
                float2 p = uv * 0.25 + float2(0.5, 0);

                return tex2D(_BaseMap, p);
            }
            ENDHLSL
        }
    }
}
