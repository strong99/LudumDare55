namespace LDJam55.Game.Services;

public interface FrameAggegrate {
    /// <summary>
    /// Only returns the Frame's id
    /// </summary>
    public String Id { get; }
    /// <summary>
    /// Includes the full id from Frame to Line
    /// </summary>
    public String Path { get; }
    void Update(Double delta);
}
