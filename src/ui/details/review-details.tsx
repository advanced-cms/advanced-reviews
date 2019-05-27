import React from "react";
import { IReviewComponentStore, NewPinDto, PinLocation } from "../store/review-store";
import { inject, observer } from "mobx-react";
import Button from "@material/react-button";
import { IReactionDisposer, reaction } from "mobx";
import { DropDownMenu } from "../common/drop-down-menu";
import Comment from "../comment/comment";
import ScreenshotDialog from "../screenshot-dialog/screenshot-dialog";
import LocationComment from "../location-comment/location-comment";
import ScrollArea from "react-scrollbar";

import "./review-details.scss";

interface ReviewDetailsProps {
    iframe?: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
    resources?: ReviewResources;
    currentEditLocation: PinLocation;
    onCancel: () => void;
}

@inject("reviewStore")
@inject("resources")
@observer
export class ReviewDetails extends React.Component<ReviewDetailsProps, NewPinDto> {
    commentsChangedReaction: IReactionDisposer;
    private scrollable: any;

    componentDidMount(): void {
        this.scrollToBottom();
    }

    constructor(props: ReviewDetailsProps) {
        super(props);
        this.state = {
            currentPriority: this.props.currentEditLocation.priority,
            currentScreenshot: null,
            screenshotMode: false,
            currentCommentText: ""
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

    componentWillUnmount(): void {
        this.commentsChangedReaction();
    }

    scrollToBottom = () => {
        setTimeout(() => {
            this.scrollable.scrollBottom();
        }, 0);
    };

    addNewComment = () => {
        this.props.reviewStore.addComment(this.state.currentCommentText, this.state.currentScreenshot).then(() => {
            this.setState({ screenshotMode: false, currentScreenshot: null, currentCommentText: "" });
        });
    };

    updateComment = (comment: string, screenshot: string) => {
        this.setState({ currentScreenshot: screenshot, currentCommentText: comment });
    };

    render() {
        this.props.currentEditLocation.updateCurrentUserLastRead();

        const canSave: boolean = this.state.currentCommentText.trim() !== "";

        const res = this.props.resources!;

        return (
            <div className="review-details">
                <div className="first-comment">
                    <Comment comment={this.props.currentEditLocation.firstComment} amplify />
                </div>
                <ScrollArea
                    speed={0.8}
                    className="comments-list"
                    horizontal={false}
                    ref={ref => (this.scrollable = ref)}
                >
                    {this.props.currentEditLocation.comments.map((comment, idx) => (
                        <Comment key={idx} comment={comment} />
                    ))}
                </ScrollArea>
                {!this.props.currentEditLocation.isDone && (
                    <>
                        <LocationComment
                            value={this.state.currentCommentText}
                            currentScreenshot={this.state.currentScreenshot}
                            onToggle={() => this.setState({ screenshotMode: !this.state.screenshotMode })}
                            onChange={(comment, screenshot) => {
                                this.updateComment(comment, screenshot);
                            }}
                        />
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
                        <div className="actions">
                            <Button onClick={this.props.onCancel}>{res.dialog.close}</Button>
                            <Button disabled={!canSave} onClick={this.addNewComment}>
                                {res.dialog.addcomment}
                            </Button>
                        </div>
                    </>
                )}
            </div>
        );
    }
}
