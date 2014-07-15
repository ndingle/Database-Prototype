Imports dbLib
Imports System.Text
Imports System.Security.Cryptography

Module Database

    Public db As New dbManager


    Public Sub InitDatabase()

        'Check if the database exists first
        If Not IO.File.Exists("data.mdb") Then

            'Here we have a quick call to CreateDatabase which will create a blank database 
            'and connect to it automatically
            db.CreateDatabase("data.mdb")

            'Now the database has been created, fill it with tables
            CreateTables()

        Else

            'Since the database exists, we just open it instead
            db.OpenDatabase("data.mdb")

        End If

        'Here we perform required checks
        CheckDatabase()

    End Sub


    Public Sub CheckDatabase()

        'Here we perform and checks that the database must pass to run the program

        'Check: An adminstrator user exists to login
        Dim reader As OleDb.OleDbDataReader = db.GetRows("users", , "permissions='Administrator'")

        'Ensure there is a reader we can use
        If reader IsNot Nothing Then

            'Check for results
            If Not reader.HasRows Then

                'OK since we have no users, make the user add in one
                MsgBox("No administrator users have been found." & vbCrLf & "Please add one administrator account to continue.", MsgBoxStyle.Information, "No admin users")

                'Make a new form in memory
                Dim frm As New frmAddUser

                'Set the form to forcefully add a user
                frm.forcedUser = True

                'Show the form
                frm.ShowDialog()

            End If

        End If

        'Close the reader to save memory
        reader.Close()

    End Sub


    Public Sub CreateTables()

        'Here we create the tables from scratch, this is only done if the database was just created

        'This variable will hold all of the fields for us to add
        Dim fields As New dbFields()

        'Add in each field one by one
        fields.Add("id", "int", True, , True, True)             'Their id field
        fields.Add("username", "varchar", False, 30)            'Their username
        fields.Add("user_password", "varchar", False, 32)       'I had to call it something other than 'password', stupid access doesn't like it
        fields.Add("date_created", "date", False)               'When their account was created
        fields.Add("permissions", "varchar", False, 50)         'What level permissions they have
        fields.Add("active", "bit", False)                      'bit = Boolean, if their account is active or not

        'Add the new table to the database
        db.CreateTable("users", fields)

    End Sub


    Public Sub AddUser(ByVal username As String, ByVal password As String, ByVal permissions As String)

        'Here we add a user to the database
        '
        'NOTE: This sub makes use of an MD5 hash to store the password. You should NEVER
        'store a user's password as raw text and even an MD5 isn't that great either. 
        'Read up on hashes here: https://www.youtube.com/watch?v=33QT7xohUvI

        'OK create the list of fields for us to add
        Dim fields As New dbFields()

        'Add in the fields and the data
        fields.Add("username", username)                        'Add the username
        fields.Add("user_password", GetMd5Hash(password))       'Add the password, but store the MD5 hash only. See ABOVE about hashes
        fields.Add("permissions", permissions)                  'Add the permissions
        fields.Add("date_created", Now)                         'Add the current date and time to the date_created field
        fields.Add("active", 0)                                 '0 = true, -1 = false. This is for the active field

        'Add the data into the field
        db.AddRow("users", fields)

    End Sub


    Public Sub EditUser(ByVal id As Integer, ByVal username As String, ByVal password As String, ByVal permissions As String)

        'Here we edit the given user (by their id)

        Dim fields As New dbFields()

        'Add the new data
        fields.Add("username", username)                        'Add the username
        fields.Add("user_password", GetMd5Hash(password))       'Add the password, but store the MD5 hash only. See AddUser Sub about hashes
        fields.Add("permissions", permissions)                  'Add the permissions

        'Update th data in the record
        db.UpdateRow("users", fields, "id=" & id.ToString)

    End Sub


    Public Sub DeleteUser(ByVal id As Integer)

        'Here we remove a user (by their id only)
        db.DeleteRow("users", "id=" & id.ToString)

    End Sub


    Public Function CheckLogin(ByVal username As String, ByVal password As String) As Boolean

        'Here we query the database for the username and check the password works

        'Check if we have any empty fields first (Trim = removes spaces on front and back of string)
        If username.Trim.Length = 0 Or password.Trim.Length = 0 Then
            Return False
        End If

        'Get the password hash
        Dim hash As String = GetMd5Hash(password)

        'Query the database for the user's data
        Dim reader As OleDb.OleDbDataReader = db.GetRows("users", , "username='" & username & "'")

        'Ensure we have data
        If reader IsNot Nothing Then

            'Now get the data and check it
            If reader.Read() Then

                'Check the data
                If hash = reader("user_password") Then
                    Return True
                End If

            End If

        End If

        'Close the reader to save memory
        reader.Close()

        'OK, something failed, either username wasn't found or password's didn't match
        Return False

    End Function


    Function GetMd5Hash(ByVal input As String) As String

        'This function was taken directly from Microsoft's Documentation (MSDN: http://msdn.microsoft.com/en-us/library/system.security.cryptography.md5(v=vs.110).aspx)

        'Create an md5 object
        Dim md5Hash As MD5 = MD5.Create()

        ' Convert the input string to a byte array and compute the hash. 
        Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input))

        ' Create a new Stringbuilder to collect the bytes 
        ' and create a string. 
        Dim sBuilder As New StringBuilder()

        ' Loop through each byte of the hashed data  
        ' and format each one as a hexadecimal string. 
        Dim i As Integer
        For i = 0 To data.Length - 1
            sBuilder.Append(data(i).ToString("x2"))
        Next i

        ' Return the hexadecimal string. 
        Return sBuilder.ToString()

    End Function 'GetMd5Hash


End Module
