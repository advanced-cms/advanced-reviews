import React, { useRef, useState } from "react";
import { DropDownMenu } from "./drop-down-menu";

import { List, ListItem, ListItemGraphic, ListItemText } from "@episerver/ui-framework";
import MaterialIcon from "@material/react-material-icon";

interface MenuItem {
    name: string;
    icon?: string;
    onSelected: () => void;
}

interface ContextMenuProps {
    title: string;
    icon: string;
    menuItems?: MenuItem[];
}

export function ContextMenu(props: ContextMenuProps) {
    const dropDownMenuRef = useRef(null);
    const [title, setTitle] = useState();

    const onSelected = (index: number) => {
        const menuItem = props.menuItems[index];
        menuItem.onSelected();
        setTitle(menuItem.name);
        if (dropDownMenuRef.current) {
            dropDownMenuRef.current.closeMenu();
        }
    };

    const list = (
        <List singleSelection handleSelect={onSelected}>
            {props.menuItems.map((item) => (
                <ListItem key={item.name}>
                    {item.icon ? <ListItemGraphic graphic={<MaterialIcon icon={item.icon} />} /> : null}
                    <ListItemText primaryText={item.name} />
                </ListItem>
            ))}
        </List>
    );

    return (
        <DropDownMenu ref={dropDownMenuRef} icon={props.icon} title={title}>
            {list}
        </DropDownMenu>
    );
}
