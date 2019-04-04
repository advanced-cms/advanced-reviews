import React from "react";
import { observer, inject } from 'mobx-react';
import "./Styles.scss";
import { IReviewComponentStore, ReviewLocation } from './reviewStore';
import ReviewEditorDialog from "./dialog/reviewEditorDialog";
import ReviewLocationComponent from './reviewLocationComponent';

interface ReviewLocationCollectionProps {
  iframe?: HTMLIFrameElement;
  reviewStore?: IReviewComponentStore
}

interface ReviewLocationCollectionState {
  isDialogOpen: boolean
}

@inject('reviewStore')
@observer
export default class ReviewLocationsCollection extends React.Component<ReviewLocationCollectionProps, ReviewLocationCollectionState> {

  constructor(props: ReviewLocationCollectionProps) {
    super(props);
    this.state = {
      isDialogOpen: false
    }
  }

  private showReview(incrementBy: number): void {
    const { reviewLocations, dialog } = this.props.reviewStore!;
    let reviewIndex = reviewLocations.indexOf(dialog.currentEditLocation) + incrementBy;
    if (reviewIndex >= reviewLocations.length) {
      reviewIndex = 0;
    } else if (reviewIndex < 0) {
      reviewIndex = reviewLocations.length - 1;
    }
    dialog.showDialog(reviewLocations[reviewIndex]);
  }

  showDialog(location: ReviewLocation): void {
    const { dialog } = this.props.reviewStore!;
    dialog.showDialog(location);
    this.setState({
      isDialogOpen: true
    });
    location.updateCurrentUserLastRead();
  }

  onCloseDialog(action: string): void {
    if (action !== "save") {
      this.setState({
        isDialogOpen: false
      });
      return;
    }

    const { saveDialog } = this.props.reviewStore!;
    saveDialog().then(() => {
      this.setState({
        isDialogOpen: false
      });
    }).catch(e => {
      //TODO: handle server exceptions
      alert(e.message);
    });
  }

  onLocationClick = (e, location) => {
    e.stopPropagation();
    this.showDialog(location);
  };
  render() {
    const { reviewLocations } = this.props.reviewStore!;

    return (
      <div>
        {reviewLocations.map(location =>
          <ReviewLocationComponent key={location.id} location={location} showDialog={(e) => this.onLocationClick(e, location)} />
        )}
        <ReviewEditorDialog
          iframe={this.props.iframe}
          isDialogOpen={this.state.isDialogOpen}
          onPrevClick={() => this.showReview(-1)}
          onNextClick={() => this.showReview(1)}
          onCloseDialog={(action) => this.onCloseDialog(action)}
        />
      </div>)
  };
}
