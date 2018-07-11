<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.WebForm2" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <form runat="server">
            <script>
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

                function checkUsernameMatch() {
                    //Get values from username input
                    var password = $("[id$='inpUsernameRegister']").val();

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

                //Check if passwords match when loaded from previous tries
                $(document).ready(function () {
                    //checkPasswordMatch();
                });
                //Assign the function to the key up event
                $(document).ready(function () {
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
                <div id="fedUsername" class="invalid-feedback" runat="server"></div>
            </div>
            <div class="form-group">
                <label for="username">Email</label>
                <input id="inpEmailRegister" required="" class="form-control" placeholder="Email" runat="server"/>
                <div class="invalid-feedback">Email is not valid.</div>
            </div>
            <div class="form-group">
                <label for="inpPasswordRegister">Password</label>
                <input id="inpPasswordRegister" required="" type="password" class="form-control" placeholder="Password" runat="server"/>
            </div>
            <div class="form-group">
                <input id="inpRePasswordRegister" required="" type="password" class="form-control" placeholder="Re-enter password" runat="server"/>
                <div class="invalid-feedback">Passwords do not match.</div>
            </div>
                <div id="fedPasswordHelpBlock" class="alert alert-danger" runat="server">
            </div>
            <div class="form-group">
                <button id="btnSubmitRegister" class="btn btn-lg btn-primary btn-block" type="submit" onserverclick="RegisterNewUser" runat="server">Login</button>
            </div>
            <div class="form-group text-center">
                Have an account? <a href="Login">Login here</a>
            </div>
        </form>
    </div>
</asp:Content>