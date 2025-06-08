using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveToClosest : ICommand
{
    // --- Cached singletons ---
    private static Tilemap s_ground;
    private static Tilemap s_obstacles;
    private static PlayerLocator s_playerLocator;

    // --- Runtime state ---
    private Transform m_transform;
    private Transform m_target;
    private List<Vector3> m_path = new List<Vector3>();
    private int m_currentIndex;
    private float m_speed;

    // --- Settings ---
    private const float CollisionRadius = 1f;
    private static readonly LayerMask PlayerLayer = LayerMask.GetMask("Player");

    public void Enter(UnitContext ctx)
    {
        Debug.Log("[MoveToClosest] Entering command.");
        CacheTilemaps();
        CachePlayerLocator();

        m_transform = ctx.Transform;
        m_speed = ctx.MoveSpeed;

        LocateNearestPlayer();
        BuildPath();
    }

    public bool Execute(UnitContext ctx, float deltaTime)
    {
        // If we've collided with the player, swap to circle-walk
        if (Physics2D.OverlapCircle(m_transform.position, CollisionRadius, PlayerLayer) is Collider2D hit
            && hit.CompareTag("Player"))
        {
            EnqueueCircleWalk(ctx);
            return true;
        }

        // If no valid path or no target, end this command
        if (m_target == null || m_path.Count == 0)
            return true;

        // Step along the computed path
        MoveAlongPath(deltaTime);
        return false;
    }

    public void Exit(UnitContext ctx)
    {
        m_path.Clear();
        m_currentIndex = 0;
    }

    // --- Caching ---
    private void CacheTilemaps()
    {
        if (s_ground == null || s_obstacles == null)
        {
            var groundGo = GameObject.Find("Ground");
            var obsGo = GameObject.Find("Obstacles");
            if (!groundGo || !obsGo)
                throw new System.Exception("[MoveToClosest] Missing 'GroundTilemap' or 'Obstacles' in scene.");

            s_ground = groundGo.GetComponent<Tilemap>();
            s_obstacles = obsGo.GetComponent<Tilemap>();
        }
    }

    private void CachePlayerLocator()
    {
        if (s_playerLocator == null)
        {
            s_playerLocator = NetworkManager.Singleton.GetComponent<PlayerLocator>();
            if (s_playerLocator == null)
                throw new System.Exception("[MoveToClosest] PlayerLocator not found on NetworkManager.");
        }
    }

    // --- Player target & path setup ---
    private void LocateNearestPlayer()
    {
        m_target = null;
        float bestDist = float.MaxValue;
        foreach (var p in s_playerLocator.GetPlayers())
        {
            float d = Vector2.Distance(m_transform.position, p.position);
            if (d < bestDist)
            {
                bestDist = d;
                m_target = p;
            }
        }
    }

    private void BuildPath()
    {
        m_path.Clear();
        m_currentIndex = 0;
        if (m_target == null) return;

        var start = s_ground.WorldToCell(m_transform.position);
        var goal = s_ground.WorldToCell(m_target.position);

        // Validate both cells
        if (!IsCellValid(start) || !IsCellValid(goal))
            return;

        m_path = FindPath(start, goal);
    }

    private bool IsCellValid(Vector3Int cell)
    {
        if (!s_ground.cellBounds.Contains(cell)) return false;
        if (!s_ground.HasTile(cell)) return false;
        if (s_obstacles.HasTile(cell)) return false;
        return true;
    }

    // --- Movement ---
    private void MoveAlongPath(float dt)
    {
        if (m_currentIndex >= m_path.Count)
            return;

        Vector3 waypoint = m_path[m_currentIndex];
        waypoint.z = m_transform.position.z;
        Vector3 newPos = Vector3.MoveTowards(m_transform.position, waypoint, m_speed * dt);
        m_transform.position = newPos;

        if (Vector2.Distance(newPos, waypoint) < 0.05f)
            m_currentIndex++;
    }

    private void EnqueueCircleWalk(UnitContext ctx)
    {
        float radius = 3f;
        float angularSpeed = Mathf.PI / 4f;
        float duration = 5f;
        Vector3 center = m_transform.position - new Vector3(radius, 0f, 0f);
        ctx.Controller.PushCommand(new CircleWalk(center, radius, angularSpeed, duration), true);
    }

    // --- A* Pathfinding ---
    private List<Vector3> FindPath(Vector3Int start, Vector3Int goal)
    {
        var openSet = new List<Node> { new Node(start, null, 0, Heuristic(start, goal)) };
        var closedSet = new HashSet<Vector3Int>();

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => a.FCost.CompareTo(b.FCost));
            var current = openSet[0];
            if (current.cell == goal)
                return RetracePath(current);

            openSet.RemoveAt(0);
            closedSet.Add(current.cell);

            foreach (var nbr in GetNeighbors(current.cell))
            {
                if (!IsCellValid(nbr) || closedSet.Contains(nbr))
                    continue;

                bool diagonal = nbr.x != current.cell.x && nbr.y != current.cell.y;
                if (diagonal && IsDiagonalBlocked(current.cell, nbr))
                    continue;

                float cost = current.gCost + (diagonal ? 1.41421356f : 1f);
                var existing = openSet.Find(n => n.cell == nbr);
                if (existing == null)
                    openSet.Add(new Node(nbr, current, cost, Heuristic(nbr, goal)));
                else if (cost < existing.gCost)
                {
                    existing.gCost = cost;
                    existing.parent = current;
                }
            }
        }

        return new List<Vector3>();
    }

    private bool IsDiagonalBlocked(Vector3Int from, Vector3Int to)
    {
        var stepX = new Vector3Int(to.x, from.y, to.z);
        var stepY = new Vector3Int(from.x, to.y, to.z);
        return !IsCellValid(stepX) || !IsCellValid(stepY);
    }

    private List<Vector3> RetracePath(Node endNode)
    {
        var stack = new Stack<Node>();
        for (var node = endNode; node != null; node = node.parent)
            stack.Push(node);

        var path = new List<Vector3>();
        while (stack.Count > 0)
        {
            var n = stack.Pop();
            path.Add(s_ground.GetCellCenterWorld(n.cell));
        }
        return path;
    }

    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int c)
    {
        yield return new Vector3Int(c.x + 1, c.y, c.z);
        yield return new Vector3Int(c.x - 1, c.y, c.z);
        yield return new Vector3Int(c.x, c.y + 1, c.z);
        yield return new Vector3Int(c.x, c.y - 1, c.z);
        yield return new Vector3Int(c.x + 1, c.y + 1, c.z);
        yield return new Vector3Int(c.x + 1, c.y - 1, c.z);
        yield return new Vector3Int(c.x - 1, c.y + 1, c.z);
        yield return new Vector3Int(c.x - 1, c.y - 1, c.z);
    }

    private float Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        float D = 1f;
        float D2 = 1.41421356f;
        return D * (dx + dy) + (D2 - 2f * D) * Mathf.Min(dx, dy);
    }

    private class Node
    {
        public Vector3Int cell;
        public Node parent;
        public float gCost, hCost;
        public float FCost => gCost + hCost;
        public Node(Vector3Int cell, Node parent, float gCost, float hCost)
        {
            this.cell = cell;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }
}
