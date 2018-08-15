<%@ Page Title="Register" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.RegisterForm" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNavTitle" runat="server">
    Register
</asp:Content>

<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <div class="form-group">
            <label for="username">Username</label>
            <asp:CustomValidator ID="valUsername" runat="server" ControlToValidate="inpUsername" ClientValidationFunction="wrappedUsername" Display="None" ValidationGroup="grpRegister" ValidateEmptyText="True" OnServerValidate="validateUsername"></asp:CustomValidator>
            <input id="inpUsername" required="" class="form-control" placeholder="Username" runat="server" />
            <div id="fedUsername" class="invalid-feedback" runat="server">Username can only contain alphanumeric characters and must be 25 characters or less.</div>
        </div>
        <div id="altUsernameTaken" class="alert alert-danger" runat="server">
            Username has been taken.
        </div>
        <div class="form-group">
            <label for="username">Email</label>
            <asp:CustomValidator ID="valEmail" runat="server" ControlToValidate="inpEmail" ClientValidationFunction="wrappedEmail" Display="None" ValidationGroup="grpRegister" ValidateEmptyText="True" OnServerValidate="validateEmail"></asp:CustomValidator>
            <input id="inpEmail" required="" class="form-control" placeholder="Email" type="email" runat="server" />
            <div class="invalid-feedback">Email is not valid.</div>
        </div>
        <div id="altEmailTaken" class="alert alert-danger" runat="server">
            Email already in use.
        </div>
        <div class="form-group">
            <label for="inpPassword">Password</label>
            <asp:CustomValidator ID="valPassword" runat="server" ControlToValidate="inpPassword" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpRegister" OnServerValidate="checkPassword"></asp:CustomValidator>
            <input id="inpPassword" required="" type="password" class="form-control" placeholder="Password" runat="server" />
            <div class="invalid-feedback" id="passwordFeedback"></div>
        </div>
        <div class="form-group">
            <asp:CustomValidator ID="valRePassword" runat="server" ControlToValidate="inpRePassword" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpRegister" ValidateEmptyText="True" OnServerValidate="checkPassword"></asp:CustomValidator>
            <input id="inpRePassword" required="" type="password" class="form-control" placeholder="Password" runat="server" />
            <div class="invalid-feedback">Passwords do not match.</div>
        </div>
        <asp:CustomValidator ID="valPasswordRule" runat="server" ControlToValidate="inpPassword" ClientValidationFunction="wrappedPasswordRule" Display="None" ValidationGroup="grpRegister" ValidateEmptyText="True" OnServerValidate="checkPassword"></asp:CustomValidator>
        <div class="form-group">
            <asp:Button id="btnSubmitRegister" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" enableviewstate="False" validationgroup="grpRegister" Text="Register" OnClick="RegisterClick" />
            <div id="altError" class="invalid-feedback" runat="server">Your account could not be created at this time. Please try again later.</div>
        </div>
        <div class="form-group text-center">
            Have an account? <a href="Login">Login here</a>
        </div>
    </div>
</asp:Content>
