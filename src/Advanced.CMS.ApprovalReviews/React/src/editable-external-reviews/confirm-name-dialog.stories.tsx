import { action } from "@storybook/addon-actions";
import { storiesOf } from "@storybook/react";
import React from "react";

import ConfirmDialog from "./confirm-name-dialog";

storiesOf("External editable reviews", module).add("Confirm user name dialog", () => {
    return <ConfirmDialog onClose={action("onclose")} open={true} initialUserName="reviewer1" />;
});
