import React, { useEffect, useState } from "react";

import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import TextField, { Input, HelperText } from "@material/react-text-field";
import MaterialIcon from "@material/react-material-icon";

import "@material/react-dialog/index.scss";
import "@material/react-material-icon/index.scss";
import "@material/react-text-field/index.scss";
import "@material/react-floating-label/index.scss";

import "./external-review-share-dialog.scss";

export interface LinkShareResult {
    email: string;
    message: string;
}

interface ShareDialogProps {
    open: boolean;
    onClose(linkShare: LinkShareResult): void;
}

const ShareDialog = ({ open, onClose }: ShareDialogProps) => {
    const [email, setEmail] = useState<string>("");
    const [isValidEmail, setIsValidEmail] = useState<boolean>(false);
    const [message, setMessage] = useState<string>("");

    useEffect(() => {
        setEmail("");
        setMessage("");
    }, [open]);

    const onDialogClose = (action: string) => {
        if (action !== "save") {
            onClose(null);
            return;
        }

        onClose({
            email,
            message
        });
    };

    const onEmailTextChanged = (e: React.FormEvent<HTMLTextAreaElement>) => {
        const newValue = e.currentTarget.value;
        setEmail(newValue);
        setIsValidEmail(checkIsValidEmail(newValue));
    };

    const emailReg = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    const checkIsValidEmail = (str: string): boolean => {
        return emailReg.test(str);
    };

    return (
        <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onDialogClose}>
            <DialogTitle>Share link with external editor</DialogTitle>
            <DialogContent className="share-dialog-content">
                <div className="text-field-container">
                    <TextField
                        label="Email address"
                        leadingIcon={<MaterialIcon icon="mail" />}
                        style={{ width: 350 }}
                        dense
                        autoFocus
                        required
                    >
                        <Input value={email} onChange={onEmailTextChanged} isValid={isValidEmail} />
                    </TextField>
                </div>
                <div className="text-field-container">
                    <TextField
                        label="Email message"
                        textarea
                        required
                        helperText={<HelperText>Text [#link#] will be replaced with link to content.</HelperText>}
                    >
                        <Input rows={15} value={message} onChange={e => setMessage(e.currentTarget.value)} />
                    </TextField>
                </div>
            </DialogContent>
            <DialogFooter>
                <DialogButton dense action="cancel">
                    Cancel
                </DialogButton>
                <DialogButton
                    raised
                    dense
                    action="save"
                    disabled={!isValidEmail}
                    isDefault
                    icon={<MaterialIcon icon="send" />}
                >
                    Send
                </DialogButton>
            </DialogFooter>
        </Dialog>
    );
};

export default ShareDialog;
