import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import { format, parse } from "date-fns";
import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import { Checkbox, Input, TextField } from "@episerver/ui-framework";
import { ReviewLink } from "./external-review-links-store";
import "@material/react-dialog/index.scss";

interface LinkEditDialogProps {
    reviewLink: ReviewLink;
    onClose(validTo: Date, pinCode: string, displayName: string): void;
    resources: ExternalReviewResources;
    open: boolean;
    pinCodeSecurityEnabled: boolean;
    pinCodeLength: number;
}

/**
 * Dialog component used to edit link created in manage links
 */
const LinkEditDialog = observer(
    ({ reviewLink, onClose, open, resources, pinCodeSecurityEnabled, pinCodeLength }: LinkEditDialogProps) => {
        const [displayName, setDisplayName] = useState<string>(reviewLink.displayName || "");
        const [validDate, setValidDate] = useState<string>(format(reviewLink.validTo, "YYYY-MM-DD"));
        const [pinCode, setPinCode] = useState<string>(reviewLink.pinCode || "");
        const [shouldUpdatePinCode, setShouldUpdatePinCode] = useState<boolean>(!reviewLink.pinCode);

        const onCloseDialog = (action: string) => {
            if (validDate && action !== "save") {
                onClose(null, null, null);
                return;
            }
            const newPin = shouldUpdatePinCode ? pinCode: null;
            onClose(parse(validDate), newPin, displayName);
        };

        const updatePinCode = (event: React.FormEvent<HTMLInputElement>) => {
            const newValue = event.currentTarget.value;
            if (!!newValue && !/^\d+$/.test(newValue)) {
                return;
            }
            setPinCode(newValue);
        };

        return (
            <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onCloseDialog}>
                <DialogTitle>{resources.list.editdialog.title}</DialogTitle>
                <DialogContent>
                    <div>
                        <TextField label={resources.list.editdialog.displayname}>
                                <Input
                                    value={displayName}
                                    onChange={(event: React.FormEvent<HTMLInputElement>) => setDisplayName(event.currentTarget.value)}
                                />
                        </TextField>
                    </div>
                    <div>
                        Valid to:{" "}
                        <input
                            type="date"
                            value={validDate}
                            onChange={e => {
                                setValidDate(e.currentTarget.value);
                            }}
                        />
                    </div>
                    {(pinCodeSecurityEnabled && !!reviewLink.pinCode) && (
                        <>
                        <Checkbox
                            nativeControlId="updatePin"
                            checked={shouldUpdatePinCode}
                            onChange={() => setShouldUpdatePinCode(!shouldUpdatePinCode)}
                        />
                        <label htmlFor="updatePin">{resources.list.editdialog.pincheckboxlabel}</label>
                        </>
                    )}
                    {pinCodeSecurityEnabled && !reviewLink.isEditable && (
                        <div>
                            <TextField label={resources.list.editdialog.pincode}>
                                <Input
                                    disabled={!shouldUpdatePinCode}
                                    value={pinCode}
                                    onChange={updatePinCode}
                                    type="password"
                                    maxLength={pinCodeLength}
                                />
                            </TextField>
                            <div>{!!pinCode ? resources.list.editdialog.linksecured : resources.list.editdialog.linknotsecured}</div>
                        </div>
                    )}
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
    }
);

export default LinkEditDialog;
