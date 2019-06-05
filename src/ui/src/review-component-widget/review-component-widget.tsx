import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "mobx-react";
import declare from "dojo/_base/declare";
import WidgetBase from "dijit/_WidgetBase";
import ApplicationSettings from "epi-cms/ApplicationSettings";
import _ContentContextMixin from "epi-cms/_ContentContextMixin";
import ReviewService from "episerver-addons-review/advancedReviewService";
import IframeWithPins from "../iframe-with-pins/iframe-with-pins";
import res from "epi/i18n!epi/cms/nls/reviewcomponent";

import { createStores } from "../store/review-store";

export default declare([WidgetBase, _ContentContextMixin], {
    postCreate: function() {
        //TODO: async
        this._reviewService = new ReviewService();
        this.own(this._reviewService);
        this.stores = createStores(this._reviewService, res);
        this.stores.reviewStore.load();
        this.stores.reviewStore.currentUser = ApplicationSettings.userName;
        this.stores.reviewStore.currentLocale = this.language;

        ReactDOM.render(
            <Provider {...this.stores}>
                <IframeWithPins iframe={this.iframe} />
            </Provider>,
            this.domNode
        );
    },
    contextChanged: function() {
        this.stores.reviewStore.load();
    },
    destroy: function() {
        ReactDOM.unmountComponentAtNode(this.domNode);
    }
});
