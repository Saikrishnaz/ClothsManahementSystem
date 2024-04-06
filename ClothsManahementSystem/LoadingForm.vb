Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class LoadingForm
    Dim i As Integer = 100
    Private Sub LoadingForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Interval = i
        ProgressBar1.Value = ProgressBar1.Value + 1
        lbl_Percent.Text = ProgressBar1.Value
        If ProgressBar1.Value = 100 Then
            Timer1.Stop()
            LoginFrom.Show()
            Me.Hide()
        End If
    End Sub
End Class