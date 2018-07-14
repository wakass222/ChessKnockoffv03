<%@ Page Title="Account" Language="C#" MasterPageFile="~/BaseWithHeaderNavLogin.master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="ChessKnockoff.WebForm3" %>
<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLogin" runat="server">
    <div class="inputForm mx-auto">
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="mb-2">Change your password</h2>
        </div>
        <div id="altSuccess" class="alert alert-success" role="alert" runat="server">
            Your password was succesfully changed.
        </div>
        <div class="form-group">
            <label for="inpCurrentPassword">Current password</label>
            <input id="inpCurrentPassword" required="" type="password" class="form-control" placeholder="Password" runat="server"/>
        </div>
        <div class="form-group">
            <label for="inpPasswordReset">New password</label>
            <asp:CustomValidator ID="valPassword" runat="server" ControlToValidate="inpPassword" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpReset" ValidateEmptyText="True" OnServerValidate="checkPassword"></asp:CustomValidator>
            <input id="inpPassword" required="" type="password" class="form-control" placeholder="Password" runat="server"/>
        </div>
        <div class="form-group">
            <asp:CustomValidator ID="valRePassword" runat="server" ControlToValidate="inpRePassword" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpReset" ValidateEmptyText="True" OnServerValidate="checkPassword"></asp:CustomValidator>
            <input id="inpRePassword" required="" type="password" class="form-control" placeholder="Re-enter password" runat="server"/>
            <div class="invalid-feedback">Passwords do not match.</div>
        </div>
        <div class="form-group">
            <asp:Button id="btnSubmitAccount" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" Text="Change password" ValidationGroup="grpReset" OnClick="ChangePassword" />
        </div>
        <div id="altError" class="alert alert-danger" role="alert" runat="server">
        </div>
    </div>
</asp:Content>
