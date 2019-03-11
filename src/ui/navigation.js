import React from "react";
import ReactDOM from "react-dom";
import {Navigation} from "@episerver/platform-navigation";

import '@episerver/ui-framework/dist/main.css';
import '@episerver/platform-navigation/dist/main.css';

const decodeHtmlEntity = (str) => {
    return str.replace(/&#(\d+);/g, function(match, dec) {
        return String.fromCharCode(dec);
    });
};

const onNavigate = (item) => {
    //TODO: handle onclicks which are used by Report Center in CMS-13473
    if (item.url.startsWith("javascript:")) {
        const a = document.createElement("a");
        a.innerHTML = "a";
        a.setAttribute("onclick", decodeHtmlEntity(item.url));
        a.setAttribute("href", "#");
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    } else {
        window.location.href = item.url;
    }
};

const div = document.createElement("div");
//TODO: fix this in CMS-13474
div.style.height = "50px";
document.body.prepend(div);
ReactDOM.render(<Navigation structure={window.platformNavigation.structure} currentProduct={window.platformNavigation.currentProduct} currentLevelOne={window.platformNavigation.currentLevelOne} onNavigate={onNavigate}></Navigation>, div);