export default function(node: HTMLElement, external: boolean) {
    const rect = node.getBoundingClientRect();

    // In external edit scenario we need to add scrolling positions as the bounding rect is viewport-specific
    // However, in edit mode the iframe is inside .epi-editorViewport and the bounding rect does include the scroll position so there
    // is no need to add anything
    let domOffset: { x: number; y: number } = { x: 0, y: 0 };
    if (external) {
        domOffset = { x: node.ownerDocument.defaultView.pageXOffset, y: node.ownerDocument.defaultView.pageYOffset };
    }

    return {
        top: rect.top + domOffset.y,
        left: rect.left + domOffset.x
    };
}
