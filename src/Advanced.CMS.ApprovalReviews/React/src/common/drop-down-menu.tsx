import React, { useState, useRef } from "react";

import { Corner, MenuSurface } from "@episerver/ui-framework";
import MaterialIcon from "@material/react-material-icon";

import "./drop-down-menu.scss";

interface DropDownMenuProps {
    title?: string;
    icon: string;
    open: boolean;
    children: React.ReactNode;
}

export function DropDownMenu(props: DropDownMenuProps) {
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const anchorElementRef = useRef();

    const openMenu = () => {
        setIsMenuOpen(true);
    };

    const closeMenu = () => {
        setIsMenuOpen(false);
    };

    return (
        <div className="mdc-menu-surface--anchor" ref={anchorElementRef}>
            <MaterialIcon className="menu-button" icon={props.icon} onClick={openMenu} title={props.title} />
            <MenuSurface
                className="epi-context-menu"
                open={isMenuOpen}
                anchorCorner={Corner.BOTTOM_LEFT}
                onClose={closeMenu}
                anchorElement={anchorElementRef.current}
            >
                {props.children}
            </MenuSurface>
        </div>
    );
}
