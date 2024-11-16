import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore } from "../store/review-store";

import { IconButton, TextButton } from "@episerver/ui-framework";
import MaterialIcon from "@material/react-material-icon";

import "./pin-navigator.scss";

interface PinNavigatorProps {
    resources?: ReviewResources;
    reviewStore?: IReviewComponentStore;
}

class PinNavigator extends React.Component {
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
            nextTitle =
                nextTitle +
                ": " +
                this.props.reviewStore.resolvePropertyDisplayName(reviewLocations[currentItemIndex + 1].propertyName);
        }

        const isPrevEnabled = currentItemIndex > 0;
        let prevTitle = "prev";
        if (isPrevEnabled) {
            prevTitle =
                prevTitle +
                ": " +
                this.props.reviewStore.resolvePropertyDisplayName(reviewLocations[currentItemIndex - 1].propertyName);
        }

        return (
            <div className="pin-navigator">
                {reviewLocations.length > 1 && (
                    <>
                        <TextButton
                            className="next-prev-icon"
                            title={prevTitle}
                            aria-pressed="false"
                            disabled={!isPrevEnabled}
                        >
                            <MaterialIcon icon="chevron_left" onClick={() => this.showReview(-1)} />
                        </TextButton>
                        <span className="pager">
                            {currentItemIndex + 1} / {reviewLocations.length}
                        </span>
                        <TextButton
                            className="next-prev-icon"
                            title={nextTitle}
                            onClick={() => this.showReview(1)}
                            disabled={!isNextEnabled}
                        >
                            <MaterialIcon icon="chevron_right" />
                        </TextButton>
                    </>
                )}
                <IconButton className="next-prev-icon" title={res.panel.gobacktolist} aria-pressed="false">
                    <MaterialIcon icon="list" onClick={() => (this.props.reviewStore.editedPinLocation = null)} />
                </IconButton>
            </div>
        );
    }
}

export default inject("reviewStore")(inject("resources")(observer(PinNavigator)));
