Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.IO

Public Class EmployeeForm
    Dim connectionString As String = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\source\repos\ClothsManahementSystem\ClothsManahementSystem\ClothsManagementData.mdf;Integrated Security=True"
    Dim con As New SqlConnection(connectionString)
    Dim lbltrusetedendYear As Integer = DateTime.Now.Year - 2000
    Dim i As Integer = 150
    Dim i2 As Integer = 100
    Dim DastBordTotalQnty As Integer = 0
    Dim DastBordTotalprice As Integer = 0
    Dim DastBordTotalAvailableQuntity As Integer = 0
    Dim DastBordTotalemployees As Integer = 0
    Dim DastBordTotalDealers As Integer = 0
    Private Sub EmployeeForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Populatedvg(con, "EmployeeData", DataGridView1)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_EmployeeSearchIDForFeilds, "EmployeeID", "EmployeeData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_StockNameSeachToFillDGV, "StockName", "StockData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_PurchaseStockName, "StockName", "StockData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_SellStockName, "StockName", "StockData", 0)
        Populatedvg(con, "SupplierData", DataGridView2)
        FillPersonalDetailsInFeilds()
        FillcomboBox(con, Txt_PurchaseSupplierName, "SupplierData", "SupplierName")
        LoadItemsFromFile()
        Populatedvg(con, "StockData", DataGridView3)
        Populatedvg(con, "PurchaseOrSaleData", DataGridView6)
        Populatedvg(con, "BillData", DataGridView7)
        Txt_PurchaseInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        Txt_SellInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        BindDataToDataGridView()
        ' Call the function to create the columns once during the form load
        table = CreateAddcolumnToDatagridview()
        DataGridView4.DataSource = table
        Selltable = CreateAddcolumnToSellDatagridview()
        DataGridView5.DataSource = Selltable
        Txt_PurchaseTotalQuantity.Clear()
        Txt_SellTotalQuantity.Clear()
        DataGridView4.ForeColor = Color.Black
        ReSizeDatagridViews()
        Populatedvg(con, "PurchaseOrSaleData", DataGridView6)
        displaygenratedIDnumber(con, "BillData")
        loadTrustedSinceLabel()
        DastBordTotalQnty = stocksoldquantity()
        DastBordTotalprice = stocksoldtotalAmount()
        DastBordTotalAvailableQuntity = stocksoldtotalAvaiableQuntity()
        DastBordTotalemployees = RetrieveTotalEmployee()
        DastBordTotalDealers = RetrieveTotalDealers()
        loadTotalqntyLabel()
        loadTotalPriceLabel()
        loadTotalAvaiableQuntityLabel()
        loadTotalEmployeeLabel()
        loadTotalDealersLabel()
    End Sub
    Sub ReSizeDatagridViews()
        '-------ClothsWare datagridview-----------
        DataGridView3.Font = New Font("Rockwell", 10, FontStyle.Regular)
        DataGridView3.ForeColor = Color.Black
        DataGridView3.Font = New Font("Rockwell", 10, FontStyle.Regular)
        '-------Purchase datagridview-----------
        DataGridView4.Font = New Font("Rockwell", 10, FontStyle.Regular)
        DataGridView4.ForeColor = Color.Black
        DataGridView4.Font = New Font("Rockwell", 10, FontStyle.Regular)
        '-------Sales datagridview-----------
        DataGridView5.Font = New Font("Rockwell", 10, FontStyle.Regular)
        DataGridView5.ForeColor = Color.Black
        DataGridView5.Font = New Font("Rockwell", 10, FontStyle.Regular)
        '-------SaleAndPurchaseData datagridview-----------
        DataGridView6.Font = New Font("Rockwell", 10, FontStyle.Regular)
        DataGridView6.ForeColor = Color.Black
        DataGridView6.Font = New Font("Rockwell", 10, FontStyle.Regular)
        '-------BillData datagridview-----------
        DataGridView7.Font = New Font("Rockwell", 10, FontStyle.Regular)
        DataGridView7.ForeColor = Color.Black
        DataGridView7.Font = New Font("Rockwell", 10, FontStyle.Regular)
    End Sub
    Private Function IsAllEmployeeFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_EmployeeAddharNo.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_EmployeeAddress.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_EmployeeEmailAddress.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_EmployeeName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_EmployeePhnNo.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_EmployeeSalary.Text) OrElse
            Txt_EmployeeGender.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllEmployeeControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_EmployeeAddharNo, Txt_EmployeeAddress, Txt_EmployeeEmailAddress, Txt_EmployeeName, Txt_EmployeePhnNo, Txt_EmployeeSalary}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_EmployeeGender.SelectedIndex = -1
        DTP_EmployeeDOB.Value = Date.Today
        DTP_EmployeeDOJ.Value = Date.Today
    End Sub
    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        If Not IsAllEmployeeFieldsFilled() Then
            MsgBox("Please fill in all the Employee details.")
            Exit Sub
        End If

        Try
            Dim age As Integer = AgeCalculator(DTP_EmployeeDOB)
            Dim CusPassWord As String = GeneratePassword(Txt_EmployeeName)
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO EmployeeData (Name,  PhoneNo,EmailAddress,Salary,AddharNo,DateOfJoining,DateOfBirth, Gender,Age,Address,Password) " &
                        "VALUES (@Name,  @PhoneNo,@EmailAddress,@Salary,@AddharNo,@DateOfJoining,@DateOfBirth, @Gender,@Age,@Address,@Password)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_EmployeeName.Text), Txt_EmployeeName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_EmployeePhnNo.Text), Txt_EmployeePhnNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@EmailAddress", If(Not String.IsNullOrEmpty(Txt_EmployeeEmailAddress.Text), Txt_EmployeeEmailAddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Salary", If(Not String.IsNullOrEmpty(Txt_EmployeeSalary.Text), Txt_EmployeeSalary.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@AddharNo", If(Not String.IsNullOrEmpty(Txt_EmployeeAddharNo.Text), Txt_EmployeeAddharNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DateOfJoining", DTP_EmployeeDOJ.Value.Date)
                cmd.Parameters.AddWithValue("@DateOfBirth", DTP_EmployeeDOB.Value.Date)
                cmd.Parameters.AddWithValue("@Age", age)
                cmd.Parameters.AddWithValue("@Gender", If(Txt_EmployeeGender.SelectedIndex <> -1, Txt_EmployeeGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_EmployeeAddress.Text), Txt_EmployeeAddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Password", CusPassWord)
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("New Employee : " & Txt_EmployeeName.Text & "'s  Data has been added Successfully ")

                ' Clear controls...
                ClearAllEmployeeControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "EmployeeData", DataGridView1)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Txt_EmployeeSearchIDForFeilds.Enabled = True
        Else
            Txt_EmployeeSearchIDForFeilds.Enabled = False
        End If
    End Sub



    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        ClearAllEmployeeControls()
    End Sub

    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        If Not IsAllEmployeeFieldsFilled() Then
            MsgBox("Please fill in all the Employee details.")
            Exit Sub
        End If

        Try
            Dim age As Integer = AgeCalculator(DTP_EmployeeDOB)
            Dim CusPassWord As String = GeneratePassword(Txt_EmployeeName)
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "UPDATE EmployeeData SET Name = @Name, PhoneNo = @PhoneNo, EmailAddress = @EmailAddress, " &
                          "Salary = @Salary, AddharNo = @AddharNo, DateOfJoining = @DateOfJoining, " &
                          "DateOfBirth = @DateOfBirth, Gender = @Gender, Age = @Age, Address = @Address, " &
                          "Password = @Password WHERE EmployeeID = @EmployeeID" ' Assuming EmployeeID is the primary key

            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@EmployeeID", Txt_EmployeeSearchIDForFeilds.Text) ' Replace employeeID with the specific employee ID you want to update
                cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_EmployeeName.Text), Txt_EmployeeName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_EmployeePhnNo.Text), Txt_EmployeePhnNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@EmailAddress", If(Not String.IsNullOrEmpty(Txt_EmployeeEmailAddress.Text), Txt_EmployeeEmailAddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Salary", If(Not String.IsNullOrEmpty(Txt_EmployeeSalary.Text), Txt_EmployeeSalary.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@AddharNo", If(Not String.IsNullOrEmpty(Txt_EmployeeAddharNo.Text), Txt_EmployeeAddharNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DateOfJoining", DTP_EmployeeDOJ.Value.Date)
                cmd.Parameters.AddWithValue("@DateOfBirth", DTP_EmployeeDOB.Value.Date)
                cmd.Parameters.AddWithValue("@Age", age)
                cmd.Parameters.AddWithValue("@Gender", If(Txt_EmployeeGender.SelectedIndex <> -1, Txt_EmployeeGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_EmployeeAddress.Text), Txt_EmployeeAddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Password", CusPassWord)
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Employee with ID: " & Txt_EmployeeSearchIDForFeilds.Text & " data has been updated successfully")

                ' Clear controls...
                ClearAllEmployeeControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "EmployeeData", DataGridView1)

    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If Not String.IsNullOrEmpty(Txt_EmployeeSearchIDForDGV.Text) Then
                DataGridView1.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = "EmployeeID"
                Dim tablename As String = "EmployeeData"

                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", Txt_EmployeeSearchIDForDGV.Text)
                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView1.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the Employee ID To Search")
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2GradientButton26_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton26.Click
        If Not String.IsNullOrEmpty(Txt_EmployeeSearchIDForFeilds.Text) Then

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM EmployeeData WHERE EmployeeID = @EmployeeID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@EmployeeID", Txt_EmployeeSearchIDForFeilds.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_EmployeeName.Text = reader(1) ' Name
                        DTP_EmployeeDOB.Value = reader(7) ' Date of Birth
                        DTP_EmployeeDOJ.Value = reader(6) ' Date of Joining
                        Txt_EmployeeGender.Text = reader(8) ' Gender
                        Txt_EmployeePhnNo.Text = reader(2) ' Phone Number
                        Txt_EmployeeEmailAddress.Text = reader(3) ' Email
                        Txt_EmployeeAddress.Text = reader(10) ' Address
                        Txt_EmployeeAddharNo.Text = reader(5) ' Addhar no
                        Txt_EmployeeSalary.Text = reader(4) ' Salary

                    Else
                        MessageBox.Show("Employee ID number not found.")
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Please enter a Empnoyee ID number.")
        End If

        Populatedvg(con, "EmployeeData", DataGridView1)
    End Sub


    Private Function IsAllSupplierFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_SupplierAddresss.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SupplierAge.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SupplierName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SupplierPhoneNo.Text) OrElse
            Txt_SupplierGender.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllSupplierControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_SupplierAddresss, Txt_SupplierAge, Txt_SupplierName, Txt_SupplierPhoneNo}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_SupplierGender.SelectedIndex = -1
    End Sub
    Private Sub Guna2GradientButton7_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton7.Click
        If Not IsAllSupplierFieldsFilled() Then
            MsgBox("Please fill in all the Supplier details.")
            Exit Sub
        End If

        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO SupplierData (SupplierName,PhoneNumber,SupplierAge,SupplierGender,SupplierAddress) " &
                        "VALUES (@SupplierName,@PhoneNumber,@SupplierAge,@SupplierGender,@SupplierAddress)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@SupplierName", If(Not String.IsNullOrEmpty(Txt_SupplierName.Text), Txt_SupplierName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNumber", If(Not String.IsNullOrEmpty(Txt_SupplierPhoneNo.Text), Txt_SupplierPhoneNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@SupplierAge", If(Not String.IsNullOrEmpty(Txt_SupplierAge.Text), Txt_SupplierAge.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@SupplierGender", If(Txt_SupplierGender.SelectedIndex <> -1, Txt_SupplierGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@SupplierAddress", If(Not String.IsNullOrEmpty(Txt_SupplierAddresss.Text), Txt_SupplierAddresss.Text, DBNull.Value))
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("New Supplier : " & Txt_SupplierName.Text & "'s  Data has been added Successfully ")

                ' Clear controls...
                ClearAllSupplierControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "SupplierData", DataGridView2)
    End Sub

    Private Sub Guna2GradientButton8_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton8.Click
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If Not String.IsNullOrEmpty(Txt_SupplierIdForfDGV.Text) Then
                DataGridView2.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = "SuplierID"
                Dim tablename As String = "SupplierData"

                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", Txt_SupplierIdForfDGV.Text)
                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView2.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the Supplier ID To Search")
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2GradientButton28_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton28.Click
        If Not String.IsNullOrEmpty(Txt_SupplierIdForfeildSearch.Text) Then

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM SupplierData WHERE SuplierID = @SuplierID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@SuplierID", Txt_SupplierIdForfeildSearch.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_SupplierName.Text = reader(1) ' Name
                        Txt_SupplierPhoneNo.Text = reader(2) ' PhoneNumber
                        Txt_SupplierAge.Text = reader(3) ' Phone Number
                        Txt_SupplierGender.Text = reader(4) ' Email
                        Txt_SupplierAddresss.Text = reader(5) ' address

                    Else
                        MessageBox.Show("Supplier ID number not found.")
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Please enter a Supplier ID number.")
        End If

        Populatedvg(con, "SupplierData", DataGridView2)
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked Then
            Txt_SupplierIdForfeildSearch.Enabled = True
        Else
            Txt_SupplierIdForfeildSearch.Enabled = False
        End If
    End Sub

    Private Sub Guna2GradientButton6_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton6.Click

        If Not IsAllSupplierFieldsFilled() Then
            MsgBox("Please fill in all the Supplier details.")
            Exit Sub
        End If

        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()

            ' Assuming SupplierID is the unique identifier for each supplier record
            Dim query As String = "UPDATE SupplierData SET SupplierName = @SupplierName, PhoneNumber = @PhoneNumber, " &
                                  "SupplierAge = @SupplierAge, SupplierGender = @SupplierGender, SupplierAddress = @SupplierAddress " &
                                  "WHERE SuplierID = @SuplierID"

            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@SuplierID", Txt_SupplierIdForfeildSearch.Text) ' Replace supplierID with the specific supplier ID you want to update
                cmd.Parameters.AddWithValue("@SupplierName", If(Not String.IsNullOrEmpty(Txt_SupplierName.Text), Txt_SupplierName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNumber", If(Not String.IsNullOrEmpty(Txt_SupplierPhoneNo.Text), Txt_SupplierPhoneNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@SupplierAge", If(Not String.IsNullOrEmpty(Txt_SupplierAge.Text), Txt_SupplierAge.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@SupplierGender", If(Txt_SupplierGender.SelectedIndex <> -1, Txt_SupplierGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@SupplierAddress", If(Not String.IsNullOrEmpty(Txt_SupplierAddresss.Text), Txt_SupplierAddresss.Text, DBNull.Value))

                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Supplier with ID: " & Txt_SupplierIdForfeildSearch.Text & " data has been updated successfully")

                ' Clear controls...
                ClearAllSupplierControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "SupplierData", DataGridView2)

    End Sub

    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
        ClearAllSupplierControls()
    End Sub

    Private Sub Guna2GradientButton24_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton24.Click
        If Not String.IsNullOrEmpty(Txt_EmppID.Text) Then
            con.Open()
            Try
                ' Check if the patient ID exists in the database
                Dim query = "SELECT * FROM EmployeeData WHERE EmployeeID = @EmployeeID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@EmployeeID", Txt_EmppID.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        Txt_EmppName.Text = reader(1) ' Name
                        DPT_EmppDOB.Value = reader(7) ' Date of Birth
                        Txt_EmppGender.Text = reader(8) ' gender
                        Txt_EmppPhnNo.Text = reader(2) ' PhnNo
                        Txt_EmppEmailAddress.Text = reader(3) ' Email
                        Txt_EmppPassword.Text = reader(11) ' Password
                        DPT_EmppDOJ.Value = reader(6) ' DOJ
                        Txt_EmppAddress.Text = reader(10) ' Address
                        Txt_EmppSalary.Text = reader(4) ' Salary
                        Txt_EmppAge.Text = reader(9) ' Age
                        Txt_EmppAddharNo.Text = reader(5) ' Addhar
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End If
    End Sub


    Private Sub FillPersonalDetailsInFeilds()
        If Not String.IsNullOrEmpty(Txt_EmppID.Text) Then
            con.Open()
            Try
                ' Check if the patient ID exists in the database
                Dim query = "SELECT * FROM EmployeeData WHERE EmployeeID = @EmployeeID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@EmployeeID", Txt_EmppID.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        Txt_EmppName.Text = reader(1) ' Name
                        DPT_EmppDOB.Value = reader(7) ' Date of Birth
                        Txt_EmppGender.Text = reader(8) ' gender
                        Txt_EmppPhnNo.Text = reader(2) ' PhnNo
                        Txt_EmppEmailAddress.Text = reader(3) ' Email
                        Txt_EmppPassword.Text = reader(11) ' Password
                        DPT_EmppDOJ.Value = reader(6) ' DOJ
                        Txt_EmppAddress.Text = reader(10) ' Address
                        Txt_EmppSalary.Text = reader(4) ' Salary
                        Txt_EmppAddharNo.Text = reader(5) ' Addhar
                        Txt_EmppAge.Text = reader(9) ' Age
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End If
    End Sub

    Private Sub Guna2GradientButton27_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton27.Click
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
        If Not String.IsNullOrEmpty(Txt_EmppID.Text) Then
            Try
                ' Check if the employee ID exists in the database
                Dim query = "UPDATE EmployeeData SET Name = @Name, PhoneNo = @PhoneNo, EmailAddress = @EmailAddress, " &
                    "Salary = @Salary, AddharNo = @AddharNo, DateOfJoining = @DateOfJoining, " &
                    "DateOfBirth = @DateOfBirth, Gender = @Gender, Age = @Age, Address = @Address, " &
                    "Password = @Password WHERE EmployeeID = @EmployeeID"

                Using cmd As New SqlCommand(query, con)
                    ' Set parameter values...'
                    cmd.Parameters.AddWithValue("@EmployeeID", Txt_EmppID.Text)
                    cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_EmppName.Text), Txt_EmppName.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_EmppPhnNo.Text), Txt_EmppPhnNo.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@EmailAddress", If(Not String.IsNullOrEmpty(Txt_EmppEmailAddress.Text), Txt_EmppEmailAddress.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Salary", If(Not String.IsNullOrEmpty(Txt_EmppSalary.Text), Txt_EmppSalary.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@AddharNo", If(Not String.IsNullOrEmpty(Txt_EmppAddharNo.Text), Txt_EmppAddharNo.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@DateOfJoining", DPT_EmppDOJ.Value.Date)
                    cmd.Parameters.AddWithValue("@DateOfBirth", DPT_EmppDOB.Value.Date)
                    cmd.Parameters.AddWithValue("@Age", If(Not String.IsNullOrEmpty(Txt_EmppAge.Text), Txt_EmppAge.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Gender", If(Txt_EmppGender.SelectedIndex <> -1, Txt_EmppGender.SelectedItem.ToString(), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_EmppAddress.Text), Txt_EmppAddress.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Password", If(Not String.IsNullOrEmpty(Txt_EmppPassword.Text), Txt_EmppPassword.Text, DBNull.Value))

                    ' Execute the update query...
                    con.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                    con.Close()

                    If rowsAffected > 0 Then
                        MessageBox.Show("Employee with ID: " & Txt_EmppID.Text & " data has been updated successfully")
                        ' Optionally, clear controls or perform any additional tasks after successful update
                    Else
                        MessageBox.Show("No records were updated for Employee ID: " & Txt_EmppID.Text)
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If

    End Sub
    Private Function IsAllStockFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_StockQuantity.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_StockName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_StockPrice.Text) OrElse
            Txt_StockCategory.SelectedIndex = -1 OrElse
            Txt_StockType.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllStockControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_StockQuantity, Txt_StockName, Txt_StockPrice, Txt_StockIDSeachToFillFeilds, Txt_StockNameSeachToFillDGV}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_StockCategory.SelectedIndex = -1
        Txt_StockType.SelectedIndex = -1
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked Then
            Txt_StockAddCategory.Enabled = True
        Else
            Txt_StockAddCategory.Enabled = False
        End If
    End Sub

    Private Sub SaveItemsToFile()
        Try
            Dim allItems As New List(Of String)

            ' Add items from Txt_StockCategory
            For Each item As String In Txt_StockCategory.Items
                allItems.Add(item)
            Next

            ' Add items from ComboBox9
            For Each item As String In Txt_SellCategory.Items
                If Not allItems.Contains(item) Then
                    allItems.Add(item)
                End If
            Next

            ' Write all items to the file
            Using writer As New StreamWriter("items.txt")
                For Each item As String In allItems
                    writer.WriteLine(item)
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show("Error while saving items: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadItemsFromFile()
        Try
            If File.Exists("items.txt") Then
                Dim lines As String() = File.ReadAllLines("items.txt")

                ' Clear existing items in both ComboBoxes before loading new ones
                Txt_StockCategory.Items.Clear()
                Txt_SellCategory.Items.Clear()
                Txt_PurchaseCategory.Items.Clear()

                For Each line As String In lines
                    Txt_StockCategory.Items.Add(line)
                    Txt_SellCategory.Items.Add(line)
                    Txt_PurchaseCategory.Items.Add(line)
                Next
            End If
        Catch ex As Exception
            MessageBox.Show("Error while loading items: " & ex.Message)
        End Try
    End Sub

    Private Sub Guna2GradientButton12_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton12.Click
        Try
            Dim newItem As String = Txt_StockAddCategory.Text.Trim()

            If Not String.IsNullOrEmpty(newItem) Then
                ' Check if the item already exists in both ComboBoxes
                If Not Txt_StockCategory.Items.Contains(newItem) AndAlso Not Txt_SellCategory.Items.Contains(newItem) Then
                    Txt_StockCategory.Items.Add(newItem)
                    Txt_SellCategory.Items.Add(newItem)
                    Txt_PurchaseCategory.Items.Add(newItem)
                    SaveItemsToFile() ' Save items to file when a new item is added
                Else
                    MessageBox.Show("Item already exists.")
                End If
            Else
                MessageBox.Show("Please enter a valid item.")
            End If
            Txt_StockAddCategory.Clear()
            CheckBox4.Checked = False
        Catch ex As Exception
            MessageBox.Show("Error while adding item: " & ex.Message)
        End Try

    End Sub

    Private Sub Guna2GradientButton29_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton29.Click
        Try
            Dim selectedItem As String = Txt_StockAddCategory.Text.Trim()

            If Not String.IsNullOrEmpty(selectedItem) Then
                ' Check if the item exists in both ComboBoxes
                If Txt_StockCategory.Items.Contains(selectedItem) AndAlso Txt_SellCategory.Items.Contains(selectedItem) Then
                    Txt_StockCategory.Items.Remove(selectedItem)
                    Txt_SellCategory.Items.Remove(selectedItem)
                    Txt_PurchaseCategory.Items.Remove(selectedItem)
                    SaveItemsToFile() ' Save items to file after removing an item
                Else
                    MessageBox.Show("Item not found.")
                End If
            Else
                MessageBox.Show("Please enter a valid item to remove.")
            End If
            Txt_StockAddCategory.Clear()
            CheckBox4.Checked = False
        Catch ex As Exception
            MessageBox.Show("Error while removing item: " & ex.Message)
        End Try
    End Sub

    Private Sub Txt_StockSupplierName_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles Txt_PurchaseSupplierName.SelectionChangeCommitted
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If

        Try
            Dim Query As String = "SELECT * FROM SupplierData WHERE SupplierName = @SupplierName"
            Dim cmd As New SqlCommand(Query, con)
            cmd.Parameters.AddWithValue("@SupplierName", Txt_PurchaseSupplierName.SelectedValue.ToString())

            Dim dt As New DataTable
            Dim reader As SqlDataReader = cmd.ExecuteReader()

            If reader.HasRows Then
                While reader.Read()
                    Txt_PurchaseSupplierID.Text = reader(0).ToString()
                    ' You can retrieve other columns and assign values to respective controls as needed
                End While
            Else
                ' Handle the case where no records are found based on the SupplierName
                MessageBox.Show("No records found for the selected supplier.")
            End If

            reader.Close()
        Catch ex As Exception
            ' Handle any exceptions that might occur during database operations
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

    End Sub

    Private Sub Guna2GradientButton10_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton10.Click
        If Not IsAllStockFieldsFilled() Then
            MsgBox("Please fill in all the Stock details.")
            Exit Sub
        End If

        Try

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO StockData (StockName,  StockType,StockPrice,StockCategory,AvailableQuantity) " &
                        "VALUES (@StockName,  @StockType,@StockPrice,@StockCategory,@AvailableQuantity)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@StockName", If(Not String.IsNullOrEmpty(Txt_StockName.Text), Txt_StockName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@StockType", If(Txt_StockType.SelectedIndex <> -1, Txt_StockType.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@StockPrice", If(Not String.IsNullOrEmpty(Txt_StockPrice.Text), Txt_StockPrice.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@StockCategory", If(Txt_StockCategory.SelectedIndex <> -1, Txt_StockCategory.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@AvailableQuantity", If(Not String.IsNullOrEmpty(Txt_StockQuantity.Text), Txt_StockQuantity.Text, DBNull.Value))
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("New Stock : " & Txt_StockName.Text & "'s  Data has been added Successfully ")

                ' Clear controls...
                ClearAllStockControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "StockData", DataGridView3)
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked Then
            Txt_StockIDSeachToFillFeilds.Enabled = True
        Else
            Txt_StockIDSeachToFillFeilds.Enabled = False
        End If
    End Sub

    Private Sub Guna2GradientButton11_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton11.Click
        If Not IsAllStockFieldsFilled() Then
            MsgBox("Please fill in all the Stock details.")
            Exit Sub
        End If


        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "UPDATE StockData SET StockName = @StockName, StockType = @StockType, StockPrice = @StockPrice, StockCategory = @StockCategory, AvailableQuantity = @AvailableQuantity WHERE StockID = @StockID"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@StockName", If(Not String.IsNullOrEmpty(Txt_StockName.Text), Txt_StockName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@StockID", If(Not String.IsNullOrEmpty(Txt_StockIDSeachToFillFeilds.Text), Txt_StockIDSeachToFillFeilds.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@StockType", If(Txt_StockType.SelectedIndex <> -1, Txt_StockType.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@StockPrice", If(Not String.IsNullOrEmpty(Txt_StockPrice.Text), Txt_StockPrice.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@StockCategory", If(Txt_StockCategory.SelectedIndex <> -1, Txt_StockCategory.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@AvailableQuantity", If(Not String.IsNullOrEmpty(Txt_StockQuantity.Text), Txt_StockQuantity.Text, DBNull.Value))

                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Stock data for " & Txt_StockName.Text & " has been updated successfully.")

                ' Clear controls...
                ClearAllStockControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "StockData", DataGridView3)

    End Sub

    Private Sub Guna2GradientButton9_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton9.Click
        ClearAllStockControls()
    End Sub

    Private Sub Guna2GradientButton30_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton30.Click
        If Not String.IsNullOrEmpty(Txt_StockIDSeachToFillFeilds.Text) Then

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM StockData WHERE StockID = @StockID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@StockID", Txt_StockIDSeachToFillFeilds.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_StockName.Text = reader(1) ' Name
                        Txt_StockType.Text = reader(2) ' Stock Type
                        Txt_StockPrice.Text = reader(3) ' Stock Price
                        Txt_StockCategory.Text = reader(4) ' Stock Category
                        Txt_StockQuantity.Text = reader(5) ' Stock quantity

                    Else
                        MessageBox.Show("Stock ID number not found.")
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Please enter a Stock ID number.")
        End If

        Populatedvg(con, "StockData", DataGridView3)
    End Sub

    Private Sub Guna2GradientButton13_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton13.Click
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If Not String.IsNullOrEmpty(Txt_StockNameSeachToFillDGV.Text) Then
                DataGridView3.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = "StockName"
                Dim tablename As String = "StockData"

                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", Txt_StockNameSeachToFillDGV.Text)
                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView3.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the Stock Name To Search")
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Txt_StockNameSeachToFillDGV_TextChanged(sender As Object, e As EventArgs) Handles Txt_StockNameSeachToFillDGV.TextChanged
        If Txt_StockNameSeachToFillDGV.Text = "" Then
            Populatedvg(con, "StockData", DataGridView3)
        End If
    End Sub

    Private Sub TextBox26_TextChanged(sender As Object, e As EventArgs) Handles Txt_PurchaseStockName.TextChanged
        If Txt_PurchaseStockName.Text = "" Then
            Txt_PurchaseStockPrice.Clear()
        Else

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM StockData WHERE StockName = @StockName"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@StockName", Txt_PurchaseStockName.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_PurchaseStockPrice.Text = reader(3) ' stock price
                        Txt_PurchaseCategory.Text = reader(4) ' stock price
                        TextBox3.Text = reader(5) ' stock price
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End If

    End Sub


    Private Sub TextBox32_TextChanged(sender As Object, e As EventArgs) Handles Txt_PurchaseStockPrice.TextChanged
        ' Check if both TextBoxes have non-empty and numeric values
        If Not String.IsNullOrEmpty(Txt_PurchaseStockPrice.Text) AndAlso Not String.IsNullOrEmpty(Txt_PurchaseQuantity.Text) Then
            Dim value1 As Decimal
            Dim value2 As Decimal

            ' Try to parse the text values to Decimal
            If Decimal.TryParse(Txt_PurchaseQuantity.Text, value1) AndAlso Decimal.TryParse(Txt_PurchaseStockPrice.Text, value2) Then
                ' Perform the multiplication and assign the result to TextBox42
                Txt_PurchaseSubTotal.Text = (value1 * value2).ToString()
            Else
                ' Handle cases where the entered values are not valid numbers
                MessageBox.Show("Please enter valid numeric values.")
            End If
        Else
            ' Handle cases where one or both TextBoxes are empty
            'clearox.Show("Both TextBoxes must have non-empty values.")
        End If
    End Sub



    Sub AssignTotalPriceToTextbox()
        If TextBox1.Text = "" Then
            TextBox1.Text = Txt_PurchaseSubTotal.Text
            'Txt_PurchaseStockPrice.Clear()
            'Txt_PurchaseQuantity.Clear()
            ' Txt_PurchaseStockName.Clear()
            ' Txt_PurchaseSubTotal.Clear()
        Else
            ' Check if both TextBoxes have non-empty and numeric values
            If Not String.IsNullOrEmpty(Txt_PurchaseSubTotal.Text) AndAlso Not String.IsNullOrEmpty(TextBox1.Text) Then
                Dim value1 As Decimal
                Dim value2 As Decimal

                ' Try to parse the text values to Decimal
                If Decimal.TryParse(Txt_PurchaseSubTotal.Text, value1) AndAlso Decimal.TryParse(TextBox1.Text, value2) Then
                    ' Perform the multiplication and assign the result to TextBox42
                    TextBox1.Text = (value1 + value2).ToString()
                    '  Txt_PurchaseStockPrice.Clear()
                    ' Txt_PurchaseQuantity.Clear()
                    '  Txt_PurchaseStockName.Clear()
                    ' Txt_PurchaseSubTotal.Clear()
                Else
                    ' Handle cases where the entered values are not valid numbers
                    MessageBox.Show("Please enter valid numeric values.")
                End If
            Else
                ' Handle cases where one or both TextBoxes are empty
                'MessageBox.Show("Both TextBoxes must have non-empty values.")
            End If
        End If
    End Sub
    Sub AssignTotalQuantityToTextbox()

        If Not String.IsNullOrEmpty(Txt_PurchaseQuantity.Text) Then
            If String.IsNullOrEmpty(Txt_PurchaseTotalQuantity.Text) Then
                Txt_PurchaseTotalQuantity.Text = Txt_PurchaseQuantity.Text
            Else
                ' Use Integer.TryParse to safely convert string to integer
                Dim currentQuantity As Integer = 0
                If Integer.TryParse(Txt_PurchaseQuantity.Text, currentQuantity) Then
                    Dim totalQuantity As Integer = 0
                    If Integer.TryParse(Txt_PurchaseTotalQuantity.Text, totalQuantity) Then
                        ' Accumulate the total quantity
                        Txt_PurchaseTotalQuantity.Text = (totalQuantity + currentQuantity).ToString()
                    Else
                        ' Handle conversion failure for total quantity
                        MessageBox.Show("Invalid value for total quantity.")
                    End If
                Else
                    ' Handle conversion failure for current quantity
                    MessageBox.Show("Invalid value for current quantity.")
                End If
            End If
        End If

    End Sub

    Private Sub TextBox25_TextChanged(sender As Object, e As EventArgs) Handles Txt_PurchaseQuantity.TextChanged
        ' Check if both TextBoxes have non-empty and numeric values
        If Not String.IsNullOrEmpty(Txt_PurchaseStockPrice.Text) AndAlso Not String.IsNullOrEmpty(Txt_PurchaseQuantity.Text) Then
            Dim value1 As Decimal
            Dim value2 As Decimal

            ' Try to parse the text values to Decimal
            If Decimal.TryParse(Txt_PurchaseQuantity.Text, value1) AndAlso Decimal.TryParse(Txt_PurchaseStockPrice.Text, value2) Then
                ' Perform the multiplication and assign the result to TextBox42
                Txt_PurchaseSubTotal.Text = (value1 * value2).ToString()
            Else
                ' Handle cases where the entered values are not valid numbers
                MessageBox.Show("Please enter valid numeric values.")
            End If
        Else
            ' Handle cases where one or both TextBoxes are empty
            'clearox.Show("Both TextBoxes must have non-empty values.")
        End If


    End Sub
    Private Function CreateAddcolumnToDatagridview() As DataTable
        Dim table As New DataTable("table")
        table.Columns.Add("InvoiceNo", Type.GetType("System.String"))
        table.Columns.Add("TradeType", Type.GetType("System.String"))
        table.Columns.Add("SpecificType", Type.GetType("System.String"))
        table.Columns.Add("Name", Type.GetType("System.String"))
        table.Columns.Add("SupID", Type.GetType("System.Int32"))
        table.Columns.Add("StockName", Type.GetType("System.String"))
        table.Columns.Add("Category", Type.GetType("System.String"))
        table.Columns.Add("AvailableQuantity", Type.GetType("System.Int32"))
        table.Columns.Add("Quantity", Type.GetType("System.Int32"))
        table.Columns.Add("Price", Type.GetType("System.Decimal"))
        table.Columns.Add("SubTotal", Type.GetType("System.Decimal"))
        table.Columns.Add("BillDate", Type.GetType("System.DateTime"))

        Return table
    End Function

    ' Use the returned DataTable to bind data to the DataGridView

    Private Function IsAllPurchaseFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_PurchaseInvoiceNo.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_PurchaseQuantity.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_PurchaseSubTotal.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_PurchaseSupplierID.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_PurchaseStockName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_PurchaseStockPrice.Text) OrElse
            Txt_PurchaseCategory.SelectedIndex = -1 OrElse
            Txt_PurchaseSupplierName.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllPurchaseControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_PurchaseInvoiceNo, Txt_PurchaseQuantity, Txt_PurchaseSubTotal, Txt_PurchaseSupplierID, Txt_PurchaseStockName, Txt_PurchaseStockPrice}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_PurchaseCategory.SelectedIndex = -1
        Txt_PurchaseSupplierName.SelectedIndex = -1
        DTP_PurchaseDate.Value = Date.Today
    End Sub
    ' Use the returned DataTable to bind data to the DataGridView
    Private Sub BindDataToDataGridView()
        ' Create the DataTable using the method
        Dim data As DataTable = CreateAddcolumnToDatagridview()

        ' Assuming DataGridView4 is the name of your DataGridView control
        DataGridView4.DataSource = data
    End Sub
    Private table As DataTable ' Declare globally


    Private Sub Guna2GradientButton14_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton14.Click
        If IsAllPurchaseFieldsFilled() Then
            AssignTotalPriceToTextbox()
            AssignTotalQuantityToTextbox()

            ' Create a new row and populate it with TextBox values
            Dim row As DataRow = table.NewRow()
            row("InvoiceNo") = Txt_PurchaseInvoiceNo.Text
            row("TradeType") = "Purchase"
            row("SpecificType") = "Supplier"
            ' Assuming Txt_PurchaseSupplierName is a ComboBox bound to a DataTable or DataView
            If Txt_PurchaseSupplierName.SelectedItem IsNot Nothing Then
                ' Check if the selected item is a DataRowView
                If TypeOf Txt_PurchaseSupplierName.SelectedItem Is DataRowView Then
                    ' Retrieve the DataRowView
                    Dim selectedDataRowView As DataRowView = CType(Txt_PurchaseSupplierName.SelectedItem, DataRowView)

                    ' Access the specific column value (e.g., assuming "SupplierName" column)
                    Dim selectedName As String = selectedDataRowView("SupplierName").ToString()

                    ' Assign the value to the DataGridView row ("Name" column)
                    row("Name") = selectedName
                Else
                    MessageBox.Show("Selected item is not a DataRowView.")
                End If
            Else
                MessageBox.Show("Please select a supplier name.")
            End If
            row("SupID") = Txt_PurchaseSupplierID.Text
            row("StockName") = Txt_PurchaseStockName.Text
            If Txt_PurchaseCategory.SelectedItem IsNot Nothing Then
                Dim selectedCategory As String = Txt_PurchaseCategory.SelectedItem.ToString()
                row("Category") = selectedCategory
            Else
                MessageBox.Show("Please select a category.")
            End If
            row("AvailableQuantity") = Convert.ToInt32(TextBox3.Text)
            row("Quantity") = Convert.ToInt32(Txt_PurchaseQuantity.Text)
            row("Price") = Txt_PurchaseStockPrice.Text
            row("SubTotal") = Txt_PurchaseSubTotal.Text
            row("BillDate") = DTP_PurchaseDate.Value.Date

            ' Add the row to the DataTable
            table.Rows.Add(row)
            ' Refresh the DataGridView to reflect changes
            DataGridView4.DataSource = Nothing
            DataGridView4.DataSource = table
            ' Clear TextBoxes after adding data to the DataTable
            Dim controlsToClear() As Control = {Txt_PurchaseQuantity, Txt_PurchaseSubTotal, Txt_PurchaseStockName, Txt_PurchaseStockPrice}
            For Each control As Control In controlsToClear
                control.Text = String.Empty
            Next
            Txt_PurchaseCategory.SelectedIndex = -1
        Else
            MessageBox.Show("Please fill in all TextBoxes.")
        End If
    End Sub

    Private Sub Guna2GradientButton15_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton15.Click
        ClearAllPurchaseControls()
        TextBox1.Clear()
    End Sub

    Private Sub Guna2GradientButton17_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton17.Click
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
        Try

            For Each row As DataGridViewRow In DataGridView5.Rows
                Dim avlbQnty As Integer
                Dim purchaseQnty As Integer

                If Integer.TryParse(row.Cells("AvailableQuantity").Value.ToString(), avlbQnty) AndAlso
           Integer.TryParse(row.Cells("Quantity").Value.ToString(), purchaseQnty) Then
                    Dim newQnty As Integer = avlbQnty + purchaseQnty

                    Using cmd As New SqlCommand("UPDATE StockData SET AvailableQuantity = @AvailableQuantity WHERE StockName = @StockName", con)
                        ' Set parameter values...'
                        cmd.Parameters.AddWithValue("@AvailableQuantity", newQnty)
                        cmd.Parameters.AddWithValue("@StockName", If(row.Cells("StockName").Value IsNot Nothing, row.Cells("StockName").Value.ToString(), DBNull.Value))

                        con.Open()
                        cmd.ExecuteNonQuery()
                        con.Close()
                    End Using
                Else
                    ' Handle parsing failure
                    MessageBox.Show("Failed to parse quantity values.")
                End If
            Next


            For Each row As DataGridViewRow In DataGridView4.Rows
                Using cmd As New SqlCommand("INSERT INTO PurchaseOrSaleData (InvoiceNo, TradeType, SpecificType, Name, IDorPhoneNo, StockName, Category, Quantity, Price, SubTotal, BillDate) " &
                                    "VALUES (@InvoiceNo, @TradeType, @SpecificType, @Name, @IDorPhoneNo, @StockName, @Category, @Quantity, @Price, @SubTotal, @BillDate)", con)
                    ' Set parameter values...'
                    cmd.Parameters.AddWithValue("@InvoiceNo", If(row.Cells("InvoiceNo").Value IsNot Nothing, row.Cells("InvoiceNo").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@TradeType", If(row.Cells("TradeType").Value IsNot Nothing, row.Cells("TradeType").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@SpecificType", If(row.Cells("SpecificType").Value IsNot Nothing, row.Cells("SpecificType").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Name", If(row.Cells("Name").Value IsNot Nothing, row.Cells("Name").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@IDorPhoneNo", If(row.Cells("SupID").Value IsNot Nothing, row.Cells("SupID").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@StockName", If(row.Cells("StockName").Value IsNot Nothing, row.Cells("StockName").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Category", If(row.Cells("Category").Value IsNot Nothing, row.Cells("Category").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Quantity", If(row.Cells("Quantity").Value IsNot Nothing, row.Cells("Quantity").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Price", If(row.Cells("Price").Value IsNot Nothing, row.Cells("Price").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@SubTotal", If(row.Cells("SubTotal").Value IsNot Nothing, row.Cells("SubTotal").Value, DBNull.Value))
                    cmd.Parameters.AddWithValue("@BillDate", If(row.Cells("BillDate").Value IsNot Nothing, row.Cells("BillDate").Value, DBNull.Value))
                    ' Execute the query...'
                    ' Add other parameters in a similar manner...

                    ' Open connection and execute the query
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()
                End Using
            Next
            Dim query As String = "INSERT INTO BillData (InvoiceNo, TradeType, SpecificType, Name, IDorPhoneNo, TotalQuantity, TotalPrice, BillDate) " &
                        "VALUES (@InvoiceNo, @TradeType, @SpecificType, @Name, @IDorPhoneNo, @TotalQuantity,@TotalPrice, @BillDate)"
            Using Billcmd As New SqlCommand(query, con)
                ' Set parameter values...'
                Billcmd.Parameters.AddWithValue("@InvoiceNo", If(Not String.IsNullOrEmpty(Txt_PurchaseInvoiceNo.Text), Txt_PurchaseInvoiceNo.Text, DBNull.Value))
                Billcmd.Parameters.AddWithValue("@TradeType", "Purchase")
                Billcmd.Parameters.AddWithValue("@SpecificType", "Supplier")
                Billcmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_PurchaseSupplierName.Text), Txt_PurchaseSupplierName.Text, DBNull.Value))
                Billcmd.Parameters.AddWithValue("@IDorPhoneNo", If(Not String.IsNullOrEmpty(Txt_PurchaseSupplierID.Text), Txt_PurchaseSupplierID.Text, DBNull.Value))
                Billcmd.Parameters.AddWithValue("@TotalQuantity", If(Not String.IsNullOrEmpty(Txt_PurchaseTotalQuantity.Text), Txt_PurchaseTotalQuantity.Text, DBNull.Value))
                Billcmd.Parameters.AddWithValue("@TotalPrice", If(Not String.IsNullOrEmpty(TextBox1.Text), TextBox1.Text, DBNull.Value))
                Billcmd.Parameters.AddWithValue("@BillDate", DTP_PurchaseDate.Value.Date)
                con.Open()
                Billcmd.ExecuteNonQuery()
                con.Close()
            End Using
            MsgBox("Purchase data for Invoice No :" & Txt_PurchaseInvoiceNo.Text & " has been updated successfully.")

        Catch ex As Exception
            ' Display specific error message...
            MsgBox("Error: " & ex.Message)
        End Try
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
        ' Finally, repopulate the DataGridView and generate a unique invoice number
        Populatedvg(con, "PurchaseOrSaleData", DataGridView6)
        Txt_PurchaseInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        Txt_SellInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        DataGridView4.Columns.Clear()
        displaygenratedIDnumber(con, "BillData")

    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles Txt_SellStockName.TextChanged
        If Txt_SellStockName.Text = "" Then
            Txt_SellStockPrice.Clear()
            Txt_SellCategory.SelectedIndex = -1
        Else

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM StockData WHERE StockName = @StockName"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@StockName", Txt_SellStockName.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_SellStockPrice.Text = reader(3) ' stock price
                        Txt_SellAvlbQunty.Text = reader(5) ' stock price
                        Txt_SellCategory.Text = reader(4) ' stock Category
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End If
    End Sub

    Private Sub TextBox38_TextChanged(sender As Object, e As EventArgs) Handles Txt_SellQuantity.TextChanged
        ' Check if both TextBoxes have non-empty and numeric values
        If Not String.IsNullOrEmpty(Txt_SellStockPrice.Text) AndAlso Not String.IsNullOrEmpty(Txt_SellQuantity.Text) Then
            Dim value1 As Decimal
            Dim value2 As Decimal

            ' Try to parse the text values to Decimal
            If Decimal.TryParse(Txt_SellStockPrice.Text, value1) AndAlso Decimal.TryParse(Txt_SellQuantity.Text, value2) Then
                ' Perform the multiplication and assign the result to TextBox42
                Txt_SellSubTotalPrice.Text = (value1 * value2).ToString()
            Else
                ' Handle cases where the entered values are not valid numbers
                MessageBox.Show("Please enter valid numeric values.")
            End If
        Else
            ' Handle cases where one or both TextBoxes are empty
            'clearox.Show("Both TextBoxes must have non-empty values.")
        End If

    End Sub

    Private Sub TextBox41_TextChanged(sender As Object, e As EventArgs) Handles Txt_SellStockPrice.TextChanged
        ' Check if both TextBoxes have non-empty and numeric values
        If Not String.IsNullOrEmpty(Txt_SellStockPrice.Text) AndAlso Not String.IsNullOrEmpty(Txt_SellQuantity.Text) Then
            Dim value1 As Decimal
            Dim value2 As Decimal

            ' Try to parse the text values to Decimal
            If Decimal.TryParse(Txt_SellStockPrice.Text, value1) AndAlso Decimal.TryParse(Txt_SellQuantity.Text, value2) Then
                ' Perform the multiplication and assign the result to TextBox42
                Txt_SellSubTotalPrice.Text = (value1 * value2).ToString()
            Else
                ' Handle cases where the entered values are not valid numbers
                MessageBox.Show("Please enter valid numeric values.")
            End If
        Else
            ' Handle cases where one or both TextBoxes are empty
            'clearox.Show("Both TextBoxes must have non-empty values.")
        End If
    End Sub


    Sub AssignTotalPriceToSellTextbox()
        If Txt_SellTotalPrice.Text = "" Then
            Txt_SellTotalPrice.Text = Txt_SellSubTotalPrice.Text
        Else
            ' Check if both TextBoxes have non-empty and numeric values
            If Not String.IsNullOrEmpty(Txt_SellSubTotalPrice.Text) AndAlso Not String.IsNullOrEmpty(Txt_SellTotalPrice.Text) Then
                Dim value1 As Decimal
                Dim value2 As Decimal

                ' Try to parse the text values to Decimal
                If Decimal.TryParse(Txt_SellSubTotalPrice.Text, value1) AndAlso Decimal.TryParse(Txt_SellTotalPrice.Text, value2) Then
                    ' Perform the multiplication and assign the result to TextBox42
                    Txt_SellTotalPrice.Text = (value1 + value2).ToString()
                Else
                    ' Handle cases where the entered values are not valid numbers
                    MessageBox.Show("Please enter valid numeric values.")
                End If
            Else
                ' Handle cases where one or both TextBoxes are empty
                'MessageBox.Show("Both TextBoxes must have non-empty values.")
            End If
        End If
    End Sub
    Sub AssignTotalQuantityToSellTextbox()

        If Not String.IsNullOrEmpty(Txt_SellQuantity.Text) Then
            If String.IsNullOrEmpty(Txt_SellTotalQuantity.Text) Then
                Txt_SellTotalQuantity.Text = Txt_SellQuantity.Text
            Else
                ' Use Integer.TryParse to safely convert string to integer
                Dim currentQuantity As Integer = 0
                If Integer.TryParse(Txt_SellQuantity.Text, currentQuantity) Then
                    Dim totalQuantity As Integer = 0
                    If Integer.TryParse(Txt_SellTotalQuantity.Text, totalQuantity) Then
                        ' Accumulate the total quantity
                        Txt_SellTotalQuantity.Text = (totalQuantity + currentQuantity).ToString()
                    Else
                        ' Handle conversion failure for total quantity
                        MessageBox.Show("Invalid value for total quantity.")
                    End If
                Else
                    ' Handle conversion failure for current quantity
                    MessageBox.Show("Invalid value for current quantity.")
                End If
            End If
        End If

    End Sub

    Private Sub Guna2GradientButton23_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton23.Click
        Dim Qnty As Integer = Convert.ToInt32(Txt_SellAvlbQunty.Text)
        If Qnty < 6 And Qnty > 0 Then
            MsgBox("This stock is running low; please reorder this stock")
            Exit Sub
        End If
        If IsAllSellFieldsFilled() Then
            AssignTotalPriceToSellTextbox()
            AssignTotalQuantityToSellTextbox()

            ' Create a new row and populate it with TextBox values
            Dim row As DataRow = Selltable.NewRow()
            row("InvoiceNo") = Txt_SellInvoiceNo.Text
            row("TradeType") = "Sale"
            row("SpecificType") = "Customer"
            row("Name") = Txt_SellCusName.Text
            row("CusNumber") = Txt_SellCusNumber.Text
            row("StockName") = Txt_SellStockName.Text
            If Txt_SellCategory.SelectedItem IsNot Nothing Then
                Dim selectedCategory As String = Txt_SellCategory.SelectedItem.ToString()
                row("Category") = selectedCategory
            Else
                MessageBox.Show("Please select a category.")
            End If
            row("Quantity") = Convert.ToInt32(Txt_SellQuantity.Text)
            row("AvlbQuantity") = Convert.ToInt32(Txt_SellAvlbQunty.Text)
            row("Price") = Txt_SellStockPrice.Text
            row("SubTotal") = Txt_SellSubTotalPrice.Text
            row("BillDate") = DTP_SellBillDate.Value.Date

            ' Add the row to the DataTable
            Selltable.Rows.Add(row)
            ' Refresh the DataGridView to reflect changes
            DataGridView5.DataSource = Nothing
            DataGridView5.DataSource = Selltable
            ' Clear TextBoxes after adding data to the DataTable
            Dim controlsToClear() As Control = {Txt_SellQuantity, Txt_SellSubTotalPrice, Txt_SellStockName, Txt_SellStockPrice, Txt_SellAvlbQunty}
            For Each control As Control In controlsToClear
                control.Text = String.Empty
            Next
            Txt_SellCategory.SelectedIndex = -1
        Else
            MessageBox.Show("Please fill in all TextBoxes.")
        End If
    End Sub


    Private Function CreateAddcolumnToSellDatagridview() As DataTable
        Dim table As New DataTable("table")
        table.Columns.Add("InvoiceNo", Type.GetType("System.String"))
        table.Columns.Add("TradeType", Type.GetType("System.String"))
        table.Columns.Add("SpecificType", Type.GetType("System.String"))
        table.Columns.Add("Name", Type.GetType("System.String"))
        table.Columns.Add("CusNumber", Type.GetType("System.String"))
        table.Columns.Add("StockName", Type.GetType("System.String"))
        table.Columns.Add("Category", Type.GetType("System.String"))
        table.Columns.Add("Quantity", Type.GetType("System.Int32"))
        table.Columns.Add("AvlbQuantity", Type.GetType("System.Int32"))
        table.Columns.Add("Price", Type.GetType("System.Decimal"))
        table.Columns.Add("SubTotal", Type.GetType("System.Decimal"))
        table.Columns.Add("BillDate", Type.GetType("System.DateTime"))

        Return table
    End Function

    ' Use the returned DataTable to bind data to the DataGridView

    Private Function IsAllSellFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_SellInvoiceNo.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SellQuantity.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SellSubTotalPrice.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SellCusNumber.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SellCusName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SellStockName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_SellStockPrice.Text) OrElse
            Txt_SellCategory.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllSellControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_SellInvoiceNo, Txt_SellQuantity, Txt_SellSubTotalPrice, Txt_SellCusNumber, Txt_SellCusName, Txt_SellStockName, Txt_SellStockPrice}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_SellCategory.SelectedIndex = -1

        DTP_SellBillDate.Value = Date.Today
    End Sub
    ' Use the returned DataTable to bind data to the DataGridView
    Private Sub BindDataToSellDataGridView()
        ' Create the DataTable using the method
        Dim data As DataTable = CreateAddcolumnToSellDatagridview()

        ' Assuming DataGridView4 is the name of your DataGridView control
        DataGridView4.DataSource = data
    End Sub
    Private Selltable As DataTable ' Declare globally

    ' Your code accessing DataGridView5
    Private Sub Guna2GradientButton20_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton20.Click
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
        If DataGridView5 IsNot Nothing AndAlso DataGridView5.Rows.Count > 0 Then

            Try

                For Each row As DataGridViewRow In DataGridView5.Rows
                    Dim avlbQnty As Integer
                    Dim sellQnty As Integer

                    If Integer.TryParse(row.Cells("AvlbQuantity").Value.ToString(), avlbQnty) AndAlso
           Integer.TryParse(row.Cells("Quantity").Value.ToString(), sellQnty) Then
                        Dim newQnty As Integer = avlbQnty - sellQnty

                        Using cmd As New SqlCommand("UPDATE StockData SET AvailableQuantity = @AvailableQuantity WHERE StockName = @StockName", con)
                            ' Set parameter values...'
                            cmd.Parameters.AddWithValue("@AvailableQuantity", newQnty)
                            cmd.Parameters.AddWithValue("@StockName", If(row.Cells("StockName").Value IsNot Nothing, row.Cells("StockName").Value.ToString(), DBNull.Value))

                            con.Open()
                            cmd.ExecuteNonQuery()
                            con.Close()
                        End Using
                    Else
                        ' Handle parsing failure
                        MessageBox.Show("Failed to parse quantity values.")
                    End If
                Next


                For Each row As DataGridViewRow In DataGridView5.Rows
                    Using cmd As New SqlCommand("INSERT INTO PurchaseOrSaleData (InvoiceNo, TradeType, SpecificType, Name, IDorPhoneNo, StockName, Category, Quantity, Price, SubTotal, BillDate) " &
                                        "VALUES (@InvoiceNo, @TradeType, @SpecificType, @Name, @IDorPhoneNo, @StockName, @Category, @Quantity, @Price, @SubTotal, @BillDate)", con)
                        ' Set parameter values...'

                        cmd.Parameters.AddWithValue("@InvoiceNo", If(row.Cells("InvoiceNo").Value IsNot Nothing, row.Cells("InvoiceNo").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@TradeType", If(row.Cells("TradeType").Value IsNot Nothing, row.Cells("TradeType").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@SpecificType", If(row.Cells("SpecificType").Value IsNot Nothing, row.Cells("SpecificType").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@Name", If(row.Cells("Name").Value IsNot Nothing, row.Cells("Name").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@IDorPhoneNo", If(row.Cells("CusNumber").Value IsNot Nothing, row.Cells("CusNumber").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@StockName", If(row.Cells("StockName").Value IsNot Nothing, row.Cells("StockName").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@Category", If(row.Cells("Category").Value IsNot Nothing, row.Cells("Category").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@Quantity", If(row.Cells("Quantity").Value IsNot Nothing, row.Cells("Quantity").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@Price", If(row.Cells("Price").Value IsNot Nothing, row.Cells("Price").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@SubTotal", If(row.Cells("SubTotal").Value IsNot Nothing, row.Cells("SubTotal").Value, DBNull.Value))
                        cmd.Parameters.AddWithValue("@BillDate", If(row.Cells("BillDate").Value IsNot Nothing, row.Cells("BillDate").Value, DBNull.Value))
                        ' Execute the query...'
                        ' Add other parameters in a similar manner...

                        ' Open connection and execute the query
                        con.Open()
                        cmd.ExecuteNonQuery()
                        con.Close()
                    End Using
                Next

                Dim query As String = "INSERT INTO BillData (InvoiceNo, TradeType, SpecificType, Name, IDorPhoneNo, TotalQuantity, TotalPrice, BillDate) " &
                            "VALUES (@InvoiceNo, @TradeType, @SpecificType, @Name, @IDorPhoneNo, @TotalQuantity,@TotalPrice, @BillDate)"
                Using Billcmd As New SqlCommand(query, con)
                    ' Set parameter values...'
                    Billcmd.Parameters.AddWithValue("@InvoiceNo", If(Not String.IsNullOrEmpty(Txt_SellInvoiceNo.Text), Txt_SellInvoiceNo.Text, DBNull.Value))
                    Billcmd.Parameters.AddWithValue("@TradeType", "Sale")
                    Billcmd.Parameters.AddWithValue("@SpecificType", "Customer")
                    Billcmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_SellCusName.Text), Txt_SellCusName.Text, DBNull.Value))
                    Billcmd.Parameters.AddWithValue("@IDorPhoneNo", If(Not String.IsNullOrEmpty(Txt_SellCusNumber.Text), Txt_SellCusNumber.Text, DBNull.Value))
                    Billcmd.Parameters.AddWithValue("@TotalQuantity", If(Not String.IsNullOrEmpty(Txt_SellTotalQuantity.Text), Txt_SellTotalQuantity.Text, DBNull.Value))
                    Billcmd.Parameters.AddWithValue("@TotalPrice", If(Not String.IsNullOrEmpty(Txt_SellTotalPrice.Text), Txt_SellTotalPrice.Text, DBNull.Value))
                    Billcmd.Parameters.AddWithValue("@BillDate", DTP_SellBillDate.Value.Date)
                    con.Open()
                    Billcmd.ExecuteNonQuery()
                    con.Close()
                End Using
                MsgBox("Purchase data for Invoice No :" & Txt_SellInvoiceNo.Text & " has been updated successfully.")
                ClearAllSellControls()
                Txt_SellTotalQuantity.Clear()

            Catch ex As Exception
                ' Display specific error message...
                MsgBox("Error: " & ex.Message)
            End Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End If

        ' Finally, repopulate the DataGridView and generate a unique invoice number
        Populatedvg(con, "PurchaseOrSaleData", DataGridView6)
        Txt_SellInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        Txt_PurchaseInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        DataGridView5.Columns.Clear()
        displaygenratedIDnumber(con, "BillData")

    End Sub

    Private Sub Guna2GradientButton18_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton18.Click
        ClearAllSellControls()
        Txt_SellTotalPrice.Clear()
        DataGridView5.Columns.Clear()
        Txt_SellTotalQuantity.Clear()
    End Sub



    Sub displaygenratedIDnumber(con As SqlConnection, tblname As String)
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
        con.Open()
        Dim query As String = "SELECT IDENT_CURRENT ('" & tblname & "') AS GENRATEDNUMBER"
        Dim Cmd As New SqlCommand(query, con) 'new
        Dim result As Object = Cmd.ExecuteScalar()

        If result IsNot DBNull.Value Then
            Dim genratednumber As Integer = Convert.ToInt32(result)
            Label41.Text = (genratednumber + 1).ToString()
            Label44.Text = (genratednumber + 1).ToString()
        End If
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
    End Sub

    Private Sub Guna2GradientButton25_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton25.Click
        Try
            If Not String.IsNullOrEmpty(TextBox44.Text) Then
                DataGridView6.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = ""

                If ComboBox1.SelectedItem IsNot Nothing Then
                    If ComboBox1.SelectedIndex = 0 Then
                        Colmntype = "InvoiceNo"
                    ElseIf ComboBox1.SelectedIndex = 1 Then
                        Colmntype = "Name"
                    ElseIf ComboBox1.SelectedIndex = 2 Then
                        Colmntype = "StockName"
                    ElseIf ComboBox1.SelectedIndex = 3 Then
                        Colmntype = "Category"
                    ElseIf ComboBox1.SelectedIndex = 4 Then
                        Colmntype = "IDorPhoneNo"
                    Else
                        MsgBox("Select Type You want to Search")
                        Exit Sub
                    End If
                End If


                Dim tablename As String = "PurchaseOrSaleData"

                If Radio_PAndSSales.Checked = True Then
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition AND TradeType = 'Sale' AND BillDate BETWEEN @StartDate AND @EndDate "
                ElseIf Radio_PAndSPurchase.Checked = True Then
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition AND TradeType = 'Purchase' AND BillDate BETWEEN @StartDate AND @EndDate "
                ElseIf Radio_PAndSBoth.Checked = True Then
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"
                    If DTP_PAndSStrtDate.Value.Date <> DateTime.MinValue AndAlso DTP_PAndSEndDate.Value.Date <> DateTime.MinValue Then
                        sql &= " AND BillDate BETWEEN @StartDate AND @EndDate"
                    End If
                Else
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"
                End If

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox44.Text)
                adapter.SelectCommand.Parameters.AddWithValue("@StartDate", DTP_PAndSStrtDate.Value.Date)
                adapter.SelectCommand.Parameters.AddWithValue("@EndDate", DTP_PAndSEndDate.Value.Date)

                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView6.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the data For " & ComboBox1.SelectedItem.ToString)
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            con.Open()

            Dim Colmntype As String = ""
            Dim tablename As String = "PurchaseOrSaleData"

            If ComboBox1.SelectedItem IsNot Nothing Then
                If ComboBox1.SelectedIndex = 0 Then
                    Colmntype = "InvoiceNo"
                ElseIf ComboBox1.SelectedIndex = 1 Then
                    Colmntype = "Name"
                ElseIf ComboBox1.SelectedIndex = 2 Then
                    Colmntype = "StockName"
                ElseIf ComboBox1.SelectedIndex = 3 Then
                    Colmntype = "Category"
                ElseIf ComboBox1.SelectedIndex = 4 Then
                    Colmntype = "IDorPhoneNo"
                Else
                    MsgBox("Select Type You want to Search")
                    Exit Sub
                End If
            End If

            Dim Query As String = "SELECT " & Colmntype & " FROM " & tablename
            Dim Cmd As New SqlCommand(Query, con)
            Dim reader As SqlDataReader = Cmd.ExecuteReader()
            Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

            While reader.Read
                ' Check if the column index is valid
                If Not reader.IsDBNull(0) Then
                    ElementsToSuggest.Add(reader.GetString(0)) ' Use GetString to retrieve the column's value as a string
                End If
            End While

            TextBox44.AutoCompleteCustomSource = ElementsToSuggest
        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2GradientButton21_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton21.Click
        DataGridView6.Columns.Clear()
        con.Open()

        Dim sql As String = ""
        Dim tablename As String = "PurchaseOrSaleData"
        If Radio_PAndSSales.Checked = True Then
            sql = "SELECT * FROM " & tablename & " WHERE TradeType = 'Sale' AND BillDate BETWEEN @StartDate AND @EndDate "
        ElseIf Radio_PAndSSales.Checked = True Then
            sql = "SELECT *  FROM " & tablename & " WHERE TradeType = 'Purchase' AND BillDate BETWEEN @StartDate AND @EndDate "
        Else
            sql = "SELECT * FROM " & tablename & " WHERE BillDate BETWEEN @StartDate AND @EndDate "
        End If
        Dim adapter As SqlDataAdapter
        adapter = New SqlDataAdapter(sql, con)
        adapter.SelectCommand.Parameters.AddWithValue("@StartDate", DTP_PAndSStrtDate.Value.Date)
        adapter.SelectCommand.Parameters.AddWithValue("@EndDate", DTP_PAndSEndDate.Value.Date)
        Dim builder As SqlCommandBuilder
        builder = New SqlCommandBuilder(adapter)
        Dim ds As DataSet
        ds = New DataSet
        adapter.Fill(ds)
        DataGridView6.DataSource = ds.Tables(0)
        con.Close()
    End Sub

    Sub DisplaySelectedPurchaseAndSale()
        If Radio_PAndSSales.Checked = True Then
            PopulateSalesAndPurchaseDGV(con, "PurchaseOrSaleData", DataGridView6, "Sale")
            DataGridView6.Refresh()
        ElseIf Radio_PAndSPurchase.Checked = True Then
            PopulateSalesAndPurchaseDGV(con, "PurchaseOrSaleData", DataGridView6, "Purchase")
            DataGridView6.Refresh()
        Else
            Populatedvg(con, "PurchaseOrSaleData", DataGridView6)
        End If
    End Sub

    Private Sub Radio_PAndSSales_CheckedChanged(sender As Object, e As EventArgs) Handles Radio_PAndSSales.CheckedChanged
        DisplaySelectedPurchaseAndSale()
    End Sub

    Private Sub Radio_PAndSPurchase_CheckedChanged(sender As Object, e As EventArgs) Handles Radio_PAndSPurchase.CheckedChanged
        DisplaySelectedPurchaseAndSale()
    End Sub

    Private Sub Radio_PAndSBoth_CheckedChanged(sender As Object, e As EventArgs) Handles Radio_PAndSBoth.CheckedChanged
        DisplaySelectedPurchaseAndSale()
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Try
            con.Open()

            Dim Colmntype As String = ""
            Dim tablename As String = "BillData"

            If ComboBox2.SelectedItem IsNot Nothing Then
                If ComboBox2.SelectedIndex = 0 Then
                    Colmntype = "BillNo"
                ElseIf ComboBox2.SelectedIndex = 1 Then
                    Colmntype = "InvoiceNo"
                ElseIf ComboBox2.SelectedIndex = 2 Then
                    Colmntype = "Name"
                ElseIf ComboBox2.SelectedIndex = 3 Then
                    Colmntype = "StockName"
                ElseIf ComboBox2.SelectedIndex = 4 Then
                    Colmntype = "Category"
                ElseIf ComboBox2.SelectedIndex = 5 Then
                    Colmntype = "IDorPhoneNo"
                Else
                    MsgBox("Select Type You want to Search")
                    Exit Sub
                End If
            End If

            Dim Query As String = "SELECT " & Colmntype & " FROM " & tablename
            Dim Cmd As New SqlCommand(Query, con)
            Dim reader As SqlDataReader = Cmd.ExecuteReader()
            Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

            While reader.Read
                ' Check if the column index is valid
                If Not reader.IsDBNull(0) Then
                    ' Handle different data types (string and Int32)
                    If reader.GetFieldType(0) Is GetType(String) Then
                        ElementsToSuggest.Add(reader.GetString(0)) ' Use GetString to retrieve the column's value as a string
                    ElseIf reader.GetFieldType(0) Is GetType(Integer) Then
                        ElementsToSuggest.Add(reader.GetInt32(0).ToString()) ' Convert Int32 to string and add to AutoCompleteStringCollection
                    End If
                End If
            End While

            TextBox2.AutoCompleteCustomSource = ElementsToSuggest
        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2GradientButton16_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton16.Click
        Try
            If Not String.IsNullOrEmpty(TextBox2.Text) Then
                DataGridView7.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = ""

                If ComboBox2.SelectedItem IsNot Nothing Then
                    If ComboBox2.SelectedIndex = 0 Then
                        Colmntype = "BillNo"
                    ElseIf ComboBox2.SelectedIndex = 1 Then
                        Colmntype = "InvoiceNo"
                    ElseIf ComboBox2.SelectedIndex = 2 Then
                        Colmntype = "Name"
                    ElseIf ComboBox2.SelectedIndex = 3 Then
                        Colmntype = "StockName"
                    ElseIf ComboBox2.SelectedIndex = 4 Then
                        Colmntype = "Category"
                    ElseIf ComboBox2.SelectedIndex = 5 Then
                        Colmntype = "IDorPhoneNo"
                    Else
                        MsgBox("Select Type You want to Search")
                        Exit Sub
                    End If
                End If


                Dim tablename As String = "BillData"

                If Radio_PAndSSales.Checked = True Then
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition AND TradeType = 'Sale' AND BillDate BETWEEN @StartDate AND @EndDate "
                ElseIf Radio_PAndSPurchase.Checked = True Then
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition AND TradeType = 'Purchase' AND BillDate BETWEEN @StartDate AND @EndDate "
                ElseIf Radio_PAndSBoth.Checked = True Then
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"
                    If DTP_BillStrtDate.Value.Date <> DateTime.MinValue AndAlso DTP_BillEndDate.Value.Date <> DateTime.MinValue Then
                        sql &= " AND BillDate BETWEEN @StartDate AND @EndDate"
                    End If
                Else
                    sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"
                End If

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox2.Text)
                adapter.SelectCommand.Parameters.AddWithValue("@StartDate", DTP_BillStrtDate.Value.Date)
                adapter.SelectCommand.Parameters.AddWithValue("@EndDate", DTP_BillEndDate.Value.Date)

                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView7.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the data For " & ComboBox2.SelectedItem.ToString)
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2GradientButton19_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton19.Click
        DataGridView7.Columns.Clear()
        con.Open()

        Dim sql As String = ""
        Dim tablename As String = "BillData"
        If Radio_BillSales.Checked = True Then
            sql = "SELECT * FROM " & tablename & " WHERE TradeType = 'Sale' AND BillDate BETWEEN @StartDate AND @EndDate "
        ElseIf Radio_billSales.Checked = True Then
            sql = "SELECT *  FROM " & tablename & " WHERE TradeType = 'Purchase' AND BillDate BETWEEN @StartDate AND @EndDate "
        Else
            sql = "SELECT * FROM " & tablename & " WHERE BillDate BETWEEN @StartDate AND @EndDate "
        End If
        Dim adapter As SqlDataAdapter
        adapter = New SqlDataAdapter(sql, con)
        adapter.SelectCommand.Parameters.AddWithValue("@StartDate", DTP_BillStrtDate.Value.Date)
        adapter.SelectCommand.Parameters.AddWithValue("@EndDate", DTP_BillEndDate.Value.Date)
        Dim builder As SqlCommandBuilder
        builder = New SqlCommandBuilder(adapter)
        Dim ds As DataSet
        ds = New DataSet
        adapter.Fill(ds)
        DataGridView7.DataSource = ds.Tables(0)
        con.Close()
    End Sub

    Sub DisplaySelectedBill()
        If Radio_BillSales.Checked = True Then
            PopulateBilDGV(con, "BillData", DataGridView7, "Sale")
            DataGridView6.Refresh()
        ElseIf Radio_BillPurchase.Checked = True Then
            PopulateBilDGV(con, "BillData", DataGridView7, "Purchase")
            DataGridView6.Refresh()
        Else
            Populatedvg(con, "BillData", DataGridView7)
        End If
    End Sub
    Private Sub Radio_BillSales_CheckedChanged(sender As Object, e As EventArgs) Handles Radio_BillSales.CheckedChanged
        DisplaySelectedBill()
    End Sub

    Private Sub Radio_BillPurchase_CheckedChanged(sender As Object, e As EventArgs) Handles Radio_BillPurchase.CheckedChanged
        DisplaySelectedBill()
    End Sub

    Private Sub Radio_BillBoth_CheckedChanged(sender As Object, e As EventArgs) Handles Radio_BillBoth.CheckedChanged
        DisplaySelectedBill()
    End Sub


    Sub loadTrustedSinceLabel()
        If Guna2TabControl1.SelectedTab Is TabPage1 Then
            Timer_TrustedScince.Start()
            Label86.Text = 0
        Else
            Timer_TrustedScince.Start()
            Label86.Text = 0
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer_TrustedScince.Tick

        Timer_TrustedScince.Interval = i
            Label86.Text = Label86.Text + 1
            If Label86.Text = lbltrusetedendYear Then
                Timer_TrustedScince.Stop()
            End If
            If Not Guna2TabControl1.SelectedTab Is TabPage1 Then
                If Timer_TrustedScince.Enabled = False Then
                    Timer_TrustedScince.Start()
                    Label86.Text = 0
                Else
                    Timer_TrustedScince.Start()
                    Label86.Text = 0
                End If
            End If


    End Sub

    Private Sub Guna2GradientButton31_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton31.Click
        Populatedvg(con, "EmployeeData", DataGridView1)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_EmployeeSearchIDForFeilds, "EmployeeID", "EmployeeData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_StockNameSeachToFillDGV, "StockName", "StockData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_PurchaseStockName, "StockName", "StockData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_SellStockName, "StockName", "StockData", 0)
        Populatedvg(con, "SupplierData", DataGridView2)
        FillPersonalDetailsInFeilds()
        FillcomboBox(con, Txt_PurchaseSupplierName, "SupplierData", "SupplierName")
        LoadItemsFromFile()
        Populatedvg(con, "StockData", DataGridView3)
        Populatedvg(con, "PurchaseOrSaleData", DataGridView6)
        Populatedvg(con, "BillData", DataGridView7)
        Txt_PurchaseInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        Txt_SellInvoiceNo.Text = GenerateUniqueInvoiceNo(con, "PurchaseOrSaleData")
        BindDataToDataGridView()
        ' Call the function to create the columns once during the form load
        table = CreateAddcolumnToDatagridview()
        DataGridView4.DataSource = table
        Selltable = CreateAddcolumnToSellDatagridview()
        DataGridView5.DataSource = Selltable
        Txt_PurchaseTotalQuantity.Clear()
        Txt_SellTotalQuantity.Clear()
        DataGridView4.ForeColor = Color.Black
        ReSizeDatagridViews()
        Populatedvg(con, "PurchaseOrSaleData", DataGridView6)
        displaygenratedIDnumber(con, "BillData")
        loadTrustedSinceLabel()
        DastBordTotalQnty = stocksoldquantity()
        DastBordTotalprice = stocksoldtotalAmount()
        DastBordTotalAvailableQuntity = stocksoldtotalAvaiableQuntity()
        DastBordTotalemployees = RetrieveTotalEmployee()
        DastBordTotalDealers = RetrieveTotalDealers()
        loadTotalqntyLabel()
        loadTotalPriceLabel()
        loadTotalAvaiableQuntityLabel()
        loadTotalEmployeeLabel()
        loadTotalDealersLabel()
    End Sub

    Function stocksoldquantity() As Integer
        Dim totalQty As Integer
        Try
            con.Open()

            Dim query As String = "SELECT SUM(TotalQuantity) FROM BillData"
            Using cmd As New SqlCommand(query, con)
                ' Execute the query and get the total quantity
                Dim totalQuantity As Object = cmd.ExecuteScalar()

                ' Check if the query returned a value
                If totalQuantity IsNot Nothing AndAlso Not IsDBNull(totalQuantity) Then
                    ' Handle the retrieved total quantity (convert it to the appropriate data type if needed)
                    totalQty = Convert.ToInt32(totalQuantity)
                    ' ... do something with totalQty
                Else
                    ' Handle the case when no value is returned from the query
                    MessageBox.Show("No quantity data found.")
                End If
            End Using
        Catch ex As Exception
            ' Handle the exception (e.g., display an error message)
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
        Return totalQty
    End Function


    Sub loadTotalqntyLabel()
        If Guna2TabControl1.SelectedTab Is TabPage1 Then
            Timer_TotalQuntity.Start()
            Label78.Text = 0
        Else
            Timer_TotalQuntity.Start()
            Label78.Text = 0
        End If
    End Sub
    Private Sub Timer_TotalQuntity_Tick(sender As Object, e As EventArgs) Handles Timer_TotalQuntity.Tick
        Timer_TotalQuntity.Interval = i2
        Label78.Text = Label78.Text + 1
        If Label78.Text = DastBordTotalQnty Then
            Timer_TotalQuntity.Stop()
        End If
        If Not Guna2TabControl1.SelectedTab Is TabPage1 Then
            If Timer_TotalQuntity.Enabled = False Then
                Timer_TotalQuntity.Start()
                Label78.Text = 0
            Else
                Timer_TotalQuntity.Start()
                Label78.Text = 0
            End If
        End If
    End Sub

    Function stocksoldtotalAmount() As Integer
        Dim totalAmount As Integer
        Try
            con.Open()

            Dim query As String = "SELECT SUM(TotalPrice) FROM BillData"
            Using cmd As New SqlCommand(query, con)
                ' Execute the query and get the total quantity
                Dim TotalpriceofsoldGoods As Object = cmd.ExecuteScalar()

                ' Check if the query returned a value
                If TotalpriceofsoldGoods IsNot Nothing AndAlso Not IsDBNull(TotalpriceofsoldGoods) Then
                    ' Handle the retrieved total quantity (convert it to the appropriate data type if needed)
                    totalAmount = Convert.ToInt32(TotalpriceofsoldGoods)
                    ' ... do something with totalQty
                Else
                    ' Handle the case when no value is returned from the query
                    MessageBox.Show("No quantity data found.")
                End If
            End Using
        Catch ex As Exception
            ' Handle the exception (e.g., display an error message)
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
        Return totalAmount
    End Function
    Sub loadTotalPriceLabel()
        If Guna2TabControl1.SelectedTab Is TabPage1 Then
            Timer_TotalPrice.Start()
            Label83.Text = stocksoldtotalAmount() - 500
        Else
            Timer_TotalPrice.Start()
            Label83.Text = stocksoldtotalAmount() - 500
        End If
    End Sub
    Private Sub Timer_TotalPrice_Tick(sender As Object, e As EventArgs) Handles Timer_TotalPrice.Tick
        Timer_TotalPrice.Interval = 5
        Label83.Text = Label83.Text + 1
        If Label83.Text = DastBordTotalprice Then
            Timer_TotalPrice.Stop()
        End If
        If Not Guna2TabControl1.SelectedTab Is TabPage1 Then

            If Timer_TotalPrice.Enabled = False Then
                Timer_TotalPrice.Start()
                Label83.Text = 0
            Else
                Timer_TotalPrice.Start()
                Label83.Text = 0
            End If
        End If
    End Sub

    Private Sub Timer_TotalAvailable_Tick(sender As Object, e As EventArgs) Handles Timer_TotalAvailable.Tick

        Timer_TotalAvailable.Interval = i2
        Label81.Text = Label81.Text + 1
        If Label81.Text = DastBordTotalAvailableQuntity Then
            Timer_TotalAvailable.Stop()
        End If
        If Not Guna2TabControl1.SelectedTab Is TabPage1 Then

            If Timer_TotalAvailable.Enabled = False Then
                Timer_TotalAvailable.Start()
                Label81.Text = 0
            Else
                Timer_TotalAvailable.Start()
                Label81.Text = 0
            End If
        End If
    End Sub
    Sub loadTotalAvaiableQuntityLabel()
        If Guna2TabControl1.SelectedTab Is TabPage1 Then
            Timer_TotalAvailable.Start()
            Label81.Text = 0
        Else
            Timer_TotalAvailable.Start()
            Label81.Text = 0
        End If
    End Sub
    Function stocksoldtotalAvaiableQuntity() As Integer
        Dim totalAvaiableQuntity As Integer
        Try
            con.Open()

            Dim query As String = "SELECT SUM(AvailableQuantity) FROM StockData"
            Using cmd As New SqlCommand(query, con)
                ' Execute the query and get the total quantity
                Dim AvailableQuantityofGoods As Object = cmd.ExecuteScalar()

                ' Check if the query returned a value
                If AvailableQuantityofGoods IsNot Nothing AndAlso Not IsDBNull(AvailableQuantityofGoods) Then
                    ' Handle the retrieved total quantity (convert it to the appropriate data type if needed)
                    totalAvaiableQuntity = Convert.ToInt32(AvailableQuantityofGoods)
                    ' ... do something with totalQty
                Else
                    ' Handle the case when no value is returned from the query
                    MessageBox.Show("No quantity data found.")
                End If
            End Using
        Catch ex As Exception
            ' Handle the exception (e.g., display an error message)
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
        Return totalAvaiableQuntity
    End Function

    Sub loadTotalEmployeeLabel()
        If Guna2TabControl1.SelectedTab Is TabPage1 Then
            Timer_TotalEmployee.Start()
            Label89.Text = 0
        Else
            Timer_TotalEmployee.Start()
            Label89.Text = 0
        End If
    End Sub

    Function RetrieveTotalEmployee() As Integer
        Dim totalEmployee As Integer = 0 ' Initialize to a default value
        Try
            con.Open()

            Dim query As String = "SELECT COUNT(Name) FROM EmployeeData"
            Using cmd As New SqlCommand(query, con)
                ' Execute the query and get the total quantity
                Dim totalEmployeesInDB As Object = cmd.ExecuteScalar()

                ' Check if the query returned a value
                If totalEmployeesInDB IsNot Nothing AndAlso Not IsDBNull(totalEmployeesInDB) Then
                    totalEmployee = Convert.ToInt32(totalEmployeesInDB)
                Else
                    MessageBox.Show("No quantity data found.")
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
        Return totalEmployee
    End Function

    Private Sub Timer_TotalEmployee_Tick(sender As Object, e As EventArgs) Handles Timer_TotalEmployee.Tick
        Timer_TotalEmployee.Interval = i2
        Label89.Text = Label89.Text + 1
        If Label89.Text = RetrieveTotalEmployee() Then
            Timer_TotalEmployee.Stop()
        End If
        If Not Guna2TabControl1.SelectedTab Is TabPage1 Then

            If Timer_TotalEmployee.Enabled = False Then
                Timer_TotalEmployee.Start()
                Label89.Text = 0
            Else
                Timer_TotalEmployee.Start()
                Label89.Text = 0
            End If
        End If
    End Sub

    Function RetrieveTotalDealers() As Integer
        Dim totalDealers As Integer = 0 ' Initialize to a default value
        Try
            con.Open()

            Dim query As String = "SELECT COUNT(SupplierName) FROM SupplierData"
            Using cmd As New SqlCommand(query, con)
                ' Execute the query and get the total quantity
                Dim totalDealersInDB As Object = cmd.ExecuteScalar()

                ' Check if the query returned a value
                If totalDealersInDB IsNot Nothing AndAlso Not IsDBNull(totalDealersInDB) Then
                    totalDealers = Convert.ToInt32(totalDealersInDB)
                Else
                    MessageBox.Show("No quantity data found.")
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            con.Close()
        End Try
        Return totalDealers
    End Function

    Sub loadTotalDealersLabel()
        If Guna2TabControl1.SelectedTab Is TabPage1 Then
            Timer_TotalDealers.Start()
            Label92.Text = 0
        Else
            Timer_TotalDealers.Start()
            Label92.Text = 0
        End If
    End Sub

    Private Sub Timer_TotalDealers_Tick(sender As Object, e As EventArgs) Handles Timer_TotalDealers.Tick
        Timer_TotalDealers.Interval = i2
        Label92.Text = Label92.Text + 1
        If Label92.Text = RetrieveTotalDealers() Then
            Timer_TotalDealers.Stop()
        End If
        If Not Guna2TabControl1.SelectedTab Is TabPage1 Then

            If Timer_TotalDealers.Enabled = False Then
                Timer_TotalDealers.Start()
                Label92.Text = 0
            Else
                Timer_TotalDealers.Start()
                Label92.Text = 0
            End If
        End If
    End Sub

    Private Sub Guna2GradientButton22_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton22.Click
        LoginFrom.Show()
        Me.Hide()
    End Sub

End Class