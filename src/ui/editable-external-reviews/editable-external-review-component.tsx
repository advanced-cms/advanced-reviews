import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "mobx-react";
import axios from "axios";

import res from "../.storybook/resources.json";
import { createStores } from "../store/review-store";

import IframeWithLocations from "../iframe-with-pins/iframe-with-pins";

interface EditableExternalReviewProps {
    iframe: HTMLIFrameElement;
}

class ExternalReviewService implements AdvancedReviewService {
    add(id: string, data: any): Promise<any> {
        let result = new Promise((resolve, reject) => {
            axios
                .post(addUrl, {
                    message: "Fred",
                    priority: "Flintstone"
                })
                .then(function(response) {
                    resolve(response);
                })
                .catch(function(error) {
                    reject(error);
                });
        });
        let r = result as any;
        r.otherwise = result.catch;

        return result;
    }

    load(): Promise<any[]> {
        alert("Load");
        let result = new Promise<any[]>(resolve => {
            resolve([]);
        });
        let r = result as any;
        r.otherwise = result.catch;
        return result;
    }
}

function EditableExternalReviewComponent({ iframe }: EditableExternalReviewProps) {
    const reviewService = new ExternalReviewService();
    const stores = createStores(reviewService, res);

    stores.reviewStore.currentUser = "";
    stores.reviewStore.currentLocale = "";
    stores.reviewStore.reviewLocations = [];

    return (
        <Provider {...stores}>
            <IframeWithLocations iframe={iframe} />
        </Provider>
    );
}

const reviewEl = document.getElementById("reviews-editor");
const addUrl = reviewEl.dataset.url;
ReactDOM.render(
    <EditableExternalReviewComponent iframe={document.getElementById("editableIframe") as HTMLIFrameElement} />,
    reviewEl
);
