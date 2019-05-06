import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, ReviewLocation, NewReview } from "./reviewStore";
import NewReviewDialog from "./new-review-dialog/new-review-dialog";
import IframeOverlay from "./iframeOverlay";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import ReviewsSlidingPanel from "./reviews-sliding-panel/reviews-sliding-panel";
import { Snackbar } from "@material/react-snackbar";
import "@material/react-snackbar/index.scss";

interface IframeState {
    newLocation: ReviewLocation;
}

interface IframeWithLocationsProps {
    iframe: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
}

@inject("reviewStore")
@observer
export default class IFrameWithLocations extends React.Component<IframeWithLocationsProps, IframeState> {
    constructor(props: IframeWithLocationsProps) {
        super(props);

        this.state = {
            newLocation: null
        };
    }

    onCloseDialog(action: string, state: NewReview): void {
        if (action !== "save") {
            this.setState({
                newLocation: null
            });
            return;
        }

        const { save } = this.props.reviewStore!;
        save(state, this.state.newLocation)
            .then(() => {
                this.setState({
                    newLocation: null
                });
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
        //TODO: Set profile value
        alert("Save profile value");
    };

    render() {
        return (
            <>
                {this.props.reviewStore.filter.reviewMode && (
                    <IframeOverlay
                        iframe={this.props.iframe}
                        reviewLocationCreated={location => this.setState({ newLocation: location })}
                    >
                        <ReviewLocationsCollection newLocation={this.state.newLocation} />
                        {this.props.reviewStore.reviewLocations.length === 0 && (
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
