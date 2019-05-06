import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore } from "../reviewStore";

import IconButton from "@material/react-icon-button";
import MaterialIcon from "@material/react-material-icon";

import "@material/react-icon-button/index.scss";
import "@material/react-material-icon/index.scss";
import "./page-navigator.scss";

interface PageNavigatorProps {
    reviewStore?: IReviewComponentStore;
}

@inject("reviewStore")
@observer
export default class PageNavigator extends React.Component<PageNavigatorProps, any> {
    showReview(incrementBy: number): void {
        const { currentLocation, reviewLocations } = this.props.reviewStore!;
        let reviewIndex = reviewLocations.indexOf(currentLocation) + incrementBy;
        if (reviewIndex >= reviewLocations.length) {
            reviewIndex = 0;
        } else if (reviewIndex < 0) {
            reviewIndex = reviewLocations.length - 1;
        }
        this.props.reviewStore.currentLocation = reviewLocations[reviewIndex]
    }

    render() {
        const { reviewLocations } = this.props.reviewStore!;

        const currentItemIndex = reviewLocations.indexOf(this.props.reviewStore.currentLocation);

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
            <>
                {reviewLocations.length > 1 && (
                    <>
                        <IconButton
                            className="next-prev-icon"
                            title={prevTitle}
                            aria-pressed="false"
                            disabled={!isPrevEnabled}
                        >
                            <MaterialIcon icon="chevron_left" onClick={() => this.showReview(-1)} />
                        </IconButton>
                        <span className="pager">
                            {currentItemIndex + 1} / {reviewLocations.length}
                        </span>
                        <IconButton
                            className="next-prev-icon"
                            title={nextTitle}
                            onClick={() => this.showReview(1)}
                            disabled={!isNextEnabled}
                        >
                            <MaterialIcon icon="chevron_right" />
                        </IconButton>
                    </>
                )}
            </>
        );
    }
}
