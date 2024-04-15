using LDJam55.Game.Models;

namespace LDJam55.Game.Services;

public delegate void OnFrameChanged(GameManager gameManager, String? fromTo, String? toId);
public delegate void OnGameFinished(GameManager gameManager);
public delegate void OnGameTick(GameManager gameManager);

public class GameManager : IDisposable {
    private readonly World _world;
    private readonly Session _session;
    private readonly Timer? _timer;
    
    public const Int32 FrameChangeAnimationTime = 1050;

    public World World { get => _world; }
    public Session Session { get => _session; }
    public Int32 MaxDuration { get => 10 * 60 * 1000; }
    public List<Ally> Allies { get; } = [];
    public FrameAggegrate Frame { get; private set; }

    public event OnFrameChanged? OnFrameChanged;
    public event OnGameFinished? OnFinished;
    public event OnGameTick? OnTick;

    private const Int32 _expectFramesPerSecond = 2;

    public GameManager(World world, Session session) {
        _world = world;
        _session = session;

        _timer = new Timer(Tick, null, 0, 1000 / _expectFramesPerSecond);

        if (_world.Frames.Count == 0) {
            throw new Exception("The world should have atleast a single frame");
        }

        var frameLines = GetFrameLines();
        var frame = frameLines.SingleOrDefault(f => f.IsMatch(_session.CurrentFrameId));
        if (frame is null) {
            frame = frameLines.FirstOrDefault(p => p.Line is not null);
            session.CurrentFrameId = frame?.Id;
            if (session.CurrentFrameId is not null
             && !session.FrameLineIds.Contains(session.CurrentFrameId)
            ) {
                session.FrameLineIds.Add(session.CurrentFrameId);
            }
        }
        Frame = CreateFrameAggegrateFor(frame.Frame as ContentFrame ?? throw new Exception("No eligible frame found. This should not be possible."), frame.Line);
    }

    private FrameAggegrate CreateFrameAggegrateFor(ContentFrame frame, FrameLine line) {
        return new ContentFrameAggegrate(this, frame, line);
    }

    private DateTime _last = DateTime.Now;
    private void Tick(Object? state) {
        var now = DateTime.Now;
        var delta = now - _last;
        _session.Timestamp = (_last = now).Ticks;
        _session.Duration += delta.Milliseconds;

        Frame?.Update(delta.Milliseconds);

        OnTick?.Invoke(this);
    }

    private record TempFrame(Frame Frame, FrameLine[]? Lines);
    public record TempFrameLine(Frame Frame, FrameLine? Line) {
        public String Id { get => Line is null ? Frame.Id : $"{Frame.Id}:{Line.Id}"; }
        public Boolean IsMatch(String? id) => id is not null && Id.StartsWith(id);
        public Boolean IsPartialOrFullMatch(String id) => Frame.Id == id || IsMatch(id);
    }

    public TempFrameLine[] GetFrameLines(Boolean ignoreTags = false) =>
         _world.Frames
            .Where(f => ignoreTags || f is not ContentFrame cf || !cf.Tags.Any(t => t is DisableTag && t.Condition?.Invoke(this) != false))
            .Select(f => new TempFrame(f, f is ContentFrame cf ? cf.Lines : null))
            .SelectMany(f => 
                f.Lines is null || f.Lines.Length == 0 ? [new TempFrameLine(f.Frame, null)] : f.Lines.Select(l => new TempFrameLine(f.Frame, l)))
            .ToArray();

