import React, { useState, useEffect } from "react";
import TextField, { Input } from "@material/react-text-field";
import { storiesOf } from "@storybook/react";
import { Provider } from "mobx-react";
import { createStores } from "../store/review-store";
import resources from "../../.storybook/resources.json";
import IframeWithPins from "./iframe-with-pins";
import FakeAdvancedReviewService from "../../.storybook/fake-advanced-review-service";

const stores = createStores(new FakeAdvancedReviewService(), resources);

function Component({ initialLocale = "en" }) {
    const [text, setText] = useState("Lina");
    const [iframeLoaded, setIframeLoaded] = useState(false);
    const [anchorElement, setAnchorElement] = useState(null);

    useEffect(() => {
        stores.reviewStore.currentUser = text;
        stores.reviewStore.currentLocale = initialLocale;

        setTimeout(() => {
            setIframeLoaded(true);
        }, 1000);
    });

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
                    overflowX: "auto"
                }}
            >
                <iframe
                    id="iframe"
                    ref={setAnchorElement}
                    style={{ width: "100%", height: "985px" }}
                    src="../../.storybook/fake_OPE.html"
                />
            </div>
            {!!anchorElement && iframeLoaded && <IframeWithPins iframe={anchorElement} />}
            <div className="user-picker">
                <TextField label="Current user" dense>
                    <Input value={text} onChange={(e: React.FormEvent<any>) => setText(e.currentTarget.value)} />
                </TextField>
            </div>
        </div>
    );
}

storiesOf("Dojo component", module)
    .add("default", () => {
        stores.reviewStore.load();
        return (
            <Provider {...stores}>
                <Component />
            </Provider>
        );
    })
    .add("Empty list", () => {
        stores.reviewStore.reviewLocations = [];
        return (
            <Provider {...stores}>
                <Component />
            </Provider>
        );
    })
    .add("Swedish locale", () => {
        stores.reviewStore.load();
        return (
            <Provider {...stores}>
                <Component initialLocale="sv" />
            </Provider>
        );
    });
