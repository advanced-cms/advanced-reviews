import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, PinLocation, NewPinDto, Dimensions } from "../store/review-store";
import NewReviewDialog from "../new-review-dialog/new-review-dialog";
import IframeOverlay from "../iframe-overlay/iframe-overlay";
import PinCollection from "../pin-collection/pin-collection";
import ReviewsSlidingPanel from "../reviews-sliding-panel/reviews-sliding-panel";
import PositionCalculator from "../position-calculator/position-calculator";

import { Snackbar } from "@material/react-snackbar";
import "@material/react-snackbar/index.scss";

interface IframeState {
    newLocation: PinLocation;
    documentSize: Dimensions;
}

interface IframeWithPinsProps {
    iframe: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
}

@inject("reviewStore")
@observer
export default class IframeWithPins extends React.Component<IframeWithPinsProps, IframeState> {
    constructor(props: IframeWithPinsProps) {
        super(props);

        this.state = {
            newLocation: null,
            documentSize: this.getIframeDimensions()
        };
    }

    private getIframeDimensions() {
        return { x: this.props.iframe.offsetWidth, y: this.props.iframe.offsetHeight };
    }

    private updateDimensions() {
        this.setState({ documentSize: this.getIframeDimensions() });
    }

    componentDidMount() {
        this.props.iframe.contentWindow.addEventListener("resize", this.updateDimensions.bind(this));
    }

    componentWillUnmount() {
        this.props.iframe.contentWindow.removeEventListener("resize", this.updateDimensions.bind(this));
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

        const positionCalculator = new PositionCalculator(
            { x: this.props.iframe.offsetWidth, y: this.props.iframe.offsetHeight },
            this.props.iframe.contentDocument
        );

        return (
            <>
                {this.props.reviewStore.filter.reviewMode && (
                    <IframeOverlay
                        iframe={this.props.iframe}
                        reviewLocationCreated={location => this.setState({ newLocation: location })}
                    >
                        <PinCollection newLocation={this.state.newLocation} positionCalculator={positionCalculator} />
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
