Imports ADOX
Imports System.Data.OleDb

Public Class dbManager

    Public Class dbNewTableField

        Public Name As String
        Public Type As String
        Public Size As String

        Sub New(ByVal name As String, ByVal type As String, ByVal size As String)

            Me.Name = name
            Me.Type = type
            Me.Size = size

        End Sub

    End Class


    Public Class dbTableField

        Public Name As String
        Public Value As String

        Sub New(ByVal name As String, ByVal value As String)

            Me.Name = name
            Me.Value = value

        End Sub

    End Class


    Private _connection As OleDb.OleDbConnection


    Public Function GetConnectionString(ByVal database As String, Optional ByVal version As Boolean = True)
        If version Then
            Return "Provider=Microsoft.Jet.OLEDB.4.0;" &
                       "Data Source=" & database & ";" &
                       "Jet OLEDB:Engine Type=5"
        Else
            Return "Provider=Microsoft.Jet.OLEDB.4.0;" &
                       "Data Source=" & database
        End If
    End Function


    Public Function CreateDatabase(ByVal database As String) As Boolean

        Try

            'Create the database ay
            Dim cat As Catalog = New Catalog

            cat.Create(GetConnectionString(database))

            cat = Nothing

            'Open the connection
            OpenDatabase(database)

            Return True

        Catch ex As Exception

            'if the database exists, connect to it
            If ex.Message = "Database already exists." Then
                OpenDatabase(database)
            Else
                MsgBox(ex.Message)
            End If

            Return False

        End Try

    End Function


    Public Sub OpenDatabase(ByVal database As String)

        'Open the database
        _connection = New OleDb.OleDbConnection(GetConnectionString(database, False))

    End Sub


    Private Function GetFieldsString(ByVal fields As List(Of dbNewTableField)) As String

        Dim result As String = ""

        'Loop through the fields
        For Each f As dbNewTableField In fields

            result &= " [" & f.Name & "]" & " " & f.Type & "(" & f.Size & "),"

        Next

        Return result.Substring(0, result.Length - 1)

    End Function


    Public Function CreateTable(ByVal table As String, ByVal fields As List(Of dbNewTableField)) As Boolean

        Try

            _connection.Open()

            'Ensure it doesn't exist first
            Dim dbSchema As DataTable = _connection.GetOleDbSchemaTable(OleDb.OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, table, "TABLE"})

            'Is the table there?
            If dbSchema.Rows.Count = 0 Then
                Dim q As String = "Create Table [" & table & "] (" & GetFieldsString(fields) & ")"
                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                cmd.ExecuteNonQuery()
            End If

            _connection.Close()

            Return True

        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try

    End Function


    Function AddRow(table As String, data As List(Of dbTableField)) As Boolean

        'Add the data to the table

        Try

            'Check table exists
            _connection.Open()

            'Add in the data


            _connection.Close()

            Return True

        Catch ex As Exception

            MsgBox(ex.Message)
            Return False

        End Try



    End Function


    Function Test()

        _connection.Open()

        Dim q As String = "SELECT * FROM test"
        Dim cmd As New OleDb.OleDbCommand(q, _connection)
        Dim reader As OleDbDataReader = cmd.ExecuteReader()

        While reader.Read()
            MsgBox(reader.GetString(0) + ", " _
               + reader.GetString(1))
        End While

        _connection.Close()

    End Function

End Class
