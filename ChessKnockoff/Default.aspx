<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ChessKnockoff.WebForm6" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <script type="text/javascript">
        //Once the DOM has fully loaded
        $(document).ready(
            //Add the doge class to the body which will add the background
            $("body").addClass("doge")
        );
    </script>
    <div class="overlayContainer">
        <div class="row">
            <div class="overlayText mx-auto text-center">
                <h1 class="display-2">Such pawns.</h1>
                <h1>Many surprise...</h1>
            </div>
        </div>
    </div>
</asp:Content>
