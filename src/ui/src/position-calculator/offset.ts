export default function(node: HTMLElement, doc: HTMLDocument) {
    const rect = node.getBoundingClientRect();

    return {
        top: rect.top + doc.body.scrollTop,
        left: rect.left + doc.body.scrollLeft
    };
}
