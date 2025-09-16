using System.Collections.Generic;  // List<T> や IEnumerable<T> を使用
using UnityEngine;                  // Vector2, Mathf, Random, Vector2Int などの Unity API

public class PoissonDiskSampler2DCircle
{
    private const int k = 30;

    private readonly float radius;
    private readonly float radius2;
    private readonly float cellSize;
    private Vector2[,] grid;

    readonly List<Vector2> active = new();
    private readonly List<Vector2> samples = new();


    public PoissonDiskSampler2DCircle(float radius)
    {
        this.radius = radius;
        this.radius2 = radius * radius;
        this.cellSize = radius / Mathf.Sqrt(2);
        int gridSize = Mathf.CeilToInt((radius * 2) / cellSize);
        grid = new Vector2[gridSize, gridSize];
    }


    public IEnumerable<Vector2> Samples()
    {
        // 初回サンプル（円内ランダム）
        AddSample(Random.insideUnitCircle * radius);

        while (active.Count > 0)
        {
            var index = Random.Range(0, active.Count);
            var center = active[index];
            bool found = false;

            for (int i = 0; i < k; i++)
            {
                var dir = Random.insideUnitCircle.normalized;
                var r = Random.Range(radius, 2 * radius);
                var candidate = center + dir * r;

                if (candidate.sqrMagnitude > radius2) continue;
                if (!IsFarEnough(candidate)) continue;

                AddSample(candidate);
                found = true;
                break;
            }

            if (!found) active.RemoveAt(index);
        }

        return samples;
    }
    
    private bool IsFarEnough(Vector2 pt)
    {
        var gp = GridPos(pt);
        int range = 2;
        for (int y = Mathf.Max(0, gp.y - range); y <= Mathf.Min(grid.GetLength(1) - 1, gp.y + range); y++)
            for (int x = Mathf.Max(0, gp.x - range); x <= Mathf.Min(grid.GetLength(0) - 1, gp.x + range); x++)
                if (grid[x, y] != default && (grid[x, y] - pt).sqrMagnitude < radius2)
                    return false;
        return true;
    }

    void AddSample(Vector2 pt)
    {
        active.Add(pt);
        samples.Add(pt);
        var gp = GridPos(pt);
        grid[gp.x, gp.y] = pt;

    }

    Vector2Int GridPos(Vector2 pt)
    {
        int x = Mathf.FloorToInt((pt.x + radius) / cellSize);
        int y = Mathf.FloorToInt((pt.y + radius) / cellSize);
        return new(x, y);
    }
}

