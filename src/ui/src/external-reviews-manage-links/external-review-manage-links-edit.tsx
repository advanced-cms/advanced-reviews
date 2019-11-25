import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import { format, parse } from "date-fns";
import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import { Checkbox, Input, TextButton, TextField } from "@episerver/ui-framework";
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
        const [validDate, setValidDate] = useState<string>(format(reviewLink.validTo, "YYYY-MM-DD hh:mm"));
        const [prolongVisible, setProlongVisible] = useState<boolean>(true);
        const [pinCode, setPinCode] = useState<string>(reviewLink.pinCode || "");
        const [shouldUpdatePinCode, setShouldUpdatePinCode] = useState<boolean>(!reviewLink.pinCode);

        const onCloseDialog = (action: string) => {
            if (validDate && action !== "save") {
                onClose(null, null, null);
                return;
            }
            const newPin = shouldUpdatePinCode ? pinCode : null;
            onClose(parse(validDate), newPin, displayName);
        };

        const updatePinCode = (event: React.FormEvent<HTMLInputElement>) => {
            const newValue = event.currentTarget.value;
            if (!!newValue && !/^\d+$/.test(newValue)) {
                return;
            }
            setPinCode(newValue);
        };

        const updateValidDate = () => {
            setProlongVisible(false);
            let dateCopy = new Date(reviewLink.validTo.getTime());
            const today = new Date();
            if (today > dateCopy) {
                dateCopy = today;
            }
            dateCopy.setDate(dateCopy.getDate() + 5);

            setValidDate(format(dateCopy, "YYYY-MM-DD hh:mm"));
        };
        
        return (
            <Dialog open={open} scrimClickAction="" escapeKeyAction="" onClose={onCloseDialog}>
                <DialogTitle>{resources.list.editdialog.title}</DialogTitle>
                <DialogContent className="external-link-edit-dialog">
                    <div className="field-group">
                        <TextField label={resources.list.editdialog.displayname} style={{ width: "100%" }} autoFocus>
                            <Input
                                value={displayName}
                                onChange={(event: React.FormEvent<HTMLInputElement>) =>
                                    setDisplayName(event.currentTarget.value)
                                }
                            />
                        </TextField>
                    </div>
                    <div className="field-group prolong">
                        <span>Valid to: {validDate}</span> {prolongVisible && (<TextButton onClick={updateValidDate}>Prolong</TextButton>)}
                    </div>
                    {pinCodeSecurityEnabled && !reviewLink.isEditable && (
                        <div className="field-group">
                            {!!reviewLink.pinCode && (
                                <>
                                    <Checkbox
                                        nativeControlId="pinCodeActive"
                                        checked={shouldUpdatePinCode}
                                        onChange={() => setShouldUpdatePinCode(!shouldUpdatePinCode)}
                                    />
                                    <label className="checkbox-label" htmlFor="pinCodeActive">
                                        {resources.list.editdialog.pincheckboxlabel}
                                    </label>
                                </>
                            )}
                            <TextField label={resources.list.editdialog.pincode} style={{ width: "100%" }}>
                                <Input
                                    disabled={!shouldUpdatePinCode}
                                    value={pinCode}
                                    onChange={updatePinCode}
                                    type="password"
                                    maxLength={pinCodeLength}
                                />
                            </TextField>
                            <div className="mdc-text-field-helper-line">
                                <p className="mdc-text-field-helper-text mdc-text-field-helper-text--persistent">
                                {!!pinCode
                                    ? resources.list.editdialog.linksecured
                                    : resources.list.editdialog.linknotsecured}
                                </p>
                            </div>
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
