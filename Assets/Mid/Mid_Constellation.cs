using UnityEngine;

[ExecuteAlways]
public class Mid_Constellation : MonoBehaviour
{
    [SerializeField] Material glMaterial;
    [SerializeField] Color starColor = new Color(1f, 0.92f, 0.75f, 1f);
    [SerializeField] Color lineColor = new Color(0.45f, 0.6f, 1f, 1f);
    [SerializeField] float starSize = 0.2f;
    [SerializeField] int circleSegments = 12;

    [Header("잔별")]
    [SerializeField] int backgroundStarCount = 200;
    [SerializeField] int randomSeed = 7;

    Vector3[] dipperStars = new Vector3[]
    {
        new Vector3(39.3f, 50.5f, 63.3f),
        new Vector3(42.4f, 41.8f, 67.5f),
        new Vector3(32.3f, 37.2f, 75.3f),
        new Vector3(26.4f, 42.9f, 74.6f),
        new Vector3(17.8f, 41.1f, 78.1f),
        new Vector3(10.8f, 39.3f, 80.2f),
        new Vector3(5.4f, 29.6f, 84.8f),
    };

    int[] dipperLines = new int[]
    {
        0, 1, 1, 2, 2, 3, 3, 0,
        3, 4, 4, 5, 5, 6,
    };

    Vector3[] backgroundStars;

    void OnEnable()
    {
        Random.InitState(randomSeed);
        backgroundStars = new Vector3[backgroundStarCount];
        for (int i = 0; i < backgroundStarCount; i++)
            backgroundStars[i] = new Vector3(Random.Range(-90f, 90f), Random.Range(25f, 70f), Random.Range(-90f, 90f));
    }

    void OnRenderObject()
    {
        if (glMaterial == null || backgroundStars == null) return;
        glMaterial.SetPass(0);

        float time = Application.isPlaying ? Time.time : 0f;
        Vector3 center = transform.position;

        GL.Begin(GL.LINES);

        GL.Color(lineColor);
        for (int k = 0; k < dipperLines.Length; k += 2)
        {
            GL.Vertex(center + dipperStars[dipperLines[k]]);
            GL.Vertex(center + dipperStars[dipperLines[k + 1]]);
        }

        GL.End();

        GL.Begin(GL.TRIANGLES);

        for (int i = 0; i < dipperStars.Length; i++)
        {
            float twinkle = 0.75f + 0.25f * Mathf.Sin(time * 2f + i);
            GL.Color(starColor * twinkle);
            DrawCircle(center, dipperStars[i], starSize);
        }

        for (int i = 0; i < backgroundStars.Length; i++)
        {
            float twinkle = 0.6f + 0.4f * Mathf.Sin(time * 3f + i);
            GL.Color(new Color(0.8f, 0.85f, 1f) * 0.4f * twinkle);
            DrawCircle(center, backgroundStars[i], starSize * 0.5f);
        }

        GL.End();
    }

    void DrawCircle(Vector3 center, Vector3 offset, float radius)
    {
        Vector3 p = center + offset;
        Vector3 forward = offset.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 up = Vector3.Cross(forward, right);

        for (int i = 0; i < circleSegments; i++)
        {
            float a0 = 360f / circleSegments * i * Mathf.Deg2Rad;
            float a1 = 360f / circleSegments * (i + 1) * Mathf.Deg2Rad;
            GL.Vertex(p);
            GL.Vertex(p + (right * Mathf.Cos(a0) + up * Mathf.Sin(a0)) * radius);
            GL.Vertex(p + (right * Mathf.Cos(a1) + up * Mathf.Sin(a1)) * radius);
        }
    }
}
