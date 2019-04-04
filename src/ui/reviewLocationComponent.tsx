import React, { FunctionComponent } from 'react';
import { ReviewLocation, Priority } from './reviewStore';
import classNames from "classnames";
import MaterialIcon from '@material/react-material-icon';
import priorityIconMappings from './priorityIconMappings';
import "./reviewLocationComponent.scss";

interface ReviewLocationComponentProps {
    location: ReviewLocation,
    showDialog(e: any): void
}

const ReviewLocationComponent: FunctionComponent<ReviewLocationComponentProps> = (props: ReviewLocationComponentProps) =>
    <div style={props.location.style}
        onClick={props.showDialog}
        className={classNames("reviewLocation", { "done": props.location.isDone, "new": props.location.isUpdatedReview })}>
        <svg height="28" width="28" className="point">
            <title>{props.location.formattedFirstComment}</title>
            <circle cx="14" cy="14" r="12" strokeWidth="2" fill="#c0c0c0" />
        </svg>        
        {props.location.priority !== Priority.Normal &&
            <MaterialIcon className="priority-icon" icon={priorityIconMappings[props.location.priority]} title={props.location.priority} />
        }
    </div>;

export default ReviewLocationComponent;
