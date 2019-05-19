import React, { CSSProperties, FunctionComponent, useEffect, useRef } from "react";
import { PinLocation, Priority } from "../store/review-store";
import classNames from "classnames";
import MaterialIcon from "@material/react-material-icon";
import priorityIconMappings from "../store/priority-icon-mappings";
import "./pin.scss";

interface ReviewLocationComponentProps {
    location: PinLocation;
    highlighted?: boolean;
    showDialog(e: any): void;
}

const Pin: FunctionComponent<ReviewLocationComponentProps> = (props: ReviewLocationComponentProps) => {
    const { highlighted, location, showDialog } = props;

    const div = useRef(null);

    useEffect(() => {
        if (highlighted) {
            div.current.scrollIntoView({
                block: "center",
                inline: "center"
            });
        }
    });

    const circleSize = 12;

    const style: CSSProperties = {
        zIndex: 700,
        top: location.positionY - circleSize + "px",
        left: location.positionX - circleSize + "px"
    };

    return (
        <div
            style={style}
            ref={div}
            onClick={showDialog}
            className={classNames("review-location", {
                done: location.isDone,
                new: location.isUpdatedReview,
                highlighted: highlighted
            })}
        >
            <svg height="28" width="28">
                <title>{location.formattedFirstComment}</title>
                <circle cx="14" cy="14" r={circleSize} strokeWidth="2" fill="#c0c0c0" />
                <text x="50%" y="50%" dominantBaseline="middle" textAnchor="middle" fontWeight="bold" fill="black" />
            </svg>
            {location.priority !== Priority.Normal && (
                <MaterialIcon
                    className="priority-icon"
                    icon={priorityIconMappings[location.priority]}
                    title={location.priority}
                />
            )}
        </div>
    );
};

export default Pin;
