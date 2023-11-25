using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class MakeCircleCollider : MonoBehaviour
{
    public int edgeColliderDetail = 50;
    public int lineRendererDetail = 50;
    public float circleDegrees = 360f;
    public float edgeColliderWidth = 1f;
    public float lineRendererWidth = 0.1f;

    private EdgeCollider2D edgeCollider;
    private LineRenderer lineRenderer;

    void Awake()
    {
        InitializeComponents();
        DrawCircle();
    }

    void InitializeComponents()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void DrawCircle()
    {
        DrawEdgeColliderCircle();
        DrawLineRendererCircle();
    }

    void DrawEdgeColliderCircle()
    {
        Vector2[] edgePoints = new Vector2[edgeColliderDetail + 1]; // +1 for closure
        float angleStep = circleDegrees / edgeColliderDetail;
        float radius = 1f;

        for (int i = 0; i <= edgeColliderDetail; i++) // <= for closure
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            edgePoints[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }

        edgeCollider.points = edgePoints;
        edgeCollider.edgeRadius = edgeColliderWidth;
    }

    void DrawLineRendererCircle()
    {
        lineRenderer.positionCount = lineRendererDetail + 1; // +1 for closure
        lineRenderer.startWidth = lineRendererWidth;
        lineRenderer.endWidth = lineRendererWidth;

        float angleStep = circleDegrees / lineRendererDetail;
        float radius = 1f;

        for (int i = 0; i <= lineRendererDetail; i++) // <= for closure
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            lineRenderer.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        InitializeComponents();
        DrawCircle();
    }
#endif
}
