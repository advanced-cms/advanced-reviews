define([
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/when",

    "epi/shell/command/_Command",
    "epi/dependency",
    "epi-cms/_ContentContextMixin"
],
    function (
        declare,
        lang,
        when,

        _Command,
        dependency,
        _ContentContextMixin
    ) {
        return declare([_Command, _ContentContextMixin], {
            name: "ContentReferences",
            label: "Favourite content",
            tooltip: "Favourite content",
            iconClass: 'epi-icon--medium epi-iconStar',
            canExecute: true,

            _execute: function () {
                alert(1);
            },

            contentContextChanged: function (content) {
                
            }
        });
    });
