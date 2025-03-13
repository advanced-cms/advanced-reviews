import "./external-review-share-dialog.scss";

import { Input, TextField } from "@episerver/ui-framework";
import Dialog, { DialogButton, DialogContent, DialogFooter, DialogTitle } from "@material/react-dialog";
import MaterialIcon from "@material/react-material-icon";
import React, { useEffect, useState } from "react";

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
            message,
        });
    };

    const onEmailTextChanged = (e: React.FormEvent<any>) => {
        const newValue = e.currentTarget.value;
        setEmail(newValue);
        setIsValidEmail(checkIsValidEmail(newValue));
    };

    const emailReg = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    const checkIsValidEmail = (str: string): boolean => {
        return emailReg.test(str);
    };

    const textAreaProps = {
        textarea: true,
    };

    const isValidProps = {
        isValid: isValidEmail,
    };

    return (
        <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onDialogClose}>
            <DialogTitle>{resources.sharedialog.dialogtitle}</DialogTitle>
            <DialogContent className="share-dialog-content">
                <div className="text-field-container">
                    <TextField
                        label={resources.sharedialog.emailaddresslabel}
                        style={{ width: "100%" }}
                        autoFocus
                        required
                    >
                        <Input value={email} onChange={onEmailTextChanged} {...isValidProps} />
                    </TextField>
                </div>
                <div className="text-field-container">
                    <TextField
                        label={resources.sharedialog.emailsubjectlabel}
                        autoFocus
                        required
                        style={{ width: "100%" }}
                    >
                        <Input
                            value={subject}
                            onChange={(event: React.FormEvent<any>) => setSubject(event.currentTarget.value)}
                        />
                    </TextField>
                </div>
                <div className="text-field-container">
                    <TextField
                        label={resources.sharedialog.emailmessagelabel}
                        required
                        style={{ width: "100%" }}
                        {...textAreaProps}
                    >
                        <Input
                            rows={15}
                            value={message}
                            onChange={(e: React.FormEvent<any>) => setMessage(e.currentTarget.value)}
                        />
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