    public void GoToNext() {
        if (!_session.FrameLineIds.Contains(Frame.Path)) {
            _session.FrameLineIds.Add(Frame.Path);
        }

        var frameLines = GetFrameLines();

        var idx = Array.FindIndex(frameLines, l => l.IsMatch(_session.CurrentFrameId));
        if (idx + 1 < frameLines.Length) {
            TempFrameLine next;

            // Some frame lines can't be repeated, if the next frame doesn't contain any to revisit/first visit continue
            do {
                next = frameLines[++idx];

                if (next.Frame is Branch branch) {
                    var nextId = branch.Determinator(this);
                    if (nextId is not null) {
                        if (_session.FrameLineIds.Contains(next.Id)) {
                            _session.FrameLineIds.Add(next.Id);
                        }
                        if (_session.FrameLineIds.Contains(nextId)) {
                            _session.FrameLineIds.Add(nextId);
                        }
                        next = frameLines.SingleOrDefault(f => f.Id == nextId) ?? throw new Exception($"Unable to find {nextId}");
                    }
                    else {
                        next = frameLines[++idx];
                    }
                }
            }
            while ((next.Frame is not ContentFrame cf
                 || next.Line is null
                 || next.Line is Invisible
                 || (!next.Line.Revistable && _session.FrameLineIds.Contains(next.Id))
                   )
                && idx + 1 < frameLines.Length
            );

            if (next.Line is not GameEnd && next.Frame is ContentFrame ncf) {
                var oldFrameId = _session.CurrentFrameId;
                var newFrameId = _session.CurrentFrameId = next.Id;
                Frame = CreateFrameAggegrateFor(ncf, next.Line);
                OnFrameChanged?.Invoke(this, oldFrameId, newFrameId);
                return;
            }
        }

        OnFinished?.Invoke(this);
    }

    public void GoTo(String id) {
        if (!_session.FrameLineIds.Any(f=>f.StartsWith(id))) {
            return;
        }

        var next = GetFrameLines().FirstOrDefault(p => 
            p.IsPartialOrFullMatch(id) 
         && p.Line is not null 
         && p.Line.Revistable 
         && _session.FrameLineIds.Contains(p.Id)
        );
        if (next is null
         || next.Frame is not ContentFrame cf
        ) {
            return;
        }

        var oldFrameId = _session.CurrentFrameId;
        var newFrameId = _session.CurrentFrameId = next.Id;
        Frame = CreateFrameAggegrateFor(cf, next.Line);
        OnFrameChanged?.Invoke(this, oldFrameId, newFrameId);
    }

    public void Dispose() {
        _timer?.Dispose();
    }

    public List<OuterFrame> GetNeighbourFrames(Int32 offset, String? fromId, String? toId) {
        var currentFrameId = Frame.Id;
        fromId ??= currentFrameId;
        toId ??= currentFrameId;

        var frames = _world.Frames.Where(p => p is not Invisible).ToArray();
        var fromIdx = Array.FindIndex(frames, f => f.Id == fromId || (f is ContentFrame cf && cf.Lines.Any(l=>$"{f.Id}:{l.Id}" == fromId)));
        var toIdx = Array.FindIndex(frames, f => f.Id == toId || (f is ContentFrame cf && cf.Lines.Any(l => $"{f.Id}:{l.Id}" == toId)));

        var range = toIdx - fromIdx;
        return GetNeighbourFrames(offset, range);
    }

    public List<OuterFrame> GetNeighbourFrames(Int32 offset, Int32 range = 0) {
        var frames = _world.Frames.Where(p => p is not Invisible).ToArray();

        var currentFrameId = Frame.Id;
        var idx = Array.FindIndex(frames, f => f.Id == currentFrameId);

        var min = Math.Min(0, range);
        var max = Math.Max(0, range);

        var cached = new List<OuterFrame>();
        for (var i = min - offset; i <= max + offset; ++i) {
            var ex = idx + i;
            var frame = frames.ElementAtOrDefault(ex);
            if (i == 0) {

            }
            if (frame is ContentFrame cf && !cf.Tags.Any(t=>t is DisableTag dt && dt.Condition?.Invoke(this) != false)) {
                var line = cf.Lines.FirstOrDefault(l=>$"{frame.Id}:{l.Id}" == _session.CurrentFrameId);
                var innerTags = line?.Tags.Where(t => t.Condition?.Invoke(this) != false) ?? [];

                var props = cf.Tags.OfType<IsPropTag>().Where(t=>t.Condition?.Invoke(this) != false)
                    .Union(innerTags.OfType<IsPropTag>())
                    .ToArray();

                var tags = cf.Tags.Where(t => t is not IsPropTag)
                    .Union(innerTags.Where(t => t is not IsPropTag))
                    .ToArray();

                cached.Add(new OuterFrame(frame.Id, i, frame.GetType().Name, tags, props));
            }
            else if (i < min || i > max) {
                cached.Add(new OuterFrame($"placeholder-{ex}".ToString(), i, "normal", [], []));
            }
        }
        return cached;
    }
}

public record OuterFrameProp(String Id, String? Layer);
public record OuterFrame(String Id, Int32 Position, String Type, IsTag[] Tags, IsPropTag[] Props);
