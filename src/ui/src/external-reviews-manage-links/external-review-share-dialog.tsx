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
    subject: string;
    message: string;
}

interface ShareDialogProps {
    open: boolean;
    onClose(linkShare: LinkShareResult): void;
    initialSubject?: string;
    initialMessage?: string;
    resources: ExternalReviewResources;
}

const ShareDialog = ({ open, onClose, initialSubject, initialMessage, resources }: ShareDialogProps) => {
    const [email, setEmail] = useState<string>("");
    const [subject, setSubject] = useState<string>(initialSubject);
    const [isValidEmail, setIsValidEmail] = useState<boolean>(false);
    const [message, setMessage] = useState<string>(initialMessage);

    useEffect(() => {
        setEmail("");
        setMessage(initialMessage);
    }, [open]);

    const onDialogClose = (action: string) => {
        if (action !== "save") {
            onClose(null);
            return;
        }

        onClose({
            email,
            subject,
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
            <DialogTitle>{resources.sharedialog.dialogtitle}</DialogTitle>
            <DialogContent className="share-dialog-content">
                <div className="text-field-container">
                    <TextField
                        label={resources.sharedialog.emailaddresslabel}
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
                        label={resources.sharedialog.emailsubjectlabel}
                        dense
                        autoFocus
                        required
                        style={{ width: "100%" }}
                    >
                        <Input value={subject} onChange={event => setSubject(event.currentTarget.value)} />
                    </TextField>
                </div>
                <div className="text-field-container">
                    <TextField
                        label={resources.sharedialog.emailmessagelabel}
                        textarea
                        required
                        helperText={<HelperText>{resources.sharedialog.messagehint}</HelperText>}
                    >
                        <Input rows={15} value={message} onChange={e => setMessage(e.currentTarget.value)} />
                    </TextField>
                </div>
            </DialogContent>
            <DialogFooter>
                <DialogButton dense action="cancel">
                    {resources.sharedialog.cancelbutton}
                </DialogButton>
                <DialogButton
                    raised
                    dense
                    action="save"
                    disabled={!isValidEmail}
                    isDefault
                    icon={<MaterialIcon icon="send" />}
                >
                    {resources.sharedialog.sendbutton}
                </DialogButton>
            </DialogFooter>
        </Dialog>
    );
};

export default ShareDialog;
