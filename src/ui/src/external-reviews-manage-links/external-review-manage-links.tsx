import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import { format } from "date-fns";
import { ContextMenu } from "../common/context-menu";
import Confirmation from "../confirmation/confirmation";
import { IconButton } from "@episerver/ui-framework";
import MaterialIcon from "@material/react-material-icon";
import List, { ListItemGraphic, ListItem, ListItemText } from "@material/react-list";
import { IExternalReviewStore, ReviewLink } from "./external-review-links-store";
import ShareDialog, { LinkShareResult } from "./external-review-share-dialog";

import "@material/react-list/index.scss";
import "./external-review-manage-links.scss";

interface ExternalReviewWidgetContentProps {
    store: IExternalReviewStore;
    resources: ExternalReviewResources;
    editableLinksEnabled: boolean;
}

/**
 * Component used to render list of external review links
 */
const ExternalReviewWidgetContent = observer(
    ({ store, resources, editableLinksEnabled }: ExternalReviewWidgetContentProps) => {
        const [currentLinkToDelete, setLinkToDelete] = useState<ReviewLink>(null);
        const [currentLinkToShare, setLinkToShare] = useState<ReviewLink>(null);

        const onDelete = (action: boolean) => {
            setLinkToDelete(null);
            if (!action) {
                return;
            }
            store.delete(currentLinkToDelete);
        };

        const onShareDialogClose = (shareLink: LinkShareResult) => {
            setLinkToShare(null);
            if (shareLink === null) {
                return;
            }
            store.share(currentLinkToShare, shareLink.email, shareLink.subject, shareLink.message);
        };

        const options = [
            {
                name: resources.list.viewlink,
                icon: "pageview",
                onSelected: () => {
                    store.addLink(false);
                }
            },
            {
                name: resources.list.editlink,
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
                        <span>{resources.list.emptylist}</span>
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
                                    {editableLinksEnabled && <ListItemGraphic graphic={icon} />}
                                    <ListItemText
                                        primaryText={link}
                                        secondaryText={
                                            resources.list.itemvalidto +
                                            ": " +
                                            format(item.validTo, "MMM Do YYYY HH:mm")
                                        }
                                    />
                                    <IconButton
                                        className="item-action"
                                        disabled={!item.isActive}
                                        title={resources.list.sharetitle}
                                        onClick={() => setLinkToShare(item)}
                                    >
                                        <MaterialIcon icon="share" />
                                    </IconButton>
                                    <IconButton
                                        className="item-action"
                                        title={resources.list.deletetitle}
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
                    {editableLinksEnabled ? (
                        <ContextMenu icon="playlist_add" title="" menuItems={options} />
                    ) : (
                        <IconButton title="Add link" onClick={() => store.addLink(false)}>
                            <MaterialIcon icon="playlist_add" />
                        </IconButton>
                    )}
                </div>
                {!!currentLinkToDelete && (
                    <Confirmation
                        title={resources.removedialog.title}
                        description={resources.removedialog.description}
                        okName={resources.removedialog.ok}
                        cancelName={resources.removedialog.cancel}
                        open={!!currentLinkToDelete}
                        onCloseDialog={onDelete}
                    />
                )}

                {!!currentLinkToShare && (
                    <ShareDialog
                        open={!!currentLinkToShare}
                        onClose={onShareDialogClose}
                        initialSubject={store.initialMailSubject}
                        initialMessage={
                            currentLinkToShare && currentLinkToShare.isEditable
                                ? store.initialEditMailMessage
                                : store.initialViewMailMessage
                        }
                        resources={resources}
                    />
                )}
            </>
        );
    }
);

export default ExternalReviewWidgetContent;
