namespace DayNote.Services;

public class LockService
{
    private readonly PinRepository _pinRepo;
    private bool _initialized;

    public LockService(PinRepository pinRepo)
    {
        _pinRepo = pinRepo;
    }

    public bool IsLocked { get; private set; } = false;
    public bool PinEnabled { get; private set; } = false;

    public event Action? OnChanged;

    public async Task InitializeAsync()
    {
        if (_initialized) return;   // prevents re-locking
        _initialized = true;

        PinEnabled = await _pinRepo.HasPinAsync();
        IsLocked = PinEnabled;
        OnChanged?.Invoke();
    }

    public void Lock()
    {
        if (!PinEnabled) return;
        IsLocked = true;
        OnChanged?.Invoke();
    }

    public void Unlock()
    {
        IsLocked = false;
        OnChanged?.Invoke();
    }

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

    public async Task SetOrChangePinAsync(string newPin)
    {
        await _pinRepo.SetPinAsync(newPin);
        PinEnabled = true;
        IsLocked = true;
        OnChanged?.Invoke();
    }

    public async Task DisablePinAsync()
    {
        await _pinRepo.DisableAsync();
        PinEnabled = false;
        IsLocked = false;
        OnChanged?.Invoke();
    }
}
