import React from "react";
import { storiesOf } from "@storybook/react";
import DrawablePreview from "../screenshots/drawable-preview";
import screenshots from "../screenshots/screenshots.json";

storiesOf("Drawable preview", module)
    .add("with image background", () => {
        return (
            <>
                <DrawablePreview
                    src={screenshots.idylla800x450}
                    width={800}
                    height={450}
                    onCancel={() => (document.getElementById("resultImage").src = "#")}
                    onApplyDrawing={result => (document.getElementById("resultImage").src = result)}
                />
                <img alt="" id="resultImage" />
            </>
        );
    })
    .add("without image background", () => {
        return (
            <>
                <DrawablePreview
                    width={800}
                    height={450}
                    onCancel={() => (document.getElementById("resultImage2").src = "#")}
                    onApplyDrawing={result => (document.getElementById("resultImage2").src = result)}
                />
                <img alt="" id="resultImage2" />
            </>
        );
    });
