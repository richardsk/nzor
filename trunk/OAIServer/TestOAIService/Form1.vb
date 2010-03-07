Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Dim cl As New oai.OAIPMHServiceClient("WSHttpBinding_IOAIPMHService")

        Dim xe As XElement = cl.Identify("plant_names")
        TextBox1.Text = xe.ToString()
    End Sub
End Class
