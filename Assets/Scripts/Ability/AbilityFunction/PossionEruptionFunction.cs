using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PossionEruptionFunction : AbilityFunction
{
    [SerializeField] PossionEruptionData m_data;
    
    private NetworkObjectPool m_pool;
    private Tilemap m_walkable;
    private Tilemap m_obstacle;

    protected override void Use()
    {
        List<Vector2> positions = GeneratePoissonDiskPoints(1, m_data.Radius, m_data.MinRange, m_data.ProjectileAmount);
        foreach (Vector2 pos in positions)
        {
            Vector3 worldPos = transform.position + new Vector3(pos.x, pos.y, 0);
            var p = m_pool.GetNetworkObject(m_data.Projectile, worldPos, Quaternion.identity);
            p.GetComponent<NetworkObject>().Spawn(true);
            p.GetComponent<Projectile>().Config(322, transform.position, worldPos);
        }

        m_ability.HasEnded = true;
    }

    public override void Init(AbilityData data)
    {
        base.Init(data);
        m_walkable = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();
        m_obstacle = GameObject.Find("Obstacles").GetComponent<Tilemap>();
        m_data = data as PossionEruptionData;
        m_pool = GameObject.FindWithTag("NetworkObjectPool").GetComponent<NetworkObjectPool>();
    }

    private List<Vector2> GeneratePoissonDiskPoints(float minRadius, float maxRadius, float minDist, int targetCount, int maxAttempts = 30)
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2> { Vector2.zero };

        while (spawnPoints.Count > 0 && points.Count < targetCount)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool added = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
                float dist = UnityEngine.Random.Range(minDist, minDist * 2);
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                float candidateDist = candidate.magnitude;
                if (candidateDist < minRadius || candidateDist > maxRadius)
                    continue;

                Vector3 worldPos = transform.position + new Vector3(candidate.x, candidate.y, 0);
                Vector3Int cell = m_walkable.WorldToCell(worldPos);

                if (!m_walkable.HasTile(cell) || m_obstacle.HasTile(cell))
                    continue;

                bool valid = true;
                foreach (Vector2 p in points)
                {
                    if ((p - candidate).sqrMagnitude < minDist * minDist)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }
}
