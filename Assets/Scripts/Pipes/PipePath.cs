using System.Collections.Generic;
using UnityEngine;

/** @brief Validates directed pipe ports, including entry direction, gaps and loops. */
public static class PipePath
{
    private static readonly Vector2Int[] Steps = {new Vector2Int(-1,0),new Vector2Int(0,1),new Vector2Int(1,0),new Vector2Int(0,-1)};
    public static bool Trace(PipeInfo[][] board, out List<Vector2Int> path, out Vector2Int failure)
    {
        path = new List<Vector2Int>(); failure = new Vector2Int(-1,-1);
        if (board == null) return false;
        var cell = failure;
        for (int r=0;r<board.Length;r++)
            if (board[r] != null) for(int c=0;c<board[r].Length;c++)
                if (board[r][c] != null && board[r][c].type == PipeType.source) cell = new Vector2Int(r,c);
        if (cell.x < 0) return false;
        int travel = (int)board[cell.x][cell.y].direction;
        var visited = new HashSet<Vector2Int>();
        while (cell.x >= 0 && cell.x < board.Length && board[cell.x] != null && cell.y >= 0 && cell.y < board[cell.x].Length)
        {
            failure = cell;
            if (!visited.Add(cell)) return false;
            var pipe = board[cell.x][cell.y];
            if (pipe == null || pipe.type == PipeType.empty) return false;
            if (path.Count > 0)
            {
                if (pipe.type == PipeType.source) return false;
                if (pipe.type == PipeType.sink)
                {
                    if (travel != ((int)pipe.direction + 2) % 4) return false;
                    path.Add(cell); return true;
                }
                if (travel != (int)pipe.direction) return false;
            }
            path.Add(cell);
            travel = pipe.type == PipeType.turn ? ((int)pipe.direction+1)%4 : (int)pipe.direction;
            cell += Steps[travel];
        }
        failure = cell;
        return false;
    }
}
