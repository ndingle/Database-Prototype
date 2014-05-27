Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Create the database
        Dim test As New dbManager

        test.CreateDatabase("H:\Test.mdb")
        'test.OpenDatabase("H:\Test.mdb")

        Dim fields As New List(Of dbManager.dbNewTableField)
        fields.Add(New dbManager.dbNewTableField("Title", "TEXT", "4"))
        fields.Add(New dbManager.dbNewTableField("Firstname", "TEXT", "25"))
        fields.Add(New dbManager.dbNewTableField("Lastname", "TEXT", "25"))

        test.CreateTable("test", fields)
        test.Test()

    End Sub

End Class

