import { action } from "@storybook/addon-actions";
import { storiesOf } from "@storybook/react";
import React from "react";

import res from "../../.storybook/externalResources.json";
import ShareDialog from "./external-review-share-dialog";

storiesOf("External reviews", module).add("Share dialog", () => {
    return (
        <ShareDialog
            onClose={action("onclose")}
            open={true}
            initialSubject="Review request"
            initialMessage="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi non lorem efficitur, luctus tortor non, placerat orci. Nunc nec mollis est, mattis ultrices quam. Etiam non commodo felis."
            resources={res}
        />
    );
});
