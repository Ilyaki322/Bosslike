using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private Tilemap m_tilemap;
    [SerializeField] private Tilemap m_obstacles;

    private Dictionary<Vector3Int, int> m_costs = new();
    private Dictionary<Vector3Int, Vector3> m_directions = new();
    private Vector3 m_target;

    private int m_frameCounter = 0;
    private int m_frameUpdate = 60;

    private void Update()
    {
        m_frameCounter++;
        if (m_frameCounter >= m_frameUpdate)
        {
            m_frameCounter = 0;
            RebuildFlowField();
        }
    }

    public Vector3 GetFlowDirection(Vector3 worldPos)
    {
        Vector3Int cell = m_tilemap.WorldToCell(worldPos);
        return m_directions.TryGetValue(cell, out var dir) ? dir : Vector3.zero;
    }

    public GameObject GetTarget() => GameObject.FindWithTag("Player");

    public void RebuildFlowField()
    {
        var t = GameObject.FindWithTag("Player");
        if (t == null) return;
        else m_target = t.transform.position;

        m_costs.Clear();
        m_directions.Clear();

        Vector3Int targetCell = m_tilemap.WorldToCell(m_target);

        Queue<Vector3Int> queue = new();
        queue.Enqueue(targetCell);
        m_costs[targetCell] = 0;

        // Directions for neighbors (4-way or 8-way)
        Vector3Int[] directions = {
            new(1, 0, 0), new(-1, 0, 0),
            new(0, 1, 0), new(0, -1, 0),
            new(1, 1, 0), new(-1, -1, 0),
            new(-1, 1, 0), new(1, -1, 0)
        };

        // Step 1: Flood Fill - Assign Cost
        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            int currentCost = m_costs[current];

            foreach (var dir in directions)
            {
                Vector3Int neighbor = current + dir;
                if (!m_tilemap.HasTile(neighbor)) continue;
                if (m_obstacles.HasTile(neighbor)) continue;
                if (m_costs.ContainsKey(neighbor)) continue;

                m_costs[neighbor] = currentCost + 1;
                queue.Enqueue(neighbor);
            }
        }

        // Step 2: Assign Flow Directions
        foreach (var kvp in m_costs)
        {
            Vector3Int cell = kvp.Key;
            int cost = kvp.Value;

            Vector3Int bestDir = Vector3Int.zero;
            int bestCost = cost;

            foreach (var dir in directions)
            {
                Vector3Int neighbor = cell + dir;
                if (m_costs.TryGetValue(neighbor, out int neighborCost) && neighborCost < bestCost)
                {
                    bestCost = neighborCost;
                    bestDir = dir;
                }
            }

            m_directions[cell] = bestDir == Vector3Int.zero
                ? Vector3.zero
                : m_tilemap.CellToWorld(cell + bestDir) - m_tilemap.CellToWorld(cell);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        foreach (var kvp in m_directions)
        {
            Vector3 worldPos = m_tilemap.GetCellCenterWorld(kvp.Key);
            Vector3 dir = kvp.Value;
            Gizmos.DrawLine(worldPos, worldPos + dir.normalized * 0.4f);
        }

        // Draw target
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_target, 0.2f);
    }
}

