import React from "react";
import { storiesOf } from "@storybook/react";
import ScreenshotDialog from "../screenshots/screenshot-dialog";
import { action } from '@storybook/addon-actions';

storiesOf("Screenshot picker", module).add("default", () => {
    return (
        <>
            <iframe
                id="iframe"
                style={{ width: "100%", height: "985px" }}
                src="./stories/fake_OPE.html"
            />
            <ScreenshotDialog
                maxWidth={500}
                maxHeight={300}
                iframe="iframe"
                onImageSelected={action("image selected")}
                toggle={() => {}}
            />
        </>
    );
});
