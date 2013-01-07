Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Dim cl As New oai.OAIPMHServiceClient("WSHttpBinding_IOAIPMHService")

        'Dim xe As XElement = cl.Identify("plant_names")
        'TextBox1.Text = xe.ToString()



        cl.Open()
        Dim result As XElement = cl.ListRecords("plant_names", "2005-01-01", "2005-09-01", String.Empty, String.Empty, "nzor")
        Dim oai As XNamespace = "http://www.openarchives.org/OAI/2.0/"
        Dim resumptionToken As XElement = result.Element(oai + "resumptionToken")
        result = cl.ListRecords("plant_names", "2005-01-01", "2005-09-01", String.Empty, resumptionToken.Value, "nzor")





        
        'xe = cl.GetRecord("plant_names", "AFA5BA4D-092E-48B9-89F1-0B812F68ADFB", "nzor")
        'TextBox1.Text = xe.ToString()


    End Sub
End Class
