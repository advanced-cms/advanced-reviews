import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import { format, parse } from "date-fns";
import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import { Checkbox, Input, TextField } from "@episerver/ui-framework";
import { ReviewLink } from "./external-review-links-store";
import "@material/react-dialog/index.scss";

interface LinkEditDialogProps {
    reviewLink: ReviewLink;
    onClose(validTo: Date, pinCode: string): void;
    resources: ExternalReviewResources;
    open: boolean;
    pinCodeSecurityEnabled: boolean;
}

/**
 * Dialog component used to edit link created in manage links
 */
const LinkEditDialog = observer(
    ({ reviewLink, onClose, open, resources, pinCodeSecurityEnabled }: LinkEditDialogProps) => {
        const [validDate, setValidDate] = useState<string>(format(reviewLink.validTo, "YYYY-MM-DD"));
        const [pinCode, setPinCode] = useState<string>(reviewLink.pinCode);
        const [shouldUpdatePinCode, setShouldUpdatePinCode] = useState<boolean>(!reviewLink.pinCode);

        const onCloseDialog = (action: string) => {
            if (validDate && action !== "save") {
                onClose(null, null);
                return;
            }
            const newPin = shouldUpdatePinCode ? pinCode: null;
            onClose(parse(validDate), newPin);
        };

        const updatePinCode = (event: React.FormEvent<HTMLInputElement>) => {
            const newValue = event.currentTarget.value;
            if (!!newValue && !/^\d+$/.test(newValue)) {
                return;
            }
            setPinCode(newValue);
        };

        const additionalInputProps = {
            maxLength: 4
        };

        //TODO: pin code should allow to add only 4 digits
        return (
            <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onCloseDialog}>
                <DialogTitle>{resources.list.editdialog.title}</DialogTitle>
                <DialogContent>
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
                        <label htmlFor="updatePin">Update PIN code</label>
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
                                    {...additionalInputProps}
                                />
                            </TextField>
                            <div>{!!pinCode ? "Link secured with PIN code" : "Link with no PIN code security"}</div>
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
