import "@material/react-dialog/index.scss";

import Dialog, { DialogButton, DialogContent, DialogFooter, DialogTitle } from "@material/react-dialog";
import React from "react";

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
    onCloseDialog,
}: ConfirmationDialogProps) => {
    return (
        <Dialog
            open={open}
            scrimClickAction=""
            escapeKeyAction=""
            onClose={(action) => onCloseDialog(action === "save")}
        >
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
