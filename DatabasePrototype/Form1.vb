Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Create the database
        Dim test As New dbManager

        test.CreateDatabase("H:\Test.mdb")
        'test.OpenDatabase("H:\Test.mdb")

        Dim fields As New List(Of dbManager.dbTableField)
        fields.Add(New dbManager.dbTableField("Title", "TEXT", "4"))
        fields.Add(New dbManager.dbTableField("Firstname", "TEXT", "25"))
        fields.Add(New dbManager.dbTableField("Lastname", "TEXT", "25"))

        test.CreateTable("test", fields)

        fields = New List(Of dbManager.dbTableField)
        fields.Add(New dbManager.dbTableField("Title", "Mr"))
        fields.Add(New dbManager.dbTableField("Firstname", "Testy"))
        fields.Add(New dbManager.dbTableField("Lastname", "Test"))
        test.AddRow("test", fields)

    End Sub

End Class

