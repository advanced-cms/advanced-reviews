import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, PinLocation, NewPinDto } from "../store/review-store";
import NewReviewDialog from "../new-review-dialog/new-review-dialog";
import IframeOverlay from "../iframe-overlay/iframe-overlay";
import PinCollection from "../pin-collection/pin-collection";
import ReviewsSlidingPanel from "../reviews-sliding-panel/reviews-sliding-panel";
import { Snackbar } from "@material/react-snackbar";
import "@material/react-snackbar/index.scss";
import PositionCalculator from "../position-calculator/position-calculator";

interface IframeState {
    newLocation: PinLocation;
}

interface IframeWithPinsProps {
    iframe: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
}

@inject("reviewStore")
@observer
export default class IframeWithPins extends React.Component<IframeWithPinsProps, IframeState> {
    private readonly positionCalculator: PositionCalculator;

    constructor(props: IframeWithPinsProps) {
        super(props);

        this.positionCalculator = new PositionCalculator(this.props.iframe);

        this.state = {
            newLocation: null
        };
    }

    onCloseDialog(action: string, state: NewPinDto): void {
        if (action !== "save") {
            this.setState({
                newLocation: null
            });
            return;
        }

        this.props.reviewStore
            .save(state, this.state.newLocation)
            .then(createdLocation => {
                this.setState({
                    newLocation: null
                });
                // show the pin details only if there's a different pin open currently
                if (this.props.reviewStore.editedPinLocation) {
                    this.props.reviewStore.editedPinLocation = createdLocation;
                }
            })
            .catch(e => {
                //TODO: handle server exceptions
                alert(e.message);
            });

        //TODO: show screenshot after save this.setState({ isScreenshotMode: true });
    }

    onIntroClose = (reason): void => {
        if (reason !== "action") {
            return;
        }
        localStorage.setItem("reviewIntro", "false");
    };

    render() {
        const showReviewIntro: boolean =
            this.props.reviewStore.reviewLocations.length === 0 && localStorage.getItem("reviewIntro") !== "false";

        return (
            <>
                {this.props.reviewStore.filter.reviewMode && (
                    <IframeOverlay
                        iframe={this.props.iframe}
                        reviewLocationCreated={location => this.setState({ newLocation: location })}
                    >
                        <PinCollection
                            newLocation={this.state.newLocation}
                            positionCalculator={this.positionCalculator}
                        />
                        {showReviewIntro && (
                            <Snackbar
                                timeoutMs={10000}
                                onClose={this.onIntroClose}
                                message="You are now in content review mode. Click on text to create new review entry."
                                actionText="Do not show this again"
                                stacked={true}
                            />
                        )}
                    </IframeOverlay>
                )}
                <ReviewsSlidingPanel iframe={this.props.iframe} />
                {this.state.newLocation && (
                    <NewReviewDialog
                        currentEditLocation={this.state.newLocation}
                        iframe={this.props.iframe}
                        onCloseDialog={(action, state) => this.onCloseDialog(action, state)}
                    />
                )}
            </>
        );
    }
}
