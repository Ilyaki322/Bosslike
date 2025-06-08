using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TriangleFrontalFunction : AbilityFunction, IDamageCollider
{
    TriangleFrontalData m_data;
    UnitController m_controller;

    MeshFilter m_mesh;
    MeshRenderer m_meshRenderer;

    Vector3 m_middle;
    Vector3 m_p0;
    Vector3 m_p1;
    Vector3 m_p2;

    public event Action<Collider2D[]> OnDetected;

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_data = data as TriangleFrontalData;
        m_controller = GetComponentInParent<UnitController>();
        m_mesh = gameObject.AddComponent<MeshFilter>();
        m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
        m_meshRenderer.sortingOrder = 3;

        var mat = new Material(m_data.Mat);
        m_meshRenderer.material = mat;

        //createTriangle();
    }

    protected override void Use()
    {
        createTriangle();

        m_meshRenderer.material.SetVector("_Direction", (transform.TransformPoint(m_middle) - transform.parent.position).normalized);
        m_meshRenderer.material.SetVector("_Corner", transform.parent.position);

        StartCoroutine(charge());
        
    }

    private IEnumerator charge()
    {
        float time = 0;
        while (time < m_data.ChargeTime)
        {
            time += Time.deltaTime;
            m_meshRenderer.material.SetFloat("_Blend", time / m_data.ChargeTime);
            yield return null;
        }

        m_meshRenderer.material.SetFloat("_Blend", 0f);
        m_mesh.mesh.Clear();
        checkColliders();
        m_ability.HasEnded = true;
    }

    private void checkColliders()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.TransformPoint(m_middle), m_data.Distance * m_data.Distance, m_data.HitLayerMask);
        List<Collider2D> detected = new List<Collider2D>();

        foreach (var hit in hits)
        {
            if (pointInTriangle(hit.transform.position, m_p0, m_p1, m_p2))
            {
                detected.Add(hit);
            }
        }

        if (detected.Count > 0)
        {
            OnDetected?.Invoke(detected.ToArray());
        }
    }

    private void createTriangle()
    {
        float degrees = m_controller.GetRotationAngle();
        float radians = degrees * Mathf.Deg2Rad;
        Vector2 forward = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        m_middle = Vector3.zero + m_data.Distance * (Vector3)forward;
        Vector3 opp = new Vector3((m_middle - Vector3.zero).y, -(m_middle - Vector3.zero).x, 0);

        var vertex = new Vector3[3];
        vertex[0] = Vector3.zero;
        vertex[1] = m_middle + opp * -m_data.Fov;
        vertex[2] = m_middle + opp * m_data.Fov;
        m_p0 = transform.TransformPoint(vertex[0]);
        m_p1 = transform.TransformPoint(vertex[1]);
        m_p2 = transform.TransformPoint(vertex[2]);

        var uv = new Vector2[3];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 0);

        var triangles = new int[3] { 0, 1, 2 };

        Mesh mesh = new Mesh();
        mesh.vertices = vertex;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        m_mesh.mesh = mesh;

        m_meshRenderer.material.SetFloat("_Blend", 0f);
        m_meshRenderer.material.SetFloat("_MaxDistance", m_data.Distance);
    }

    bool pointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float sign(Vector2 a, Vector2 b, Vector2 c) =>
            (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);

        bool b1 = sign(pt, v1, v2) < 0.0f;
        bool b2 = sign(pt, v2, v3) < 0.0f;
        bool b3 = sign(pt, v3, v1) < 0.0f;

        return ((b1 == b2) && (b2 == b3));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.parent.position, transform.TransformPoint(m_middle));
    }
}
