<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebForm6.aspx.cs" Inherits="ChessKnockoff.WebForm6" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Restore Focus after background postback</title>
    <script type="text/javascript">
      var postbackElement = null;
      function RestoreFocus(source, args)
      {
        document.getElementById(postbackElement.id).focus();
      }
      function SavePostbackElement(source, args)
      {
        postbackElement = args.get_postBackElement();
      }
      function AddRequestHandler()
      {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(RestoreFocus);
        prm.add_beginRequest(SavePostbackElement);
      }
    </script>
</head>
<body onload="AddRequestHandler()">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm" runat="server"></asp:ScriptManager>
    <div>
        <asp:UpdatePanel runat="server" ID="updPanel1">
          <ContentTemplate>
            <asp:Panel ID="panel1" runat="server">
                <asp:RadioButtonList ID="rbList" runat="server" AutoPostBack="true">
                  <asp:ListItem Text="Rb 1" Value="1"></asp:ListItem>
                  <asp:ListItem Text="Rb 2" Value="2"></asp:ListItem>
                  <asp:ListItem Text="Rb 3" Value="3"></asp:ListItem>
                  <asp:ListItem Text="Rb 4" Value="4"></asp:ListItem>
                  <asp:ListItem Text="Rb 5" Value="5"></asp:ListItem>
                </asp:RadioButtonList><br />
                <asp:DropDownList ID="ddSample" runat="server" AutoPostBack="true">
                  <asp:ListItem Text="Item 1" Value="1"></asp:ListItem>
                  <asp:ListItem Text="Item 2" Value="2"></asp:ListItem>
                  <asp:ListItem Text="Item 3" Value="3"></asp:ListItem>
                </asp:DropDownList>
            </asp:Panel>
          </ContentTemplate>
        </asp:UpdatePanel>
      </div>
    </form>
</body>
</html>