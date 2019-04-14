import React from "react";
import { observer, inject } from 'mobx-react';
import "./Styles.scss";
import { IReviewComponentStore, ReviewLocation } from './reviewStore';
import ReviewLocationComponent from './reviewLocationComponent';

interface ReviewLocationCollectionProps {
  onLocationClick(reviewLocation: ReviewLocation): void;
  reviewStore?: IReviewComponentStore;
  currentLocation: ReviewLocation;
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
    const locations = [...filteredReviewLocations];

    if (this.props.currentLocation && !locations.some(location => location === this.props.currentLocation)) {
        locations.push(this.props.currentLocation);
    }

    return (
      <div>
        {filter.showPoints &&
         locations.map(location =>
          <ReviewLocationComponent key={location.id || "unsaved"} location={location} showDialog={(e) => this.onLocationClick(e, location)} />
        )}
      </div>)
  };
}
