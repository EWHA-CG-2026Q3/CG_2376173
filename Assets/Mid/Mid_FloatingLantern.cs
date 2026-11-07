using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class Mid_FloatingLantern : MonoBehaviour
{
    [Header("공전")]
    [SerializeField] Vector3 orbitCenter = Vector3.zero;
    [SerializeField] float orbitRadius = 2f;
    [SerializeField] float orbitSpeedDeg = 8f;
    [SerializeField] float phaseDeg = 0f;

    [Header("높이")]
    [SerializeField] float minHeight = 0.3f;
    [SerializeField] float maxHeight = 2.2f;
    [SerializeField] float riseSeconds = 12f;

    [Header("자전")]
    [SerializeField] float spinSpeedDeg = 30f;
    [SerializeField] float swayDeg = 8f;
    [SerializeField] float scale = 1.4f;

    [Header("불빛")]
    [SerializeField] Transform lightTarget;
    [SerializeField] Vector3 lightLocalOffset = new Vector3(0f, 0.12f, 0f);

    Mesh cachedMesh;
    Vector3[] baseVertices;
    Vector3[] baseNormals;
    Vector3[] verts;
    Vector3[] norms;

    void LateUpdate()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null) return;

        if (mesh != cachedMesh)
        {
            cachedMesh = mesh;
            baseVertices = mesh.vertices;
            baseNormals = mesh.normals;
            verts = new Vector3[baseVertices.Length];
            norms = new Vector3[baseNormals.Length];
        }

        float time = Application.isPlaying ? Time.time : 0f;
        Matrix4x4 M = BuildMatrix(time);

        for (int i = 0; i < baseVertices.Length; i++)
        {
            verts[i] = M.MultiplyPoint(baseVertices[i]);
            norms[i] = M.MultiplyVector(baseNormals[i]).normalized;
        }
        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.RecalculateBounds();

        if (lightTarget != null) lightTarget.position = M.MultiplyPoint(lightLocalOffset);
    }

    public Matrix4x4 BuildMatrix(float time)
    {
        float orbitAngle = phaseDeg + orbitSpeedDeg * time;

        float rise = 0.5f - 0.5f * Mathf.Cos(2f * Mathf.PI * (time / riseSeconds + phaseDeg / 360f));
        float height = Mathf.Lerp(minHeight, maxHeight, rise);

        float spin = spinSpeedDeg * time;
        float sway = swayDeg * Mathf.Sin(time * 1.3f + phaseDeg);

        Matrix4x4 toCenter = Matrix4x4.Translate(orbitCenter);
        Matrix4x4 orbit = Matrix4x4.Rotate(Quaternion.Euler(0f, orbitAngle, 0f));
        Matrix4x4 lift = Matrix4x4.Translate(new Vector3(orbitRadius, height, 0f));
        Matrix4x4 spinAndSway = Matrix4x4.Rotate(Quaternion.Euler(sway, spin, sway * 0.5f));
        Matrix4x4 size = Matrix4x4.Scale(Vector3.one * scale);

        return toCenter * orbit * lift * spinAndSway * size;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        const int n = 48;
        for (int i = 0; i < n; i++)
        {
            Vector3 a = orbitCenter + Quaternion.Euler(0, 360f / n * i, 0) * new Vector3(orbitRadius, minHeight, 0);
            Vector3 b = orbitCenter + Quaternion.Euler(0, 360f / n * (i + 1), 0) * new Vector3(orbitRadius, minHeight, 0);
            Gizmos.DrawLine(a, b);
        }
        Matrix4x4 M = BuildMatrix(Application.isPlaying ? Time.time : 0f);
        Gizmos.DrawWireSphere(M.GetColumn(3), 0.1f);
    }
}
