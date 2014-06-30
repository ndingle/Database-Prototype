Imports ADOX
Imports System.Data.OleDb

''' <summary>
''' An object that is sent to dbManager which specifies fields and data. 
''' It's can be used for adding tables, viewing data from tables and more.
''' </summary>
''' <remarks>Uses a list of dbTableField</remarks>
Public Class dbFields

    Dim _fields As List(Of dbTableField)

    ''' <summary>
    ''' Readonly property returns a dbTableField by index
    ''' </summary>
    ''' <param name="index">A 0 based array refers to the index of the field.</param>
    ''' <value>dbTableField</value>
    ''' <returns>The specified dbTableField</returns>
    ''' <remarks>If index is out of range, Nothing is returned</remarks>
    Default ReadOnly Property Item(ByVal index As Integer) As dbTableField
        Get
            If index >= 0 And index < _fields.Count Then
                Return _fields(index)
            Else
                Return Nothing
            End If
        End Get
    End Property


    ReadOnly Property Items As List(Of dbTableField)
        Get
            Return _fields
        End Get
    End Property


    Sub New()

        _fields = New List(Of dbTableField)

    End Sub


    Public Sub Add(ByVal name As String, ByVal value As Object)

        _fields.Add(New dbTableField(name, value))

    End Sub


    Public Sub Add(ByVal name As String, ByVal type As String, ByVal cannotBeNull As Boolean, Optional ByVal size As Integer = 0, Optional ByVal primaryKeyField As Boolean = False, Optional ByVal autoIncrement As Boolean = False)

        _fields.Add(New dbTableField(name, type, cannotBeNull, size, primaryKeyField, autoIncrement))

    End Sub


    Public Sub Reset()

        _fields.Clear()

    End Sub


End Class


Public Class dbTableField


    Public Name As String
    Public Type As String
    Public Size As String
    Public Value As Object
    Public Primary As Boolean = False
    Public AutoIncrement As Boolean = False
    Public NotNull As Boolean = False


    Sub New(ByVal name As String, ByVal value As Object)

        Me.Name = name
        Me.Value = value

    End Sub


    Sub New(ByVal name As String, ByVal type As String, ByVal notNull As Boolean, Optional ByVal size As String = "", Optional ByVal primary As Boolean = False, Optional ByVal autoIncrement As Boolean = False)

        Me.Name = name
        Me.Type = type
        Me.Size = size
        Me.NotNull = notNull
        Me.Primary = primary
        Me.AutoIncrement = autoIncrement

    End Sub


End Class


