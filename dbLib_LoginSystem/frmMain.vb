Public Class frmMain

    Private Sub RefreshList()

        'Get all the users from the database and display them here
        Dim reader As OleDb.OleDbDataReader = db.GetRows("users")

        'Ensure we have data first, otherwise don't bother
        If reader IsNot Nothing Then

            'Clear out the listview
            lvUsers.Items.Clear()

            'Loop through each user that you've found
            While reader.Read

                'We make up the columns for our ListView
                Dim columns(2) As String
                columns(0) = reader("id")
                columns(1) = reader("username")
                columns(2) = reader("permissions")

                'Make a ListViewItem so we can add it in
                Dim lvItem As New ListViewItem(columns)
                lvUsers.Items.Add(lvItem)

            End While

        End If

        'Close the reader to save memory
        reader.Close()

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Get all the users we have
        RefreshList()

    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        'Open the add user form
        Dim frm As New frmAddUser
        frm.ShowDialog(Me)

        'Since we might have added users, refresh
        RefreshList()

    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        'Remove the selected items
        For Each item As ListViewItem In lvUsers.Items

            'Call the delete user sub with the user's ID
            DeleteUser(Convert.ToInt32(item.SubItems(0).Text))

        Next

        'We deleted users, refresh
        RefreshList()

    End Sub

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click

        'If they have a user selected, then edit it
        If lvUsers.SelectedIndices.Count > 0 Then

            'Create a new form in memory
            Dim frm As New frmAddUser

            'Get the user's id and place it in the form
            frm.userEditId = Convert.ToInt32(lvUsers.SelectedItems(0).SubItems(0).Text)

            'Show the form
            frm.ShowDialog()

            'Since changes might have occurred, refresh the list
            RefreshList()

        End If

    End Sub
End Class
