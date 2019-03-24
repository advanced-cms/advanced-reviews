import React from "react";
import { observer } from 'mobx-react';
import { Comment as CommentItem } from './../reviewStore';
import {DropDownMenu} from "./../drop-down-menu";

import './comment.scss';

interface CommentProps {
    comment: CommentItem
}

@observer
export default class PageNavigator extends React.Component<CommentProps, any> {
    render() {
        return (
            <div className="comment">
                <div className="avatar">
                    <img src={this.props.comment.authorAvatarUrl} />
                </div>
                <div className="message">
                    <div>
                        <span className="author">{this.props.comment.author}</span>
                        <span className="date" title={this.props.comment.formattedDate}>{this.props.comment.userFriendlyDate}</span>
                        {comment.screenshot && <DropDownMenu icon="image">
                            <img src={comment.screenshot} />
                        </DropDownMenu>}
                    </div>
                    </div>
                    <p>{this.props.comment.text}</p>
                </div>
            </div>
        );
    }
}
