using LDJam55.Game.Models;

namespace LDJam55.Game.Services;

public interface ContentFrameLineAggegrate {
    public String Id { get; }
    public String Path { get; }
    public FrameLine FrameLine { get; }

    void Update(Double delta);
}
