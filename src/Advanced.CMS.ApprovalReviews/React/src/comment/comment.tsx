import "./comment.scss";

import MaterialIcon from "@material/react-material-icon";
import classNames from "classnames";
import { inject, observer } from "mobx-react";
import React from "react";

import { DropDownMenu } from "../common/drop-down-menu";
import { Comment as CommentItem, IReviewComponentStore } from "../store/review-store";

interface CommentProps {
    reviewStore?: IReviewComponentStore;
    resources?: ReviewResources;
    comment: CommentItem;
    amplify?: boolean;
    isImportant?: boolean;
    isDone?: boolean;
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
                        {this.props.isImportant && <MaterialIcon icon="priority_high" title={res.priority.important} />}
                        <span className="author" title={this.props.comment.author}>
                            {this.props.comment.author}
                        </span>
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
                    <p className={classNames({ amplify: this.props.amplify, done: this.props.isDone })}>
                        {this.props.comment.text}
                    </p>
                </div>
            </div>
        );
    }
}
