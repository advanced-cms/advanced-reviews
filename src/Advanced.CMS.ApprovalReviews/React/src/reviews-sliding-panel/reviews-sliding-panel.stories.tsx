import { storiesOf } from "@storybook/react";
import { Provider } from "mobx-react";
import React from "react";

import FakeAdvancedReviewService from "../../.storybook/fake-advanced-review-service";
import resources from "../../.storybook/resources.json";
import { createStores } from "../store/review-store";
import SlidingPanel from "./reviews-sliding-panel";

const stores = createStores(new FakeAdvancedReviewService(), resources);

storiesOf("Sliding panel", module).add("default", () => {
    stores.reviewStore.load();
    return (
        <Provider {...stores}>
            <SlidingPanel />
        </Provider>
    );
});
