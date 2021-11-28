import React, {useState, useEffect} from "react";
import ReactDOM from "react-dom";
import axios from "axios";

interface ReviewLocation {
    contentLink: string;
    serializedReview: string;
}

interface ReviewGroup {
    id: string;
    contentLinks: ReviewLocation[];
}

interface AdminPluginProps {
    controllerUrl: string;
}

function AdminPluginComponent(props: AdminPluginProps) {
    const [currentContentLink, setCurrentContentLink] = useState<string>(null);
    const [currentJSON, setCurrentJSON] = useState<string>(null);
    const [currentException, setCurrentException] = useState<string>(null);
    const [data, setData] = useState<ReviewGroup[]>([]);

    const refreshList = () => {
        axios.get(`${props.controllerUrl}/GetAll`).then(data => {
            setData(data.data);
        });
    };

    useEffect(() => {
        refreshList();
    }, []);

    const onDeleteClick = (contentLink: string) => {
        if (!confirm("Are you sure to remove this review?")) {
            return;
        }

        axios
            .post(`${props.controllerUrl}/DeleteReviewLocation`, {
                contentLink: contentLink
            })
            .then(refreshList);
    };

    const changeReviewLocation = (reviewLocation: ReviewLocation): string => {
        setCurrentContentLink(reviewLocation.contentLink);
        try {
            var parsed = JSON.parse(reviewLocation.serializedReview);
            parsed.forEach(reviewLocation => {
                try {
                    reviewLocation.data = JSON.parse(reviewLocation.data);
                } catch (ex) {
                }
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
        <div className="root">
            <h2>Saved PIN viewer</h2>
            <h6>Click on version numbers to see all saved user comments</h6>
            <div className="reviews-list">
                <ul className="list">
                    {data.map(x => (
                        <li key={x.id || "[no ContentLink]"}>
                            <span className="main-link">{x.id || "[no ContentLink]"}</span>
                            <ul>
                                {x.contentLinks.map(c => (
                                    <li className="row" key={c.contentLink || "[empty]"}>
                                        <a title={`View saved JSON for ${c.contentLink} ID`} href="#"
                                           onClick={() => changeReviewLocation(c)}>
                                            {c.contentLink || "[empty]"}
                                        </a>
                                        <a className="delete" href="#" onClick={() => onDeleteClick(c.contentLink)}>
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
        </div>
    );
}

const root = document.getElementById("admin-plugin-container");

ReactDOM.render(<AdminPluginComponent controllerUrl={root.dataset.controllerUrl}/>, root);
