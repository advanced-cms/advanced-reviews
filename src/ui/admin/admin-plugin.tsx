import React, { useState } from "react";
import ReactDOM from "react-dom";

interface ReviewLocation {
    contentLink: string;
    serializedReview: string;
}

interface ReviewGroup {
    id: string;
    contentLinks: ReviewLocation[];
}

interface AdminPluginProps {
    data: ReviewGroup[];
    onDeleteClick(contentLink: string): void;
}

function AdminPluginComponent(props: AdminPluginProps) {
    const [currentContentLink, setCurrentContentLink] = useState<string>(null);
    const [currentJSON, setCurrentJSON] = useState<string>(null);
    const [currentException, setCurrentException] = useState<string>(null);

    const changeReviewLocation = (reviewLocation: ReviewLocation): string => {
        setCurrentContentLink(reviewLocation.contentLink);
        try {
            var parsed = JSON.parse(reviewLocation.serializedReview);
            parsed.forEach(reviewLocation => {
                try {
                    reviewLocation.data = JSON.parse(reviewLocation.data);
                } catch (ex) {}
            });
            parsed = JSON.stringify(parsed, null, 2);
            setCurrentJSON(parsed);
            setCurrentException(null);
            return parsed;
        } catch (ex) {
            setCurrentException(ex.message);
            setCurrentJSON(reviewLocation.serializedReview);
        }
    };

    return (
        <div className="reviews-list">
            <ul className="list">
                {props.data.map(x => (
                    <li key={x.id || "[no ContentLink]"}>
                        <span className="main-link">{x.id || "[no ContentLink]"}</span>
                        <ul>
                            {x.contentLinks.map(c => (
                                <li className="row" key={c.contentLink || "[empty]"}>
                                    <a href="#" onClick={() => changeReviewLocation(c)}>
                                        {c.contentLink || "[empty]"}
                                    </a>
                                    <a className="delete" href="#" onClick={() => props.onDeleteClick(c.contentLink)}>
                                        Delete
                                    </a>
                                </li>
                            ))}
                        </ul>
                    </li>
                ))}
            </ul>
            {currentJSON && (
                <div className="details">
                    <h3>{currentContentLink}</h3>
                    {currentException && <div>{currentException}</div>}
                    <div>
                        <pre>{currentJSON}</pre>
                    </div>
                </div>
            )}
        </div>
    );
}

ReactDOM.render(
    <AdminPluginComponent data={window["allReviewLocations"]} onDeleteClick={window["onDeleteClick"]} />,
    document.getElementById("admin-plugin-container")
);
