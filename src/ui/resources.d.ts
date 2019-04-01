interface ReviewResorces_Dialog {
    /** Save */
    save: string;
    /** close */
    close: string;
    /** Uncheck to reopen the task */
    taskdone: string;
    /** Mark task as done */
    tasknotdone: string;
    /** Add screenshot */
    addscreenshot: string;
}

interface ReviewResorces {
    dialog: ReviewResorces_Dialog;
}

