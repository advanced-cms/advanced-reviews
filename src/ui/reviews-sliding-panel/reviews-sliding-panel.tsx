import React from "react";
import { observer, inject } from "mobx-react";
import { IReviewComponentStore, ReviewLocation } from "../reviewStore";
import MaterialIcon from "@material/react-material-icon";
import IconButton from "@material/react-icon-button";
import Switch from "@material/react-switch";
import List, { ListItem, ListItemText } from "@material/react-list";

import "@material/react-switch/index.scss";
import "./reviews-sliding-panel.scss";

interface SlidingPanelProps {
    reviewStore?: IReviewComponentStore;
    onEditClick(reviewLocation: ReviewLocation): void;
}

@inject("reviewStore")
@observer
export default class SlidingPanel extends React.Component<SlidingPanelProps, any> {
    constructor(props: SlidingPanelProps) {
        super(props);
        this.state = {
            panelVisible: true
        };
    }

    onSelected(index: number): void {
        //this.props.reviewStore!.reviewLocations[index]
        //TODO: implement scroll into view for point
    }

    onEditClick(e: any, location: ReviewLocation) {
        e.stopPropagation();
        this.props.onEditClick(location);
    }

    render() {
        const { reviewLocations, filter } = this.props.reviewStore!;

        return (
            <>
                <IconButton className="panel-toggle" onClick={() => this.setState({ panelVisible: true })}>
                    <MaterialIcon icon="format_list_bulleted" />
                </IconButton>
                {this.state.panelVisible && (
                    <div className="panel-container">
                        <div>
                            <div className="filter main-filter">
                                <Switch
                                    nativeControlId="modeSwitcher"
                                    checked={filter.showPoints}
                                    onChange={() => (filter.showPoints = !filter.showPoints)}
                                />
                                <label htmlFor="modeSwitcher">Display Review Overlay</label>

                                <IconButton
                                    className="close-panel"
                                    onClick={() => this.setState({ panelVisible: false })}
                                >
                                    <MaterialIcon icon="close" />
                                </IconButton>
                            </div>
                            {filter.showPoints && (
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
                            )}
                        </div>
                        <div>
                            <List singleSelection handleSelect={this.onSelected} className="locations">
                                {reviewLocations.map((location, index) => (
                                    <ListItem key={location.id}>
                                        <ListItemText primaryText={location.displayName} />
                                        <IconButton
                                            className="edit"
                                            onClick={e => this.onEditClick(e, location)}
                                            title="Edit"
                                        >
                                            <MaterialIcon icon="edit" />
                                        </IconButton>
                                    </ListItem>
                                ))}
                            </List>
                        </div>
                    </div>
                )}
            </>
        );
    }
}
