<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Reset.aspx.cs" Inherits="ChessKnockoff.WebForm2" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <script>
            //Define the ID's of the controls
            var inpPassword = "#inpPasswordReset";
            var inpRePassword = "#inpRePasswordReset";

            //Create the wrapped functions
            function wrappedPassword(sender, args) {
                wrapperMatch(sender, args, checkPasswordMatch, inpPassword, inpRePassword);
            }

            //Assign the function to the key up event once the DOM has completely loaded
            $(document).ready(function () {
                checkPasswordMatch(inpPassword, inpRePassword);

                $(inpPassword).add(inpPassword).keyup(function () {
                    checkPasswordMatch(inpPassword, inpRePassword);
                });
            });
        </script>
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
        <h2 class="signinheading mb-2">Reset password</h2>
        </div>
        <div class="form-group">
            <label for="username">New password</label>
            <asp:CustomValidator ID="valPasswordReset" runat="server" ControlToValidate="inpPasswordReset" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpReset" ValidateEmptyText="True" OnServerValidate="checkPassword"></asp:CustomValidator>
            <input id="inpPasswordReset" required="" type="password" class="form-control" placeholder="Password" runat="server"/>
        </div>
        <div class="form-group">
            <asp:CustomValidator ID="valRePasswordReset" runat="server" ControlToValidate="inpRePasswordReset" ClientValidationFunction="wrappedPassword" Display="None" ValidationGroup="grpReset" ValidateEmptyText="True" OnServerValidate="checkPassword"></asp:CustomValidator>
            <input id="inpRePasswordReset" required="" type="password" class="form-control" placeholder="Re-enter password" runat="server"/>
            <div class="invalid-feedback">Passwords do not match.</div>
        </div>
        <div class="form-group">
            <asp:Button id="btnSubmitReset" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" Text="Change password" ValidationGroup="grpReset" OnClick="ResetPassword" />
        </div>
        <div id="altError" class="alert alert-danger" role="alert" runat="server">
        </div>
    </div>
</asp:Content>
