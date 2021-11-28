import React from "react";
import ReactDOM from "react-dom";
import declare from "dojo/_base/declare";
import WidgetBase from "dijit/_WidgetBase";
import _ContentContextMixin from "epi-cms/_ContentContextMixin";
import ExternalReviewService from "advanced-cms-external-review/external-review-service";
import { ExternalReviewStore } from "./external-review-links-store";
import ManageLinks from "./external-review-manage-links";
import res from "epi/i18n!epi/cms/nls/externalreviews";

/**
 * Edit Mode component used to list external links
 */
export default declare([WidgetBase, _ContentContextMixin], {
    postCreate: function() {
        if (!this.params.isEnabled) {
            return;
        }

        this._reviewService = new ExternalReviewService();

        this.own(this._reviewService);

        this.store = new ExternalReviewStore(this._reviewService);
        this.own(this.store);
        this.store.load();
        this.store.initialMailSubject = this.params.initialMailSubject;
        this.store.initialViewMailMessage = this.params.initialViewMailMessage;
        this.store.initialEditMailMessage = this.params.initialEditMailMessage;

        ReactDOM.render(
            <ManageLinks
                store={this.store}
                editableLinksEnabled={this.params.editableLinksEnabled}
                pinCodeSecurityEnabled={this.params.pinCodeSecurityEnabled}
                prolongDays={this.params.prolongDays}
                pinCodeSecurityRequired={this.params.pinCodeSecurityRequired}
                pinCodeLength={this.params.pinCodeLength}
                resources={res}
                availableVisitorGroups={this.params.availableVisitorGroups}
            />,
            this.domNode
        );
    },
    contextChanged: function() {
        if (!this.params.isPublicPreviewEnabled) {
            return;
        }

        if (
            !this._currentContext ||
            (this._currentContext.type !== "epi.cms.project" && this._currentContext.type !== "epi.cms.contentdata")
        ) {
            return;
        }

        this.store.load();
    },
    destroy: function() {
        if (!this.params.isPublicPreviewEnabled) {
            return;
        }

        ReactDOM.unmountComponentAtNode(this.domNode);
    }
});
