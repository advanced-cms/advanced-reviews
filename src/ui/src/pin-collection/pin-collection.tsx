import React from "react";
import { observer, inject } from "mobx-react";
import "./pin-collection.scss";
import { IReviewComponentStore, PinLocation } from "../store/review-store";
import Pin from "../pin/pin";

interface ReviewLocationCollectionProps {
    reviewStore?: IReviewComponentStore;
    newLocation?: PinLocation;
}

@inject("reviewStore")
@observer
export default class PinCollection extends React.Component<ReviewLocationCollectionProps> {
    onLocationClick = (e, location: PinLocation) => {
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
                    <Pin
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
