import React, { useState, useEffect } from "react";
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
}

function AdminPluginComponent(props: AdminPluginProps) {
    const [currentLocation, setCurrentLocation] = useState<ReviewLocation>(null);

    return (
        <div className="reviews-list">
            <ul className="list">
                {props.data.map(x => 
                    <li key={x.id || '[no ContentLink]'}>
                      <span className="main-link">{x.id || '[no ContentLink]'}</span>
                      <ul>
                          {x.contentLinks.map(c=>
                            <li key={c.contentLink || '[empty]'}>
                               <a href="#" onClick={() => setCurrentLocation(c)}>{c.contentLink || '[empty]'}</a>
                            </li>
                            )}
                      </ul>
                    </li>
                    )}                
            </ul>
            {currentLocation && 
            <div className="details">
                <h3>{currentLocation.contentLink}</h3>
                <div>{currentLocation.serializedReview}</div>
            </div>
            }
        </div>
    );
}

ReactDOM.render(
    <AdminPluginComponent data={window["allReviewLocations"]} />, document.getElementById("admin-plugin-container")
);
