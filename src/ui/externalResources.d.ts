interface ExternalReviewResources_Removedialog {
    /** Cancel */
    cancel: string;
    /** Are you sure you want to remove link? */
    description: string;
    /** Ok */
    ok: string;
    /** Remove link */
    title: string;
}

interface ExternalReviewResources_List {
    /** delete */
    deletetitle: string;
    /** There are no external links for this content */
    emptylist: string;
    /** Valid to */
    itemvalidto: string;
    /** share */
    sharetitle: string;
    /** View */
    viewlink: string;
    /** Edit */
    editlink: string;
}

interface ExternalReviewResources_Sharedialog {
    /** Cancel */
    cancelbutton: string;
    /** Share link to the content with external editor */
    dialogtitle: string;
    /** Email message */
    emailmessagelabel: string;
    /** Email subject */
    emailsubjectlabel: string;
    /** Email address */
    emailaddresslabel: string;
    /** Text [#link#] will be replaced with link to content. */
    messagehint: string;
    /** Send */
    sendbutton: string;
}

interface ExternalReviewResources {
    removedialog: ExternalReviewResources_Removedialog;
    list: ExternalReviewResources_List;
    sharedialog: ExternalReviewResources_Sharedialog;
}
