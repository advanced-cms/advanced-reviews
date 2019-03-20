import React, { useState, useEffect } from "react";
import ReactDOM from "react-dom";
import ReviewLocationsCollection from "./reviewLocationsCollection";
import { stores } from "./reviewStore";
import { Provider } from 'mobx-react';
import TextField, { Input } from '@material/react-text-field';

//TODO: async
stores.reviewStore.load();

function Component() {
    const [text, setText] = useState("Lina");

    useEffect(() => {
        stores.reviewStore.currentUser = text;
      });

    return (
        <div>
            <div>
                <TextField label='Current user' dense><Input value={text}
                    onChange={(e) => setText(e.currentTarget.value)} />
                </TextField>
            </div>
            <ReviewLocationsCollection />
        </div>
    );
}

ReactDOM.render(<Provider {...stores}>
    <Component />
</Provider>, document.getElementById("index"));


//TODO: remove
//https://medium.com/teachable/getting-started-with-react-typescript-mobx-and-webpack-4-8c680517c030
//https://www.nealbuerger.com/2018/11/11/getting-started-with-mobx-5-and-typescript-3-react-16-6/
