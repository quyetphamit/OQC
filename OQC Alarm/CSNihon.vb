Public Class CSNihon
    Dim xMonthCS(30) As String
    Dim yMonthCS(30) As Integer
    Dim TotalError As Integer
    Dim TimeCheck As String
    Public Sub FormatDateOfMonth(ByVal year As Integer, ByVal month As Integer)
        Select Case month
            Case 1, 3, 5, 7, 8, 10, 12
                ReDim xMonthCS(30)
                ReDim yMonthCS(30)
            Case 4, 6, 9, 11
                ReDim xMonthCS(29)
                ReDim yMonthCS(29)
            Case 2
                If (((year Mod 4 = 0) And (year Mod 100 <> 0) Or (year Mod 400 = 0))) Then
                    ReDim yMonthCS(28)
                    ReDim xMonthCS(28)
                Else
                    ReDim yMonthCS(27)
                    ReDim xMonthCS(27)
                End If
        End Select
        For index = 1 To xMonthCS.Length
            xMonthCS(index - 1) = index
            yMonthCS(index - 1) = 0
        Next
    End Sub

    Private Sub CSNihon_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Timer1.Enabled = False
        Timer1.Interval = 500
        CSNihon_Load(sender, e)
        Me.Hide()
        MainForm.Show()
    End Sub
    Private Sub CSNihon_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
        Timer1.Interval = 500
        LoadMonth()
    End Sub
    Public Sub LoadMonth()
        FormatDateOfMonth(Val(Now.Year), Val(Now.Month))
        TextBox1.Clear()
        TotalError = 0
        Using connect As New Database1Entities1
            GroupBox1.Text = Now.Year & "-" & Now.Month.ToString().PadLeft(2, "0"c) & " Report"
            TimeCheck = Now.Year & Now.Month.ToString().PadLeft(2, "0"c)
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Nihondesans Where s.DateProduction.Contains(TimeCheck)).ToList
            If DateError.Count > 0 Then
                For Each ngay In DateError
                    yMonthCS(Val(Mid(ngay.DateProduction, 7, 2)) - 1) = ngay.ErrorTotal
                    TotalError = TotalError + ngay.ErrorTotal
                Next
            End If
            TextBox1.Text = Me.Text & " :" & Chr(13) & Chr(10)
            TextBox1.Text = TextBox1.Text & "Tổng Lỗi: " & TotalError & Chr(13) & Chr(10)
        End Using
        Chart1.Series("Error").Points.DataBindXY(xMonthCS, yMonthCS)
    End Sub
    Public Sub SearchMonth()
        FormatDateOfMonth(Val(ComboYear.Text), Val(ComboMonth.Text))
        TextBox1.Clear()
        TotalError = 0
        Using connect As New Database1Entities1
            GroupBox1.Text = ComboYear.Text & "-" & ComboMonth.Text & " Report"
            TimeCheck = ComboYear.Text & ComboMonth.Text
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Nihondesans Where s.DateProduction.Contains(TimeCheck)).ToList
            If DateError.Count > 0 Then
                For Each ngay In DateError
                    yMonthCS(Val(Mid(ngay.DateProduction, 7, 2)) - 1) = ngay.ErrorTotal
                    TotalError = TotalError + ngay.ErrorTotal
                Next
            End If
            TextBox1.Text = Me.Text & " :" & Chr(13) & Chr(10)
            TextBox1.Text = TextBox1.Text & "Tổng Lỗi: " & Chr(13) & Chr(10)
            TextBox1.Text = TextBox1.Text & TotalError
        End Using
        Chart1.Series("Error").Points.DataBindXY(xMonthCS, yMonthCS)
    End Sub

    Private Sub ButtonCancel_Click(sender As System.Object, e As System.EventArgs) Handles ButtonCancel.Click
        ComboMonth.Text = ""
        ComboYear.Text = ""
        LoadMonth()
    End Sub

    Private Sub ButtonSearch_Click(sender As System.Object, e As System.EventArgs) Handles ButtonSearch.Click
        If ComboMonth.Text <> "" And ComboYear.Text <> "" Then
            SearchMonth()
        Else
            MessageBox.Show("You need to choice year and month", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        TimeDateWorking.Text = Now
    End Sub
End Class