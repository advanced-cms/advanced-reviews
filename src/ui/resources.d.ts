interface ReviewResources_Command {
    /** Advanced Review */
    label: string;
    /** Toggle advanced review */
    tooltip: string;
    /** Turn off Advanced Review */
    labelenabled: string;
    /** Turn on Advanced Review */
    labelnotenabled: string;
}

interface ReviewResources_Dialog {
    /** Add comment */
    addcomment: string;
    /** Change priority */
    changepriority: string;
    /** cancel */
    close: string;
    /** Describe the issue */
    describe: string;
    /** Review edit */
    reviewedit: string;
    /** Save */
    save: string;
}

interface ReviewResources_Panel {
    /** Attach screenshot */
    attachscreenshot: string;
    /** Click to edit */
    clicktoedit: string;
    /** Collapse review panel */
    collapse: string;
    /** Expand review panel */
    expand: string;
    /** Go back to list */
    gobacktolist: string;
    /** Open details */
    opendetails: string;
    /** Remove screenshot */
    removescreenshot: string;
    /** Resolved */
    resolved: string;
    /** Review mode */
    reviewmode: string;
    /** Show screenshot */
    showscreenshot: string;
    /** Show active */
    showactive: string;
    /** Show resolved */
    showresolved: string;
    /** Show unread */
    showunread: string;
}

interface ReviewResources_Screenshot {
    /** Apply */
    apply: string;
    /** cancel */
    cancel: string;
    /** Clear */
    clear: string;
    /** Crop and highlight the area you want to comment */
    cropandhighlight: string;
}

interface ReviewResources_Priority {
    /** Important */
    important: string;
    /** Normal */
    normal: string;
    /** Trivial */
    trivial: string;
}

interface ReviewResources {
    command: ReviewResources_Command;
    dialog: ReviewResources_Dialog;
    panel: ReviewResources_Panel;
    screenshot: ReviewResources_Screenshot;
    priority: ReviewResources_Priority;
}
