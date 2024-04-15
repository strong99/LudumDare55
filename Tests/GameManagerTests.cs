using LDJam55.Game.Models;
using LDJam55.Game.Services;

namespace LDJam55.Tests;

[TestClass]
public class GameManagerTests {
    [TestMethod]
    public void InitGameManager() {
        var world = new World([
            new ContentFrame("#1", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
        ]);
        var session = new Session();

        var gameManager = new GameManager(world, session);
        gameManager.GoToNext();

        Assert.IsNotNull(gameManager.Frame);

        var outcomeId = "#1:#1A";
        Assert.AreEqual(outcomeId, session.CurrentFrameId, "Expects the current frame to be changed");
        Assert.AreEqual(outcomeId, gameManager.Frame.Path, "Expects a new frame id to be present");

        var cf = gameManager.Frame as ContentFrameAggegrate;
        Assert.IsNotNull(cf, "Expected frame to be an aggegrate");
        Assert.IsNotNull(cf.Line, "Expected frame line to be an aggegrate");
    }

    [TestMethod]
    public void GoToNextUnvisitedFrame() {
        var world = new World([
            new ContentFrame("#1", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
            new ContentFrame("#2", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
        ]);
        var session = new Session();
        session.CurrentFrameId = "#1:#1A";
        session.FrameLineIds.Add("#1:#1A");

        var gameManager = new GameManager(world, session);
        gameManager.GoToNext();

        Assert.IsNotNull(gameManager.Frame);

        var outcomeId = "#2:#1A";
        Assert.AreEqual(outcomeId, session.CurrentFrameId, "Expects the current frame to be changed");
        Assert.AreEqual(outcomeId, gameManager.Frame.Path, "Expects a new frame id to be present");
    }

    [TestMethod]
    public void GoToNextVisitedFrame() {
        var world = new World([
            new ContentFrame("#1", [], [new Story("#1A", true, [], "narrator", "I don't know")]),
            new ContentFrame("#2", [], [new Story("#1A", true, [], "narrator", "I don't know")]),
        ]);
        var session = new Session();
        session.CurrentFrameId = "#1:#1A";
        session.FrameLineIds.Add("#1:#1A");
        session.FrameLineIds.Add("#2:#1A");

        var gameManager = new GameManager(world, session);
        gameManager.GoToNext();

        Assert.IsNotNull(gameManager.Frame);

        var outcomeId = "#2:#1A";
        Assert.AreEqual(outcomeId, session.CurrentFrameId, "Expects the current frame to be changed");
        Assert.IsTrue(session.FrameLineIds.Contains(outcomeId), "Expects a new frame id to be present");
        Assert.AreEqual(outcomeId, gameManager.Frame.Path, "Expects a new frame id to be present");
    }

    [TestMethod]
    public void DoNotGoToUnvisitedFrame() {
        var world = new World([
            new ContentFrame("#1", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
            new ContentFrame("#2", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
        ]);
        var session = new Session();
        session.CurrentFrameId = "#1:#1A";
        session.FrameLineIds.Add("#1:#1A");

        var gameManager = new GameManager(world, session);
        gameManager.GoTo("#2:#1a");

        Assert.IsNotNull(gameManager.Frame);

        var outcomeId = "#1:#1A";
        Assert.AreEqual(outcomeId, session.CurrentFrameId, "Expects the current frame to be changed");
        Assert.AreEqual(outcomeId, gameManager.Frame.Path, "Expects a new frame id to be present");
    }

    [TestMethod]
    public void DoNotGoToUnrevisitableFrame() {
        var world = new World([
            new ContentFrame("#1", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
            new ContentFrame("#2", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
        ]);
        var session = new Session();
        session.CurrentFrameId = "#1:#1A";
        session.FrameLineIds.Add("#1:#1A");
        session.FrameLineIds.Add("#2:#1A");

        var gameManager = new GameManager(world, session);
        gameManager.GoTo("#2:#1a");

        Assert.IsNotNull(gameManager.Frame);

        var outcomeId = "#1:#1A";
        Assert.AreEqual(outcomeId, session.CurrentFrameId, "Expects the current frame to be changed");
        Assert.AreEqual(outcomeId, gameManager.Frame.Path, "Expects a new frame id to be present");
    }

    [TestMethod]
    public void GoToVisitedFrame() {
        var world = new World([
            new ContentFrame("#1", [], [new Story("#1A", false, [], "narrator", "I don't know")]),
            new ContentFrame("#2", [], [new Story("#1A", true, [], "narrator", "I don't know")]),
        ]);
        var session = new Session();
        session.CurrentFrameId = "#1:#1A";
        session.FrameLineIds.Add("#1:#1A");
        session.FrameLineIds.Add("#2:#1A");

        var gameManager = new GameManager(world, session);
        gameManager.GoTo("#2");

        var outcomeId = "#2:#1A";

        Assert.IsNotNull(gameManager.Frame);
        Assert.AreEqual(outcomeId, session.CurrentFrameId, "Expects the current frame to be changed");
        Assert.AreEqual(outcomeId, gameManager.Frame.Path, "Expects a new frame id to be present");
    }

    [TestMethod]
    public void GetNeighbouringFrameLines() {
        var world = new World([
            new Branch("#1", false, g => null),
            new ContentFrame("#2", [], [new Story("#1A", true, [], "narrator", "I don't know")]),
            new ContentFrame("#3", [], []),
            new ContentFrame("#4", [], [
                new Story("#1A", false, [], "narrator", "I don't know"),
                new Story("#1A", false, [], "narrator", "I don't know")
            ]),
            new ContentFrame("#5", [new DisableTag(g=>true)], [])
        ]);

        var session = new Session();
        session.CurrentFrameId = "#1:#1A";
        session.FrameLineIds.Add("#1:#1A");

        var gameManager = new GameManager(world, session);
        var frames = gameManager.GetFrameLines();

        Assert.IsNotNull(gameManager.Frame);
        Assert.AreEqual(5, frames.Length, "Expected 4 lines");
        Assert.IsTrue(frames[1].Line is Story, "Expected a story line as start");
        Assert.IsTrue(frames[2].Line is null, "Expected no line");
        Assert.IsTrue(frames.Any(p => p.Id != "#5"), "Expected no line #5");
    }
}