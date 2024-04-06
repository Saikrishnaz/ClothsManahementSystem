Imports System.Data.SqlClient

Module Module1
    Function AgeCalculator(datetimepicker)
        Dim Age As Integer = Date.Today.Year - datetimepicker.Value.Date.Year
        Return Age
    End Function

    Public Sub Populatedvg(con As SqlConnection, tablename As String, datagrid As DataGridView)
        ' Function to generate a Temporary Table from the Database into a DataGridView
        datagrid.Columns.Clear()
        con.Open()
        Dim sql = "SELECT * FROM " & tablename ' Remove the single quotes
        Dim adapter As SqlDataAdapter
        adapter = New SqlDataAdapter(sql, con)
        Dim builder As SqlCommandBuilder
        builder = New SqlCommandBuilder(adapter) ' This line should work correctly
        Dim ds As DataSet
        ds = New DataSet
        adapter.Fill(ds)
        datagrid.DataSource = ds.Tables(0)
        con.Close()
    End Sub

    Function GeneratePassword(TextBox1 As TextBox) As String
        If String.IsNullOrEmpty(TextBox1.Text) Then
            Return "@123"
        Else
            Dim cleanedText = TextBox1.Text.Replace(" ", "")
            Return cleanedText & "@123"
        End If
    End Function
    Sub AutoCompleteSearchBoxForTextBoxesTypeINt(con As SqlConnection, textbox1 As TextBox, ColumnName As String, TableName As String, columnNameIndex As Integer)
        con.Open()
        Dim Query As String = "SELECT " & ColumnName & " FROM " & TableName
        Dim Cmd As New SqlCommand(Query, con)
        Dim reader As SqlDataReader
        reader = Cmd.ExecuteReader
        Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

        While reader.Read
            ' Convert the Int32 column value to a string using ToString()
            ElementsToSuggest.Add(reader.GetInt32(columnNameIndex).ToString())
        End While

        textbox1.AutoCompleteCustomSource = ElementsToSuggest
        con.Close()
    End Sub
    Sub AutoCompleteSearchBoxForTextBoxesTypeString(con As SqlConnection, textbox1 As TextBox, ColumnName As String, TableName As String, columnNameIndex As Integer)
        con.Open()
        Dim Query As String = "SELECT " & ColumnName & " FROM " & TableName
        Dim Cmd As New SqlCommand(Query, con)
        Dim reader As SqlDataReader
        reader = Cmd.ExecuteReader
        Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

        While reader.Read
            ElementsToSuggest.Add(reader.GetString(columnNameIndex)) ' Use GetString to retrieve the column's value as a string

        End While
        textbox1.AutoCompleteCustomSource = ElementsToSuggest
        con.Close()
    End Sub


    Sub FillcomboBox(con As SqlConnection, cmbx As ComboBox, tblName As String, ColumnName As String)
        ' Open the connection
        con.Open()

        ' Create a SQL command to select all data from the specified table
        Dim cmd As New SqlCommand("SELECT * FROM " & tblName, con)

        ' Create a data adapter and a DataTable
        Dim adapter As New SqlDataAdapter(cmd)
        Dim Tbl As New DataTable

        ' Fill the DataTable with data from the database
        adapter.Fill(Tbl)

        ' Set the ComboBox's data source and member bindings
        cmbx.DataSource = Tbl
        cmbx.DisplayMember = ColumnName
        cmbx.ValueMember = ColumnName

        ' Close the connection
        con.Close()
    End Sub


    Sub FillSelectedColumnIncomboBox(con As SqlConnection, cmbx As ComboBox, tblName As String, ColumnName As String, ColumnName2 As String, ClmData As String)
        Try
            ' Open the connection
            con.Open()

            ' Create a SQL command with parameters to select data from the specified table based on the condition
            Dim query As String = "SELECT * FROM " & tblName & " WHERE " & ColumnName2 & " = @ClmData"
            Dim cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@ClmData", ClmData)

            ' Create a data adapter and a DataTable
            Dim adapter As New SqlDataAdapter(cmd)
            Dim Tbl As New DataTable

            ' Fill the DataTable with data from the database
            adapter.Fill(Tbl)

            ' Set the ComboBox's data source and member bindings
            cmbx.DataSource = Tbl
            cmbx.DisplayMember = ColumnName
            cmbx.ValueMember = ColumnName

        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)

        Finally
            ' Close the connection
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Public Function GenerateUniqueInvoiceNo(ByVal con As SqlConnection, ByVal tblName As String) As String
        con.Open()

        Dim InvoiceNo As String = ""
        Dim isUnique As Boolean = False

        ' Loop until a unique account number is generated
        While Not isUnique
            ' Generate a random 10-digit account number
            Dim rnd As New Random()
            Dim tempInvoiceNo As String = GenerateRandomInvoiceNo()

            ' Check if the generated account number already exists in the table
            Dim query As String = "SELECT COUNT(*) FROM " & tblName & " WHERE InvoiceNo = @InvoiceNo"
            Dim cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@InvoiceNo", InvoiceNo)
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

            If count = 0 Then
                ' If the InvoiceNo is unique, assign it and break the loop
                InvoiceNo = tempInvoiceNo
                isUnique = True
            End If
        End While

        con.Close()
        Return InvoiceNo
    End Function

    Public Function GenerateRandomInvoiceNo() As String
        ' Generate a random 10-digit account number
        Dim rnd As New Random()
        Dim InvoiceNo As String = rnd.Next(1000000, 999999999).ToString()
        Return InvoiceNo
    End Function

    Public Sub PopulateSalesAndPurchaseDGV(con As SqlConnection, tablename As String, datagrid As DataGridView, Tradetype As String)
        ' Function to generate a Temporary Table from the Database into a DataGridView
        datagrid.Columns.Clear()

        Try
            con.Open()
            Dim sql As String = "SELECT * FROM " & tablename & " WHERE TradeType = @TradeType"
            Dim adapter As SqlDataAdapter = New SqlDataAdapter()
            Dim command As SqlCommand = New SqlCommand(sql, con)

            ' Use parameters to prevent SQL injection
            command.Parameters.AddWithValue("@TradeType", Tradetype)

            adapter.SelectCommand = command
            Dim ds As DataSet = New DataSet()
            adapter.Fill(ds)
            datagrid.DataSource = ds.Tables(0)
        Catch ex As Exception
            ' Handle exceptions or display error messages
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub
    Public Sub PopulateBilDGV(con As SqlConnection, tablename As String, datagrid As DataGridView, Tradetype As String)
        ' Function to generate a Temporary Table from the Database into a DataGridView
        datagrid.Columns.Clear()

        Try
            con.Open()
            Dim sql As String = "SELECT * FROM " & tablename & " WHERE TradeType = @TradeType"
            Dim adapter As SqlDataAdapter = New SqlDataAdapter()
            Dim command As SqlCommand = New SqlCommand(sql, con)

            ' Use parameters to prevent SQL injection
            command.Parameters.AddWithValue("@TradeType", Tradetype)

            adapter.SelectCommand = command
            Dim ds As DataSet = New DataSet()
            adapter.Fill(ds)
            datagrid.DataSource = ds.Tables(0)
        Catch ex As Exception
            ' Handle exceptions or display error messages
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

End Module
