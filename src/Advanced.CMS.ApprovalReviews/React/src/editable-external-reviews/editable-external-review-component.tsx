import React, { useState } from "react";
import ReactDOM from "react-dom";
import { Provider } from "mobx-react";
import axios from "axios";

import res from "../../.storybook/resources.json";
import { createStores } from "../store/review-store";

import IframeWithPins from "../iframe-with-pins/iframe-with-pins";
import ConfirmNameDialog from "./confirm-name-dialog";

import "@episerver/ui-framework/dist/main.css";

interface EditableExternalReviewProps {
    iframe: HTMLIFrameElement;
}

class ExternalReviewService implements AdvancedReviewService {
    add(id: string, data: any): Promise<any> {
        let result = new Promise((resolve, reject) => {
            axios
                .post(addUrl, {
                    token: token,
                    id: id,
                    data: JSON.stringify(data)
                })
                .then(function(response) {
                    resolve(response.data);
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
        const parseResponse = reviewLocations => {
            return reviewLocations
                .map(x => {
                    var reviewLocation;
                    try {
                        reviewLocation = JSON.parse(x.data);
                    } catch (exception) {
                        reviewLocation = null;
                    }
                    return {
                        id: x.id,
                        data: reviewLocation
                    };
                })
                .filter(x => !!x.data);
        };

        let pins = [];
        try {
            pins = parseResponse(JSON.parse(initialPins));
        } catch (ex) {
            console.warn("Couldn't load pins", ex);
        }

        let result = new Promise<any[]>(resolve => {
            resolve(pins);
        });
        let r = result as any;
        r.otherwise = result.catch;
        return result;
    }

    remove(id: string): Promise<any> {
        const result = new Promise((resolve, reject) => {
            axios
                .post(removeUrl, {
                    token: token,
                    id: id
                })
                .then(function() {
                    resolve();
                })
                .catch(function(error) {
                    reject(error);
                });
        });

        // make Promise compatible with dojo
        const r = result as any;
        r.__proto__.otherwise = r.__proto__.catch;

        return result;
    }
}

const reviewService = new ExternalReviewService();
const stores = createStores(reviewService, res);

stores.reviewStore.currentUser = "";
stores.reviewStore.currentLocale = "";
stores.reviewStore.reviewLocations = [];
stores.reviewStore.options = {};
stores.reviewStore.reviewUrl = "";

function EditableExternalReviewComponent({ iframe }: EditableExternalReviewProps) {
    const [showUserNameDialog, setShowUserNameDialog] = useState<boolean>(true);
    stores.reviewStore.options = JSON.parse(options);

    const setUserName = (newUserName: string) => {
        stores.reviewStore.currentUser = newUserName;
        try {
            const properties = {};
            const meta = JSON.parse(metadata);
            Object.keys(meta).forEach(key => {
                const newKey = key[0].toUpperCase() + key.substring(1);
                properties[newKey] = meta[key];
            });
            stores.reviewStore.updateDisplayNamesDictionary(properties);
        } catch (ex) {
            console.warn("Couldn't parse metadata", ex);
        }
        stores.reviewStore.load();
        setShowUserNameDialog(false);
    };

    return (
        <>
            {showUserNameDialog ? (
                <ConfirmNameDialog open={true} initialUserName={userName} onClose={setUserName} />
            ) : (
                <Provider {...stores}>
                    <IframeWithPins iframe={iframe} external />
                </Provider>
            )}
        </>
    );
}

const reviewEl = document.getElementById("reviews-editor");
const addUrl = reviewEl.dataset.addUrl;
const removeUrl = reviewEl.dataset.removeUrl;
const userName: string = reviewEl.dataset.user;
const initialPins: string = reviewEl.dataset.pins;
const metadata: string = reviewEl.dataset.metadata;
const options: string = reviewEl.dataset.options;
const token: string = reviewEl.dataset.token;
const avatarUrl: string = reviewEl.dataset.avatarUrl;

stores.reviewStore.avatarUrl = avatarUrl;

ReactDOM.render(
    <EditableExternalReviewComponent iframe={document.getElementById("editableIframe") as HTMLIFrameElement} />,
    reviewEl
);
