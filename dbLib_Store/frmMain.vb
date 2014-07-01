Public Class frmMain

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'TODO: Remove this
        IO.File.Delete("store.mdb")

        'TEST
        InitDatabase()

    End Sub

    Private Sub btnOrder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOrder.Click

        'Show the form
        Dim frm As New frmOrder
        frm.ShowDialog(Me)

    End Sub
End Class
