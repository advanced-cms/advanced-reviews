import "./external-review-manage-links.scss";
import "@episerver/ui-framework/dist/main.css";

import { IconButton, List, ListItem, ListItemGraphic, ListItemText } from "@episerver/ui-framework";
import MaterialIcon from "@material/react-material-icon";
import { format } from "date-fns";
import { observer } from "mobx-react-lite";
import React, { useState } from "react";

import { ContextMenu } from "../common/context-menu";
import Confirmation from "../confirmation/confirmation";
import { IExternalReviewStore, ReviewLink } from "./external-review-links-store";
import LinkEditDialog from "./external-review-manage-links-edit";
import ShareDialog, { LinkShareResult } from "./external-review-share-dialog";

export interface VisitorGroup {
    id: string;
    name: string;
}

export interface ExternalReviewWidgetContentProps {
    store: IExternalReviewStore;
    resources: ExternalReviewResources;
    availableVisitorGroups: VisitorGroup[];
    editableLinksEnabled: boolean;
    pinCodeSecurityEnabled: boolean;
    pinCodeSecurityRequired?: boolean;
    pinCodeLength: number;
    prolongDays: number;
}

/**
 * Component used to render list of external review links
 */
const ExternalReviewWidgetContent = observer(
    ({
        store,
        resources,
        availableVisitorGroups,
        editableLinksEnabled,
        pinCodeSecurityEnabled,
        pinCodeSecurityRequired,
        pinCodeLength,
        prolongDays,
    }: ExternalReviewWidgetContentProps) => {
        const [currentLinkToDelete, setLinkToDelete] = useState<ReviewLink>(null);
        const [currentLinkToShare, setLinkToShare] = useState<ReviewLink>(null);
        const [currentLinkToEdit, setLinkToEdit] = useState<ReviewLink>(null);

        const isPinRequired = pinCodeSecurityEnabled && pinCodeSecurityRequired;

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

        const onEditClose = async (validTo: Date, pinCode: string, displayName: string, visitorGroups: string[]) => {
            setLinkToEdit(null);
            if (!validTo) {
                return;
            }
            if (currentLinkToEdit.isPersisted) {
                store.edit(currentLinkToEdit, validTo, pinCode, displayName, visitorGroups);
                return;
            }

            if (isPinRequired && !pinCode) {
                return;
            }

            const reviewLink = await store.addLink(currentLinkToEdit.isEditable);
            store.edit(reviewLink, null, pinCode, displayName, visitorGroups);
        };

        const addNewLink = (isEditable) => {
            if (isEditable || !isPinRequired) {
                store.addLink(isEditable);
                return;
            }

            const temporaryLink = new ReviewLink(null, null, null, null, isEditable);
            setLinkToEdit(temporaryLink);
        };

        const options = [
            {
                name: resources.list.viewlink,
                icon: "pageview",
                onSelected: () => addNewLink(false),
            },
            {
                name: resources.list.editlink,
                icon: "rate_review",
                onSelected: () => addNewLink(true),
            },
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
                                    {item.displayName || item.token}
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
                                            format(item.validTo, "MMM do yyyy HH:mm")
                                        }
                                    />
                                    <div className="info-icons">
                                        {item.pinCode && pinCodeSecurityEnabled && (
                                            <MaterialIcon
                                                icon="lock"
                                                className="link-secured"
                                                title={resources.list.editdialog.linksecured}
                                            />
                                        )}
                                        {item.visitorGroups && item.visitorGroups.length > 0 && (
                                            <MaterialIcon
                                                icon="groups"
                                                className="link-secured"
                                                title="Visitor groups applied"
                                            />
                                        )}
                                        {item.projectId > 0 && (
                                            <span
                                                className="dijitReset dijitInline dijitIcon epi-iconProject"
                                                title={resources.list.projectname + ": " + item.projectName}
                                            ></span>
                                        )}
                                    </div>
                                    <IconButton
                                        className="item-action"
                                        title={resources.list.editlink}
                                        onClick={() => setLinkToEdit(item)}
                                    >
                                        <MaterialIcon icon="edit" />
                                    </IconButton>
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
                        <IconButton title="Add link" onClick={() => addNewLink(false)}>
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
                {!!currentLinkToEdit && (
                    <LinkEditDialog
                        reviewLink={currentLinkToEdit}
                        onClose={onEditClose}
                        resources={resources}
                        availableVisitorGroups={availableVisitorGroups}
                        open={!!currentLinkToEdit}
                        pinCodeSecurityEnabled={pinCodeSecurityEnabled}
                        pinCodeSecurityRequired={pinCodeSecurityRequired}
                        pinCodeLength={pinCodeLength}
                        prolongDays={prolongDays}
                    />
                )}
            </>
        );
    },
);

export default ExternalReviewWidgetContent;
