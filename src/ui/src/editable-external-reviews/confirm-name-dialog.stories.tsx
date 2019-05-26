import React from "react";
import { storiesOf } from "@storybook/react";
import ConfirmDialog from "./confirm-name-dialog";
import { action } from "@storybook/addon-actions";

storiesOf("External editable reviews", module).add("Confirm user name dialog", () => {
    return <ConfirmDialog onClose={action("onclose")} open={true} initialUserName="reviewer1" />;
});
