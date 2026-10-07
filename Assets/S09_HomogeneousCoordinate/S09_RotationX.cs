using UnityEngine;

// 실습: RotationZMatrixRaw()를 참고해 RotationXMatrixRaw()를 완성할 것
// 확인: Transform의 Rotation x에 같은 각도를 넣은 다이아몬드와 겹치는지 비교
// (이 오브젝트의 Transform은 기본값 Position 0, Rotation 0, Scale 1을 유지)
[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh))]
public class S09_RotationX : MonoBehaviour
{
    [SerializeField] float angle = 30f;   // x축 회전 각도, 도 단위

    // 과제: 높이(y)에 비례해 x축 방향으로 밀리는 기울이기(shear) 행렬
    // 점 (x, y, z) → (x + k·y, y, z)
    [SerializeField] bool useShear = true;  // 켜면 회전 대신 기울이기 적용
    [SerializeField] float k = 0.8f;        // (학번 끝자리 3 + 1) ÷ 5

    DiamondMesh diamondMesh;

    void OnEnable()
    {
        diamondMesh = GetComponent<DiamondMesh>();
        LogTopVertex();
    }

    void Update()
    {
        if (diamondMesh == null || diamondMesh.BaseVertices == null) return;

        float[,] R = useShear ? ShearMatrixRaw(k) : RotationXMatrixRaw(angle);
        Vector3[] baseVertices = diamondMesh.BaseVertices;
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = FromHomogeneous(MultiplyMatrixVectorRaw(R, ToHomogeneous(baseVertices[i])));
        diamondMesh.SetVertices(verts);
    }

    // 참고: z축 회전 행렬 (e₃는 그대로, e₁과 e₂가 돎)
    float[,] RotationZMatrixRaw(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new float[,] {
            { c,  -s,  0f, 0f },
            { s,   c,  0f, 0f },
            { 0f,  0f, 1f, 0f },
            { 0f,  0f, 0f, 1f }
        };
    }

    // x축 회전 행렬 (e₁은 그대로, e₂와 e₃가 돎)
    float[,] RotationXMatrixRaw(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        // TODO: x축 회전 행렬을 float[4,4]로 반환
        //       (4열과 4행은 z축 회전과 같음)
        //       완성하면 아래의 임시 반환(단위행렬)을 지울 것

        return new float[,] {   // 임시: 아무 변환도 하지 않는 단위행렬
            { 1f, 0f, 0f, 0f },
            { 0f, 1f, 0f, 0f },
            { 0f, 0f, 1f, 0f },
            { 0f, 0f, 0f, 1f }
        };
    }

    // Inspector에서 k를 바꿀 때마다 Console에 바로 출력됨
    void OnValidate()
    {
        LogTopVertex();
    }

    // 꼭대기 정점 (0.5, 1, 0.5)의 기울이기 결과를 출력
    void LogTopVertex()
    {
        if (!useShear) return;
        Vector3 top = new Vector3(0.5f, 1f, 0.5f);
        Vector3 result = FromHomogeneous(MultiplyMatrixVectorRaw(ShearMatrixRaw(k), ToHomogeneous(top)));
        Debug.Log($"[Shear k = {k}] 꼭대기 정점 {top} → {result}");
    }

    // 1열: e₁ 그대로 / 2열: e₂ → (k, 1, 0) / 3열: e₃ 그대로 / 4열: 원점 그대로
    float[,] ShearMatrixRaw(float k)
    {
        return new float[,] {
            { 1f, k,  0f, 0f },
            { 0f, 1f, 0f, 0f },
            { 0f, 0f, 1f, 0f },
            { 0f, 0f, 0f, 1f }
        };
    }

    Vector4 ToHomogeneous(Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 1f);
    }

    Vector3 FromHomogeneous(Vector4 h)
    {
        return new Vector3(h.x, h.y, h.z);
    }

    Vector4 MultiplyMatrixVectorRaw(float[,] M, Vector4 v)
    {
        float[] input = { v.x, v.y, v.z, v.w };
        float[] result = new float[4];
        for (int row = 0; row < 4; row++)
            for (int col = 0; col < 4; col++)
                result[row] += M[row, col] * input[col];
        return new Vector4(result[0], result[1], result[2], result[3]);
    }
}