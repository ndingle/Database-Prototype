Imports dbLib

Module Database

    'This is the interface for the database
    Private db As New dbManager


    Public Sub InitDatabase()

        'Here we ensure the database exists and is connected

        'Check if the database exists
        If IO.File.Exists("store.mdb") Then

            'It exists, so we open it
            db.OpenDatabase("store.mdb")

        Else

            'Nothing exists, create the database
            db.CreateDatabase("store.mdb")

            'Since it's a new database we need tables
            CreateTables()

        End If

    End Sub


    Private Sub CreateTables()

        'Here we create the tables and fields

        'Create a spot in memory for the fields
        Dim fields As New dbFields

        'INVENTORY TABLE
        'Enter our fields
        fields.Add("id", "int", True, , True, True)
        fields.Add("name", "varchar", True, 100)
        fields.Add("description", "memo", True, 2500)
        fields.Add("price", "currency", True)
        fields.Add("stock_number", "int", True)

        'Create the table with the above fields
        db.CreateTable("Inventory", fields)

        'CUSTOMER TABLE
        fields.Reset()
        fields.Add("id", "int", True, , True, True)
        fields.Add("title", "varchar", True, 4)
        fields.Add("firstname", "varchar", True, 35)
        fields.Add("lastname", "varchar", True, 35)
        fields.Add("address", "varchar", True, 255)
        fields.Add("suburb", "varchar", True, 50)
        fields.Add("state", "varchar", True, 3)
        fields.Add("postcode", "varchar", True, 4)
        fields.Add("country", "varchar", True, 35)

        'Create the table with the above fields
        db.CreateTable("Customers", fields)

        'ORDERS TABLE
        fields.Reset()
        fields.Add("id", "int", True, , True, True)
        fields.Add("customer_id", "int", True)
        fields.Add("ordered_date", "date", True)
        fields.Add("paid", "bit", False)

        'Create the table with the above fields
        db.CreateTable("Orders", fields)

        'ORDER ITEMS TABLE
        fields.Reset()
        fields.Add("id", "int", True, , True, True)
        fields.Add("order_id", "int", True)
        fields.Add("item_id", "int", True)
        fields.Add("quantity", "int", True)

        'Create the table with the above fields
        db.CreateTable("OrderItems", fields)

    End Sub


End Module
