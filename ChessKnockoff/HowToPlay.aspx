<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="HowToPlay.aspx.cs" Inherits="ChessKnockoff.WebForm6" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <script type="text/javascript">
        $(document).ready(
            $("body").addClass("doge")
        );
    </script>
    <div class="overlayContainer">
        <div class="row">
            <div class="overlayText mx-auto text-center">
                <h1 class="display-2">Such Chess.</h1>
                <h1>Many pawns...</h1>
            </div>
        </div>
    </div>
</asp:Content>
