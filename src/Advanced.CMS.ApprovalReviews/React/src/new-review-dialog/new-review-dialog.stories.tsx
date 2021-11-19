import React from "react";
import { storiesOf } from "@storybook/react";
import { Provider } from "mobx-react";
import { createStores, PinLocation, Comment } from "../store/review-store";
import resources from "../../.storybook/resources.json";
import NewReviewDialog from "./new-review-dialog";
import { decorate } from "@storybook/addon-actions";
import screenshots from "../../.storybook/screenshots.json";
import FakeAdvancedReviewService from "../../.storybook/fake-advanced-review-service";

const stores = createStores(new FakeAdvancedReviewService(), resources);
stores.reviewStore.currentUser = "Lina";
stores.reviewStore.load();

const reviewLocation1 = new PinLocation(stores.reviewStore, {
    id: "1",
    documentRelativePosition: { x: 10, y: 80 },
    propertyName: "Page name",
    isDone: false,
    firstComment: Comment.create("Alfred", "Rephrase it. ", stores.reviewStore, new Date("2019-01-01")),
    comments: [
        Comment.create(
            "Lina",
            "Could you describe it better?",
            stores.reviewStore,
            new Date("2019-01-02"),
            screenshots.idylla
        ),
        Comment.create(
            "Alfred",
            "Remove last sentence and include more information in first paragraph.",
            stores.reviewStore,
            new Date("2019-01-03")
        ),
        Comment.create("Lina", "Ok, done.", stores.reviewStore, new Date("2019-01-04"), screenshots.idylla),
        Comment.create(
            "Alfred",
            "I still see old text",
            stores.reviewStore,
            new Date("2019-03-18"),
            screenshots.idylla
        ),
        Comment.create(
            "Lina",
            "Probably something with the CMS. Now it should be ok",
            stores.reviewStore,
            new Date("2019-03-19")
        ),
        Comment.create("Alfred", "Looks ok.", stores.reviewStore, new Date("2019-03-19")),
        Comment.create(
            "Lina",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean sed nisi in erat posuere luctus.",
            stores.reviewStore,
            new Date("2019-03-20")
        ),
        Comment.create(
            "Alfred",
            "Vivamus sem est, aliquet eget nunc quis, imperdiet cursus sapien. Mauris ullamcorper dui ut nisl vulputate vestibulum.",
            stores.reviewStore,
            new Date("2019-03-21")
        ),
        Comment.create(
            "Lina",
            "Sed non nisi in odio facilisis aliquam eget volutpat augue. Phasellus vitae auctor risus, non luctus dolor.",
            stores.reviewStore,
            new Date("2019-03-22")
        ),
        Comment.create(
            "Alfred",
            "Integer sed libero at odio mattis sodales. Ut dapibus erat cursus porttitor malesuada.",
            stores.reviewStore,
            new Date("2019-03-23")
        )
    ]
});

const reviewLocation2 = new PinLocation(stores.reviewStore, {
    id: "1",
    documentRelativePosition: { x: 10, y: 80 },
    propertyName: "Page name",
    isDone: false,
    firstComment: Comment.create("Alfred", "Rephrase it. ", stores.reviewStore, new Date("2019-01-01")),
    comments: []
});

function createEmptyLocation(): PinLocation {
    return new PinLocation(this, {
        id: "1",
        documentRelativePosition: { x: 10, y: 80 },
        propertyName: "Content area 1",
        isDone: false,
        firstComment: {},
        comments: []
    });
}

stores.reviewStore.reviewLocations = [reviewLocation1];

const firstArg = decorate([args => args.slice(0, 1)]);

storiesOf("Dialog", module)
    .add("default", () => {
        stores.reviewStore.reviewLocations = [reviewLocation1];
        return (
            <Provider {...stores}>
                <NewReviewDialog currentEditLocation={reviewLocation1} onCloseDialog={firstArg.action("test1")} />
            </Provider>
        );
    })
    .add("with two comments", () => {
        stores.reviewStore.reviewLocations = [reviewLocation1, reviewLocation2];
        return (
            <Provider {...stores}>
                <NewReviewDialog currentEditLocation={reviewLocation1} onCloseDialog={firstArg.action("test1")} />
            </Provider>
        );
    })
    .add("with empty comment", () => {
        const location = createEmptyLocation();
        stores.reviewStore.reviewLocations = [location];
        return (
            <Provider {...stores}>
                <NewReviewDialog currentEditLocation={location} onCloseDialog={firstArg.action("test1")} />
            </Provider>
        );
    })
    .add("with long property name", () => {
        const location = createEmptyLocation();
        location.propertyName = "veryyyy long propertyyyyyyyyy nameeeeeeeeeeeeeeeeeeeeee";
        stores.reviewStore.reviewLocations = [location];
        return (
            <Provider {...stores}>
                <NewReviewDialog currentEditLocation={location} onCloseDialog={firstArg.action("test1")} />
            </Provider>
        );
    });
