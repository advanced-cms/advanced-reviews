import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, boolean, select } from "@storybook/addon-knobs";
import { action } from "@storybook/addon-actions";
import resources from "../.storybook/resources.json";
import ReviewLoacationComponent from "./pin";
import { Comment, Priority, PinLocation, createStores } from "../store/review-store";
import FakeAdvancedReviewService from "../.storybook/fake-advanced-review-service";

const stories = storiesOf("Pin", module);
stories.addDecorator(withKnobs);

const stores = createStores(new FakeAdvancedReviewService(), resources);
stores.reviewStore.load();

const priorityOptions = {};
priorityOptions[Priority.Important] = "Important";
priorityOptions[Priority.Normal] = "Normal";
priorityOptions[Priority.Trivial] = "Trivial";

const getReviewLocation = (
    isDone: boolean = false,
    priority: Priority = Priority.Normal,
    lastCommentFromOtherUser: boolean = false
) => {
    const user = lastCommentFromOtherUser ? stores.reviewStore.currentUser + "1" : stores.reviewStore.currentUser;
    const reviewLocation = new PinLocation(stores.reviewStore, {
        id: "25",
        propertyName: "test",
        isDone: isDone,
        positionX: 100,
        positionY: 100,
        priority: priority,
        firstComment: Comment.create(user, "aaaaa aaaaa", stores.reviewStore, new Date("2019-02-03")),
        comments: []
    });
    return reviewLocation;
};

stories
    .add("default", () => {
        const location = getReviewLocation(
            boolean("Is done", false),
            select("Priority", priorityOptions, Priority.Normal),
            boolean("Is new", false)
        );
        return <ReviewLoacationComponent location={location} showDialog={action("selectLocation")} />;
    })
    .add("done", () => (
        <ReviewLoacationComponent location={getReviewLocation(true)} showDialog={action("selectLocation")} />
    ))
    .add("high priority", () => (
        <ReviewLoacationComponent
            location={getReviewLocation(false, Priority.Important)}
            showDialog={action("selectLocation")}
        />
    ))
    .add("low priority", () => (
        <ReviewLoacationComponent
            location={getReviewLocation(false, Priority.Trivial)}
            showDialog={action("selectLocation")}
        />
    ))
    .add("updated", () => (
        <ReviewLoacationComponent
            location={getReviewLocation(false, Priority.Normal, true)}
            showDialog={action("selectLocation")}
        />
    ));
