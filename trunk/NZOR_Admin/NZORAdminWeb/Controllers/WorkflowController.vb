Imports System.Xml

Public Class WorkflowController
    Inherits System.Web.Mvc.Controller

    Private Shared syncRoot As New Object

    Private Shared m_ProviderImportFile As String = ""
    Private Shared m_ImportProgress As Integer = 0
    Private Shared m_IntegrationThread As Threading.Thread
    Private Shared m_ConsensusProgress As Integer = 0
    Private Shared m_WebCacheProgress As Integer = 0


    '
    ' GET: /Workflow

    Function Workflow() As ActionResult
        Return View()
    End Function

    Sub ImportProviderData(ByVal formValues As FormCollection)

    End Sub

    Sub RunIntegration(ByVal formValues As FormCollection)

        m_IntegrationThread = New Threading.Thread(New Threading.ThreadStart(AddressOf ProcessIntegration))
        m_IntegrationThread.Start()
        
    End Sub

    Sub StopIntegration()
        If m_IntegrationThread IsNot Nothing Then
            m_IntegrationThread.Abort()
        End If
    End Sub

    Sub RunConsensusRefresh(ByVal formValues As FormCollection)

    End Sub

    Function GetProgress(ByVal id As String, ByVal formValues As FormCollection) As ContentResult
        Me.ControllerContext.HttpContext.Response.AddHeader("cache-control", "no-cache")

        Dim val As String = "100"

        If id = "Import" Then
            val = m_ImportProgress
        ElseIf id = "Integration" Then
            val = Integration.IntegrationProcessor.Progress.ToString() + "|" + Integration.IntegrationProcessor.StatusText
        ElseIf id = "ConsensusRefresh" Then
            val = m_ConsensusProgress
        ElseIf id = "WebCacheRefresh" Then
            val = m_WebCacheProgress
        End If

        Return Content(val.ToString)
    End Function

    Sub ProcessImport()

    End Sub

    Sub ProcessIntegration()
        SyncLock (syncRoot)
            Dim doc As New XmlDocument()
            doc.Load(IO.Path.Combine(HttpContext.Request.PhysicalApplicationPath, "Configuration\IntegConfig.xml"))

            Dim setId As Integer = CInt(ConfigurationManager.AppSettings("IntegrationSetNumber"))
            Integration.IntegrationProcessor.RunIntegration(doc, setId)
        End SyncLock
    End Sub

End Class
