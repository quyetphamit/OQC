Public Class Form1
    Dim PathApplycation As String = Application.StartupPath.ToString()
    Dim PathLog1 As String = ""
    Dim PathLog2 As String = ""
    Private Sub ButtonXoa_Click(sender As Object, e As EventArgs) Handles ButtonXoa.Click
        ComboBox1.Text = ""
        ComboBox1.Enabled = True
        For index = 1 To 11
            Controls("Button" & index).BackColor = Color.WhiteSmoke
            Controls("Button" & index).ForeColor = Color.Black
            Controls("Button" & index).Enabled = False
        Next
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.Text <> "" Then
            Controls("Button" & ComboBox1.SelectedIndex + 1).BackColor = Color.Red
            Controls("Button" & ComboBox1.SelectedIndex + 1).ForeColor = Color.White
            Controls("Button" & ComboBox1.SelectedIndex + 1).Enabled = True
        End If
        ComboBox1.Enabled = False
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PathLog1 = ReadTextFile(PathApplycation & "\Setting.txt", 2)
        PathLog2 = ReadTextFile(PathApplycation & "\Setting.txt", 4)
        ComboBox1.Text = ""
        ComboBox1.Enabled = True
        For index = 1 To 11
            Controls("Button" & index).BackColor = Color.WhiteSmoke
            Controls("Button" & index).ForeColor = Color.Black
            Controls("Button" & index).Enabled = False
        Next
    End Sub
    Public Sub CreatLog()
        If System.IO.Directory.Exists(PathLog1) = False Then
            System.IO.Directory.CreateDirectory(PathLog1)
        End If
        Dim FileNameWIP As String = PathLog1 & "\" & Now.ToString("ddMMyyyy_HHmmss") & "_" & ComboBox1.Text & ".TXT"
        Dim textlogwip As New System.IO.StreamWriter(FileNameWIP, True)
        textlogwip.WriteLine(ComboBox1.Text) ' ghi thong tin vao file log
        textlogwip.Close()
        If System.IO.Directory.Exists(PathLog2) = False Then
            System.IO.Directory.CreateDirectory(PathLog2)
        End If
        Dim FileNameWIP2 As String = PathLog2 & "\" & Now.ToString("ddMMyyyy_HHmmss") & "_" & ComboBox1.Text & ".TXT"
        Dim textlogwip2 As New System.IO.StreamWriter(FileNameWIP2, False)
        textlogwip2.WriteLine(ComboBox1.Text) ' ghi thong tin vao file log
        textlogwip2.Close()
        Controls("Button" & ComboBox1.SelectedIndex + 1).BackColor = Color.WhiteSmoke
        Controls("Button" & ComboBox1.SelectedIndex + 1).ForeColor = Color.Black
        Controls("Button" & ComboBox1.SelectedIndex + 1).Enabled = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        CreatLog()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        CreatLog()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        CreatLog()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        CreatLog()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        CreatLog()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        CreatLog()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        CreatLog()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        CreatLog()
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        CreatLog()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        CreatLog()
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        CreatLog()
    End Sub
End Class
