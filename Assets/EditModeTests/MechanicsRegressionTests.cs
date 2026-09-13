using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/** @brief Regression coverage for actual connection rules and score precision. */
public class MechanicsRegressionTests
{
    private bool Connected(PipeInfo[][] board)
    {
        List<Vector2Int> path; Vector2Int failure;
        return PipePath.Trace(board,out path,out failure);
    }
    private PipeInfo[][] Row(Direction middle,Direction sink=Direction.left)
    {
        return new[] { new[] { new PipeInfo(Direction.right,PipeType.source), new PipeInfo(middle,PipeType.straight), new PipeInfo(sink,PipeType.sink) } };
    }
    [Test] public void MatchingPortsConnect() { Assert.IsTrue(Connected(Row(Direction.right))); }
    [Test] public void BackwardStraightRejectsFlow() { Assert.IsFalse(Connected(Row(Direction.left))); }
    [Test] public void WrongSinkPortRejectsFlow() { Assert.IsFalse(Connected(Row(Direction.right,Direction.up))); }
    [Test] public void GapsRejectFlow() { var board=Row(Direction.right);board[0][1].type=PipeType.empty;Assert.IsFalse(Connected(board)); }
    [Test] public void MissingSourceRejectsFlow() { var board=Row(Direction.right);board[0][0].type=PipeType.empty;Assert.IsFalse(Connected(board)); }
    [Test] public void OutsideBoardRejectsFlow() { Assert.IsFalse(Connected(Row(Direction.up))); }
    [Test] public void ElbowMustAcceptIncomingDirection()
    {
        var board=PipeBoards.Create(new Vector2Int(0,0),new Vector2Int(0,3),new Vector2Int(3,3));
        Assert.IsTrue(Connected(board)); board[0][3].direction=Direction.down; Assert.IsFalse(Connected(board));
    }
    [Test] public void EveryAuthoredBoardHasAValidSolution()
    {
        var go=new GameObject("Test pipe generator");
        try
        {
            var generator=go.AddComponent<PipeGenerator>();
            foreach(var name in new[]{"easyLevels","mediumLevels","hardLevels"})
            {
                var boards=(PipeInfo[][][])typeof(PipeGenerator).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(generator);
                foreach(var board in boards) Assert.IsTrue(Connected(board),name+" contains an unsolvable board");
            }
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }
    [TestCase(12050,"12.05")]
    [TestCase(60010,"60.01")]
    [TestCase(12990,"12.99")]
    public void ScorePreservesFractionalSeconds(int milliseconds,string expected)
    {
        var culture=CultureInfo.CurrentCulture;
        try { CultureInfo.CurrentCulture=new CultureInfo("de-DE");Assert.AreEqual(expected,PuzzleScore.SerializeSeconds(TimeSpan.FromMilliseconds(milliseconds))); }
        finally { CultureInfo.CurrentCulture=culture; }
    }
}
