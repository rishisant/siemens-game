using System.Collections.Generic;
using UnityEngine;

/** @brief Builds solvable arrow circuits from clockwise paths on a regular grid. */
public static class PipeBoards
{
    public static PipeInfo[][] Create(params Vector2Int[] corners)
    {
        var route = new List<Vector2Int> { corners[0] };
        for(int i=1;i<corners.Length;i++)
        {
            var delta = corners[i] - corners[i-1];
            var step = new Vector2Int(System.Math.Sign(delta.x),System.Math.Sign(delta.y));
            int length = Mathf.Abs(delta.x)+Mathf.Abs(delta.y);
            for(int j=0;j<length;j++) route.Add(route[route.Count-1]+step);
        }
        var board = new PipeInfo[5][];
        for(int r=0;r<5;r++) { board[r]=new PipeInfo[8]; for(int c=0;c<8;c++) board[r][c]=new PipeInfo(Direction.up,PipeType.empty); }
        for(int i=0;i<route.Count;i++)
        {
            Direction incoming = i>0 ? DirectionOf(route[i]-route[i-1]) : DirectionOf(route[1]-route[0]);
            Direction outgoing = i<route.Count-1 ? DirectionOf(route[i+1]-route[i]) : incoming;
            var type = i==0 ? PipeType.source : i==route.Count-1 ? PipeType.sink : incoming==outgoing ? PipeType.straight : PipeType.turn;
            if(type==PipeType.turn && ((int)incoming+1)%4!=(int)outgoing) throw new System.ArgumentException("Arrow elbows require clockwise turns.");
            var direction = type==PipeType.sink ? (Direction)(((int)incoming+2)%4) : incoming;
            board[route[i].x][route[i].y]=new PipeInfo(direction,type);
        }
        return board;
    }
    private static Direction DirectionOf(Vector2Int delta)
    {
        if(delta.x<0)return Direction.up; if(delta.x>0)return Direction.down;
        return delta.y>0 ? Direction.right : Direction.left;
    }
}
