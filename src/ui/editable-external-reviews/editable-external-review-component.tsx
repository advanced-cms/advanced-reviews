import React, { useState } from "react";
import ReactDOM from "react-dom";
import { Provider } from "mobx-react";
import axios from "axios";

import res from "../.storybook/resources.json";
import { createStores } from "../store/review-store";

import IframeWithLocations from "../iframe-with-pins/iframe-with-pins";
import ConfirmNameDialog from "./confirm-name-dialog";

interface EditableExternalReviewProps {
    iframe: HTMLIFrameElement;
}

class ExternalReviewService implements AdvancedReviewService {
    add(id: string, data: any): Promise<any> {
        let result = new Promise((resolve, reject) => {
            axios
                .post(addUrl, {
                    id: id,
                    data: JSON.stringify(data)
                })
                .then(function(response) {
                    const data = JSON.parse(response.data.substring(4)); //remove {}&&
                    resolve(data);
                })
                .catch(function(error) {
                    reject(error);
                });
        });

        // make Promise compatibile with dojo
        let r = result as any;
        r.__proto__.otherwise = r.__proto__.catch;

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

const reviewService = new ExternalReviewService();
const stores = createStores(reviewService, res);

stores.reviewStore.currentUser = "";
stores.reviewStore.currentLocale = "";
stores.reviewStore.reviewLocations = [];

function EditableExternalReviewComponent({ iframe }: EditableExternalReviewProps) {
    const [showUserNameDialog, setShowUserNameDialog] = useState<boolean>(true);

    const setUserName = (newUserName: string) => {
        stores.reviewStore.currentUser = newUserName;
        setShowUserNameDialog(false);
    };

    return (
        <>
            {showUserNameDialog ? (
                <ConfirmNameDialog open={true} initialUserName={userName} onClose={setUserName} />
            ) : (
                <Provider {...stores}>
                    <IframeWithLocations iframe={iframe} />
                </Provider>
            )}
        </>
    );
}

const reviewEl = document.getElementById("reviews-editor");
const addUrl = reviewEl.dataset.url;
const userName: string = reviewEl.dataset.user;
ReactDOM.render(
    <EditableExternalReviewComponent iframe={document.getElementById("editableIframe") as HTMLIFrameElement} />,
    reviewEl
);
