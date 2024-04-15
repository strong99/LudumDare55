using LDJam55.Game.Models;
using LDJam55.Game.Pages.Frames.Widgets;

namespace LDJam55.Game.Services;
public class BasicTimeWidgetModel : TimeWidgetModel {
    private readonly GameManager _gameManager;

    public event Action? OnTick;

    public Double Duration { get => _gameManager.Session.Duration; }
    public Double MaxDuration { get => _gameManager.MaxDuration; }

    public BasicTimeWidgetModel(GameManager gameManager) {
        _gameManager = gameManager;
    }

    public void Update() {
        OnTick?.Invoke();
    }
}

public interface EnemyAggegrate {
    public String Name { get; }
    public String Type { get; }
    public String Variant { get; }
    public Boolean IsDead { get; }
    public IReadOnlyDictionary<String, Single> Health { get; }
    public IReadOnlyDictionary<String, Single> MaxHealth { get; }
}

public class EnemyAggegrateSource : EnemyAggegrate {
    private readonly Enemy _enemy;
    public String Name { get => _enemy.Name; }
    public String Type { get => _enemy.Type; }
    public String Variant { get => _enemy.Variant; }
    public Boolean IsDead { get => _health.Any(p => p.Value <= 0); }
    public IReadOnlyDictionary<String, Single> Health { get => _health; }
    private Dictionary<String, Single> _health;

    public IReadOnlyDictionary<String, Single> MaxHealth { get => _maxHealth; }
    private Dictionary<String, Single> _maxHealth;

    public EnemyAggegrateSource(Enemy enemy) {
        _enemy = enemy;
        _maxHealth = _enemy.CalculateAttributes();
        _health = new(MaxHealth);
    }

    public void CalculateAttributes() {
        var c = _enemy.CalculateAttributes();
        _maxHealth.Clear();
        foreach (var x in c) {
            _maxHealth[x.Key] = x.Value;
        }
    }

    public event Action<EnemyAggegrate, Dictionary<String, Single>>? OnDamageApplied;

    public void TakeDamage(Dictionary<String, Single> damage) {
        foreach(var d in damage) {
            if (_health.TryGetValue(d.Key, out var value)) {
                _health[d.Key] = Math.Max(0, value - d.Value);
            }
        }
        OnDamageApplied?.Invoke(this, damage);
    }
}

public interface AllyAggegrate {
    public String Id { get; }
    public IReadOnlyDictionary<String, Single> Attack { get; }

}

public class AllyAggegrateSource : AllyAggegrate {
    private readonly Ally _ally;
    public String Id { get => _ally.Id; }
    public IReadOnlyDictionary<String, Single> Attack { get => _attack; }
    private Dictionary<String, Single> _attack;
    public IReadOnlyDictionary<String, Equipment> Equipment { get => _ally.Equipment; }

    public AllyAggegrateSource(Ally ally) {
        _ally = ally;
        _attack = _ally.CalculateAttributes();
    }

    public void CalculateAttributes() {
        var c = _ally.CalculateAttributes();
        _attack.Clear();
        foreach (var x in c) {
            _attack[x.Key] = x.Value;
        }
    }

    public void TakeDamage(Dictionary<String, Single> damage) {
        foreach(var d in damage) {
            if (_attack.TryGetValue(d.Key, out var value)) {
                _attack[d.Key] = Math.Max(0, value - d.Value);
            }
        }
    }
}

public record FrameProgressionAggegrate(String Id, Boolean Repeatable, Boolean Unlocked, String Type, String[] Tags);

public class ProgressionWidgetModel {
    private static String GetFrameTypeName(String typeName) => typeName.Replace(nameof(FrameProgressionAggegrate), "");
    public FrameProgressionAggegrate[] Frames { get; }
    public String ActiveFrameId { get; }
    public ProgressionWidgetModel(GameManager gameManager, String activeFrameId) {
        Frames = gameManager.World.Frames.Where(p => p is not Invisible && (p is not ContentFrame cf || cf.Lines.Length > 0)).Select(p => new FrameProgressionAggegrate(p.Id, p is ContentFrame cf && cf.Lines.Any(l => l.Revistable), gameManager.Session.FrameLineIds.Any(x => x.StartsWith(p.Id)), GetFrameTypeName(p.GetType().Name), p is not ContentFrame cfp ? [] : cfp.Tags.Where(p=>p is not IsPropTag && p.Condition?.Invoke(gameManager) != false).Select(t=>t.Id).ToArray())).ToArray();
        ActiveFrameId = activeFrameId;
    }
}

