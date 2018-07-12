<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Reset.aspx.cs" Inherits="ChessKnockoff.WebForm2" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <form class="inputForm mx-auto" runat="server">
        <script>
            function checkPasswordMatch() {
                //Get elements
                var inpPassword = $("[id$='inpPasswordReset']");
                var inpPasswordConfirm = $("[id$='inpRePasswordReset']");

                //Check of they match
                if (inpPassword.val() == "") {
                    //If there is nothing them show no extra styling
                    //Each browser will will require it to be fiiled in anyway since all inputs are required
                    inpPassword.add(inpPasswordConfirm).removeClass("is-valid is-invalid");
                }
                else if (inpPassword.val() == inpPasswordConfirm.val()) //Check if they match and are not empty
                {
                    //Show success
                    inpPassword.add(inpPasswordConfirm).addClass("is-valid").removeClass("is-invalid");
                }
                else {
                    //Show error if they are not empty
                    inpPassword.add(inpPasswordConfirm).removeClass("is-valid").addClass("is-invalid");
                }
            }

            //Assign the function to the key up event once the DOM has completely loaded
            $(document).ready(function () {
                //Run check on load in case of postback
                checkPasswordMatch();

                $("[id$='inpPasswordReset'], [id$='inpRePasswordReset']").keyup(checkPasswordMatch);
            });
        </script>
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
        <h2 class="signinheading mb-2">Reset password</h2>
        </div>
        <div class="form-group">
            <label for="username">New password</label>
            <input id="inpPasswordReset" required="" type="password" class="form-control" placeholder="Password" runat="server"/>
        </div>
        <div class="form-group">
            <input id="inpRePasswordReset" required="" type="password" class="form-control" placeholder="Re-enter password" runat="server"/>
            <div class="invalid-feedback">Passwords do not match.</div>
        </div>
        <div class="form-group">
            <button id="btnSubmitReset" class="btn btn-lg btn-primary btn-block" type="submit" onserverclick="ResetPassword" runat="server">Change password</button>
        </div>
        <div id="altError" class="alert alert-danger" role="alert" runat="server">
        </div>
    </form>
</asp:Content>
