<%@ Page Title="Forgot" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="ChessKnockoff.WebForm7" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNavTitle" runat="server">
    Forgot
</asp:Content>

<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <div class="form-group">
        <label for="email">Email</label>
            <asp:CustomValidator ID="valEmail" runat="server" ControlToValidate="inpEmail" ClientValidationFunction="wrappedEmail" Display="None" ValidationGroup="grpForgot" ValidateEmptyText="True" OnServerValidate="validateEmail"></asp:CustomValidator>
            <input type="email" id="inpEmail" class="form-control" placeholder="Email" required autofocus="" runat="server" />
            <div class="invalid-feedback">Email is not valid.</div>
        </div>
        <div id="altEmailSent" class="alert alert-success" role="alert" runat="server">
            Reset link was sent to that email.
        </div>
        <div id="altEmailFail" class="alert alert-warning" role="alert" runat="server">
            That email does not exist or has not been confirmed.
        </div>
        <div class="form-group">
            <asp:Button id="btnForgotSubmit" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" Text="Submit" ValidationGroup="grpForgot" OnClick="EmailClick" />
        </div>
    </div>
</asp:Content>
