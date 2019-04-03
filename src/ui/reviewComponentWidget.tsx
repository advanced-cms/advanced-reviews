import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "mobx-react";
import declare from "dojo/_base/declare";
import topic from "dojo/topic";
import WidgetBase from "dijit/_WidgetBase";
import ReviewService from "alloy-review/advancedReviewService";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import res from "epi/i18n!epi/cms/nls/reviewcomponent";

import { createStores } from "./reviewStore";
import IframeOverlay from "./iframeOverlay";

export default declare([WidgetBase], {
    postCreate: function() {
        //TODO: async
        this._reviewService = new ReviewService();
        this.own(this._reviewService);
        const stores = createStores(this._reviewService, res);
        stores.reviewStore.load();
        
        topic.subscribe("toggle:reviews", () => {
            this.domNode.style.display = this.domNode.style.display === "none" ? "block" : "none";
        });

        ReactDOM.render(
            <Provider {...stores}>
                <IframeOverlay iframe={this.iframe}>
                    <ReviewLocationsCollection iframe={this.iframe} />
                </IframeOverlay>
            </Provider>,
            this.domNode
        );
    },
    destroy: function() {
        ReactDOM.unmountComponentAtNode(this.domNode);
    }
});
