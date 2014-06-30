Public Class frmAddUser

    'Determines if they are editing or adding users. -1 is adding, >= 0 is editing
    Public userEditId As Integer = -1
    Public forcedUser As Boolean = False

    Private Sub frmAddUser_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Let's check if we are editing a user or adding a new one
        If userEditId = -1 Then

            'Adding a new one

            'Set the permissions to the first item in the list (so it's not blank to start with)
            cmbPermissions.SelectedIndex = 0

        Else

            'Editing an existing user

            'First we need their details
            Dim reader As OleDb.OleDbDataReader = db.GetRows("users", , "id=" & userEditId)

            'Read the information and put it into our text boxes
            reader.Read()
            txtUsername.Text = reader("username")
            cmbPermissions.Text = reader("permissions")

            'Now change a few objects so it looks like we've editing
            Me.Text = "Editing user..."
            btnAdd.Text = "&Edit"

            'Close the reader to save memory
            reader.Close()

        End If

        'This section is for the first user account created, we disable the cancel button and stuff
        If forcedUser Then

            'Disable the ability to close the form
            btnCancel.Enabled = False
            cmbPermissions.Text = "Administrator"
            cmbPermissions.Enabled = False

        End If

    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        'Ensure all the information is there
        If txtUsername.Text = "" Or txtPassword1.Text = "" Then

            'No information, bad user!!
            MsgBox("Please provide a username and a password.", MsgBoxStyle.Information, "Missing information")
            Exit Sub

        End If

        'We need to ensure the username field meets our standards
        If txtUsername.Text.IndexOf(" ") > -1 Or txtUsername.Text.Length < 6 Then

            MsgBox("Username must not contain spaces and must contain at least 6 characters.", MsgBoxStyle.Information, "Bad username")
            Exit Sub

        End If

        'Ensure the passwords match
        If txtPassword1.Text <> txtPassword2.Text Then

            'Passwords don't match, tell them
            MsgBox("The passwords you've entered don't match", MsgBoxStyle.Information, "Password Mismatch")
            Exit Sub

        End If

        'We need to ensure the password field meets our standards
        If txtPassword1.Text.Length < 6 Then

            MsgBox("Password must contain at least 6 characters.", MsgBoxStyle.Information, "Bad password")
            Exit Sub

        End If

        'Check if we should be adding or editing
        If userEditId = -1 Then

            'OK if we got this far, the information is pretty good so add it in
            AddUser(txtUsername.Text, txtPassword1.Text, cmbPermissions.Text)

        Else

            'OK we are editing
            EditUser(userEditId, txtUsername.Text, txtPassword1.Text, cmbPermissions.Text)

        End If

        'Now the user has been added, close the form
        Me.Close()

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click

        'Operation cancelled, close the form
        Me.Close()

    End Sub

End Class