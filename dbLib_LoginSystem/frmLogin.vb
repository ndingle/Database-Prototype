Public Class frmLogin

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click

        'Close the form
        Me.Close()

    End Sub

    Private Sub frmLogin_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Call the database init function, which lives in Database.vb
        InitDatabase()

    End Sub

    Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogin.Click

        'Query the database and check if the username and password match
        If CheckLogin(txtUsername.Text, txtPassword1.Text) Then

            'Show the form
            frmMain.Show()
            Me.Close()

        Else

            'Something went wrong with the username or password
            MsgBox("Either username or password is incorrect, try again.", MsgBoxStyle.Information, "Login error")

        End If

    End Sub

End Class