import React from "react";
import { storiesOf } from "@storybook/react";
import DrawablePreview from "./drawable-preview";
import screenshots from "../../.storybook/screenshots.json";
import { createStores } from "../store/review-store";
import FakeAdvancedReviewService from "../../.storybook/fake-advanced-review-service";
import resources from "../../.storybook/resources";
import { Provider } from "mobx-react";

const stores = createStores(new FakeAdvancedReviewService(), resources);

storiesOf("Drawable preview", module)
    .add("with image background", () => {
        return (
            <Provider {...stores}>
                <>
                    <DrawablePreview
                        src={screenshots.idylla800x450}
                        width={800}
                        height={450}
                        onCancel={() => (document.getElementById("resultImage").src = "#")}
                        onApplyDrawing={result => (document.getElementById("resultImage").src = result)}
                    />
                    <img alt="" id="resultImage" />
                </>
            </Provider>
        );
    })
    .add("without image background", () => {
        return (
            <Provider {...stores}>
                <>
                    <DrawablePreview
                        width={800}
                        height={450}
                        onCancel={() => (document.getElementById("resultImage2").src = "#")}
                        onApplyDrawing={result => (document.getElementById("resultImage2").src = result)}
                    />
                    <img alt="" id="resultImage2" />
                </>
            </Provider>
        );
    });
