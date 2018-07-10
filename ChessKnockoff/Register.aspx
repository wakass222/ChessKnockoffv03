<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ChessKnockoff.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <div class="inputForm mx-auto">
        <form id="form1" runat="server">
            <asp:ScriptManager ID="scriptManagerRegister" runat="server"></asp:ScriptManager>
            <script type="text/javascript">
                var idSelected;
                console.log(idSelected);
                function usernamePost() {
                    __doPostBack("inpUsernameRegister", "");
                     $("input").focusin(function () {
                             idSelected = this.id;
                        });
                };

                function emailPost() {
                    __doPostBack("inpEmailRegister", "");
                        $("input").focusin(function () {
                             idSelected = this.id;
                        });
                };


                function passwordRePost() {
                    __doPostBack("inpPasswordRegister", "");
                        $("input").focusin(function () {
                             idSelected = this.id;
                        });
                };

                $(document).ready(function () {
                    if (idSelected != null) {
                        $("#" + idSelected).focus();
                        idSelected = null;
                    }
                });
            </script>
            <div class="text-center">
                <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
                <h2 class="signinheading mb-2">Register</h2>
            </div>
            <asp:UpdatePanel ID="updatePanelRegister" runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <label for="username">Username</label>
                        <asp:TextBox id="inpUsernameRegister" required="" onkeyup="usernamePost()" class="form-control" placeholder="Username" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
                        <div id="fedUsername" class="invalid-feedback" runat="server"></div>
                    </div>
                    <div class="form-group">
                        <label for="username">Email</label>
                        <asp:TextBox id="inpEmailRegister" required="" onkeyup="emailPost()" class="form-control" placeholder="Email" runat="server" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
                        <div class="invalid-feedback">Email is not valid.</div>
                    </div>
                    <div class="form-group">
                        <label for="inpPasswordRegister">Password</label>
                        <asp:TextBox id="inpPasswordRegister" required="" onkeyup="passwordPost()" type="password" class="form-control" placeholder="Password" runat="server" AutoPostBack="False" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:TextBox id="inpRePasswordRegister" required="" onkeyup="passwordPost()" type="password" class="form-control" placeholder="Re-enter password" runat="server" AutoPostBack="False" ValidateRequestMode="Disabled" ViewStateMode="Enabled"></asp:TextBox>
                        <div class="invalid-feedback">Passwords do not match.</div>
                    </div>
                        <div id="fedPasswordHelpBlock" class="alert alert-danger" runat="server">
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="form-group">
                <button id="btnSubmitRegister" class="btn btn-lg btn-primary btn-block" type="submit" onserverclick="RegisterNewUser" runat="server">Login</button>
            </div>
            <div class="form-group text-center">
                Forgot your password? <a href="Reset">Reset here</a>
            </div>
        </form>
    </div>
</asp:Content>