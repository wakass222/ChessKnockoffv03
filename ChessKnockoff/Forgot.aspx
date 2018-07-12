<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="ChessKnockoff.WebForm7" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <script type="text/javascript">
        //Define the ID's
        var inpEmail = "#inpEmailForgot";

        //Create the wrapped functions
        function wrappedEmail(sender, args) {
            wrapperMatch(sender, args, checkEmailRule, inpEmail);
        }

        //Assign the function to the key up event once the DOM has completely loaded
        $(document).ready(function () {
            //Evaluate the rules on page load once in case a post back occurs
            checkEmailRule(inpEmail);

            $(inpEmail).keyup(function () {
                checkEmailRule(inpEmail);
            });
        });
    </script>
    <div class="inputForm mx-auto">
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="signinheading mb-2">Reset password</h2>
        </div>
        <div class="form-group">
        <label for="email">Email</label>
            <asp:CustomValidator ID="valEmailForgot" runat="server" ControlToValidate="inpEmailForgot" ClientValidationFunction="wrappedEmail" Display="None" ValidationGroup="grpForgot" ValidateEmptyText="True"></asp:CustomValidator>
            <input type="email" id="inpEmailForgot" class="form-control" name="email" placeholder="Email" required autofocus="" runat="server"/>
            <div class="invalid-feedback">Email is not valid.</div>
        </div>
        <div class="form-group">
            <asp:Button id="btnForgotSubmit" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" Text="Submit" ValidationGroup="grpForgot" />
        </div>
        <div id="altEmailSent" class="alert alert-success" role="alert" runat="server">
            Reset link was sent to that email.
        </div>
        <div id="altEmailFail" class="alert alert-warning" role="alert" runat="server">
            That email was not found or was not confirmed.
        </div>
    </div>
</asp:Content>
