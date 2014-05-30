Public Class frmMain

    'Create the database
    Dim db As New dbManager

    Private Sub CreateDatabase()

        db.CreateDatabase("myDatabase.mdb")

        'Create a table with primary fields
        Dim fields As New dbManager.dbFields
        fields.Add("ID", "AUTOINCREMENT", True, "", True)
        fields.Add("Title", "varchar", False, "4")
        fields.Add("Firstname", "varchar", False, "25")
        fields.Add("Lastname", "varchar", False, "25")
        fields.Add("Address", "varchar", False, "150")
        fields.Add("Suburb", "varchar", False, "50")
        fields.Add("State", "varchar", False, "3")
        fields.Add("Postcode", "varchar", False, "4")
        fields.Add("Country", "varchar", False, "25")

        'OK, time to add the actual table then ay
        db.CreateTable("Customers", fields)

        'Add 10 random rows ay
        For i = 0 To 9

            fields.Reset()
            fields.Add("Title", "Mr")
            fields.Add("Firstname", "Testy")
            fields.Add("Lastname", "Test" & i)
            db.AddRow("Customers", fields)

        Next

    End Sub


    Private Sub RefreshListView()

        'Remove the existing items
        lvCustomers.Items.Clear()

        'Get the table info
        Dim reader As OleDb.OleDbDataReader = db.GetRows("Customers")

        If reader IsNot Nothing Then

            While reader.Read

                'Create a new row data
                Dim columns(3) As String

                columns(0) = reader("ID")
                columns(1) = reader("Title")
                columns(2) = reader("Firstname")
                columns(3) = reader("Lastname")

                'Create the row item and add
                Dim lvItem As New ListViewItem(columns)
                lvCustomers.Items.Add(lvItem)

            End While

            reader.Close()

        End If

    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If IO.File.Exists("myDatabase.mdb") Then
            db.OpenDatabase("myDatabase.mdb")
        Else
            CreateDatabase()
        End If

        'Get all the data and read it into our awesome table
        RefreshListView()

    End Sub

    Private Sub lvCustomers_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles lvCustomers.ColumnWidthChanged
        If lvCustomers.Columns(0).Width > 0 Then lvCustomers.Columns(0).Width = 0
    End Sub

    Private Sub lvCustomers_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lvCustomers.MouseDoubleClick

        'Update the selected field
        Dim lvitem As ListViewItem = lvCustomers.GetItemAt(e.Location.X, e.Location.Y)
        txtID.Text = lvitem.SubItems(0).Text

        txtTitle.Text = lvitem.SubItems(1).Text
        txtTitle.Tag = lvitem.SubItems(1).Text

        txtFirstname.Text = lvitem.SubItems(2).Text
        txtFirstname.Tag = lvitem.SubItems(2).Text

        txtLastname.Text = lvitem.SubItems(3).Text
        txtLastname.Tag = lvitem.SubItems(3).Text

    End Sub


    Private Sub UpdateRow(id As Integer)

        Dim lvItem As ListViewItem = Nothing

        'Find the correct row first
        For i = 0 To lvCustomers.Items.Count - 1

            'Check if the id's match
            If CInt(lvCustomers.Items(i).SubItems(0).Text) = id Then
                'FOUND IT!
                lvItem = lvCustomers.Items(i)
                Exit For
            End If

        Next

        'Just ensure that we found something first
        If lvItem IsNot Nothing Then

            'Update the columns
            lvItem.SubItems(1).Text = txtTitle.Text
            lvItem.SubItems(2).Text = txtFirstname.Text
            lvItem.SubItems(3).Text = txtLastname.Text

            lvItem.Selected = True

        End If

    End Sub


    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        If lvCustomers.SelectedItems.Count > 0 Then

            'Do they want to delete selected items
            If MsgBox("Are you sure you want to delete the selected items?", MsgBoxStyle.YesNo, "Confirm Delete") = MsgBoxResult.Yes Then

                'Try to delete the row
                If db.DeleteRow("Customers", "ID=" & lvCustomers.SelectedItems(0).Text) Then
                    'OK then delete the list view row
                    lvCustomers.SelectedItems(0).Remove()
                End If

            End If

        End If

    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click

        'Setup the fields to be a new row
        txtID.Text = "<NEW>"
        txtTitle.Clear()
        txtFirstname.Clear()
        txtLastname.Clear()
        btnSave.Visible = True

    End Sub

    Private Sub txtLastname_LostFocus(sender As Object, e As EventArgs) Handles txtLastname.LostFocus, txtFirstname.LostFocus, txtTitle.LostFocus

        'Ensure we have an id
        If txtID.Text.Trim.Length > 0 And txtID.Text.ToUpper <> "<NEW>" Then

            'Create a new list of fields
            Dim fields As New List(Of dbManager.dbTableField)

            'If the field changed, then update
            If txtTitle.Text <> txtTitle.Tag Then fields.Add("Title", txtTitle.Text))
            If txtFirstname.Text <> txtFirstname.Tag Then fields.Add("Firstname", txtFirstname.Text))
            If txtLastname.Text <> txtLastname.Tag Then fields.Add("Lastname", txtLastname.Text))

            'Check that something has changed!
            If fields.Count > 0 Then

                'OK update then
                If db.UpdateRow("Customers", fields, "ID=" & CInt(txtID.Text)) Then

                    'Refresh that one row
                    UpdateRow(CInt(txtID.Text))

                End If

            End If

        End If

    End Sub


    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        'Check if we are actually in add mode
        If txtID.Text.ToUpper = "<NEW>" Then

            'Build the list fields
            Dim fields As New List(Of dbManager.dbTableField)
            fields.Add("Title", txtTitle.Text))
            fields.Add("Firstname", txtFirstname.Text))
            fields.Add("Lastname", txtLastname.Text))

            'Call on the database
            db.AddRow("Customers", fields)

            'Add to the listview
            RefreshListView()

            'Select the newly added item
            lvCustomers.Items(lvCustomers.Items.Count - 1).Selected = True

            'Clear the boxes
            txtID.Clear()
            txtTitle.Clear()
            txtFirstname.Clear()
            txtLastname.Clear()
            btnSave.Visible = False

        End If

    End Sub


End Class

