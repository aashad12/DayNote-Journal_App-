window.theme = {
    get: (key) => localStorage.getItem(key),
    set: (key, value) => localStorage.setItem(key, value),

    apply: (theme) => {
        const body = document.body;

        body.classList.remove("theme-light", "theme-dark");
        body.classList.add(theme === "dark" ? "theme-dark" : "theme-light");
    }
};
