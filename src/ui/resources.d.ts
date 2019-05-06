interface ReviewResorces_Dialog {
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

interface ReviewResorces_Panel {
    /** Click to edit */
    clicktoedit: string;
    /** Add screenshot */
    addscreenshot: string;
    /** Task resolved. Click to reopen. */
    taskdone: string;
    /** Click to resolve. */
    tasknotdone: string;
}

interface ReviewResorces {
    dialog: ReviewResorces_Dialog;
    panel: ReviewResorces_Panel;
}

