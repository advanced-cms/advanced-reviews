import React from "react";
import classNames from "classnames";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, Comment as CommentItem } from "../store/review-store";
import { DropDownMenu } from "../common/drop-down-menu";

import "./comment.scss";

interface CommentProps {
    reviewStore?: IReviewComponentStore;
    resources?: ReviewResources;
    comment: CommentItem;
    amplify?: Boolean;
}

@inject("resources")
@inject("reviewStore")
@observer
export default class Comment extends React.Component<CommentProps, any> {
    render() {
        const { getUserAvatarUrl } = this.props.reviewStore!;

        const res = this.props.resources!;

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
                            <div className="screenshot">
                                <DropDownMenu icon="image" title={res.panel.showscreenshot}>
                                    <img src={this.props.comment.screenshot} />
                                </DropDownMenu>
                            </div>
                        )}
                    </div>
                    <p className={classNames({ amplify: this.props.amplify })}>{this.props.comment.text}</p>
                </div>
            </div>
        );
    }
}
