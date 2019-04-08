import React, { useState, useEffect } from "react";
import TextField, { Input } from "@material/react-text-field";
import { storiesOf } from "@storybook/react";
import { Provider } from "mobx-react";
import { createStores } from "../reviewStore";
import resources from './resources.json';
import IframeWithLocations from "../IframeWithLocations";
import FakeAdvancedReviewService from "./FakeAdvancedReviewService";

const stores = createStores(new FakeAdvancedReviewService(), resources);

function Component() {
    const [text, setText] = useState("Lina");
    const [anchorElement, setAnchorElement] = useState(null);

    useEffect(() => {
        stores.reviewStore.currentUser = text;
    });

    return (
        <div>
            <div id="iframeWrapper" style={{width: "100%", height: "800px", position: "absolute", "top": "0", overflowY: "scroll", overflowX: "auto"}}>
                <iframe id="iframe" ref={setAnchorElement} style={{width: "100%", height: "985px"}} src="./stories/fake_OPE.html"/>
            </div>
            {!!anchorElement &&
                <IframeWithLocations iframe={anchorElement} />
            }
            <div className="user-picker">
                <TextField label="Current user" dense>
                    <Input value={text} onChange={e => setText(e.currentTarget.value)} />
                </TextField>
            </div>
        </div>
    );
}

storiesOf("Dojo component", module).add("default", () => {
    stores.reviewStore.load();
    return <Provider {...stores}>
        <Component />
    </Provider>
}).add("Empty list", () => {
    stores.reviewStore.reviewLocations = [];
    return (<Provider {...stores}>
        <Component />
    </Provider>)
});
