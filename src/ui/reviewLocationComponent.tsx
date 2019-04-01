import React, { FunctionComponent } from 'react';
import { ReviewLocation, Priority } from './reviewStore';
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
        className="reviewLocation">
        <svg height="28" width="28" className="point">
            <title>{props.location.formattedFirstComment}</title>
            <circle cx="14" cy="14" r="12" stroke="black" strokeWidth="2" fill="#c0c0c0" />
            <text x="50%" y="50%" dominantBaseline="middle" textAnchor="middle" fontWeight="bold" fill="black">{props.location.id}</text>
        </svg>
        {props.location.isUpdatedReview &&
            <svg height="14" width="14" className="updated-flag">
                <circle cx="7" cy="7" r="7" fill="#F7542B" />
            </svg>
        }
        {props.location.priority !== Priority.Normal &&
            <MaterialIcon className="priority-icon" icon={priorityIconMappings[props.location.priority]} title={props.location.priority} />
        }
    </div>;

export default ReviewLocationComponent;
