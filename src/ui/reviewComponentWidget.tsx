import React from "react";
import ReactDOM from "react-dom";
import { Provider } from 'mobx-react';
import declare from "dojo/_base/declare";
import WidgetBase from "dijit/_WidgetBase";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import res from "epi/i18n!epi/cms/nls/reviewcomponent";

import { stores } from "./reviewStore";

//TODO: async
stores.reviewStore.load();
stores.resources = res;

export default declare([WidgetBase], {
    postCreate: function () {
        ReactDOM.render(<Provider {...stores}><ReviewLocationsCollection /></Provider>, this.domNode);
    },
    destroy: function () {
        ReactDOM.unmountComponentAtNode(this.domNode);
    }
});
