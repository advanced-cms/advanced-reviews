import { storiesOf } from "@storybook/react";
import React from "react";

import res from "../../.storybook/externalResources.json";
import ExternalReviewWidgetContent, { ExternalReviewWidgetContentProps } from "./external-review-manage-links";
import FakeReviewLinksStore from "./fake-review-links-store";

const visitorGroups = [
    { name: "Visitor group 1", id: "1" },
    { name: "Visitor group 2", id: "2" },
];

const getDefaultProps = (store: FakeReviewLinksStore): ExternalReviewWidgetContentProps => ({
    store: store,
    editableLinksEnabled: true,
    resources: res,
    availableVisitorGroups: visitorGroups,
    pinCodeSecurityEnabled: false,
    pinCodeLength: 4,
    prolongDays: 5,
});

storiesOf("External reviews/Review component", module)
    .add("default", () => {
        const store = new FakeReviewLinksStore();
        store.initialMailSubject = "Review request";
        store.initialEditMailMessage =
            "EDIT: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed egestas rutrum lacus eget dapibus. Aenean eleifend commodo felis vitae convallis.";
        store.initialViewMailMessage =
            "VIEW: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed egestas rutrum lacus eget dapibus. Aenean eleifend commodo felis vitae convallis.";
        return <ExternalReviewWidgetContent {...getDefaultProps(store)} />;
    })
    .add("empty", () => {
        const store = new FakeReviewLinksStore();
        store.links = [];
        return <ExternalReviewWidgetContent {...getDefaultProps(store)} />;
    })
    .add("only view links", () => {
        const store = new FakeReviewLinksStore();
        store.links = [];
        return <ExternalReviewWidgetContent {...getDefaultProps(store)} editableLinksEnabled={false} />;
    })
    .add("with PIN security", () => {
        const store = new FakeReviewLinksStore();
        return (
            <ExternalReviewWidgetContent {...getDefaultProps(store)} pinCodeSecurityEnabled={true} pinCodeLength={4} />
        );
    })
    .add("with required PIN security", () => {
        const store = new FakeReviewLinksStore();
        return (
            <ExternalReviewWidgetContent
                {...getDefaultProps(store)}
                pinCodeSecurityEnabled={true}
                pinCodeSecurityRequired={true}
                pinCodeLength={4}
            />
        );
    })
    .add("with PIN security not enabled", () => {
        const store = new FakeReviewLinksStore();
        return (
            <ExternalReviewWidgetContent {...getDefaultProps(store)} pinCodeSecurityEnabled={false} pinCodeLength={4} />
        );
    });
