Imports ADOX
Imports System.Data.OleDb

Public Class dbManager


    Public Class dbTableField


        Public Name As String
        Public Type As String
        Public Size As String
        Public Value As Object
        Public Primary As Boolean = False


        Sub New(ByVal name As String, ByVal value As Object)

            Me.Name = name
            Me.Value = value

        End Sub


        Sub New(name As String, type As String, Optional size As String = "", Optional primary As Boolean = False)

            Me.Name = name
            Me.Type = type
            Me.Size = size
            Me.Primary = primary

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


    Private Function GetFieldsString(ByVal fields As List(Of dbTableField)) As String

        Dim result As String = ""
        Dim primaryField As String = ""

        'Loop through the fields
        For Each f As dbTableField In fields

            'Add in the field type
            If f.Type.ToUpper = "TEXT" Or f.Type.ToUpper = "VARCHAR" Then
                result &= " " & f.Name & " " & f.Type & "(" & f.Size & ")"
            Else
                result &= " " & f.Name & " " & f.Type
            End If

            'Is it primary
            If f.Primary Then
                result &= " NOT NULL AUTO_INCREMENT"
                primaryField = f.Name
            End If


            'Add the comma
            result &= ","

        Next

        'Add in the primary allocation
        If primaryField <> "" Then
            result &= "PRIMARY KEY (" & primaryField & ")"
        Else
            result = result.Substring(0, result.Length - 1)
        End If

        Return result

    End Function


    Public Function CreateTable(ByVal table As String, ByVal fields As List(Of dbTableField)) As Boolean

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


    Private Function GetColumnNames(data As List(Of dbTableField)) As String

        Dim result As String = ""

        'Go through the columns
        For Each column As dbTableField In data
            result &= column.Name & ","
        Next

        Return result.Substring(0, result.Length - 1)

    End Function


    Private Function GetValues(data As List(Of dbTableField)) As String

        Dim result As String = ""

        'Go through the columns
        For Each column As dbTableField In data
            result &= "'" & column.Value & "',"
        Next

        Return result.Substring(0, result.Length - 1)

    End Function


    Function AddRow(table As String, data As List(Of dbTableField)) As Boolean

        'Ensure we have enough data first
        If data.Count > 0 And table.Trim.Length > 0 Then

            'Add the data to the table

            Try

                'Check table exists
                _connection.Open()

                'Add in the data
                Dim q As String = "INSERT INTO " & table & "(" & GetColumnNames(data) & ")" & " VALUES " & "(" & GetValues(data) & ")"
                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                cmd.ExecuteNonQuery()

                _connection.Close()

                Return True

            Catch ex As Exception

                MsgBox(ex.Message)
                Return False

            End Try
        Else
            Return False
        End If

    End Function


    Function GetRows(table As String, Optional columns As List(Of String) = Nothing, Optional condition As String = "") As OleDbDataReader

        'Ensure we have all the data to start with
        If table.Trim.Length > 0 Then

            'Default to all columns
            Dim fields As String = "*"

            'If specified the get all
            If columns.Count > 0 Then
                fields = ""

                For Each column As String In columns
                    fields = column & ","
                Next

                'Snip off the end
                fields = fields.Substring(0, fields.Length - 1)

            End If

            Try

                _connection.Open()

                Dim q As String = "SELECT " & fields & " FROM " & table

                'Add the condition if there is one
                If condition.Trim.Length > 0 Then
                    q &= " WHERE " & condition
                End If

                'Execute the sql
                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                _connection.Close()

                Return cmd.ExecuteReader()

            Catch ex As Exception
                MsgBox(ex.Message)
                _connection.Close()
                Return Nothing
            End Try

        Else
            Return Nothing
        End If

    End Function

End Class
