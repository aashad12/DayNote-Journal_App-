namespace DayNote.Components.Services
{
    public class ThemeService
    {
        private const string Key = "app_theme"; // "light" or "dark"

        public string CurrentTheme { get; private set; } = "light";

        public event Action? OnChange;

        public void Load()
        {
            try
            {
                var saved = Preferences.Default.Get(Key, "light");
                CurrentTheme = string.IsNullOrWhiteSpace(saved) ? "light" : saved;
            }
            catch
            {
                CurrentTheme = "light";
            }
        }

        public void Toggle()
        {
            CurrentTheme = (CurrentTheme == "dark") ? "light" : "dark";

            try
            {
                Preferences.Default.Set(Key, CurrentTheme);
            }
            catch { }

            OnChange?.Invoke();
        }
    }
}
