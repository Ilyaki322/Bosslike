using UnityEngine;

public class FrontalTest : MonoBehaviour
{
    private Vector3[] m_vertex;
    private Vector2[] m_uv;
    private int[] m_triangles;

    [SerializeField] Material m_material;
    [SerializeField, Range(0, 1)] float m_blend;

    private void Start()
    {
        m_vertex = new Vector3[3];
        m_vertex[0] = Vector3.zero;
        m_vertex[1] = new Vector3(0f, 5f, 0f);
        m_vertex[2] = new Vector3(5f, 0f, 0f);

        m_uv = new Vector2[3];
        m_uv[0] = new Vector2(0, 0);
        m_uv[1] = new Vector2(0, 1);
        m_uv[2] = new Vector2(1, 0);

        m_triangles = new int[3] { 0, 1, 2 };

        Mesh mesh = new Mesh();
        mesh.vertices = m_vertex;
        mesh.uv = m_uv;
        mesh.triangles = m_triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        gameObject.AddComponent<MeshFilter>().mesh = mesh;

        var mr = gameObject.AddComponent<MeshRenderer>();
        mr.sortingOrder = 10;
        var mat = new Material(m_material);
        mr.material = mat;

        Vector3 worldV0 = transform.TransformPoint(m_vertex[0]);
        Vector3 worldV1 = transform.TransformPoint(m_vertex[1]);
        Vector3 worldV2 = transform.TransformPoint(m_vertex[2]);

        Vector3 median = (0.5f * (worldV1 + worldV2)) - worldV0;

        mat.SetVector("_Corner", worldV0);
        mat.SetVector("_Direction", median.normalized);
        mat.SetFloat("_MaxDistance", median.magnitude);
    }

    private void Update()
    {
        GetComponent<MeshRenderer>().material.SetFloat("_Blend", m_blend); 
    }
}
