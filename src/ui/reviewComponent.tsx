import React from "react";
import ReactDOM from "react-dom";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import { stores } from "./reviewStore";
import {Provider} from 'mobx-react';

//TODO: async
stores.reviewStore.load();

ReactDOM.render(<Provider {...stores}>
    <ReviewLocationsCollection />
</Provider>, document.getElementById("index"));


//TODO: remove
//https://medium.com/teachable/getting-started-with-react-typescript-mobx-and-webpack-4-8c680517c030
//https://www.nealbuerger.com/2018/11/11/getting-started-with-mobx-5-and-typescript-3-react-16-6/
