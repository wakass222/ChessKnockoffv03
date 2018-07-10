<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <form id="form1" runat="server">
            <asp:ScriptManager ID="scriptManagerRegister" runat="server"></asp:ScriptManager>
            <div class="text-center">
                <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
                <h2 class="signinheading mb-2">Register</h2>
            </div>
            <div class="form-group">
                <label for="username">Username</label>
                <asp:TextBox id="inpUsernameRegister" required="" class="form-control" placeholder="Username" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Disabled"></asp:TextBox>
                <div id="fedUsername" class="invalid-feedback" runat="server"></div>
            </div>
            <div class="form-group">
                <label for="username">Email</label>
                <asp:TextBox id="inpEmailRegister" required="" class="form-control" placeholder="Email" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Disabled"></asp:TextBox>
                <div class="invalid-feedback">Email is not valid.</div>
            </div>
            <div class="form-group">
                <label for="inpPasswordRegister">Password</label>
                <asp:TextBox id="inpPasswordRegister" required="" type="password" class="form-control" placeholder="Password" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Disabled"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:TextBox id="inpRePasswordRegister" required="" type="password" class="form-control" placeholder="Re-enter password" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Disabled"></asp:TextBox>
                <div class="invalid-feedback">Passwords do not match.</div>
            </div>
                <div id="fedPasswordHelpBlock" class="alert alert-danger" runat="server">
            </div>
            <div class="form-group">
                <button id="btnSubmitRegister" class="btn btn-lg btn-primary btn-block" type="submit" onserverclick="RegisterNewUser" runat="server">Login</button>
            </div>
            <div class="form-group text-center">
                Forgot your password? <a href="Reset">Reset here</a>
            </div>
        </form>
    </div>
</asp:Content>