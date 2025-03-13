import "./location-comment.scss";

import { IconButton, Input, TextField } from "@episerver/ui-framework";
import MaterialIcon from "@material/react-material-icon";
import { inject } from "mobx-react";
import React, { useEffect, useState } from "react";

import { DropDownMenu } from "../common/drop-down-menu";

interface LocationCommentProps {
    currentScreenshot: string;
    value: string;
    resources?: ReviewResources;
    onToggle: () => void;
    onChange: (comment: string, screenshot: string) => void;
    allowScreenshotAttachments: boolean;
}

const LocationComment = inject("resources")((props: LocationCommentProps) => {
    const [commentInput, setCommentInput] = useState(null);

    useEffect(() => {
        if (commentInput) {
            commentInput.inputElement.focus();
        }
    });

    const resources = props.resources!;

    const textAreaProps = {
        textarea: true,
    };

    return (
        <>
            <TextField
                className="location-comment-field"
                label={`${resources.dialog.addcomment}...`}
                {...textAreaProps}
            >
                <Input
                    ref={(input: any) => setCommentInput(input)}
                    value={props.value}
                    onChange={(e: React.FormEvent<any>) =>
                        props.onChange(e.currentTarget.value, props.currentScreenshot)
                    }
                />
            </TextField>
            {props.allowScreenshotAttachments && (
                <>
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
                                <img src={props.currentScreenshot} alt="screenshot" />
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
            )}
        </>
    );
});

export default LocationComment;
