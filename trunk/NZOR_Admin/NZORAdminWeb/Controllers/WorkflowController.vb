Public Class WorkflowController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /Workflow

    Function Workflow() As ActionResult
        Return View()
    End Function

    Sub ImportProviderData(ByVal formValues As FormCollection)

    End Sub

    Sub RunIntegration(ByVal formValues As FormCollection)

    End Sub

    Sub RunConsensusRefresh(ByVal formValues As FormCollection)

    End Sub

    Function GetProgress(ByVal id As String, ByVal formValues As FormCollection) As ContentResult
        Me.ControllerContext.HttpContext.Response.AddHeader("cache-control", "no-cache")

        Dim val As Integer = 100

        If id = "Integration" Then
            val = 50 'todo
        ElseIf id = "ConsensusRefresh" Then
            val = 50 'todo
        End If

        Return Content(val.ToString)
    End Function

End Class
