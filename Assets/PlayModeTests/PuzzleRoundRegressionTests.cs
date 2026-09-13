using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/** @brief Exercises round transitions, matching and restart in the real scenes. */
public class PuzzleRoundRegressionTests
{
    [UnityTest]
    public IEnumerator WiresPauseBetweenRoundsAndResetConnections()
    {
        SceneManager.LoadScene("WireGame"); yield return null; yield return null;
        var generator=Object.FindObjectOfType<WireGenerator>();
        Assert.AreEqual(1,generator.level);
        for(int round=1;round<=6;round++)
        {
            var sockets=Object.FindObjectsOfType<PlugBehavior>();
            var wires=Object.FindObjectsOfType<PoweredWireBehavior>();
            Assert.AreEqual(round,sockets.Length);
            Assert.AreEqual(round,wires.Length);
            foreach(var wire in wires)
            {
                foreach(var socket in sockets)
                {
                    if(socket.plugS.connectionId!=wire.powerWireS.connectionId) Assert.IsFalse(socket.TryConnect(wire));
                }
                foreach(var socket in sockets)
                    if(socket.plugS.connectionId==wire.powerWireS.connectionId) Assert.IsTrue(socket.TryConnect(wire));
            }
            yield return null; yield return null;
            Assert.IsFalse(generator.gameStopwatch.IsRunning);
            Assert.IsFalse(generator.levelStopwatch.IsRunning);
            Assert.AreEqual(0,Object.FindObjectsOfType<PlugStats>().Length);
            if(round<6) { generator.StartLevel(); yield return null; yield return null; Assert.IsTrue(generator.gameStopwatch.IsRunning); }
        }
        Assert.AreEqual(6,generator.level);
        generator.gameOverManager.RestartButton(); yield return null; yield return null;
        Assert.AreEqual(1,Object.FindObjectOfType<WireGenerator>().level);
    }
    [UnityTest]
    public IEnumerator PipesFinishAllBoardsAndRestart()
    {
        SceneManager.LoadScene("PipeGame"); yield return null; yield return null;
        var generator=Object.FindObjectOfType<PipeGenerator>();
        var flags=BindingFlags.Instance|BindingFlags.NonPublic;
        foreach(var name in new[]{"easyLevels","mediumLevels","hardLevels"})
        {
            var current=(PipeInfo[][])typeof(PipeGenerator).GetField("currentLevel",flags).GetValue(generator);
            var templates=(PipeInfo[][][])typeof(PipeGenerator).GetField(name,flags).GetValue(generator);
            PipeInfo[][] solution=null;
            foreach(var t in templates)
            {
                bool matches=true;
                for(int r=0;r<5;r++) for(int c=0;c<8;c++) if(t[r][c].type!=current[r][c].type)matches=false;
                if(matches)solution=t;
            }
            Assert.IsNotNull(solution);
            foreach(var pipe in Object.FindObjectsOfType<PipeBehavior>())
                for(int i=0;i<4 && pipe.pipeInfo.direction!=solution[pipe.row][pipe.col].direction;i++) pipe.SendMessage("RotatePipe");
            Assert.IsTrue(generator.CheckSolution(current));
            generator.CheckSolutionButton();
            float deadline=Time.realtimeSinceStartup+8;
            while((bool)typeof(PipeGenerator).GetField("checking",flags).GetValue(generator) && Time.realtimeSinceStartup<deadline)yield return null;
            Assert.IsFalse((bool)typeof(PipeGenerator).GetField("checking",flags).GetValue(generator));
        }
        Assert.IsFalse(generator.gameTime.IsRunning);
        Assert.AreEqual(0,Object.FindObjectsOfType<PipeBehavior>().Length);
        generator.pipeGameOverManager.RestartButton();yield return null;yield return null;
        Assert.Greater(Object.FindObjectsOfType<PipeBehavior>().Length,0);
    }
}
