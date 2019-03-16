import React, { HTMLProps, ReactElement } from "react";
import { DropDownMenu } from "./drop-down-menu";

import List, {
    ListItem,
    ListItemGraphic,
    ListItemText,
} from "@material/react-list";
import MaterialIcon from '@material/react-material-icon';

interface menuItemProps {
    name: string;
    icon?: string;
    onSelected: () => void;
}

interface ContextMenuProps {
    icon: string;
    menuItems?: menuItemProps[];
}

export class ContextMenu extends React.Component<ContextMenuProps, any> {
    dropDownMenu: DropDownMenu;

    onSelected = (index: number) => {
        this.props.menuItems[index].onSelected();
        this.dropDownMenu.closeMenu();
    };

    render() {
        const list = <List singleSelection handleSelect={this.onSelected}>
            {this.props.menuItems.map(item =>
                <ListItem key={item.name}>
                    {item.icon ? <ListItemGraphic graphic={<MaterialIcon icon={item.icon}/>} /> : <ListItemText primaryText={item.name}/>}
                </ListItem>
            )}
        </List>;

        return (
            <DropDownMenu
                ref={instance => {
                    this.dropDownMenu = instance;
                }}
                icon={this.props.icon}
            >
                {list}
            </DropDownMenu>
        );
    }
}
