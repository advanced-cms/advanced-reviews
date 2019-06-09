import React, { useEffect, useState } from "react";
import { inject } from "mobx-react";
import TextField, { Input } from "@material/react-text-field";
import IconButton from "@material/react-icon-button";
import MaterialIcon from "@material/react-material-icon";
import { DropDownMenu } from "../common/drop-down-menu";

import "./location-comment.scss";

interface LocationCommentProps {
    currentScreenshot: string;
    value: string;
    resources?: ReviewResources;
    onToggle: () => void;
    onChange: (comment: string, screenshot: string) => void;
}

const LocationComment = inject("resources")((props: LocationCommentProps) => {
    const [commentInput, setCommentInput] = useState(null);

    useEffect(() => {
        if (commentInput) {
            commentInput.inputElement.focus();
        }
    });

    const resources = props.resources!;

    return (
        <>
            <TextField className="location-comment-field" label={`${resources.dialog.addcomment}...`} dense textarea>
                <Input
                    ref={(input: any) => setCommentInput(input)}
                    value={props.value}
                    onChange={e => props.onChange(e.currentTarget.value, props.currentScreenshot)}
                />
            </TextField>
            {!props.currentScreenshot && (
                <IconButton
                    className="attach-screenshot"
                    title={resources.panel.attachscreenshot}
                    onClick={() => props.onToggle()}
                >
                    <MaterialIcon icon="image" />
                </IconButton>
            )}
            {props.currentScreenshot && (
                <div className="attach-screenshot">
                    <DropDownMenu icon="image" title={resources.panel.showscreenshot}>
                        <img src={props.currentScreenshot} />
                    </DropDownMenu>
                    <IconButton
                        onClick={() => props.onChange(props.value, null)}
                        title={resources.panel.removescreenshot}
                    >
                        <MaterialIcon icon="remove" />
                    </IconButton>
                </div>
            )}
        </>
    );
});

export default LocationComment;
