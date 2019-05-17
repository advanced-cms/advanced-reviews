import React from "react";
import ReactDOM from "react-dom";
import declare from "dojo/_base/declare";
import WidgetBase from "dijit/_WidgetBase";
import _ContentContextMixin from "epi-cms/_ContentContextMixin";
import ExternalReviewService from "alloy-external-review/external-review-service";
import { ExternalReviewStore } from "./external-review-links-store";
import ManageLinks from "./external-review-manage-links";
//import res from "epi/i18n!epi/cms/nls/reviewcomponent";

/**
 * Edit Mode component used to list external links
 */
export default declare([WidgetBase, _ContentContextMixin], {
    postCreate: function() {
        this._reviewService = new ExternalReviewService();

        this.own(this._reviewService);

        this.store = new ExternalReviewStore(this._reviewService);
        this.own(this.store);
        this.store.load();
        this.store.initialViewMailMessage = this.params.initialViewMailMessage;
        this.store.initialEditMailMessage = this.params.initialEditMailMessage;

        ReactDOM.render(<ManageLinks store={this.store} />, this.domNode);
    },
    contextChanged: function() {
        this.store.load();
    },
    destroy: function() {
        ReactDOM.unmountComponentAtNode(this.domNode);
    }
});
