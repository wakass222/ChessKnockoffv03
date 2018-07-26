<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNavNoLogo.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChessKnockoff.DefaultForm" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNavNoLogo" runat="server">
    <script type="text/javascript">
        //Once the DOM has fully loaded
        $(document).ready(
            //Add the doge class to the body which will add the background
            $("body").addClass("doge")
        );
    </script>
            <div class="overlayText mx-auto text-center">
                <h1 class="display-2">Such pawns.</h1>
                <h1>Many surprise...</h1>
            </div>
</asp:Content>