public class BattleFrameAggegrate : ContentFrameLineAggegrate, IDisposable {
    private readonly GameManager _gameManager;
    public String Id { get => FrameLine.Id; }
    public String Path { get => $"{CFrame.Id}:{FrameLine.Id}"; }
    public String Title { get => CFrame.Title ?? ""; }
    public Double Duration { get => _gameManager.Session.Duration; }
    public ContentFrame CFrame { get; }
    public FrameLine FrameLine { get => BattleFrameLine; }
    public Battle BattleFrameLine { get; }
    public BasicTimeWidgetModel TimeWidgetModel { get; }
    public IReadOnlyCollection<EnemyAggegrate> Enemies { get => _enemies; }
    private List<EnemyAggegrateSource> _enemies = new();
    public IReadOnlyCollection<AllyAggegrate> Allies { get => _allies; }
    private List<AllyAggegrateSource> _allies = new();

    public ProgressionWidgetModel ProgressionWidget { get; }
    public Double SessionDuration { get => _gameManager.Session.Duration; }
    public Double MaxSessionDuration { get => _gameManager.MaxDuration; }

    public event Action<BattleFrameAggegrate>? OnTick;

    public BattleFrameAggegrate(GameManager gameManager, ContentFrame frame, Battle line) {
        _gameManager = gameManager;
        CFrame = frame;
        BattleFrameLine = line;

        ProgressionWidget = new(gameManager, frame.Id);
        TimeWidgetModel = new BasicTimeWidgetModel(gameManager);

        foreach (var enemy in line.Enemies) {
            _enemies.Add(new EnemyAggegrateSource(enemy));
        }

        foreach (var ally in gameManager.Session.Allies) {
            _allies.Add(new AllyAggegrateSource(ally));
        }

        _gameManager.OnTick += HandleOnTick;

    }

    public void HandleOnTick(GameManager gameManager) { 
        OnTick?.Invoke(this);
        TimeWidgetModel?.Update();
    }

    private Dictionary<String, Single> CalculateAttributes(Int32 divideOver = 1) {
        var totalDps = new Dictionary<String, Single>();
        foreach (var ally in Allies) {
            var singleDps = ally.Attack;
            foreach (var attribute in singleDps) {
                Single totalValue = attribute.Value;
                if (!totalDps.TryGetValue(attribute.Key, out var value)) {
                    totalValue += value;
                }
                totalDps[attribute.Key] = totalValue / divideOver;
            }
        }
        return totalDps;
    }

    public event Action<EnemyAggegrate, Dictionary<String, Single>>? OnDamageApplied;
    public event Action<Equipment>? OnEquipmentDropped;

    private Double _cooldown = 0;
    public void Update(Double delta) {
        _cooldown -= delta;
        if (!CanGoToNext && _cooldown < 0) {
            var attackDps = CalculateAttributes(_enemies.Count);
            foreach (var enemy in _enemies) {
                if (!enemy.IsDead) {
                    enemy.TakeDamage(attackDps);
                    OnDamageApplied?.Invoke(enemy, attackDps);
                }
            }

            if (CanGoToNext) {
                foreach (var reward in BattleFrameLine.Rewards) {
                    _gameManager.Session.Equipment.Add(reward);
                    OnEquipmentDropped?.Invoke(reward);
                }
            }

            _cooldown += 1000;
        }
    }

    public Boolean CanGoToNext { get => Enemies.All(e => e.IsDead); }

    public void GoTo(String id) {
        _gameManager.GoTo(id);
    }

    public void GoToNext() {
        if (CanGoToNext) {
            _gameManager.GoToNext();
        }
    }

    public void Dispose() {
        _gameManager.OnTick -= HandleOnTick;
    }
}
