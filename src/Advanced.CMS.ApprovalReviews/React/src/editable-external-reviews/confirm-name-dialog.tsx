import "@material/react-dialog/index.scss";

import { Input, TextField } from "@episerver/ui-framework";
import Dialog, { DialogButton, DialogContent, DialogFooter, DialogTitle } from "@material/react-dialog";
import React, { useState } from "react";

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

    const inputProps = {
        isValid: !!userName,
    };

    return (
        <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onDialogClose}>
            <DialogTitle>Confirm your name</DialogTitle>
            <DialogContent>
                <p>Please enter your name. It will be used as an author of the comments.</p>
                <div>
                    <TextField label="Display name" autoFocus required style={{ width: "100%" }}>
                        <Input
                            value={userName}
                            onChange={(e: React.FormEvent<any>) => setUserName(e.currentTarget.value)}
                            {...inputProps}
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
