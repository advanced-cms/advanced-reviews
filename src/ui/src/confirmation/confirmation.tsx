import React from "react";

import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import "@material/react-dialog/index.scss";

interface ConfirmationDialogProps {
    title: string;
    description: string;
    okName: string;
    cancelName: string;
    open: boolean;
    onCloseDialog(action: boolean): void;
}

const ConfirmationDialog = ({
    title,
    description,
    okName,
    cancelName,
    open,
    onCloseDialog
}: ConfirmationDialogProps) => {
    return (
        <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={action => onCloseDialog(action === "save")}>
            <DialogTitle>{title}</DialogTitle>
            <DialogContent>{description}</DialogContent>
            <DialogFooter>
                <DialogButton dense action="cancel">
                    {cancelName}
                </DialogButton>
                <DialogButton raised dense action="save" isDefault>
                    {okName}
                </DialogButton>
            </DialogFooter>
        </Dialog>
    );
};

export default ConfirmationDialog;
