<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Zn.Sample.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Zinc</title>
    <style>

        body, form {
            margin: 0;
            padding: 0;
        }

        body {
            font-family: Arial, sans-serif;
        }

        form {
            width: 800px;
            margin-left: auto;
            margin-right: auto;
        }

        .header {
            background-position: 2% 73%;
            background-repeat: no-repeat;
            padding-top: 30px;
            padding-bottom: 15px;
            padding-left: 50px;
            border-bottom: 4px solid #ededed;
            font-size: 36px;
            font-weight: bold;
        }

        li {
            margin-bottom: 20px;
        }

        a {
            color: blue;
        }

        .fixed {
            font-family: Consolas, Courier New, Courier, monospace;
        }

    </style>
</head>
<body>
    <form id="AppForm" runat="server">
        <div class="header">
            Zinc Services
        </div>

        <ul>
            <asp:Repeater ID="ServiceList" runat="server">
                <ItemTemplate>
                    <li>
                        <div>
                            <a href="<%# Eval("Name") %>Services.svc"><%# Eval("Name") %></a>
                        </div>

                        <div class="fixed">
                            <%# Eval("Namespace") %>
                        </div>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </form>
</body>
</html>
