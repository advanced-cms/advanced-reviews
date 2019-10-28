import React from "react";

import { Corner, MenuSurface } from "@episerver/ui-framework";
import MaterialIcon from "@material/react-material-icon";

import "./drop-down-menu.scss";

interface DropDownMenuProps {
    title?: string;
    icon: string;
}

export class DropDownMenu extends React.Component<DropDownMenuProps, any> {
    constructor(props: DropDownMenuProps) {
        super(props);
        this.state = {
            isMenuOpen: false,
            anchorElement: null
        };
    }

    openMenu = () => {
        this.setState({ isMenuOpen: true });
    };

    closeMenu = () => {
        this.setState({ isMenuOpen: false });
    };

    setAnchorElement = element => {
        if (this.state.anchorElement) {
            return;
        }
        this.setState({ anchorElement: element });
    };

    render() {
        return (
            <div className="mdc-menu-surface--anchor" ref={this.setAnchorElement}>
                <MaterialIcon
                    className="menu-button"
                    icon={this.props.icon}
                    onClick={this.openMenu}
                    title={this.props.title}
                />
                <MenuSurface
                    className="epi-context-menu"
                    open={this.state.isMenuOpen}
                    anchorCorner={Corner.BOTTOM_LEFT}
                    onClose={this.closeMenu}
                    anchorElement={this.state.anchorElement}
                >
                    {this.props.children}
                </MenuSurface>
            </div>
        );
    }
}
