import React from "react";
import { observer, inject } from 'mobx-react';
import "./Styles.scss";
import classNames from "classnames";
import { IReviewComponentStore } from './reviewStore';
import ReviewEditorDialog from "./reviewEditorDialog";

interface ReviewLocationCollectionProps {
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewLocationCollection extends React.Component<ReviewLocationCollectionProps, any> {
  render() {
    const {reviewLocations, showDialog} = this.props.reviewStore!;

    return (
      <div>
        {reviewLocations.map(location => (
          <div key={location.id} 
          style={location.style}
          title={location.firstComment.text} 
          className={classNames("reviewLocation", { done: location.isDone })}
          onClick={() => showDialog(location)}>{location.id}</div>
        ))}

        <ReviewEditorDialog />
      </div>)
  };
}