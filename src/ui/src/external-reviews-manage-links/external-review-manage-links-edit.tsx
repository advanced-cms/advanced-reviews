import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import { format, parse } from "date-fns";
import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import { IExternalReviewStore, ReviewLink } from "./external-review-links-store";
import "@material/react-dialog/index.scss";

interface LinkEditDialogProps {
    reviewLink: ReviewLink;
    onClose(validTo: Date): void;
    store: IExternalReviewStore;
    resources: ExternalReviewResources;
    open: boolean;
}

/**
 * Dialog component used to edit link created in manage links
 */
const LinkEditDialog = observer(({ reviewLink, onClose, open, store, resources }: LinkEditDialogProps) => {
    const [validDate, setValidDate] = useState<string>(format(reviewLink.validTo, "YYYY-MM-DD"));

    const onCloseDialog = (action: string) => {
        if (validDate && action !== "save") {
            onClose(null);
            return;
        }
        onClose(parse(validDate));
    };

    return (
        <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onCloseDialog}>
            <DialogTitle>{resources.list.editdialog.title}</DialogTitle>
            <DialogContent>
                <p>
                    Valid to:{" "}
                    <input
                        type="date"
                        value={validDate}
                        onChange={e => {
                            setValidDate(e.currentTarget.value);
                        }}
                    />
                </p>
            </DialogContent>
            <DialogFooter>
                <DialogButton dense action="cancel">
                    {resources.shared.cancel}
                </DialogButton>
                <DialogButton raised dense action="save" isDefault>
                    {resources.shared.ok}
                </DialogButton>
            </DialogFooter>
        </Dialog>
    );
});

export default LinkEditDialog;
