<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Leaderboard.aspx.cs" Inherits="ChessKnockoff.WebForm8" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="text-center">
        <h2 class="mb-2 mt-4">Leaderboard</h2>
    </div>
    <div class="table-responsive container">
        <asp:Table ID="tblLeaderboard" runat="server" CssClass="table table-bordered">
            <asp:TableHeaderRow TableSection="TableHeader">
                <asp:TableHeaderCell ID="tbhRank" runat="server">#</asp:TableHeaderCell>
                <asp:TableHeaderCell ID="tbhUsername" runat="server">Username</asp:TableHeaderCell>
                <asp:TableHeaderCell ID="tbhELO" runat="server">ELO</asp:TableHeaderCell>
            </asp:TableHeaderRow>
        </asp:Table>
    </div>
</asp:Content>
