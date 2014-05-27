Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Create the database
        Dim test As New dbManager

        test.CreateDatabase("Test.mdb")
        'test.OpenDatabase("Test.mdb")

        Dim fields As New List(Of dbManager.dbTableField)
        fields.Add(New dbManager.dbTableField("ID", "int", "", True))
        fields.Add(New dbManager.dbTableField("Title", "TEXT", "4"))
        fields.Add(New dbManager.dbTableField("Firstname", "TEXT", "25"))
        fields.Add(New dbManager.dbTableField("Lastname", "TEXT", "25"))

        test.CreateTable("test", fields)

        For i = 0 To 9

            'Dim fields As New List(Of dbManager.dbTableField)
            fields = New List(Of dbManager.dbTableField)
            fields.Add(New dbManager.dbTableField("Title", "Mr"))
            fields.Add(New dbManager.dbTableField("Firstname", "Testy"))
            fields.Add(New dbManager.dbTableField("Lastname", "Test" & i))
            test.AddRow("test", fields)

        Next

        



    End Sub

End Class

