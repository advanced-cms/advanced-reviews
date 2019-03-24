import React from "react";
import { observer, inject } from 'mobx-react';
import { IReviewComponentStore, ReviewLocation } from './../reviewStore';

import IconButton from '@material/react-icon-button';
import MaterialIcon from '@material/react-material-icon';

import '@material/react-icon-button/index.scss';
import '@material/react-material-icon/index.scss';
import './pageNavigator.scss';

interface PageNavigatorProps {
    reviewStore?: IReviewComponentStore,
    onPrevClick(): void,
    onNextClick(): void,
    reviewLocation: ReviewLocation
}

@inject('reviewStore')
@observer
export default class PageNavigator extends React.Component<PageNavigatorProps, any> {

    isNextEnabled(): boolean {
        const store = this.props.reviewStore!;
        return (store.currentItemIndex < (store.reviewLocations.length - 1)) && !store.dialog.canSave;

    }

    isPrevEnabled(): boolean {
        const store = this.props.reviewStore!; 
        return store.currentItemIndex > 0 && !store.dialog.canSave;
    }
    
    render() {
        const { dialog, reviewLocations } = this.props.reviewStore!;

        return (
            <>
                {reviewLocations.length > 1 &&
                    <>
                        <IconButton className="next-prev-icon" title="prev" aria-pressed="false" disabled={!this.isPrevEnabled()} >
                            <MaterialIcon icon="chevron_left" onClick={this.props.onPrevClick} />
                        </IconButton>
                        <span>{reviewLocations.indexOf(dialog.currentEditLocation) + 1} / {reviewLocations.length}</span>
                        <IconButton className="next-prev-icon" title="next" onClick={this.props.onNextClick} disabled={!this.isNextEnabled()}>
                            <MaterialIcon icon="chevron_right" />
                        </IconButton>
                    </>}
            </>
        );
    }
}
