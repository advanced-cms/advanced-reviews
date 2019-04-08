import React from 'react';
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore, ReviewLocation } from './reviewStore';
import ReviewEditorDialog, { ReviewDialogState } from "./dialog/reviewEditorDialog";
import IframeOverlay from "./iframeOverlay";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import ReviewsSlidingPanel from './reviews-sliding-panel/reviews-sliding-panel'
import { Snackbar } from '@material/react-snackbar';
import '@material/react-snackbar/index.scss';

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
        const { reviewLocations } = this.props.reviewStore!;
        let reviewIndex = reviewLocations.indexOf(this.state.currentLocation) + incrementBy;
        if (reviewIndex >= reviewLocations.length) {
            reviewIndex = 0;
        } else if (reviewIndex < 0) {
            reviewIndex = reviewLocations.length - 1;
        }
        this.setState({
            currentLocation: reviewLocations[reviewIndex]
        });
    }

    showDialog(location: ReviewLocation): void {
        this.setState({
            currentLocation: location
        });
    }

    onCloseDialog(action: string, state: ReviewDialogState): void {
        if (action !== "save") {
            this.setState({
                currentLocation: null
            });
            return;
        }

        const { save } = this.props.reviewStore!;
        save(state, this.state.currentLocation).then(() => {
            this.setState({
                currentLocation: null
            });
        }).catch(e => {
            //TODO: handle server exceptions
            alert(e.message);
        });
    }

    onIntroClose = (reason): void => {
        if (reason !== 'action') {
            return;
        }
        //TODO: Set profile value
        alert('Save profile value');
    }

    render() {
        return (<IframeOverlay iframe={this.props.iframe} reviewLocationCreated={(location) => this.setState({ currentLocation: location })}>
            <ReviewLocationsCollection onLocationClick={this.showDialog.bind(this)} />
            {this.props.reviewStore!.reviewLocations.length === 0 &&
                <Snackbar
                    timeoutMs={10000}
                    onClose={this.onIntroClose}
                    message="You are now in content review mode. Click on text to create new review entry."
                    actionText="Do not show this again"
                    stacked={true}
                />
            }
            <ReviewsSlidingPanel onEditClick={this.showDialog.bind(this)} />
            {this.state.currentLocation &&
                <ReviewEditorDialog
                    currentEditLocation={this.state.currentLocation}
                    iframe={this.props.iframe}
                    onPrevClick={() => this.showReview(-1)}
                    onNextClick={() => this.showReview(1)}
                    onCloseDialog={(action, state) => this.onCloseDialog(action, state)}
                />
            }
        </IframeOverlay>);
    }
};

