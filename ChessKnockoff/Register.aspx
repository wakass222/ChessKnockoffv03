<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <form id="form1" runat="server">
            <script>
                //Does not use server control validation as it can not edit the tags on the elements
                //Using javascript and the onkeyup event to submit the postback is wasteful and adds latency to the validation
                //Server validation is still needed since these functions do not disable submission
                //Function to check if passwords match
                function checkPasswordMatch() {
                    //Get values from password inputs
                    var password = $("[id$='inpPasswordRegister']").val();
                    //Class selector has to be used since asp.net creates their own
                    var confirmPassword = $("[id$='inpRePasswordRegister']").val();

                    //Check of they match
                    if (password == confirmPassword) {
                        //If they match remove the is invalid class
                        $("[id$='inpPasswordRegister'], [id$='inpRePasswordRegister']").removeClass("is-invalid");
                    }
                    else {
                        //If they dont add the invalid class
                        $("[id$='inpPasswordRegister'], [id$='inpRePasswordRegister']").addClass("is-invalid");
                    }
                }

                //Function to check if email is valid
                function checkEmail() {
                    //Get the value of the email input
                    var email = $("[id$='inpEmailRegister']").val();

                    //Create regex expression
                    var regex = /^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;

                    //Evaluate the regex expression to the email
                    var emailValid = regex.test(email);

                    //Check if the email is valid
                    if (emailValid) {
                        //If the email is valid then remove the invalid class
                        $("[id$='inpEmailRegister']").removeClass("is-invalid");
                    } else {
                        //If the email is invalid then add the invalid class
                        $("[id$='inpEmailRegister']").addClass("is-invalid");
                    }
                }

                //Function to check if username is valid
                function checkUsername() {
                    var username = $("[id$='inpUsernameRegister']").val();

                    //Create regex expression
                    var regex = /^[a-z0-9]+$/i;

                    //Evaluate the regex expression to the username
                    var usernameValid = regex.test(username)

                    //Check if the email is valid
                    if (usernameValid) {
                        //If the email is valid then remove the invalid class
                        $("[id$='inpUsernameRegister']").removeClass("is-invalid");
                    } else {
                        //If the email is invalid then add the invalid class
                        $("[id$='inpUsernameRegister']").addClass("is-invalid");
                    }
                }

                //Assign an event when the document fully loads
                $(document).ready(function () {
                    //Call the validation methods on document load
                    checkPasswordMatch();
                    checkEmail();
                    checkUsername();

                    //Assign the respective events to the inputs on key up
                    $("[id$='inpUsernameRegister']").keyup(checkUsername);
                    $("[id$='inpEmailRegister']").keyup(checkEmail);
                    $("[id$='inpPasswordRegister'], [id$='inpRePasswordRegister']").keyup(checkPasswordMatch);
                });
            </script>
            <div class="text-center">
                <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
                <h2 class="signinheading mb-2">Register</h2>
            </div>
            <div class="form-group">
                <label for="username">Username</label>
                <asp:TextBox id="inpUsernameRegister" required="" class="form-control" placeholder="Username" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
                <div class="invalid-feedback">Username can only contain alphanumeric values.</div>
            </div>
            <div class="form-group">
                <label for="username">Email</label>
                <asp:TextBox id="inpEmailRegister" required="" class="form-control" placeholder="Email" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
                <div class="invalid-feedback">Email is not valid.</div>
            </div>
            <div class="form-group">
                <label for="inpPasswordRegister">Password</label>
                <asp:TextBox id="inpPasswordRegister" required="" type="password" class="form-control" placeholder="Password" runat="server" AutoPostBack="False" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:TextBox id="inpRePasswordRegister" required="" type="password" class="form-control" placeholder="Re-enter password" runat="server" AutoPostBack="False" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
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