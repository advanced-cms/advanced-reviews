import React from "react";
import { observer, inject } from "mobx-react";
import { Priority, ReviewLocation, NewReview, IReviewComponentStore } from "../reviewStore";
import priorityIconMappings from "../priorityIconMappings";
import { ContextMenu } from "../common/context-menu";

import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import { Cell, Grid, Row } from "@material/react-layout-grid";

import "@material/react-button/index.scss";
import "@material/react-dialog/index.scss";
import "@material/react-layout-grid/index.scss";
import "@material/react-menu-surface/index.scss";
import "@material/react-text-field/index.scss";
import "./new-review-dialog.scss";
import ScreenshotDialog from "../screenshots/screenshot-dialog";
import LocationComment from "../location-comment/location-comment";

interface NewReviewDialogProps {
    iframe?: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
    resources?: ReviewResorces;
    currentEditLocation: ReviewLocation;

    onCloseDialog(action: string, state: NewReview): void;
}

@inject("reviewStore")
@inject("resources")
@observer
export default class NewReviewDialog extends React.Component<NewReviewDialogProps, NewReview> {
    constructor(props: NewReviewDialogProps) {
        super(props);
        this.state = {
            currentPriority: this.props.currentEditLocation.priority,
            currentScreenshot: null,
            screenshotMode: false,
            currentCommentText: ""
        };
    }

    updateComment = (comment: string, screenshot: string) => {
        this.setState({ currentScreenshot: screenshot, currentCommentText: comment });
    };

    render() {
        const res = this.props.resources!;

        const options = Object.keys(Priority).map(priority => {
            return {
                name: priority,
                icon: priorityIconMappings[priority],
                onSelected: () => {
                    this.setState({ currentPriority: Priority[priority] });
                }
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
                    onClose={action => this.props.onCloseDialog(action, this.state)}
                >
                    <DialogTitle>
                        <div className="header">
                            <Grid>
                                <Row>
                                    <Cell columns={9} className="review-actions left-align">
                                        {this.props.currentEditLocation.propertyName || res.dialog.reviewedit}
                                    </Cell>
                                    <Cell columns={3} className="review-actions">
                                        <ContextMenu
                                            icon={priorityIconMappings[this.state.currentPriority]}
                                            title={this.state.currentPriority}
                                            menuItems={options}
                                        />
                                    </Cell>
                                </Row>
                            </Grid>
                        </div>
                    </DialogTitle>
                    <DialogContent>
                        <Grid className="dialog-grid">
                            <Row>
                                <Cell columns={12}>
                                    <LocationComment
                                        currentScreenshot={this.state.currentScreenshot}
                                        onToggle={() =>
                                            this.setState({ screenshotMode: !this.state.screenshotMode })
                                        }
                                        onChange={(comment, screenshot) => {
                                            this.updateComment(comment, screenshot);
                                        }}
                                    />
                                </Cell>
                            </Row>
                        </Grid>
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
                        onImageSelected={output => {
                            this.setState({ currentScreenshot: output });
                        }}
                        toggle={() => this.setState({ screenshotMode: !this.state.screenshotMode })}
                    />
                )}
            </>
        );
    }
}
