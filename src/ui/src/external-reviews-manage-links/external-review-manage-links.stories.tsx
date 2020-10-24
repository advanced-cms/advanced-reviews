import React from "react";
import { storiesOf } from "@storybook/react";
import ExternalReviewWidgetContent from "./external-review-manage-links";
import FakeReviewLinksStore from "./fake-review-links-store";
import res from "../../.storybook/externalResources.json";

const visitorGroups = [{ name: "Visitor group 1", id: "1" }, { name: "Visitor group 2", id: "2" }];

storiesOf("External reviews/Review component", module)
    .add("default", () => {
        const store = new FakeReviewLinksStore();
        store.initialMailSubject = "Review request";
        store.initialEditMailMessage =
            "EDIT: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed egestas rutrum lacus eget dapibus. Aenean eleifend commodo felis vitae convallis.";
        store.initialViewMailMessage =
            "VIEW: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed egestas rutrum lacus eget dapibus. Aenean eleifend commodo felis vitae convallis.";
        return (
            <ExternalReviewWidgetContent
                store={store}
                editableLinksEnabled={true}
                resources={res}
                availableVisitorGroups={visitorGroups}
                pinCodeSecurityEnabled={false}
                pinCodeLength={4}
            />
        );
    })
    .add("empty", () => {
        const store = new FakeReviewLinksStore();
        store.links = [];
        return (
            <ExternalReviewWidgetContent
                store={store}
                editableLinksEnabled={true}
                resources={res}
                availableVisitorGroups={visitorGroups}
                pinCodeSecurityEnabled={false}
                pinCodeLength={4}
            />
        );
    })
    .add("only view links", () => {
        const store = new FakeReviewLinksStore();
        store.links = [];
        return (
            <ExternalReviewWidgetContent
                store={store}
                editableLinksEnabled={false}
                resources={res}
                availableVisitorGroups={visitorGroups}
                pinCodeSecurityEnabled={false}
                pinCodeLength={4}
            />
        );
    })
    .add("with PIN security", () => {
        const store = new FakeReviewLinksStore();
        return (
            <ExternalReviewWidgetContent
                store={store}
                editableLinksEnabled={true}
                resources={res}
                availableVisitorGroups={visitorGroups}
                pinCodeSecurityEnabled={true}
                pinCodeLength={4}
            />
        );
    })
    .add("with required PIN security", () => {
        const store = new FakeReviewLinksStore();
        return (
            <ExternalReviewWidgetContent
                store={store}
                editableLinksEnabled={true}
                resources={res}
                availableVisitorGroups={visitorGroups}
                pinCodeSecurityEnabled={true}
                pinCodeSecurityRequired={true}
                pinCodeLength={4}
            />
        );
    })
    .add("with PIN security not enabled", () => {
        const store = new FakeReviewLinksStore();
        return (
            <ExternalReviewWidgetContent
                store={store}
                editableLinksEnabled={true}
                resources={res}
                availableVisitorGroups={visitorGroups}
                pinCodeSecurityEnabled={false}
                pinCodeLength={4}
            />
        );
    });
