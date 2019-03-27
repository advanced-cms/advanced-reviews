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

    prevTitle(): string {
        let result = "prev";
        if (this.isPrevEnabled()) {
            const store = this.props.reviewStore!;
            return result + ": " + store.reviewLocations[store.currentItemIndex - 1].propertyName;
        }
        return result;
    }

    nextTitle(): string {
        let result = "next";
        if (this.isNextEnabled()) {
            const store = this.props.reviewStore!;
            return result + ": " + store.reviewLocations[store.currentItemIndex + 1].propertyName;
        }
        return result;
    }

    render() {
        const { dialog, reviewLocations } = this.props.reviewStore!;

        return (
            <>
                {reviewLocations.length > 1 &&
                    <>
                        <IconButton className="next-prev-icon" title={this.prevTitle()} aria-pressed="false" disabled={!this.isPrevEnabled()} >
                            <MaterialIcon icon="chevron_left" onClick={this.props.onPrevClick} />
                        </IconButton>
                        <span>{reviewLocations.indexOf(dialog.currentEditLocation) + 1} / {reviewLocations.length}</span>
                        <IconButton className="next-prev-icon" title={this.nextTitle()} onClick={this.props.onNextClick} disabled={!this.isNextEnabled()}>
                            <MaterialIcon icon="chevron_right" />
                        </IconButton>
                    </>}
            </>
        );
    }
}
