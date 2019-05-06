import React from "react";
import { storiesOf } from "@storybook/react";
import SlidingPanel from "../reviews-sliding-panel/reviews-sliding-panel";
import { Provider } from "mobx-react";
import {createStores} from "../reviewStore";
import FakeAdvancedReviewService from "./FakeAdvancedReviewService";
import resources from "./resources.json";

const stores = createStores(new FakeAdvancedReviewService(), resources);

storiesOf("Sliding panel", module).add("default", () => {
    stores.reviewStore.load();
    return (
        <Provider {...stores}>
            <SlidingPanel />
        </Provider>
    );
});
