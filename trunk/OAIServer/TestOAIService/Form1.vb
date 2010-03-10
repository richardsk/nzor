Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Dim cl As New oai.OAIPMHServiceClient("WSHttpBinding_IOAIPMHService")

        Dim xe As XElement = cl.Identify("plant_names")
        TextBox1.Text = xe.ToString()


        xe = cl.GetRecord("plant_names", "AFA5BA4D-092E-48B9-89F1-0B812F68ADFB", "nzor")
        TextBox1.Text = xe.ToString()

        Dim cc As New consumer.ServiceClient
        cc.GetName("", "")
    End Sub
End Class
