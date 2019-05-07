import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, Comment as CommentItem } from "../store/review-store";
import { DropDownMenu } from "../common/drop-down-menu";

import "./comment.scss";

interface CommentProps {
    reviewStore?: IReviewComponentStore;
    comment: CommentItem;
}

@inject("reviewStore")
@observer
export default class PageNavigator extends React.Component<CommentProps, any> {
    render() {
        const { getUserAvatarUrl } = this.props.reviewStore!;

        return (
            <div className="comment">
                <div className="avatar">
                    <img src={getUserAvatarUrl(this.props.comment.author)} />
                </div>
                <div className="message">
                    <div>
                        <span className="author">{this.props.comment.author}</span>
                        <span className="date" title={this.props.comment.formattedDate}>
                            {this.props.comment.userFriendlyDate}
                        </span>
                        {this.props.comment.screenshot && (
                            <DropDownMenu icon="image">
                                <img src={this.props.comment.screenshot} />
                            </DropDownMenu>
                        )}
                    </div>
                    <p>{this.props.comment.text}</p>
                </div>
            </div>
        );
    }
}
