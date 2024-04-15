using Blazored.LocalStorage;
using Blazored.SessionStorage;
using LDJam55.Game.Models;

namespace LDJam55.Game.Services;

public class SessionRepository {
    public Session Session { 
        get {
            return _session ?? New();
        }
    }
    private Session? _session;

    public Boolean Loaded { get => _session is not null; }

    private readonly ILocalStorageService _localStorage;
    private readonly ISessionStorageService _sessionStorage;
    private const String _ludumDareSaveStorageKey = "ludumdare55:save";

    public SessionRepository(ILocalStorageService localStorage, ISessionStorageService sessionStorage) {
        _localStorage = localStorage;
        _sessionStorage = sessionStorage;
    }

    public async ValueTask<Boolean> LoadFromCache() {
        if (!await HasCache()) {
            return false;
        }
        _session = await _sessionStorage.GetItemAsync<Session>(_ludumDareSaveStorageKey);
        return true;
    }
    public ValueTask SaveToCache() => _sessionStorage.SetItemAsync(_ludumDareSaveStorageKey, Session);

    /// <summary>
    /// Creates, and overwrites the current session
    /// </summary>
    public Session New() {
        _session = new Session();
        _session.Allies.Add(new("player", [
            new Models.Attribute("slash", 1)
        ], new() {
            ["melee"] = new Equipment("sword", "melee", "normal", [
                new Models.Attribute("slash", 1.1f)
            ]),
            ["armor"] = new Equipment("shield", "armor", "normal", [
                new Models.Attribute("slash", 1.05f)
            ]),
            ["range"] = new Equipment("bow", "range", "normal", [
                new Models.Attribute("slash", 1.1f)
            ]),
            ["special"] = null
        }));
        _session.Equipment.Add(
            new Equipment("longsword", "melee", "normal", [
                new Models.Attribute("slash", 1.2f),
                new Models.Attribute("pierce", 0.1f)
            ])
        );
        return _session;
    }

    public ValueTask<Boolean> HasCache() => _sessionStorage.ContainKeyAsync(_ludumDareSaveStorageKey);
    public ValueTask<Boolean> HasSave() => _localStorage.ContainKeyAsync(_ludumDareSaveStorageKey);

    public ValueTask Save() => _localStorage.SetItemAsync(_ludumDareSaveStorageKey, Session);

    public async ValueTask<Boolean> Load() {
        if (await LoadFromCache()) {
            return true;
        }

        if (!await HasSave()) {
            return false;
        }
        _session = await _localStorage.GetItemAsync<Session>(_ludumDareSaveStorageKey);
        return true;
    }
}
