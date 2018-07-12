<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.WebForm1" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <script type="text/javascript">
        //Define the ID's of the controls
        var inpUsername = "#inpUsernameRegister";
        var inpPassword = "#inpPasswordRegister";
        var inpRePassword = "#inpRePasswordRegister";
        var inpEmail = "#inpEmailRegister";
        
        //Create the wrapped functions
        function wrappedUsername(sender, args) {
            wrapperMatch(sender, args, checkUsernameRule, inpUsername);
        }

        function wrappedPassword(sender, args) {
            wrapperMatch(sender, args, checkPasswordMatch, inpPassword, inpRePassword);
        }

        function wrappedEmail(sender, args) {
            wrapperMatch(sender, args, checkEmailRule, inpEmail);
        }

        //Assign the checks on keyup once the DOM has completely loaded
        $(document).ready(function () {
            checkEmailRule(inpEmail);
            checkPasswordMatch(inpPassword, inpRePassword);
            checkUsernameRule(inpUsername);

            $(inpEmail).keyup(function () {
                checkEmailRule(inpEmail)
            });

            $(inpUsername).keyup(function () {
                checkUsernameRule(inpUsername);
            });

            $(inpPassword).add(inpRePassword).keyup(function () {
                checkPasswordMatch(inpPassword, inpRePassword);
            });
        });
    </script>
    <div class="inputForm mx-auto">
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="signinheading mb-2">Register</h2>
        </div>
        <div class="form-group">
            <label for="username">Username</label>
            <asp:CustomValidator ID="valUsernameRegister" runat="server" ControlToValidate="inpUsernameRegister" ClientValidationFunction="wrappedUsername" Display="None" ValidationGroup="grpRegister" ValidateEmptyText="True"></asp:CustomValidator>
            <input id="inpUsernameRegister" required="" class="form-control" placeholder="Username" runat="server" />
            <div id="fedUsername" class="invalid-feedback" runat="server">Username can only contain alphanumeric characters.</div>
        </div>
        <div id="altUsernameTaken" class="alert alert-danger" runat="server">
            Username has been taken.
        </div>
        <div class="form-group">
            <label for="username">Email</label>
            <asp:CustomValidator ID="valEmailRegister" runat="server" ControlToValidate="inpEmailRegister" ClientValidationFunction="wrappedEmail" Display="None" ValidationGroup="grpRegister" ValidateEmptyText="True"></asp:CustomValidator>
            <input id="inpEmailRegister" required="" class="form-control" placeholder="Email" type="email" runat="server" />
            <div class="invalid-feedback">Email is not valid.</div>
        </div>
        <div id="altEmailTaken" class="alert alert-danger" runat="server">
            Email already in use.
        </div>
        <div class="form-group">
            <label for="inpPasswordRegister">Password</label>
            <asp:CustomValidator ID="valPasswordRegister" runat="server" ControlToValidate="inpPasswordRegister" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpRegister"></asp:CustomValidator>
            <input id="inpPasswordRegister" required="" type="password" class="form-control" placeholder="Password" runat="server" />
        </div>
        <div class="form-group">
            <asp:CustomValidator ID="valRePasswordRegister" runat="server" ControlToValidate="inpRePasswordRegister" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpRegister" ValidateEmptyText="True"></asp:CustomValidator>
            <input id="inpRePasswordRegister" required="" type="password" class="form-control" placeholder="Password" runat="server" />
            <div class="invalid-feedback">Passwords do not match.</div>
        </div>
        <div id="altPassword" class="alert alert-danger" runat="server">
        </div>
        <div class="form-group">
            <asp:Button id="btnSubmitRegister" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" enableviewstate="False" validationgroup="grpRegister" Text="Register" OnClick="RegisterClick" />
            <div id="altError" class="invalid-feedback" runat="server"></div>
        </div>
        <div class="form-group text-center">
            Have an account? <a href="Login">Login here</a>
        </div>
    </div>
</asp:Content>
