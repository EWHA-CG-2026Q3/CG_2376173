using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_LotusLeafMesh : MonoBehaviour
{
    [SerializeField, Range(6, 64)] int segments = 28;
    [SerializeField] float radius = 0.35f;
    [SerializeField, Range(0, 90)] float notchDeg = 24f;
    [SerializeField] float rimLift = 0.025f;
    [SerializeField] float waveHeight = 0.012f;
    [SerializeField, Range(1, 12)] int waveCount = 5;

    Mesh mesh;
    bool dirty;

    void OnEnable() => Build();
    void OnValidate() => dirty = true;
    void Update() { if (dirty) Build(); }
    void OnDisable() { if (mesh != null) DestroyImmediate(mesh); }

    void Build()
    {
        dirty = false;
        float span = 360f - notchDeg;
        int ring = segments + 1;

        Vector3[] vertices = new Vector3[1 + ring * 2];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < ring; i++)
        {
            vertices[1 + i] = LeafPoint(i, span, radius * 0.55f, rimLift * 0.3f, 0f);
            vertices[1 + ring + i] = LeafPoint(i, span, radius, rimLift, waveHeight);
        }

        int[] triangles = new int[segments * 9];
        for (int i = 0; i < segments; i++)
        {
            int a = 1 + i, b = 1 + i + 1;
            int c = 1 + ring + i, d = 1 + ring + i + 1;
            int t = i * 9;
            triangles[t + 0] = 0; triangles[t + 1] = b; triangles[t + 2] = a;
            triangles[t + 3] = a; triangles[t + 4] = b; triangles[t + 5] = c;
            triangles[t + 6] = b; triangles[t + 7] = d; triangles[t + 8] = c;
        }

        if (mesh != null) DestroyImmediate(mesh);
        mesh = new Mesh { name = "LotusLeaf", hideFlags = HideFlags.DontSave };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    Vector3 LeafPoint(int i, float span, float r, float lift, float wave)
    {
        float deg = notchDeg * 0.5f + span / segments * i;
        float rad = deg * Mathf.Deg2Rad;
        float y = lift + wave * Mathf.Sin(rad * waveCount);
        return new Vector3(Mathf.Cos(rad) * r, y, Mathf.Sin(rad) * r);
    }
}
