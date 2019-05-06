import React from "react";
import { storiesOf } from "@storybook/react";
import SlidingPanel from "./reviews-sliding-panel";
import { Provider } from "mobx-react";
import { createStores } from "../store/review-store";
import FakeAdvancedReviewService from "../.storybook/fake-advanced-review-service";
import resources from "../.storybook/resources.json";

const stores = createStores(new FakeAdvancedReviewService(), resources);

storiesOf("Sliding panel", module).add("default", () => {
    stores.reviewStore.load();
    return (
        <Provider {...stores}>
            <SlidingPanel />
        </Provider>
    );
});
