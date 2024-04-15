using LDJam55.Game.Models;
using Attribute = LDJam55.Game.Models.Attribute;

namespace LDJam55.Game.Services;

public class WorldRepository {
    public World World { get; }

    public WorldRepository() {
        var enemy1 = new Enemy("goblin#1", "goblin", "normal", [new Attribute("slash", 20)]);
        var enemy2 = new Enemy("goblin#2", "goblin", "variant", [new Attribute("slash", 20)]);
        var enemy3 = new Enemy("knightGoblin#1", "knightGoblin", "normal", [new Attribute("slash", 40)]);
        var enemy4 = new Enemy("knightGoblin#2", "knightGoblin", "variant", [new Attribute("slash", 40)]);
        var enemy5 = new Enemy("archerGoblin#1", "archerGoblin", "normal", [new Attribute("slash", 40)]);
        var enemy6 = new Enemy("hobGoblin#1", "hobGoblin", "normal", [new Attribute("slash", 100)]);

        var destructionTag = new Tag("destruction", g => g.Session.Duration >= g.MaxDuration);
        var wonTag = new Tag("won", g => g.Session.Tags.Contains("won"));
        var lostTag = new Tag("lost", g => g.Session.Tags.Contains("lost"));
        var disableOnWonTag = new DisableTag(g => g.Session.FrameLineIds.Contains("won:won01") || g.Session.CurrentFrameId == "won:won01");
        var disableOnLostTag = new DisableTag(g => g.Session.FrameLineIds.Contains("lost:lost01") || g.Session.CurrentFrameId == "lost:lost01");       

        World = new World([
            new ContentFrame("pre05", [], []),
            new ContentFrame("pre04", [
                new PropLayerTag("mountain01","back3", []),
            ], []),
            new ContentFrame("pre03", [
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], []),
            new ContentFrame("pre02", [
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back1", []),
            ], []),
            new ContentFrame("pre01", [
                new PropLayerTag("tree01","main", [])
            ], []),
            new ContentFrame("intro01", [
                new Tag("Story"),
                new PropLayerTag("player", "main", [])
            ], [
                new Story("intro01-01", false, [], "narrator", "The King summoned you to break the siege of the Capital"),
                new Story("intro01-02", false, [], "narrator", "You can't let the King and the Capital wait. Gather your stuff and leave")
            ], Title: "Outskirts"),
            new ContentFrame("town01", [
                new Tag("Checkpoint"),
                new PropLayerTag("hill02town","back1", []),
            ], [
                new Checkpoint("town01-01", true, "town01", [
                    new Training(7000, "slash", 0.9f, 1.04f),
                    new Training(15000, "slash", 0.6f, 1.06f),
                    new Recruit(10000, 0.5f, new Ally("generic.01", [
                        new("slash", 1.1f),
                        new("pierce", 1.1f),
                        new("blunt", 1.1f)
                    ], []))
                ], []),
                new Story("town01-02", false, [], "narrator", "You summoned your horse and left")
            ], Title: "Starter's Town"),
            new ContentFrame("outskirts01", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", ["alt"])], [
                new Battle("outskirts01-01", true, "plains", [enemy1], [], [])
            ], Title: "Outskirts"),
            new ContentFrame("outskirts02", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
                new PropLayerTag("mountain01","back3", []),
            ], [
                new Battle("outskirts02-01", true, "plains", [enemy1, enemy1], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Choice("outskirts02-02-recruit", false, "narrator", "You save a healer from the goblin horde. Would you like to recruit a healer?", [
                    new("yes", (s,w)=>s.Allies.Add(new Ally("healer", [
                        new("blunt", 1.1f)
                    ], []))),
                    new("no", null)
                ], [
                    new PropLayerTag("healer", "main", [])
                ]),
                new Battle("outskirts02-03", true, "plains", [enemy1], [], [])
            ], Title: "Outskirts"),
            new ContentFrame("outskirts03", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
                new PropLayerTag("mountain01","back3", []),
            ], [
                new Battle("outskirts03-01", true, "plains", [enemy1, enemy1], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("outskirts03-02", true, "plains", [enemy2], [], []),
                new Battle("outskirts03-03", true, "plains", [enemy2, enemy3], [], [])
            ], Title: "Outskirts"),
            new ContentFrame("outskirts04", [
                new Tag("battle"),
                new PropLayerTag("mountain01","back3", []),
            ], [
                new Battle("outskirts04-01", true, "plains", [enemy1, enemy1], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("outskirts04-02", true, "plains", [enemy1, enemy2], [], []),
                new Battle("outskirts04-03", true, "plains", [enemy2, enemy2, enemy3], [], [])
            ], Title: "Outskirts"),
            new ContentFrame("outskirts05", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
            ], [
                new Battle("outskirts05-01", true, "plains", [enemy2, enemy2], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("outskirts05-02", true, "plains", [enemy3, enemy1, enemy1], [], []),
                new Battle("outskirts05-03", true, "plains", [enemy3, enemy4], [], [])
            ], Title: "Outskirts"),
            new ContentFrame("outskirts06", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("outskirts06-01", true, "plains", [enemy2, enemy2], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("outskirts06-02", true, "plains", [enemy3, enemy1, enemy1], [], []),
                new Battle("outskirts06-03", true, "plains", [enemy3, enemy4], [], [])
            ], Title: "Outskirts"),
            new ContentFrame("outskirts07", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("outskirts07-01", true, "plains", [enemy6], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("outskirts07-02", true, "plains", [enemy1, enemy1, enemy6], [], []),
                new Battle("outskirts07-03", true, "plains", [enemy1, enemy2, enemy6], [], [])
            ], Title: "Outskirts"),
            new ContentFrame("town02", [
                new Tag("Checkpoint"),
                new PropLayerTag("hill02town","back1", []),
            ], [
                new Checkpoint("town02-01", true, "town01", [
                    new Training(8000, "slash", 0.8f, 1.04f),
                    new Training(16000, "slash", 0.5f, 1.06f),
                    new Recruit(10000, 0.5f, new Ally("generic.02", [
                        new("slash", 1.2f),
                        new("pierce", 1.05f),
                        new("blunt", 1.01f)
                    ], []))
                ], [])
            ], Title: "Town of Guartuniea"),
            new ContentFrame("meadows01", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("meadows01-01", true, "plains", [enemy6], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("meadows01-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Meadows"),
            new ContentFrame("meadows02", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("meadows02-01", true, "plains", [enemy6], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("meadows02-02", true, "plains", [enemy2, enemy2, enemy4, enemy3, enemy3, enemy6], [], []),
            ], Title: "Meadows"),
            new ContentFrame("town03", [
                new Tag("Checkpoint"),
                new PropLayerTag("hill02town","back1", []),
            ], [
                new Checkpoint("town03-01", true, "town01", [
                    new Training(8000, "slash", 0.8f, 1.04f),
                    new Training(16000, "slash", 0.5f, 1.06f),
                    new Recruit(10000, 0.5f, new Ally("generic.03", [
                        new("slash", 1.01f),
                        new("pierce", 1.05f),
                        new("blunt", 1.1f)
                    ], []))
                ], [])
            ], Title: "Town of Izamurdor"),
            new ContentFrame("forest01", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("forest01-01", true, "plains", [enemy6], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("forest01-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Forest"),
            new ContentFrame("forest02", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("forest02-01", true, "plains", [enemy6], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("forest02-02", true, "plains", [enemy2, enemy2, enemy4, enemy3, enemy3, enemy6], [], []),
            ], Title: "Forest"),
            new ContentFrame("town04", [
                new Tag("Checkpoint"),
                new PropLayerTag("hill02town","back1", []),
            ], [
                new Checkpoint("town04-01", true, "town01", [
                    new Training(8000, "slash", 0.8f, 1.04f),
                    new Training(16000, "slash", 0.5f, 1.06f),
                    new Recruit(10000, 0.5f, new Ally("generic.04", [
                        new("slash", 1.05f),
                        new("pierce", 1.1f),
                        new("blunt", 1.2f)
                    ], []))
                ], [])
            ], Title: "Castle town of Rhine"),
            new ContentFrame("plains01", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("plains01-01", true, "plains", [enemy6], [
                    new Equipment("longsword #3", "melee", "normal", [
                        new("slash", 1.25f)
                    ])
                ], []),
                new Battle("plains01-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Plains"),
            new ContentFrame("plains02", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("plains02-01", true, "plains", [enemy6], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("plains02-02", true, "plains", [enemy2, enemy2, enemy4, enemy3, enemy3, enemy6], [], []),
            ], Title: "Plains"),
            new ContentFrame("plains03", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("plains03-01", true, "plains", [enemy6], [
                    new Equipment("longsword #4", "melee", "normal", [
                        new("slash", 1.3f)
                    ])
                ], []),
                new Battle("plains03-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Plains"),
            new ContentFrame("town05", [
                new Tag("Checkpoint"),
                new PropLayerTag("hill02town","back1", []),
            ], [
                new Checkpoint("town05-01", true, "town01", [
                    new Training(8000, "slash", 0.8f, 1.04f),
                    new Training(16000, "slash", 0.5f, 1.06f),
                    new Recruit(10000, 0.5f, new Ally("generic.04", [
                        new("slash", 1.05f),
                        new("pierce", 1.1f),
                        new("blunt", 1.2f)
                    ], []))
                ], [])
            ], Title: "Town of Closeness"),
            new ContentFrame("darkforest01", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("darkforest01-01", true, "plains", [enemy6], [
                    new Equipment("longsword #3", "melee", "normal", [
                        new("slash", 1.25f)
                    ])
                ], []),
                new Battle("darkforest01-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Dark forest"),
            new ContentFrame("darkforest02", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("darkforest02-01", true, "plains", [enemy6], [
                    new Equipment("longsword #2", "melee", "normal", [
                        new("slash", 1.1f)
                    ])
                ], []),
                new Battle("darkforest02-02", true, "plains", [enemy2, enemy2, enemy4, enemy3, enemy3, enemy6], [], []),
            ], Title: "Dark forest"),
            new ContentFrame("darkforest03", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("darkforest03-01", true, "plains", [enemy6], [
                    new Equipment("longsword #4", "melee", "normal", [
                        new("slash", 1.3f)
                    ])
                ], []),
                new Battle("plains03-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Dark forest"),
            new ContentFrame("darkforest04", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("darkforest04-01", true, "plains", [enemy6], [
                    new Equipment("longsword #4", "melee", "normal", [
                        new("slash", 1.3f)
                    ])
                ], []),
                new Battle("plains04-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Dark forest"),
            new ContentFrame("darkforest05", [
                new Tag("battle"),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("bush01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [
                new Battle("darkforest05-01", true, "plains", [enemy6], [
                    new Equipment("longsword #4", "melee", "normal", [
                        new("slash", 1.3f)
                    ])
                ], []),
                new Battle("plains05-02", true, "plains", [enemy2, enemy2, enemy6], [], []),
            ], Title: "Dark forest"),
            new Branch("timeExpired", false, (gameManager)=>{
                if (gameManager.Session.Duration < gameManager.MaxDuration) {
                    return "won:won01";
                }
                else {
                    return "lost:lost01";
                }
            }),
            new ContentFrame("preEndGame01", [
                destructionTag,
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [], Title: "City outskirts"),
            new ContentFrame("preEndGame02", [
                destructionTag,
                new PropLayerTag("hill01","back1", []),
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("tree01","main", ["alt"]),
                new PropLayerTag("bush01","main", ["alt"]),
            ], [], Title: "City outskirts"),
            new ContentFrame("preEndGame03", [
                destructionTag,
                new PropLayerTag("tree01","main", []),
                new PropLayerTag("hill01","back2", []),
            ], [], Title: "City outskirts"),
            new ContentFrame("won", [
                disableOnLostTag,
                new Tag("Story"),
                new PropLayerTag("hill03town","back1", []),
            ], [
                new Story("won01", false, [], "narrator", "As your arrived, your brand new legion broke the siege"),
                new Story("won02", false, [], "narrator", "The King summoned you again to thanks you and send you of to your next adventure"),
                new GameEnd("wonEnd", false, [])
            ], "Capital City"),
            new ContentFrame("lost", [
                disableOnWonTag,
                destructionTag,
                new Tag("Story"),
                new PropLayerTag("hill03town","back1", []),
            ], [
                new Story("lost01", false, [], "narrator", "You arrived at the Capital, that lay in ruins. You were to late"),
                new GameEnd("lostEnd", false, [])
            ], "Capital's Throne room"),
            new ContentFrame("post01", [destructionTag], [], Title: ""),
            new ContentFrame("post02", [destructionTag], [], Title: ""),
            new ContentFrame("post03", [destructionTag], [], Title: ""),
            new ContentFrame("post04", [destructionTag], [], Title: ""),
            new ContentFrame("post05", [destructionTag], [], Title: ""),
        ]);
    }
}
