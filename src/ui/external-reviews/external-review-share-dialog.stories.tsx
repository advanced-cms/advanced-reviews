import React from "react";
import { storiesOf } from "@storybook/react";
import ShareDialog from "./external-review-share-dialog";
import { action } from "@storybook/addon-actions";

storiesOf("External reviews/Share dialog", module).add("default", () => {
    return <ShareDialog onClose={action("onclose")} open={true} />;
});
