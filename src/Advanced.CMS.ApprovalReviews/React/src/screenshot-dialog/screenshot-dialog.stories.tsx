import { action } from "@storybook/addon-actions";
import { storiesOf } from "@storybook/react";
import { Provider } from "mobx-react";
import React, { useState } from "react";

import FakeAdvancedReviewService from "../../.storybook/fake-advanced-review-service";
import resources from "../../.storybook/resources.json";
import { createStores } from "../store/review-store";
import ScreenshotDialog from "./screenshot-dialog";

const stores = createStores(new FakeAdvancedReviewService(), resources);

export interface ComponentProps {
    propertyName?: string;
}

const Component = ({ propertyName }: ComponentProps) => {
    const [anchorElement, setAnchorElement] = useState(null);
    const [iframeLoaded, setIframeLoaded] = useState(false);

    const onIframeLoaded = () => {
        setIframeLoaded(true);
    };

    return (
        <div>
            <div
                id="iframeWrapper"
                style={{
                    width: "100%",
                    height: "800px",
                    position: "absolute",
                    top: "0",
                    overflowY: "scroll",
                    overflowX: "auto",
                }}
            >
                <iframe
                    id="iframe"
                    onLoad={onIframeLoaded}
                    ref={setAnchorElement}
                    style={{ width: "100%", height: "985px" }}
                    src="../../.storybook/fake_OPE.html"
                />
            </div>

            {!!anchorElement && iframeLoaded && (
                <ScreenshotDialog
                    propertyName={propertyName}
                    maxWidth={500}
                    maxHeight={300}
                    iframe={anchorElement}
                    onImageSelected={action("image selected")}
                    toggle={() => {}}
                />
            )}
        </div>
    );
};

storiesOf("Screenshot picker", module)
    .add("default", () => {
        return (
            <Provider {...stores}>
                <Component propertyName="" />
            </Provider>
        );
    })
    .add("with property", () => {
        return (
            <Provider {...stores}>
                <Component propertyName="MetaDescription2" />
            </Provider>
        );
    });
