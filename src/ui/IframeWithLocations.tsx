import React, { FunctionComponent, useState } from 'react';
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore, ReviewLocation } from './reviewStore';
import ReviewEditorDialog from "./dialog/reviewEditorDialog";
import IframeOverlay from "./iframeOverlay";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import ReviewsSlidingPanel from './reviews-sliding-panel/reviews-sliding-panel' 

interface IframeState {
    currentLocation: ReviewLocation;
}

interface IframeWithLocationsProps {
    iframe: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore
}


@inject('reviewStore')
@observer
export default class IFrameWithLocations extends React.Component<IframeWithLocationsProps, IframeState> {
    constructor(props: IframeWithLocationsProps) {
        super(props);

        this.state = {
            currentLocation: null
        }
    }

    showReview(incrementBy: number): void {
        const { reviewLocations, dialog } = this.props.reviewStore!;
        let reviewIndex = reviewLocations.indexOf(dialog.currentEditLocation) + incrementBy;
        if (reviewIndex >= reviewLocations.length) {
            reviewIndex = 0;
        } else if (reviewIndex < 0) {
            reviewIndex = reviewLocations.length - 1;
        }
        this.setState({
            currentLocation: reviewLocations[reviewIndex]
        });
        dialog.showDialog(this.state.currentLocation);
    }

    showDialog(location: ReviewLocation): void {
        const { dialog } = this.props.reviewStore!;
        this.setState({
            currentLocation: location
        });

        dialog.showDialog(location);
    }

    onCloseDialog(action: string): void {
        if (action !== "save") {
            this.setState({
                currentLocation: null
            });
            return;
        }

        const { saveDialog } = this.props.reviewStore!;
        saveDialog().then(() => {
            this.setState({
                currentLocation: null
            });
        }).catch(e => {
            //TODO: handle server exceptions
            alert(e.message);
        });
    }

    render() {
        return (<IframeOverlay iframe={this.props.iframe} reviewLocationCreated={(location) => this.setState({ currentLocation: location })}>
            <ReviewLocationsCollection onLocationClick={this.showDialog.bind(this)} />
            <ReviewsSlidingPanel onEditClick={this.showDialog.bind(this)} />
            <ReviewEditorDialog
                iframe={this.props.iframe}
                isDialogOpen={!!this.state.currentLocation}
                onPrevClick={() => this.showReview(-1)}
                onNextClick={() => this.showReview(1)}
                onCloseDialog={(action) => this.onCloseDialog(action)}
            />
        </IframeOverlay>);
    }
};

