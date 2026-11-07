using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_LotusMesh : MonoBehaviour
{
    [System.Serializable]
    public struct PetalLayer
    {
        [Range(3, 24)] public int count;
        public float length;
        public float width;
        [Range(0, 90)] public float tiltDeg;
        public float angleOffsetDeg;
    }

    [SerializeField] PetalLayer[] layers = new PetalLayer[]
    {
        new PetalLayer { count = 10, length = 0.30f, width = 0.14f, tiltDeg = 18f, angleOffsetDeg = 0f },
        new PetalLayer { count = 8,  length = 0.26f, width = 0.13f, tiltDeg = 45f, angleOffsetDeg = 22.5f },
        new PetalLayer { count = 6,  length = 0.20f, width = 0.11f, tiltDeg = 70f, angleOffsetDeg = 10f },
    };

    [Header("꽃잎")]
    [SerializeField, Range(2, 16)] int rows = 8;
    [SerializeField] float tipCurl = 0.06f;
    [SerializeField] float cupDepth = 0.35f;

    [Header("연밥")]
    [SerializeField, Range(3, 32)] int podSides = 12;
    [SerializeField] float podRadius = 0.06f;
    [SerializeField] float podHeight = 0.07f;

    Mesh mesh;
    bool dirty;

    void OnEnable() => Build();
    void OnValidate() => dirty = true;
    void Update() { if (dirty) Build(); }
    void OnDisable() { if (mesh != null) DestroyImmediate(mesh); }

    void Build()
    {
        dirty = false;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (PetalLayer layer in layers)
            for (int i = 0; i < layer.count; i++)
                AddPetal(vertices, triangles, 360f / layer.count * i + layer.angleOffsetDeg, layer);

        AddPod(vertices, triangles);

        if (mesh != null) DestroyImmediate(mesh);
        mesh = new Mesh { name = "Lotus", hideFlags = HideFlags.DontSave };
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void AddPetal(List<Vector3> vertices, List<int> triangles, float angleDeg, PetalLayer layer)
    {
        float a = angleDeg * Mathf.Deg2Rad;
        Vector3 outward = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
        Vector3 side = new Vector3(-Mathf.Sin(a), 0, Mathf.Cos(a));
        float tilt = layer.tiltDeg * Mathf.Deg2Rad;

        int first = vertices.Count;
        for (int j = 0; j <= rows; j++)
        {
            float t = (float)j / rows;
            float along = layer.length * t;
            float radial = along * Mathf.Cos(tilt) + podRadius * 0.5f;
            float up = along * Mathf.Sin(tilt) + tipCurl * t * t;
            float halfWidth = layer.width * 0.5f * Mathf.Sin(Mathf.PI * Mathf.Pow(t, 0.75f));

            Vector3 center = outward * radial + Vector3.up * up;
            Vector3 lift = Vector3.up * (halfWidth * cupDepth);
            vertices.Add(center - side * halfWidth + lift);
            vertices.Add(center);
            vertices.Add(center + side * halfWidth + lift);
        }

        for (int j = 0; j < rows; j++)
        {
            for (int k = 0; k < 2; k++)
            {
                int p = first + j * 3 + k;
                int q = p + 3;
                triangles.AddRange(new int[] { p, p + 1, q });
                triangles.AddRange(new int[] { p + 1, q + 1, q });
            }
        }
    }

    void AddPod(List<Vector3> vertices, List<int> triangles)
    {
        int first = vertices.Count;
        for (int i = 0; i < podSides; i++)
        {
            float angle = 360f / podSides * i * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            vertices.Add(dir * podRadius * 0.6f);
            vertices.Add(dir * podRadius + Vector3.up * podHeight);
        }
        int top = vertices.Count;
        vertices.Add(Vector3.up * podHeight);

        for (int i = 0; i < podSides; i++)
        {
            int b0 = first + i * 2, t0 = b0 + 1;
            int b1 = first + (i + 1) % podSides * 2, t1 = b1 + 1;
            triangles.AddRange(new int[] { b0, t0, b1 });
            triangles.AddRange(new int[] { b1, t0, t1 });
            triangles.AddRange(new int[] { top, t1, t0 });
        }
    }
}
