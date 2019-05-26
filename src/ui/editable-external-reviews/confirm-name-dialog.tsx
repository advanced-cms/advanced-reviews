import React, { useState } from "react";

import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import TextField, { Input, HelperText } from "@material/react-text-field";

import "@material/react-dialog/index.scss";
import "@material/react-text-field/index.scss";

interface ConfirmDialogProps {
    open: boolean;
    onClose(userName: string): void;
    initialUserName?: string;
}

const ConfirmDialog = ({ open, onClose, initialUserName }: ConfirmDialogProps) => {
    const [userName, setUserName] = useState<string>(initialUserName);

    const onDialogClose = (action: string) => {
        if (action !== "save") {
            onClose(null);
            return;
        }

        onClose(userName);
    };

    return (
        <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onDialogClose}>
            <DialogTitle>Confirm your name</DialogTitle>
            <DialogContent>
                <p>Please enter your name. It will be used as an author of the comments.</p>
                <div>
                    <TextField label="Display name" dense autoFocus required style={{ width: "100%" }}>
                        <Input
                            value={userName}
                            onChange={e => setUserName(e.currentTarget.value)}
                            isValid={!!userName}
                        />
                    </TextField>
                </div>
            </DialogContent>
            <DialogFooter>
                <DialogButton raised dense action="save" disabled={!userName} isDefault>
                    Save
                </DialogButton>
            </DialogFooter>
        </Dialog>
    );
};

export default ConfirmDialog;
