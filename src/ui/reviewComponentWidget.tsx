import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "mobx-react";
import declare from "dojo/_base/declare";
import topic from "dojo/topic";
import WidgetBase from "dijit/_WidgetBase";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import res from "epi/i18n!epi/cms/nls/reviewcomponent";

import { stores } from "./reviewStore";
import IframeOverlay from "./iframeOverlay";

//TODO: async
stores.reviewStore.load();
stores.resources = res;

export default declare([WidgetBase], {
    postCreate: function() {
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
