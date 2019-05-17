import React from "react";

import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import "@material/react-dialog/index.scss";

interface ConfirmationDialogProps {
    title: string;
    description: string;
    open: boolean;
    onCloseDialog(action: boolean): void;
}

const ConfirmationDialog = ({ title, description, open, onCloseDialog }: ConfirmationDialogProps) => {
    return (
        <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={action => onCloseDialog(action === "save")}>
            <DialogTitle>{title}</DialogTitle>
            <DialogContent>{description}</DialogContent>
            <DialogFooter>
                <DialogButton dense action="cancel">
                    Cancel
                </DialogButton>
                <DialogButton raised dense action="save" isDefault>
                    Ok
                </DialogButton>
            </DialogFooter>
        </Dialog>
    );
};

export default ConfirmationDialog;
