<%@ Page Language="c#" Codebehind="ReviewLocationsPreview.aspx.cs" AutoEventWireup="False" Inherits="AdvancedApprovalReviews.ReviewLocationsPreview" Title="aaa" %>

<asp:Content ContentPlaceHolderID="MainRegion" runat="server">

    <style type="text/css">
        .review-list {
            display: table;
            width: 100%;
        }

        .reviews-list .list,
        .reviews-list .details {
            display: table-cell;
            vertical-align: top;
        }

        .reviews-list .list {
            width: 150px;
            min-width: 150px;
        }

        .reviews-list .main-link {
             font-weight: bold;
            width: 100%;
            display: inline-block;
            padding: 2px;
            border-bottom: 1px solid #c0c0c0;
        }

        .reviews-list .list > li { margin-bottom: 15px; }

        .reviews-list .delete {
            display: none;
            float: right;
        }

        .reviews-list .list .row:hover .delete {
            display: inline-block;
        }

        .reviews-list .details {
            padding-left: 10px;
        }

        .reviews-list .details > div {
            font-family: Courier;
            word-break: break-word;
        }
    </style>

    <script type="text/javascript">
        var allReviewLocations = <%= this.AllReviewLocations %>;

        function onDeleteClick(contentLink) {
            if (confirm("Delete " + contentLink + "?")) {
                <%= ClientScript.GetPostBackEventReference(this, string.Empty) %>;
                __doPostBack("delete_review", contentLink);
            }
        }
    </script>

    <div id="admin-plugin-container"></div>
    <script type="text/javascript" src="adminPlugin.js"></script>
</asp:Content>
