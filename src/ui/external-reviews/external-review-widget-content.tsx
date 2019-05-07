import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import { format } from "date-fns";
import { ContextMenu } from "../common/context-menu";
import Confirmation from "./confirmation";
import IconButton from "@material/react-icon-button";
import MaterialIcon from "@material/react-material-icon";
import List, { ListItemGraphic, ListItem, ListItemText } from "@material/react-list";
import { IExternalReviewStore, ReviewLink } from "./external-review-store";
import "@material/react-list/index.scss";
import "./external-review-widget-content.scss";

interface ExternalReviewWidgetContentProps {
    store: IExternalReviewStore;
}

const ExternalReviewWidgetContent = observer(({ store }: ExternalReviewWidgetContentProps) => {
    const [currentLinkToDelete, setLinkToDelete] = useState<ReviewLink>(null);

    const onDelete = (action: boolean) => {
        setLinkToDelete(null);
        if (!action) {
            return;
        }
        store.delete(currentLinkToDelete);
    };

    const options = [
        {
            name: "View",
            icon: "pageview",
            onSelected: () => {
                store.addLink(false);
            }
        },
        {
            name: "Edit",
            icon: "rate_review",
            onSelected: () => {
                store.addLink(true);
            }
        }
    ];

    return (
        <>
            {store.links.length === 0 && (
                <div className="empty-list">
                    <span>There are no external links for this content</span>
                </div>
            )}

            {store.links.length > 0 && (
                <List twoLine className="external-reviews-list">
                    {store.links.map((item: ReviewLink) => {
                        const link = item.isActive ? (
                            <a href={item.linkUrl} target="_blank">
                                {item.token}
                            </a>
                        ) : (
                            <span className="item-inactive">{item.token}</span>
                        );

                        const icon = <MaterialIcon icon={item.isEditable ? "rate_review" : "pageview"} />;

                        return (
                            <ListItem key={item.token} className="list-item">
                                <ListItemGraphic graphic={icon} />
                                <ListItemText
                                    primaryText={link}
                                    secondaryText={"Valid to: " + format(item.validTo, "MMM Do YYYY HH:mm")}
                                />
                                <IconButton
                                    className="item-action"
                                    disabled={!item.isActive}
                                    title="share"
                                    onClick={() => {}}
                                >
                                    <MaterialIcon icon="share" />
                                </IconButton>
                                <IconButton
                                    className="item-action"
                                    title="delete"
                                    onClick={() => setLinkToDelete(item)}
                                >
                                    <MaterialIcon icon="delete_outline" />
                                </IconButton>
                            </ListItem>
                        );
                    })}
                </List>
            )}
            <div>
                <ContextMenu icon="playlist_add" title="" menuItems={options} />
            </div>
            <Confirmation
                title="Remove link"
                description="Are you sure you want to remove link?"
                open={!!currentLinkToDelete}
                onCloseDialog={onDelete}
            />
        </>
    );
});

export default ExternalReviewWidgetContent;
