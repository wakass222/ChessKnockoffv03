<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ChessKnockoff.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <form class="mt-1" runat="server">
            <div class="text-center">
                <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
                <h2 class="signinheading mb-2">Login</h2>
                <div id="altRegistered" class="alert alert-success" runat="server">
                    Account successfully created.
                </div>
                <div id="altMustBeLoggedIn" class="alert alert-info" runat="server">
                    You must login first.
                </div>
                <div id="altAuthentication" class="alert alert-danger" runat="server">
                </div>
            </div>
            <div class="form-group">
                <label for="username">Username</label>
                <input type="text" id="inpUsername" class="form-control" name="username" placeholder="Username" required autofocus="" runat="server"/>
            </div>
            <div class="form-group">
                <label for="password">Password</label>
                <input type="password" id="inpPassword" class="form-control" name="password" placeholder="Password" required runat="server"/>
            </div>
            <div class="form-group">
                <div class="form-check">
                    <input class="form-check-input" type="checkbox" value="" id="boxMeCheck" runat="server"/>
                    <label class="form-check-label" for="rememberMeCheck">Remember me</label>
                </div>
            </div>
            <div class="form-group">

                <button id="btnSubmitRLogin" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" onserverclick="LoginClick">Login</button>
            </div>
            <div class="form-group text-center">
                Don't have an account? <a href="Register.aspx">Register here</a>
            </div>
        </form>
    </div>
</asp:Content>