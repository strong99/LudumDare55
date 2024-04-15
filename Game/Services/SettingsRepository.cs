using Blazored.LocalStorage;

namespace LDJam55.Game.Services;

public class SettingsRepository {
    private readonly ILogger<SettingsRepository> _logger;
    private readonly ILocalStorageService _localStorage;
    private const String _settingsStorageKey = "settings:";
    private static String MusicVolumeKey { get => $"{_settingsStorageKey}musicVolume"; }
    private static String OtherVolumeKey { get => $"{_settingsStorageKey}otherVolume"; }

    public Single MusicVolume { get => _musicVolume; set { _musicVolume = Math.Clamp(value, 0, 1); SetValue(_musicVolume, MusicVolumeKey); } }
    private Single _musicVolume = 0.5f;
    public Single OtherVolume { get => _otherVolume; set { _otherVolume = Math.Clamp(value, 0, 1); SetValue(_otherVolume, OtherVolumeKey); } }
    private Single _otherVolume = 0.5f;

    public SettingsRepository(ILocalStorageService localStorage, ILogger<SettingsRepository> logger) {
        _localStorage = localStorage;
        _logger = logger;
    }

    public async ValueTask Load() {
        MusicVolume = await GetValue(_musicVolume, MusicVolumeKey);
        OtherVolume = await GetValue(_otherVolume, OtherVolumeKey);
    }

    protected async ValueTask<TValue> GetValue<TValue>(TValue defaultValue, String key) {
        if (await _localStorage.ContainKeyAsync(key)) {
            return await _localStorage.GetItemAsync<TValue>(key) ?? defaultValue;
        }
        return defaultValue;
    }

    protected async void SetValue<TValue>(TValue value, String key) {
        try {
            await _localStorage.SetItemAsync<TValue>(key, value);
        }
        catch(Exception e) {
            _logger.LogError(e, "Failed to save {value} to setting {key}", value, key);
        }
    }
}
