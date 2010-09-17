<HandleError()> _
Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        ViewData("Message") = ""

        Return View()
    End Function

    Function About() As ActionResult
        Return View()
    End Function
End Class
