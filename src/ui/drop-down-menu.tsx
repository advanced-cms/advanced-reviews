import React, { HTMLProps, ReactElement } from "react";

import MenuSurface, {Corner} from '@material/react-menu-surface';
import IconButton from '@material/react-icon-button';
import MaterialIcon from '@material/react-material-icon';

interface DropDownMenuProps {
    icon: string;
    onMenuClick?: () => void;
}

export class DropDownMenu extends React.Component<DropDownMenuProps, any> {
    constructor(props: DropDownMenuProps) {
        super(props);
        this.state = {
            isMenuOpen: false,
            anchorElement: null
        };
    }

    private _openMenu = () => {
        if (this.props.onMenuClick) {
            this.props.onMenuClick();
        } else {
            this.openMenu();
        }
    };

    openMenu = () => {
        this.setState({ isMenuOpen: true });
    };

    closeMenu = () => {
        this.setState({ isMenuOpen: false });
    };

    setAnchorElement = (element) => {
        if (this.state.anchorElement) {
            return;
        }
        this.setState({ anchorElement: element });
    };

    render() {
        return (
            <div
                className="mdc-menu-surface--anchor"
                ref={this.setAnchorElement}
            >
                <IconButton onClick={this._openMenu}>
                    <MaterialIcon icon={this.props.icon} />
                </IconButton>
                <MenuSurface className="epi-context-menu"
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
