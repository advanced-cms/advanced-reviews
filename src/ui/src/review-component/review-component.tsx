import React, { useState, useEffect } from "react";
import ReactDOM from "react-dom";
import IframeWithPins from "../iframe-with-pins/iframe-with-pins";
import { createStores } from "../store/review-store";
import resources from "../../.storybook/resources.json";
import { Provider } from "mobx-react";
import TextField, { Input } from "@material/react-text-field";
import FakeAdvancedReviewService from "../../.storybook/fake-advanced-review-service";

//TODO: async
const stores = createStores(new FakeAdvancedReviewService(), resources);
stores.reviewStore.load();
stores.resources = resources;

function Component() {
    const [text, setText] = useState("Lina");

    useEffect(() => {
        stores.reviewStore.currentUser = text;
    });

    const iframe = document.getElementById("iframe") as HTMLIFrameElement;

    return (
        <div>
            <IframeWithPins iframe={iframe} />
            <div className="user-picker">
                <TextField label="Current user" dense>
                    <Input value={text} onChange={e => setText(e.currentTarget.value)} />
                </TextField>
            </div>
        </div>
    );
}

ReactDOM.render(
    <Provider {...stores}>
        <Component />
    </Provider>,
    document.getElementById("index")
);

//TODO: remove
//https://medium.com/teachable/getting-started-with-react-typescript-mobx-and-webpack-4-8c680517c030
//https://www.nealbuerger.com/2018/11/11/getting-started-with-mobx-5-and-typescript-3-react-16-6/
