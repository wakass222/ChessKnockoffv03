<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="ChessKnockoff.WebForm4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <form class="inputForm mx-auto" runat="server">
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="signinheading mb-2">Reset password</h2>
            <div class="form-group">
            <label for="username">Email</label>
                <input type="text" id="inpEmailReset" class="form-control" name="username" placeholder="Email" required autofocus="" runat="server"/>
            </div>
            <div class="form-group">
                <button id="btnSubmitRLogin" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" onserverclick="EmailClick">Submit</button>
            </div>
            <div id="altEmailSent" class="alert alert-success" role="alert" runat="server">
                Reset link sent to that email.
            </div>
        </div>
    </form>
</asp:Content>
