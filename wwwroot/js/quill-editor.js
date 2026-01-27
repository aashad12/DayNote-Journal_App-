window.quillInterop = {
    quill: null,

    init: function (editorId) {
        this.quill = new Quill(`#${editorId}`, {
            theme: "snow",
            modules: {
                toolbar: [
                    ["bold", "italic", "underline"],
                    [{ list: "ordered" }, { list: "bullet" }],
                    [{ header: [1, 2, false] }],
                    ["clean"]
                ]
            }
        });
    },

    getHtml: function () {
        if (!this.quill) return "";
        return this.quill.root.innerHTML;
    },

    setHtml: function (editorId, html) {
        if (!this.quill) return;
        this.quill.root.innerHTML = html || "";
    }
};
