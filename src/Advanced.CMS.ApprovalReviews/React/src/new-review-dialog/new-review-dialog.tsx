import React from "react";
import { observer, inject } from "mobx-react";
import { Priority, PinLocation, NewPinDto, IReviewComponentStore } from "../store/review-store";
import priorityIconMappings from "../store/priority-icon-mappings";
import { ContextMenu } from "../common/context-menu";
import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import ScreenshotDialog from "../screenshot-dialog/screenshot-dialog";
import LocationComment from "../location-comment/location-comment";

import "./new-review-dialog.scss";

interface NewReviewDialogProps {
    iframe?: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
    resources?: ReviewResources;
    currentEditLocation: PinLocation;

    onCloseDialog(action: string, state: NewPinDto): void;
}

class NewReviewDialog extends React.Component {
    constructor(props: NewReviewDialogProps) {
        super(props);
        this.state = {
            currentPriority: this.props.currentEditLocation.priority,
            currentScreenshot: null,
            screenshotMode: false,
            currentCommentText: "",
        };
    }

    updateComment = (comment: string, screenshot: string) => {
        this.setState({ currentScreenshot: screenshot, currentCommentText: comment });
    };

    render() {
        const res = this.props.resources!;
        const reviewStore = this.props.reviewStore;

        const options = Object.keys(Priority).map((priority) => {
            return {
                name: res.priority[priority.toLowerCase()],
                icon: priorityIconMappings[priority],
                onSelected: () => {
                    this.setState({ currentPriority: Priority[priority] });
                },
            };
        });

        const canSave: boolean = this.state.currentCommentText.trim() !== "";

        return (
            <>
                <Dialog
                    className="review-dialog"
                    open={true}
                    scrimClickAction=""
                    escapeKeyAction=""
                    onClose={(action) => this.props.onCloseDialog(action, this.state)}
                >
                    <DialogTitle>
                        <div className="header">
                            <div className="left-align">
                                {reviewStore.resolvePropertyDisplayName(this.props.currentEditLocation.propertyName) ||
                                    res.dialog.reviewedit}
                            </div>
                            <div className="review-actions">
                                <ContextMenu
                                    icon={priorityIconMappings[this.state.currentPriority]}
                                    title={res.dialog.changepriority}
                                    menuItems={options}
                                />
                            </div>
                        </div>
                    </DialogTitle>
                    <DialogContent>
                        <div className="dialog-grid">
                            <LocationComment
                                value={this.state.currentCommentText}
                                currentScreenshot={this.state.currentScreenshot}
                                onToggle={() => this.setState({ screenshotMode: !this.state.screenshotMode })}
                                onChange={(comment, screenshot) => {
                                    this.updateComment(comment, screenshot);
                                }}
                                allowScreenshotAttachments={reviewStore.options.allowScreenshotAttachments}
                            />
                        </div>
                    </DialogContent>
                    <DialogFooter>
                        <DialogButton dense action="cancel">
                            {res.dialog.close}
                        </DialogButton>
                        <DialogButton raised dense action="save" isDefault disabled={!canSave}>
                            {res.dialog.save}
                        </DialogButton>
                    </DialogFooter>
                </Dialog>
                {this.state.screenshotMode && (
                    <ScreenshotDialog
                        maxWidth={500}
                        maxHeight={300}
                        iframe={this.props.iframe}
                        propertyName={this.props.currentEditLocation.propertyName}
                        documentRelativePosition={this.props.currentEditLocation.documentRelativePosition}
                        documentSize={this.props.currentEditLocation.documentSize}
                        onImageSelected={(output) => {
                            this.setState({ currentScreenshot: output });
                        }}
                        toggle={() => this.setState({ screenshotMode: !this.state.screenshotMode })}
                    />
                )}
            </>
        );
    }
}

export default inject("reviewStore")(inject("resources")(observer(NewReviewDialog)));
