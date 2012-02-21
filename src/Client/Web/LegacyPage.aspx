<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WebSharper Application Legacy Page</title>
    <WebSharper:ScriptManager runat="server" />
</head>
<body>
    <h1>Legacy Page</h1>
    <p>This is an ordinary ASPX page. <a href="<%= Page.ResolveUrl("~") %>">Go back to the sitelet Home page.</a></p>
</body>
</html>
