import React, { useState, useEffect } from 'react';
import TextField, { Input } from '@material/react-text-field';
import { storiesOf } from '@storybook/react';
import { Provider } from 'mobx-react';
import { stores } from "../reviewStore";
import ReviewLocationsCollection from "../reviewLocationsCollection";

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
          <iframe id="iframe" style={{"width": "800px", "height": "800px" }} src="stories/fake_OPE.html"></iframe>
      </div>
  );
}

storiesOf('Dojo component', module)
  .add('default', () => <Provider {...stores}>
  <Component />
  </Provider>
  );