<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ChessKnockoff.WebForm4" EnableSessionState="True" EnableViewState="False" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="mb-2">Login</h2>
            <div id="altRegistered" class="alert alert-success" runat="server">
                Account successfully created. Please also check your email inbox to verify your email.
            </div>
            <div id="altVerify" class="alert alert-danger" runat="server">
                Please check your email inbox and verify your email.
            </div>
            <div id="altMustBeLoggedIn" class="alert alert-info" runat="server">
                You must login first.
            </div>
            <div id="altAuthentication" class="alert alert-danger" runat="server">
                Username or password is incorrect.
            </div>
            <div id="altLockout" class="alert alert-danger" runat="server">
                Your account has been lockout. Try again in 20 minutes or reset your password to end the lockout.
            </div>
            <div id="altEmailConfirm" class="alert alert-success" runat="server">
                Your email has been confirmed.
            </div>
            <div id="altResetPassword" class="alert alert-success" runat="server">
                Your password has been reset.
            </div>
        </div>
        <div class="form-group">
            <label for="username">Username</label>
            <input type="text" id="inpUsername" class="form-control" placeholder="Username" required autofocus="" runat="server" autocomplete="on"/>
        </div>
        <div class="form-group">
            <label for="password">Password</label>
            <input type="password" id="inpPassword" class="form-control" placeholder="Password" required runat="server" autocomplete="on"/>
            <div class="ml-auto">
                <small id="emailHelp" class="form-text text-muted">Forgot your password? <a href="Forgot">Reset here</a></small>
            </div>
        </div>
        <div class="form-group">
            <div class="form-check">
                <input class="form-check-input" type="checkbox" value="" id="boxRememberCheck" runat="server"/>
                <label class="form-check-label" for="rememberMeCheck">Remember me</label>
            </div>
        </div>
        <div class="form-group">
            <button id="btnSubmitLogin" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" onserverclick="LoginClick">Login</button>
        </div>
        <div class="form-group text-center">
            Don't have an account? <a href="Register.aspx">Register here</a>
        </div>
    </div>
</asp:Content>
