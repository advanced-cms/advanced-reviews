import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, PinLocation } from "../store/review-store";
import MaterialIcon from "@material/react-material-icon";
import IconButton from "@material/react-icon-button";
import Switch from "@material/react-switch";
import List, { ListItem, ListItemText } from "@material/react-list";

import "@material/react-switch/index.scss";
import "./reviews-sliding-panel.scss";
import { ReviewDetails } from "../details/review-details";
import PinNavigator from "../pin-navigator/pin-navigator";
import { IReactionDisposer, reaction } from "mobx";

interface SlidingPanelProps {
    iframe?: HTMLIFrameElement;
    reviewStore?: IReviewComponentStore;
    resources?: ReviewResources;
}

const PinTypeFilters = ({ filter }) => {
    return (
        <div className="type-filters">
            <div className="filter unread">
                <Switch
                    nativeControlId="showCustom"
                    checked={filter.showUnread}
                    onChange={() => (filter.showUnread = !filter.showUnread)}
                />
                <label htmlFor="showCustom">Show Unread</label>
            </div>
            <div className="filter active">
                <Switch
                    nativeControlId="showActive"
                    checked={filter.showActive}
                    onChange={() => (filter.showActive = !filter.showActive)}
                />
                <label htmlFor="showActive">Show Active</label>
            </div>
            <div className="filter resolved">
                <Switch
                    nativeControlId="showResolved"
                    checked={filter.showResolved}
                    onChange={() => (filter.showResolved = !filter.showResolved)}
                />
                <label htmlFor="showResolved">Show Resolved</label>
            </div>
        </div>
    );
};

const Filters = ({ filter }) => {
    return (
        <div>
            <div className="filter main-filter">
                <Switch
                    nativeControlId="modeSwitcher"
                    checked={filter.reviewMode}
                    onChange={() => (filter.reviewMode = !filter.reviewMode)}
                />
                <label htmlFor="modeSwitcher">Display Review Overlay</label>
            </div>
            {filter.reviewMode && <PinTypeFilters filter={filter} />}
        </div>
    );
};

@inject("resources")
@inject("reviewStore")
@observer
export default class SlidingPanel extends React.Component<SlidingPanelProps, any> {
    locationChangedReaction: IReactionDisposer;

    constructor(props: SlidingPanelProps) {
        super(props);
        this.state = {
            panelVisible: true
        };

        this.locationChangedReaction = reaction(
            () => {
                return this.props.reviewStore.currentLocation;
            },
            () => {
                this.setState({ panelVisible: true });
            }
        );
    }

    onSelected(index: number): void {
        //this.props.reviewStore.reviewLocations[index]
        //TODO: implement scroll into view for point
    }

    showPanel = () => {
        this.props.reviewStore.currentLocation = null;
        this.setState({ panelVisible: true });
    };

    hidePanel = () => {
        this.props.reviewStore.currentLocation = null;
        this.setState({ panelVisible: false });
    };

    onEditClick(e: any, location: PinLocation) {
        e.stopPropagation();
        this.props.reviewStore.currentLocation = location;
    }

    render() {
        const { currentLocation, filter, reviewLocations } = this.props.reviewStore!;
        const res = this.props.resources!;

        return (
            <>
                <IconButton className="panel-toggle" onClick={this.showPanel}>
                    <MaterialIcon icon="format_list_bulleted" />
                </IconButton>
                {this.state.panelVisible && (
                    <div className="panel-container">
                        <h3>
                            {currentLocation && (
                                <IconButton
                                    title="Go back to list"
                                    onClick={() => (this.props.reviewStore.currentLocation = null)}
                                >
                                    <MaterialIcon icon="chevron_left" />
                                </IconButton>
                            )}
                            Review panel
                            <IconButton className="close-panel" onClick={this.hidePanel}>
                                <MaterialIcon icon="close" />
                            </IconButton>
                            <PinNavigator />
                        </h3>
                        {!currentLocation && (
                            <>
                                <Filters filter={filter} />
                                <div>
                                    <List singleSelection handleSelect={this.onSelected} className="locations">
                                        {reviewLocations.map(location => (
                                            <ListItem
                                                title={res.panel.clicktoedit}
                                                key={location.id}
                                                onClick={e => this.onEditClick(e, location)}
                                            >
                                                <ListItemText primaryText={location.displayName} />
                                                <IconButton className="edit" title="Open details">
                                                    <MaterialIcon icon="edit" />
                                                </IconButton>
                                            </ListItem>
                                        ))}
                                    </List>
                                </div>
                            </>
                        )}
                        {currentLocation && (
                            <ReviewDetails
                                iframe={this.props.iframe}
                                currentEditLocation={this.props.reviewStore.currentLocation}
                            />
                        )}
                    </div>
                )}
            </>
        );
    }
}
