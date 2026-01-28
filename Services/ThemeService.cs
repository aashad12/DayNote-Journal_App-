namespace DayNote.Services
{
    // Service responsible for managing application theme (light / dark)
    public class ThemeService
    {
        // Key used to store theme preference locally
        private const string Key = "app_theme"; // "light" or "dark"
        // Holds the currently active theme
        public string CurrentTheme { get; private set; } = "light";

        // Event triggered when theme changes (used to refresh UI)
        public event Action? OnChange;

        // Loads saved theme from local preferences
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

        // Toggles between light and dark theme and saves preference
        public void Toggle()
        {
            CurrentTheme = CurrentTheme == "dark" ? "light" : "dark";

            try
            {
                Preferences.Default.Set(Key, CurrentTheme);
            }
            catch { }

            // Notify UI components about theme change
            OnChange?.Invoke();
        }
    }
}
