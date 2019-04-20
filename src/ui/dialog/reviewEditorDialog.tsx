import React from "react";
import { reaction, IReactionDisposer } from "mobx";
import { observer, inject } from "mobx-react";
import classNames from "classnames";
import { IReviewComponentStore, Priority, ReviewLocation } from "../reviewStore";
import priorityIconMappings from '../priorityIconMappings';
import { ContextMenu } from "../common/context-menu";

import Dialog, { DialogTitle, DialogContent, DialogFooter, DialogButton } from "@material/react-dialog";
import { Cell, Grid, Row } from "@material/react-layout-grid";
import Checkbox from "@material/react-checkbox";
import TextField, { Input } from "@material/react-text-field";
import PageNavigator from "./pageNavigator";
import Comment from "./comment";

import "@material/react-button/index.scss";
import "@material/react-checkbox/index.scss";
import "@material/react-dialog/index.scss";
import "@material/react-layout-grid/index.scss";
import "@material/react-list/index.scss";
import "@material/react-menu-surface/index.scss";
import "@material/react-text-field/index.scss";
import "./reviewEditorDialog.scss";
import ScreenshotPicker from "../screenshots/screenshotPicker";
import { DropDownMenu } from "../common/drop-down-menu";

interface ReviewDialogProps {
    iframe?: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
    resources?: ReviewResorces;
    currentEditLocation: ReviewLocation;

    onPrevClick(): void;
    onNextClick(): void;
    onCloseDialog(action: string, state: ReviewDialogState): void;
}

export interface ReviewDialogState {
    currentCommentText: string;
    currentIsDone: boolean;
    currentScreenshot: null;
    currentPriority: Priority;
    isScreenshotMode: boolean;
}

@inject("reviewStore")
@inject("resources")
@observer
export default class ReviewDialog extends React.Component<ReviewDialogProps, ReviewDialogState> {
    commentsChangedReaction: IReactionDisposer;

    commentInput: any;

    constructor(props: ReviewDialogProps) {
        super(props);
        this.state = {
            currentIsDone: this.props.currentEditLocation.isDone,
            currentPriority: this.props.currentEditLocation.priority,
            currentCommentText: "",
            currentScreenshot: null,
            isScreenshotMode: false
        };

        this.commentsChangedReaction = reaction(
            () => {
                //uncomment the below code to see the trigger message
                //return state.messages.map((a) => a)
                return this.props.currentEditLocation.comments.slice();
            },
            () => {
                this.scrollToBottom();
            }
        );
    }

    componentWillUnmount() {
        this.commentsChangedReaction();
    }

    messagesEnd: HTMLDivElement;

    scrollToBottom = () => {
        setTimeout(() => {
            if (this.commentInput) {
                this.commentInput.inputElement.focus();
            }
            if (this.messagesEnd !== null) {
                this.messagesEnd.scrollIntoView({ behavior: "smooth" });
            }
        }, 0);
    };

    onDialogOpen(): void {
        this.scrollToBottom();
    }

