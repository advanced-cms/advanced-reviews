import React from "react";
import { observer, inject } from "mobx-react";
import "./Styles.scss";
import { IReviewComponentStore, ReviewLocation } from "./reviewStore";
import ReviewLocationComponent from "./reviewLocationComponent";

interface ReviewLocationCollectionProps {
    reviewStore?: IReviewComponentStore;
    newLocation?: ReviewLocation;
}

@inject("reviewStore")
@observer
export default class ReviewLocationsCollection extends React.Component<ReviewLocationCollectionProps> {
    onLocationClick = (e, location: ReviewLocation) => {
        e.stopPropagation();
        this.props.reviewStore.currentLocation = location;
    };

    render() {
        const { currentLocation, filteredReviewLocations } = this.props.reviewStore!;
        const locations = [...filteredReviewLocations];

        if (this.props.newLocation && !locations.some(location => location === this.props.newLocation)) {
            locations.push(this.props.newLocation);
        }

        return (
            <div>
                {locations.map(location => (
                    <ReviewLocationComponent
                        key={location.id || "unsaved"}
                        location={location}
                        showDialog={e => this.onLocationClick(e, location)}
                        highlighted={location === currentLocation}
                    />
                ))}
            </div>
        );
    }
}
