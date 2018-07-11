<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <form id="form3" runat="server" enableviewstate="False" class="inputForm mx-auto">
        <script>
            function checkPasswordMatch() {
                //Get elements
                var inpPassword = $("[id$='inpPasswordRegister']");
                var inpPasswordConfirm = $("[id$='inpRePasswordRegister']");

                //Check of they match
                if (inpPassword.val() == inpPasswordConfirm.val()) {
                    //Show success
                    inpPassword.add(inpPasswordConfirm).addClass("is-valid");
                    inpPassword.add(inpPasswordConfirm).removeClass("is-invalid");
                }
                else {
                    //Show error
                    inpPassword.add(inpPasswordConfirm).removeClass("is-valid");
                    inpPassword.add(inpPasswordConfirm).addClass("is-invalid");
                }
            }

            function checkEmailRule() {
                //Get element
                var inpEmail = $("[id$='inpEmailRegister']");

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

            function checkUsernameRule() {
                //Get element
                var inpUsername = $("[id$='inpUsernameRegister']");

                //Create regex for alphanumeric characters only
                var usernameRegex = /^[a-z0-9]+$/i;

                //Check it against the regex
                if (usernameRegex.test(inpUsername.val())) {
                    //Show success
                    inpUsername.addClass("is-valid");
                    inpUsername.removeClass("is-invalid");
                } else {
                    //Show error
                    inpUsername.removeClass("is-valid");
                    inpUsername.addClass("is-invalid");
                }
            }

            //Assign the function to the key up event once the DOM has completely loaded
            $(document).ready(function () {
                checkEmailRule();
                checkPasswordMatch();
                checkUsernameRule();

                $("[id$='inpEmailRegister']").keyup(checkEmailRule);
                $("[id$='inpUsernameRegister']").keyup(checkUsernameRule);
                $("[id$='inpPasswordRegister'], [id$='inpRePasswordRegister']").keyup(checkPasswordMatch);
            });
        </script>

        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="signinheading mb-2">Register</h2>
        </div>
        <div class="form-group">
            <label for="username">Username</label>
            <input id="inpUsernameRegister" required="" class="form-control" placeholder="Username" runat="server"/>
            <div id="fedUsername" class="invalid-feedback" runat="server">Username can only contain alphanumeric characters.</div>
                <div id="altUsernameTaken" class="alert alert-danger" runat="server">
                    Username has been taken.
                </div>
        </div>
        <div class="form-group">
            <label for="username">Email</label>
            <input id="inpEmailRegister" required="" class="form-control" placeholder="Email" runat="server"/>
            <div class="invalid-feedback">Email is not valid.</div>
        </div>
        <div id="altEmailTaken" class="alert alert-danger" runat="server">
            Email has already been used.
        </div>
        <div class="form-group">
            <label for="inpPasswordRegister">Password</label>
            <input id="inpPasswordRegister" required="" type="password" class="form-control" placeholder="Password" runat="server"/>
        </div>
        <div class="form-group">
            <input id="inpRePasswordRegister" required="" type="password" class="form-control" placeholder="Re-enter password" runat="server"/>
            <div class="invalid-feedback">Passwords do not match.</div>
        </div>
        <div id="altPassword" class="alert alert-danger" runat="server">
        </div>
        <div class="form-group">
            <button id="btnSubmitRegister" class="btn btn-lg btn-primary btn-block" type="submit" onserverclick="RegisterNewUser" runat="server">Register</button>
            <div id="altError" class="invalid-feedback" runat="server"></div>
        </div>
        <div class="form-group text-center">
            Have an account? <a href="Login">Login here</a>
        </div>
    </form>
</asp:Content>
