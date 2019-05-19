import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, PinLocation } from "../store/review-store";
import MaterialIcon from "@material/react-material-icon";
import IconButton from "@material/react-icon-button";
import Switch from "@material/react-switch";
import List, { ListItem, ListItemText } from "@material/react-list";
import { slide as Menu } from "react-burger-menu";

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

const PinTypeFilters = observer(({ filter }) => {
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
});

const Filters = observer(({ filter }) => {
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
});

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
                return this.props.reviewStore.editedPinLocation;
            },
            () => {
                this.setState({ panelVisible: true });
            }
        );
    }

    onSelected = (index: number): void => {
        //TODO: implement scroll into view for point
        this.props.reviewStore.selectedPinLocation = this.props.reviewStore.reviewLocations[index];
    };

    showPanel = () => {
        this.props.reviewStore.selectedPinLocation = null;
        this.props.reviewStore.editedPinLocation = null;
        this.setState({ panelVisible: true });
    };

    hidePanel = () => {
        this.props.reviewStore.selectedPinLocation = null;
        this.props.reviewStore.editedPinLocation = null;
        this.setState({ panelVisible: false });
    };

    handleStateChange = state => {
        if (state.isOpen) {
            this.showPanel();
        } else {
            this.hidePanel();
        }
    };

    onEditClick(e: any, location: PinLocation) {
        e.stopPropagation();
        this.props.reviewStore.selectedPinLocation = location;
        this.props.reviewStore.editedPinLocation = location;
    }

    render() {
        const { editedPinLocation, filter, reviewLocations } = this.props.reviewStore!;
        const res = this.props.resources!;

        var styles = {
            bmBurgerButton: {
                position: "absolute",
                width: "36px",
                height: "30px",
                right: "36px",
                top: "36px"
            },
            bmBurgerBars: {
                background: "#373a47"
            },
            bmCrossButton: {
                height: "36px",
                width: "36px"
            },
            bmMenuWrap: {
                position: "fixed",
                height: "99%"
            },
            bmMenu: {
                background: "white",
                padding: 20,
                fontSize: "1.15em",
                borderLeft: "1px solid #c0c0c0"
            },
            bmItemList: {
                height: "97%"
            },
            bmMorphShape: {
                fill: "#373a47"
            }
        };

        return (
            <Menu
                width={400}
                className="panel-container"
                styles={styles}
                isOpen={this.state.panelVisible}
                noOverlay
                disableOverlayClick
                disableAutoFocus
                right
                customCrossIcon={<MaterialIcon icon="close" />}
                customBurgerIcon={<MaterialIcon icon="format_list_bulleted" />}
                onStateChange={state => this.handleStateChange(state)}
            >
                <div>
                    <h3>
                        {editedPinLocation && (
                            <IconButton
                                title="Go back to list"
                                onClick={() => (this.props.reviewStore.editedPinLocation = null)}
                            >
                                <MaterialIcon icon="chevron_left" />
                            </IconButton>
                        )}
                        Review panel
                        {editedPinLocation && <PinNavigator />}
                    </h3>
                    {!editedPinLocation && (
                        <>
                            <Filters filter={filter} />
                            <List
                                singleSelection
                                selectedIndex={this.props.reviewStore.selectedPinLocationIndex}
                                handleSelect={activatedIndex => this.onSelected(activatedIndex)}
                                className="locations"
                            >
                                {reviewLocations.map(location => (
                                    <ListItem title={res.panel.clicktoedit} key={location.id}>
                                        <ListItemText primaryText={location.displayName} />
                                        <IconButton
                                            className="edit"
                                            title="Open details"
                                            onClick={e => this.onEditClick(e, location)}
                                        >
                                            <MaterialIcon icon="edit" />
                                        </IconButton>
                                    </ListItem>
                                ))}
                            </List>
                        </>
                    )}
                    {editedPinLocation && (
                        <ReviewDetails
                            iframe={this.props.iframe}
                            currentEditLocation={this.props.reviewStore.editedPinLocation}
                        />
                    )}
                </div>
            </Menu>
        );
    }
}
