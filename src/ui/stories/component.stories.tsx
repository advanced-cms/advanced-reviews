import React, { useState, useEffect } from "react";
import TextField, { Input } from "@material/react-text-field";
import { storiesOf } from "@storybook/react";
import { Provider } from "mobx-react";
import { createStores } from "../reviewStore";
import resources from './resources.json';
import ReviewLocationsCollection from "../reviewLocationsCollection";
import IframeOverlay from "../iframeOverlay";
import FakeAdvancedReviewService from "./FakeAdvancedReviewService";

const stores = createStores(new FakeAdvancedReviewService(), resources);
stores.reviewStore.load();

function Component() {
    const [text, setText] = useState("Lina");

    useEffect(() => {
        stores.reviewStore.currentUser = text;
    });

    const iframe = document.getElementById("iframe") as HTMLIFrameElement;

    return (
        <div>
            <IframeOverlay iframe={iframe}>
                <ReviewLocationsCollection iframe={iframe} />
            </IframeOverlay>
            <div className="user-picker">
                <TextField label="Current user" dense>
                    <Input value={text} onChange={e => setText(e.currentTarget.value)} />
                </TextField>
            </div>
        </div>
    );
}

storiesOf("Dojo component", module).add("default", () => (
    <>
        <iframe id="iframe" style={{"width": "800px", "height": "800px" }} src="stories/fake_OPE.html"></iframe>
        <Provider {...stores}>
            <Component />
        </Provider>
    </>
));
