import React from "react";
import { observer, inject } from 'mobx-react';
import "./Styles.scss";
import { IReviewComponentStore, ReviewLocation } from './reviewStore';
import ReviewLocationComponent from './reviewLocationComponent';

interface ReviewLocationCollectionProps {
  onLocationClick(reviewLocation: ReviewLocation): void;
  reviewStore?: IReviewComponentStore
}

@inject('reviewStore')
@observer
export default class ReviewLocationsCollection extends React.Component<ReviewLocationCollectionProps> {

  onLocationClick = (e, location: ReviewLocation) => {
    e.stopPropagation();
    this.props.onLocationClick(location);
  };

  render() {
    const { filteredReviewLocations, filter } = this.props.reviewStore!;

    return (
      <div>
        {filter.showPoints &&
         filteredReviewLocations.map(location =>
          <ReviewLocationComponent key={location.id} location={location} showDialog={(e) => this.onLocationClick(e, location)} />
        )}
      </div>)
  };
}
