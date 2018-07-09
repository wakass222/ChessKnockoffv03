<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <form id="form1" runat="server">
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
                <asp:TextBox id="inpUsernameRegister" required="" class="form-control" placeholder="Username" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
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
                Already have an account? <a href="Login.aspx">Login here</a>
            </div>
        </form>
    </div>
</asp:Content>