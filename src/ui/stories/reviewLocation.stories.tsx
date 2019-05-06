import React, { useState, useEffect } from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean,select } from '@storybook/addon-knobs';
import { action } from '@storybook/addon-actions';
import resources from './resources.json';
import ReviewLoacationComponent from "./../reviewLocationComponent";
import { Comment, Priority, ReviewLocation, createStores } from "./../reviewStore";
import FakeAdvancedReviewService from './FakeAdvancedReviewService';

const stories = storiesOf('Review location', module);
stories.addDecorator(withKnobs);

const stores = createStores(new FakeAdvancedReviewService(), resources);
stores.reviewStore.load();

const priorityOptions = {};
priorityOptions[Priority.Important] = 'Important';
priorityOptions[Priority.Normal] = 'Normal';
priorityOptions[Priority.Trivial] = 'Trivial';

const getReviewLocation = (isDone: boolean = false, priority: Priority = Priority.Normal, lastCommentFromOtherUser: boolean = false) => {
    const user = lastCommentFromOtherUser ? stores.reviewStore.currentUser + "1" : stores.reviewStore.currentUser;
    const reviewLocation = new ReviewLocation(stores.reviewStore, {
        id: '25',
        propertyName: 'test',
        isDone: isDone,
        positionX: 100,
        positionY: 100,
        priority: priority,
        firstComment: Comment.create(user, "aaaaa aaaaa", new Date('2019-02-03')),
        comments: []
    });
    return reviewLocation;
}

stories
    .add('default', () => {
        const location = getReviewLocation(boolean('Is done', false), select('Priority', priorityOptions, Priority.Normal), boolean('Is new', false));
        return <ReviewLoacationComponent location={location} showDialog={action('selectLocation')} />
    }
    )
    .add('done', () => <ReviewLoacationComponent location={getReviewLocation(true)} showDialog={action('selectLocation')} />)
    .add('high priority', () => <ReviewLoacationComponent location={getReviewLocation(false, Priority.Important)} showDialog={action('selectLocation')} />)
    .add('low priority', () => <ReviewLoacationComponent location={getReviewLocation(false, Priority.Trivial)} showDialog={action('selectLocation')} />)
    .add('updated', () => <ReviewLoacationComponent location={getReviewLocation(false, Priority.Normal, true)} showDialog={action('selectLocation')} />);
