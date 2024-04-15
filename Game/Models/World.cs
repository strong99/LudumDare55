namespace LDJam55.Game.Models;

public class World {
    public List<Frame> Frames { get; }
    public World(IEnumerable<Frame>? frames) {
        Frames = frames?.ToList() ?? [];
    }
}