Public Class dbManager


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


    Private Function GetFieldsString(ByVal fields As dbFields) As String

        Dim result As String = ""

        'Loop through the fields
        For Each f As dbTableField In fields.Items

            'Add in the field type
            If f.Type.ToUpper = "TEXT" Or f.Type.ToUpper = "VARCHAR" Then
                result &= " " & f.Name & " " & f.Type & "(" & f.Size & ")"
            Else
                result &= " " & f.Name & " " & f.Type
            End If

            'Check if they want null or not
            If f.NotNull Then
                result &= " NOT NULL "
            End If

            'Is the field automatically incremented 
            If f.AutoIncrement Then
                result &= " IDENTITY(1,1) "
            End If

            'Is it primary
            If f.Primary Then
                result &= " PRIMARY KEY "
            End If

            'Add the comma between fields
            result &= ","

        Next

        Return result.Substring(0, result.Length - 1).Replace("  ", " ")

    End Function


    Public Function CreateTable(ByVal table As String, ByVal fields As dbFields) As Boolean

        Try

            OpenConnection()

            'Ensure it doesn't exist first
            Dim dbSchema As DataTable = _connection.GetOleDbSchemaTable(OleDb.OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, table, "TABLE"})

            'Is the table there?
            If dbSchema.Rows.Count = 0 Then
                Dim q As String = "Create Table " & table & " (" & GetFieldsString(fields) & ")"
                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                cmd.ExecuteNonQuery()
            End If

            CloseConnection()

            Return True

        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try

    End Function


    Private Function GetColumnNames(fields As dbFields) As String

        Dim result As String = ""

        'Go through the columns
        For Each column As dbTableField In fields.Items
            result &= column.Name & ","
        Next

        Return result.Substring(0, result.Length - 1)

    End Function


    Private Function GetValues(fields As dbFields) As String

        Dim result As String = ""

        'Go through the columns
        For Each column As dbTableField In fields.Items
            result &= "'" & column.Value & "',"
        Next

        Return result.Substring(0, result.Length - 1)

    End Function


    Function AddRow(table As String, data As dbFields) As Boolean

        'Ensure we have enough data first
        If data.Items.Count > 0 And table.Trim.Length > 0 Then

            'Add the data to the table

            Try

                'Check table exists
                OpenConnection()

                'Add in the data
                Dim q As String = "INSERT INTO " & table & "(" & GetColumnNames(data) & ")" & " VALUES " & "(" & GetValues(data) & ")"
                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                cmd.ExecuteNonQuery()

                CloseConnection()

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
            If columns IsNot Nothing Then

                If columns.Count > 0 Then
                    fields = ""

                    For Each column As String In columns
                        fields = column & ","
                    Next

                    'Snip off the end
                    fields = fields.Substring(0, fields.Length - 1)
                End If

            End If

            Try

                Dim q As String = "SELECT " & fields & " FROM " & table

                'Add the condition if there is one
                If condition.Trim.Length > 0 Then
                    q &= " WHERE " & condition
                End If

                'Execute the sql
                OpenConnection()
                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                Dim result As OleDbDataReader = cmd.ExecuteReader()

                Return result

            Catch ex As Exception
                MsgBox(ex.Message)
                _connection.Close()
                Return Nothing
            End Try

        Else
            Return Nothing
        End If

    End Function


    Private Function GetUpdateColumns(ByVal columns As dbFields) As String

        Dim result As String = ""

        For Each column As dbTableField In columns.Items
            result = column.Name & "='" & column.Value & "',"
        Next

        Return result.Substring(0, result.Length - 1)

    End Function


    Function UpdateRow(ByVal table As String, ByVal columns As dbFields, ByVal whereClause As String) As Boolean

        'Ensure we have a table name
        If table.Trim.Length > 0 Then

            Try

                Dim q As String = "UPDATE " & table & " SET "

                'Ensure we have columns to work with
                If columns IsNot Nothing Then
                    If columns.Items.Count > 0 Then

                        'Break the columns up
                        q &= GetUpdateColumns(columns)

                    Else
                        Return False
                    End If
                Else
                    Return False
                End If


                If whereClause.Trim.Length > 0 Then

                    'Add on the where clause
                    q &= " WHERE " & whereClause

                End If

                'Open the connection and execute
                OpenConnection()

                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                cmd.ExecuteNonQuery()

                CloseConnection()

                Return True

            Catch ex As Exception
                MsgBox(ex.Message)
                Return False
            End Try

        Else
            Return False
        End If

    End Function


    Function DeleteRow(table As String, whereClause As String) As Boolean

        If table.Trim.Length > 0 And whereClause.Trim.Length > 0 Then

            Try

                'Setup the query string
                Dim q As String = "DELETE FROM " & table & " WHERE " & whereClause

                'Do the work
                OpenConnection()

                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                cmd.ExecuteNonQuery()

                CloseConnection()

                Return True

            Catch ex As Exception
                MsgBox(ex.Message)
                Return False
            End Try

        Else
            Return False
        End If

    End Function


    Function DeleteTable(table As String) As Boolean

        'Check if we have a table
        If table.Trim.Length > 0 Then

            Try

                'Setup the query
                Dim q As String = "DROP TABLE " & table

                'Do the work
                OpenConnection()

                Dim cmd As New OleDb.OleDbCommand(q, _connection)
                cmd.ExecuteNonQuery()

                CloseConnection()

                Return True

            Catch ex As Exception
                MsgBox(ex.Message)
                Return False
            End Try
        Else
            Return False
        End If

    End Function


    Private Sub OpenConnection()

        If _connection.State = ConnectionState.Open Then
            _connection.Close()
        End If

        'Now open
        _connection.Open()

    End Sub


    Private Sub CloseConnection()

        _connection.Close()

    End Sub


End Class
