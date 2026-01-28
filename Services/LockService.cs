namespace DayNote.Services;

// Service responsible for locking and unlocking the application using PIN
public class LockService
{
    private readonly PinRepository _pinRepo;
    private bool _initialized;

    // Inject PIN repository dependency
    public LockService(PinRepository pinRepo)
    {
        _pinRepo = pinRepo;
    }
    // Indicates whether the app is currently locked
    public bool IsLocked { get; private set; } = false;
    // Indicates whether PIN protection is enabled
    public bool PinEnabled { get; private set; } = false;

    // Event triggered when lock state changes
    public event Action? OnChanged;

    // Initializes lock state on app startup
    public async Task InitializeAsync()
    {
        if (_initialized) return;   // prevents re-locking
        _initialized = true;

        PinEnabled = await _pinRepo.HasPinAsync();
        IsLocked = PinEnabled;
        OnChanged?.Invoke();
    }

    // Locks the application manually
    public void Lock()
    {
        if (!PinEnabled) return;
        IsLocked = true;
        OnChanged?.Invoke();
    }

    // Unlocks the application
    public void Unlock()
    {
        IsLocked = false;
        OnChanged?.Invoke();
    }

    // Attempts to unlock the app using provided PIN
    public async Task<bool> TryUnlockAsync(string pin)
    {
        if (!PinEnabled)
        {
            Unlock();
            return true;
        }

        var ok = await _pinRepo.VerifyAsync(pin);
        if (ok) Unlock();
        return ok;
    }

    // Sets or changes the PIN and immediately locks the app
    public async Task SetOrChangePinAsync(string newPin)
    {
        await _pinRepo.SetPinAsync(newPin);
        PinEnabled = true;
        IsLocked = true;
        OnChanged?.Invoke();
    }

    // Disables PIN protection and unlocks the app
    public async Task DisablePinAsync()
    {
        await _pinRepo.DisableAsync();
        PinEnabled = false;
        IsLocked = false;
        OnChanged?.Invoke();
    }
}