    render() {
        this.props.currentEditLocation.updateCurrentUserLastRead();
        const { reviewLocations } = this.props.reviewStore!;
        const res = this.props.resources!;

        const customAttribute = {
            title: this.state.currentIsDone ? res.dialog.taskdone : res.dialog.tasknotdone
        };

        const options = Object.keys(Priority).map(priority => {
            return {
                name: priority,
                icon: priorityIconMappings[priority],
                onSelected: () => {
                    this.setState({ currentPriority: Priority[priority] });
                }
            };
        });

        const isNew: boolean = reviewLocations.indexOf(this.props.currentEditLocation) === -1;

        const canSave: boolean =
            (isNew && this.state.currentCommentText.trim() !== "") ||
            (!isNew && (
                this.state.currentCommentText.trim() !== "" ||
                this.state.currentIsDone !== this.props.currentEditLocation.isDone ||
                this.state.currentPriority !== this.props.currentEditLocation.priority));

        return (
            <Dialog
                className={classNames("review-dialog", { "new": isNew })}
                open={true}
                scrimClickAction=""
                escapeKeyAction=""
                onOpen={() => this.onDialogOpen()}
                onClose={(action) => this.props.onCloseDialog(action, this.state)}
            >
                <DialogTitle>
                    {!this.state.isScreenshotMode && <div className="header">
                        <Grid>
                            <Row>
                                <Cell columns={6} className="review-actions left-align">
                                    {this.props.currentEditLocation.propertyName || "Review edit"}
                                </Cell>
                                <Cell columns={6} className="review-actions">
                                    {!isNew &&
                                        <Checkbox
                                            nativeControlId="my-checkbox"
                                            {...customAttribute}
                                            checked={this.state.currentIsDone}
                                            onChange={e => (this.setState({ currentIsDone: e.target.checked }))}
                                        />
                                    }
                                    <ContextMenu
                                        icon={priorityIconMappings[this.state.currentPriority]}
                                        title={this.state.currentPriority}
                                        menuItems={options}
                                    />
                                    {!isNew &&
                                        <PageNavigator
                                            canSave={canSave}
                                            currentItemIndex={reviewLocations.indexOf(this.props.currentEditLocation)}
                                            reviewLocation={this.props.currentEditLocation}
                                            onPrevClick={this.props.onPrevClick}
                                            onNextClick={this.props.onNextClick}
                                        />
                                    }
                                </Cell>
                            </Row>
                        </Grid>
                    </div>
                    }
                    {this.state.isScreenshotMode && <>Crop and highlight the area you want to comment:</>}
                </DialogTitle>
                <DialogContent>
                    {!this.state.isScreenshotMode && (
                        <Grid className="dialog-grid">
                            <Row className="first-comment">
                                <Cell columns={12}>
                                    <strong>{this.props.currentEditLocation.firstComment.text}</strong>
                                    {this.props.currentEditLocation.firstComment.screenshot && (
                                        <DropDownMenu icon="image">
                                            <img src={this.props.currentEditLocation.firstComment.screenshot} />
                                        </DropDownMenu>
                                    )}
                                </Cell>
                            </Row>
                            <Row>
                                <Cell columns={12} className="comments-list">
                                    {this.props.currentEditLocation.comments.map((comment, idx) => (
                                        <Comment key={idx} comment={comment} />
                                    ))}
                                    <div
                                        style={{ float: "left", clear: "both" }}
                                        ref={el => {
                                            this.messagesEnd = el;
                                        }}
                                    />
                                </Cell>
                            </Row>
                            <Row>
                                <Cell columns={12}>
                                    <TextField
                                        label={isNew ? "Describe the issue" : "Add comment..."}
                                        dense textarea>
                                        <Input
                                            ref={(input: any) => this.commentInput = input}
                                            value={this.state.currentCommentText}
                                            onChange={e => (this.setState({ currentCommentText: e.currentTarget.value }))}
                                        />
                                    </TextField>
                                </Cell>
                            </Row>
                        </Grid>
                    )}
                    <ScreenshotPicker
                        maxWidth={500}
                        maxHeight={300}
                        current={this.state.currentScreenshot}
                        iframe={this.props.iframe}
                        onImageSelected={output => (this.setState({ currentScreenshot: output }))}
                        toggle={() => (this.setState({ isScreenshotMode: !this.state.isScreenshotMode }))}
                    />
                </DialogContent>
                <DialogFooter>
                    {!this.state.isScreenshotMode && (
                        <>
                            <DialogButton dense action="cancel">
                                {res.dialog.close}
                            </DialogButton>
                            <DialogButton raised dense action="save" isDefault disabled={!canSave}>
                                {res.dialog.save}
                            </DialogButton>
                        </>
                    )}
                </DialogFooter>
            </Dialog>
        );
    }
}
