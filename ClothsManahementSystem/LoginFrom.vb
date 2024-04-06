Imports System.Data.SqlClient
Public Class LoginFrom
    Dim connectionString As String = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\source\repos\ClothsManahementSystem\ClothsManahementSystem\ClothsManagementData.mdf;Integrated Security=True"
    Dim con As New SqlConnection(connectionString)
    Private Sub LoginFrom_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Guna2TabControl1.TabMenuVisible = False
    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
        Guna2TabControl1.SelectedTab = TabPage2
    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        Guna2TabControl1.SelectedTab = TabPage1
    End Sub

    Private Sub Guna2GradientButton31_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton31.Click
        Try
            Dim EmailID As String = Txt_Email.Text
            Dim password As String = Txt_Pass.Text

            Dim queryEmp As String = "SELECT EmployeeID FROM EmployeeData WHERE EmailAddress = @Email AND Password = @Password"
            Using cmd As New SqlCommand(queryEmp, con)
                con.Open()
                cmd.Parameters.AddWithValue("@Email", EmailID)
                cmd.Parameters.AddWithValue("@Password", password)

                ' Execute the query
                Dim result As Object = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    ' Login successful, retrieve the user ID
                    Dim loggedInUserID As Integer = Convert.ToInt32(result)
                    Dim Employee As New EmployeeForm()
                    MessageBox.Show("Login Successful. ID: " & loggedInUserID.ToString())
                    Employee.Txt_EmppID.Text = loggedInUserID.ToString()
                    Employee.Show()
                Else
                    ' Login failed
                    MsgBox("Invalid credentials. Please try again.")
                End If
            End Using
            Me.Hide()

            Txt_Email.Clear()
            Txt_Pass.Clear()

        Catch ex As Exception
            MessageBox.Show("Error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Txt_Pass.PasswordChar = ControlChars.NullChar ' Display actual characters
        Else
            Txt_Pass.PasswordChar = "*" ' Display asterisks
        End If
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Try
            Dim email As String = Txt_ForGotEmail.Text
            Dim dob As Date = DTP_ForGotDOB.Value.Date
            Dim newPassword As String = Txt_ForGotPass1.Text
            Dim confirmPassword As String = Txt_ForGotPass2.Text

            ' Check if the new password and confirm password match
            If newPassword = confirmPassword Then
                ' Verify the user's email and date of birth
                If VerifyStaff("EmployeeData", dob, email) Then
                    ' Update the password in the database
                    UpdatePassword("EmployeeData", email, newPassword)
                    MessageBox.Show("Password updated successfully.")
                Else
                    MessageBox.Show("Invalid email or date of birth.")
                End If
            Else
                MessageBox.Show("New password and confirm password do not match.")
            End If


            Txt_ForGotEmail.Clear()
            Txt_ForGotPass1.Clear()
            Txt_ForGotPass2.Clear()
            Txt_ForGotPass2.Clear()
            DTP_ForGotDOB.Value = Date.Today
        Catch ex As Exception
            MessageBox.Show("Error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Function VerifyStaff(TblNAme As String, dob As Date, Email As String) As Boolean
        ' Implement the logic to verify the user's email and date of birth
        ' You should query your database to check if the provided email and DOB match a user's record
        ' Return True if the user is verified, otherwise return False
        Dim query As String = "SELECT COUNT(*) FROM " & TblNAme & " WHERE EmailAddress = @Email AND DateOfBirth = @DOB"
        Using cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@Email", Email)
            cmd.Parameters.AddWithValue("@DOB", dob)
            con.Open()
            Dim result As Integer = CInt(cmd.ExecuteScalar())
            con.Close()
            Return result > 0
        End Using
    End Function

    Private Sub UpdatePassword(tblname As String, email As String, newPassword As String)
        Dim query As String = "UPDATE " & tblname & " SET PassWord = @Password WHERE EmailAddress = @Email"
        Using cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@Email", email)
            cmd.Parameters.AddWithValue("@Password", newPassword)
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using
    End Sub
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = True Then
            Txt_ForGotPass2.PasswordChar = ControlChars.NullChar ' Display actual characters
            Txt_ForGotPass1.PasswordChar = ControlChars.NullChar ' Display actual characters
        Else
            Txt_ForGotPass2.PasswordChar = "*" ' Display asterisks
            Txt_ForGotPass1.PasswordChar = "*" ' Display asterisks
        End If

    End Sub
End Class