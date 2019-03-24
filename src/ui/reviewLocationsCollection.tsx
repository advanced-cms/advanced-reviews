import React from "react";
import { observer, inject } from 'mobx-react';
import "./Styles.scss";
import classNames from "classnames";
import { IReviewComponentStore } from './reviewStore';
import ReviewEditorDialog from "./dialog/reviewEditorDialog";

interface ReviewLocationCollectionProps {
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewLocationCollection extends React.Component<ReviewLocationCollectionProps, any> {

private showReview(incrementBy: number) {
  const {reviewLocations, dialog, closeDialog} = this.props.reviewStore!;
  let reviewIndex = reviewLocations.indexOf(dialog.currentEditLocation) + incrementBy;
  if (reviewIndex >= reviewLocations.length) {
    reviewIndex = 0;
  } else if (reviewIndex < 0) {
    reviewIndex = reviewLocations.length - 1;
  }
  closeDialog("cancel");
  dialog.showDialog(reviewLocations[reviewIndex]);
}

  render() {
    const {reviewLocations, dialog, currentItemIndex} = this.props.reviewStore!;

    return (
      <div>
        {reviewLocations.map(location => (
          <div key={location.id} 
          style={location.style}
          title={location.formattedFirstComment} 
          className={classNames("reviewLocation", { done: location.isDone, "new-comment": location.isUpdatedReview })}
          onClick={() => dialog.showDialog(location)}>{location.id}</div>
        ))}

        <ReviewEditorDialog 
            onPrevClick={() => this.showReview(-1)} 
            onNextClick={() => this.showReview(1)}
        />
      </div>)
  };
}