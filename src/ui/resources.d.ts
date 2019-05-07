interface ReviewResources_Dialog {
    /** Add comment */
    addcomment: string;
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
    /** Click to edit */
    clicktoedit: string;
    /** Add screenshot */
    addscreenshot: string;
    /** Task resolved. Click to reopen. */
    taskdone: string;
    /** Active. Click to resolve. */
    tasknotdone: string;
}

interface ReviewResources {
    dialog: ReviewResources_Dialog;
    panel: ReviewResources_Panel;
}
