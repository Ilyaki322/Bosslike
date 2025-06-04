using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveToClosest : ICommand
{
    // One-time cache of the ground Tilemap
    private static Tilemap s_ground;

    // Runtime state
    private float m_speed;
    private Transform m_transform;
    private Transform m_target;
    private List<Vector3> m_path;
    private int m_currentIndex;

    // Collision settings
    private const float CollisionRadius = 1f;
    private static readonly LayerMask PlayerLayer = LayerMask.GetMask("Player");

    public void Enter(UnitContext ctx)
    {
        m_speed = ctx.MoveSpeed;
        m_transform = ctx.Transform;
        CacheGroundTilemap();
        LocateNearestPlayer(ctx);
        InitializePath();
    }

    public bool Execute(UnitContext ctx, float deltaTime)
    {
        // Collision detection
        if (Physics2D.OverlapCircle(m_transform.position, CollisionRadius, PlayerLayer) is Collider2D hit
            && hit.CompareTag("Player"))
        {
            EnqueueCircleWalk(ctx);
            return true;
        }
        MoveTowardsTarget(deltaTime);
        return false;
    }

    public void Exit(UnitContext ctx)
    {
        m_path = null;
    }

    // --- Helpers ---

    private void CacheGroundTilemap()
    {
        if (s_ground == null)
        {
            var go = GameObject.Find("GroundTilemap");
            if (go != null)
                s_ground = go.GetComponent<Tilemap>();
            else
                throw new System.Exception("[MoveToClosest] 'GroundTilemap' not found in scene.");
        }
    }

    private void LocateNearestPlayer(UnitContext ctx)
    {
        var players = ctx.PlayerLocator.GetPlayers();
        m_target = null;
        float bestDist = float.MaxValue;
        foreach (var p in players)
        {
            float d = Vector2.Distance((Vector2)m_transform.position, (Vector2)p.position);
            if (d < bestDist)
            {
                bestDist = d;
                m_target = p;
            }
        }
    }

    private void InitializePath()
    {
        m_path = new List<Vector3>();
        m_currentIndex = 0;
        if (s_ground != null && m_target != null)
        {
            Vector3Int start = s_ground.WorldToCell(m_transform.position);
            Vector3Int goal = s_ground.WorldToCell(m_target.position);
            m_path = FindPath(start, goal);
        }
    }

    private void MoveTowardsTarget(float deltaTime)
    {
        if (m_path != null && m_currentIndex < m_path.Count)
            FollowPath(deltaTime);
        else
            StraightLineChase(deltaTime);
    }

    private void FollowPath(float deltaTime)
    {
        Vector3 waypoint = m_path[m_currentIndex];
        waypoint.z = m_transform.position.z;
        Vector3 newPos = Vector3.MoveTowards(
            m_transform.position, waypoint,
            m_speed * deltaTime);
        m_transform.position = newPos;
        if (Vector2.Distance((Vector2)newPos, (Vector2)waypoint) < 0.05f)
            m_currentIndex++;
    }

    private void StraightLineChase(float deltaTime)
    {
        if (m_target == null) return;
        Vector3 goalPos = new Vector3(
            m_target.position.x,
            m_target.position.y,
            m_transform.position.z);
        Vector3 newPos = Vector3.MoveTowards(
            m_transform.position, goalPos,
            m_speed * deltaTime);
        m_transform.position = newPos;
    }

    private void EnqueueCircleWalk(UnitContext ctx)
    {
        float radius = 3f;
        float angularSpeed = Mathf.PI / 4f;
        float duration = 5f;
        Vector3 center = m_transform.position - new Vector3(radius, 0f, 0f);
        ctx.Controller.PushCommand(
            new CircleWalk(center, radius, angularSpeed, duration), true);
    }

    // --- A* Pathfinding with diagonals (Octile distance) ---

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
                if (closedSet.Contains(nbr) || !s_ground.HasTile(nbr))
                    continue;
                bool diagonal = nbr.x != current.cell.x && nbr.y != current.cell.y;
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

    private List<Vector3> RetracePath(Node endNode)
    {
        var stack = new Stack<Node>();
        var node = endNode;
        while (node != null)
        {
            stack.Push(node);
            node = node.parent;
        }
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
        // Orthogonal
        yield return new Vector3Int(c.x + 1, c.y, c.z);
        yield return new Vector3Int(c.x - 1, c.y, c.z);
        yield return new Vector3Int(c.x, c.y + 1, c.z);
        yield return new Vector3Int(c.x, c.y - 1, c.z);
        // Diagonals
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
