<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="ChessKnockoff.WebForm7" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <script type="text/javascript">
        function checkEmailRule() {
            //Get element
            var inpEmail = $("[id$='inpEmailReset']");

            //Create regex for email
            var emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;

            //Check it against the regex
            if (emailRegex.test(inpEmail.val())) {
                //Show success
                inpEmail.addClass("is-valid");
                inpEmail.removeClass("is-invalid");
            } else {
                //Show error
                inpEmail.removeClass("is-valid");
                inpEmail.addClass("is-invalid");
            }
        }

        //Assign the function to the key up event once the DOM has completely loaded
        $(document).ready(function () {
            //Evaluate the rules on page load once in case a post back occurs
            checkEmailRule();

            $("[id$='inpEmailReset']").keyup(checkEmailRule);
        });
    </script>
    <div class="inputForm mx-auto">
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="signinheading mb-2">Reset password</h2>
        </div>
        <div class="form-group">
        <label for="email">Email</label>
            <input type="text" id="inpEmailReset" class="form-control" name="email" placeholder="Email" required autofocus="" runat="server"/>
            <div class="invalid-feedback">Email is not valid.</div>
        </div>
        <div class="form-group">
            <button id="btnSubmitRLogin" class="btn btn-lg btn-primary btn-block" type="submit" runat="server" onserverclick="EmailClick">Submit</button>
        </div>
        <div id="altEmailSent" class="alert alert-success" role="alert" runat="server">
            Reset link was sent to that email.
        </div>
        <div id="altEmailFail" class="alert alert-warning" role="alert" runat="server">
            That email was not found or was not confirmed.
        </div>
    </div>
</asp:Content>
