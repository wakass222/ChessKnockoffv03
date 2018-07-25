<%@ Page Title="Login" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ChessKnockoff.LoginForm" EnableSessionState="True" EnableViewState="False" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNavTitle" runat="server">
    Login
</asp:Content>

<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <div class="text-center">
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
            <input type="password" id="inpPasswordLogin" class="form-control" placeholder="Password" required runat="server" autocomplete="on"/>
            <div class="ml-auto">
                <small id="emailHelp" class="form-text text-muted">Forgot your password? <a href="Forgot">Reset here</a></small>
            </div>
        </div>
        <div class="form-group">
            <button id="btnSubmitLogin" class="btn btn-primary btn-block" type="submit" runat="server" onserverclick="LoginClick">Login</button>
        </div>
        <div class="form-group text-center">
            Don't have an account? <a href="Register.aspx">Register here</a>
        </div>
    </div>
</asp:Content>
