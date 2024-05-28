using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollider : MonoBehaviour
{
    // Copies PolygonColider's points, and generate new EdgeCollider
    void Awake()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        if (poly == null) {
            poly = gameObject.AddComponent<PolygonCollider2D>();
        }

        List<Vector2> points = new List<Vector2>();
        foreach (Vector2 p in poly.points)
                points.Add(p);
        points.Add(new Vector2(points[0].x, points[0].y));
        
        EdgeCollider2D edge = gameObject.AddComponent<EdgeCollider2D>();
        edge.points = points.ToArray();

        Destroy(poly);
    }
}
