define([
    "dojo/_base/declare",
    "dojo/topic",
    "epi/shell/command/_Command",
    "epi-cms/_ContentContextMixin"
], function (
    declare,
    topic,
    _Command,
    _ContentContextMixin
) {
    return declare([_Command, _ContentContextMixin], {
        name: "ContentReferences",
        label: "Favourite content",
        tooltip: "Favourite content",
        iconClass: 'epi-icon--medium epi-iconStar',
        canExecute: true,

        _execute: function () {
            topic.publish("toggle:reviews");
        },

        contentContextChanged: function (content) {
            //TODO: load comments for changed content?
        }
    });
});
