import React from "react";
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore, ReviewLocation } from './../reviewStore';
import Checkbox from "@material/react-checkbox";
import MaterialIcon from '@material/react-material-icon';
import IconButton from '@material/react-icon-button';

import "@material/react-checkbox/index.scss";
import List, {
    ListItem,
    ListItemText,
} from "@material/react-list";
import './reviews-sliding-panel.scss';

interface SlidingPanelProps {
    reviewStore?: IReviewComponentStore,
    onEditClick(reviewLocation: ReviewLocation): void
}

@inject('reviewStore')
@observer
export default class SlidingPanel extends React.Component<SlidingPanelProps, any> {
    constructor(props: SlidingPanelProps) {
        super(props);
        this.state = {
            panelVisible: false,
        }
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
                {this.state.panelVisible &&
                    <div className="panel-container">
                        <div>
                            <div>
                                <label className='filter'>
                                    <Checkbox
                                        nativeControlId="showReviewLocations"
                                        checked={filter.showPoints}
                                        onChange={e => filter.showPoints = !filter.showPoints}
                                    />
                                    <span>Show reviews</span>
                                </label>

                                <IconButton className="close-panel" onClick={() => this.setState({ panelVisible: false })}>
                                    <MaterialIcon icon="close" />
                                </IconButton>
                            </div>
                            <div className="filters-container">
                                <label className='filter'>
                                    <Checkbox
                                        nativeControlId="doneTasks"
                                        checked={filter.showOnlyOpenTasks}
                                        onChange={e => filter.showOnlyOpenTasks = !filter.showOnlyOpenTasks}
                                    />
                                    <span>Only open tasks</span>
                                </label>
                                <label className='filter'>
                                    <Checkbox
                                        nativeControlId="newTasks"
                                        checked={filter.showOnlyNewTasks}
                                        onChange={e => filter.showOnlyNewTasks = !filter.showOnlyNewTasks}
                                    />
                                    <span>Only new tasks</span>
                                </label>
                            </div>
                        </div>
                        <div>
                            <List singleSelection handleSelect={this.onSelected} className="locations">
                                {reviewLocations.map((location, index) =>
                                    <ListItem key={location.id}>
                                        <ListItemText primaryText={location.displayName} />
                                        <IconButton className="edit" onClick={(e) => this.onEditClick(e, location)} title="Edit" >
                                            <MaterialIcon icon="edit" />
                                        </IconButton>
                                    </ListItem>
                                )}
                            </List>
                        </div>
                    </div>
                }
            </>
        );
    }
}