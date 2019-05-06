import React from "react";
import { storiesOf } from "@storybook/react";
import ExternalReviewWidgetContent from "../external-reviews/external-review-widget-content";
import FakeReviewLinksStore from "./fake-review-links-store";
import { action } from "@storybook/addon-actions";

storiesOf("External review component", module)
    .add("default", () => {
        const store = new FakeReviewLinksStore();
        return <ExternalReviewWidgetContent store={store} />;
    })
    .add("empty", () => {
        const store = new FakeReviewLinksStore();
        store.links = [];
        return <ExternalReviewWidgetContent store={store} />;
    });
