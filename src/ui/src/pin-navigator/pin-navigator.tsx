import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore } from "../store/review-store";

import Button from "@material/react-button";
import MaterialIcon from "@material/react-material-icon";

import "@material/react-material-icon/index.scss";
import "./pin-navigator.scss";

interface PinNavigatorProps {
    resources?: ReviewResources;
    reviewStore?: IReviewComponentStore;
}

@inject("reviewStore")
@inject("resources")
@observer
export default class PinNavigator extends React.Component<PinNavigatorProps, any> {
    showReview(incrementBy: number): void {
        const { editedPinLocation, reviewLocations } = this.props.reviewStore!;
        let reviewIndex = reviewLocations.indexOf(editedPinLocation) + incrementBy;
        if (reviewIndex >= reviewLocations.length) {
            reviewIndex = 0;
        } else if (reviewIndex < 0) {
            reviewIndex = reviewLocations.length - 1;
        }
        this.props.reviewStore.selectedPinLocation = this.props.reviewStore.editedPinLocation =
            reviewLocations[reviewIndex];
    }

    render() {
        const { reviewLocations } = this.props.reviewStore!;
        const res = this.props.resources!;

        const currentItemIndex = reviewLocations.indexOf(this.props.reviewStore.editedPinLocation);

        const isNextEnabled = currentItemIndex < reviewLocations.length - 1;
        let nextTitle = "next";
        if (isNextEnabled) {
            nextTitle = nextTitle + ": " + reviewLocations[currentItemIndex + 1].propertyName;
        }

        const isPrevEnabled = currentItemIndex > 0;
        let prevTitle = "prev";
        if (isPrevEnabled) {
            prevTitle = prevTitle + ": " + reviewLocations[currentItemIndex - 1].propertyName;
        }

        return (
            <div className="pin-navigator">
                {reviewLocations.length > 1 && (
                    <>
                        <Button
                            className="next-prev-icon"
                            title={prevTitle}
                            aria-pressed="false"
                            disabled={!isPrevEnabled}
                        >
                            <MaterialIcon icon="chevron_left" onClick={() => this.showReview(-1)} />
                        </Button>
                        <span className="pager">
                            {currentItemIndex + 1} / {reviewLocations.length}
                        </span>
                        <Button
                            className="next-prev-icon"
                            title={nextTitle}
                            onClick={() => this.showReview(1)}
                            disabled={!isNextEnabled}
                        >
                            <MaterialIcon icon="chevron_right" />
                        </Button>
                    </>
                )}
                <Button className="next-prev-icon" title={res.panel.gobacktolist} aria-pressed="false">
                    <MaterialIcon icon="list" onClick={() => (this.props.reviewStore.editedPinLocation = null)} />
                </Button>
            </div>
        );
    }
}
