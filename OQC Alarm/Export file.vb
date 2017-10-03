Public Class Export_file

    Private Sub Bt_search_Click(sender As System.Object, e As System.EventArgs) Handles Bt_search.Click
        Dim datesearch As String = ComboNam1.Text & Combothang1.Text
        Tracedate(datesearch, 1)
    End Sub
    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim datesearch As String = ComboNam2.Text & Combothang2.Text
        Tracedate(datesearch, 2)
    End Sub

    Private Sub Export_file_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Me.Hide()
        DataGridView1.DataSource = ""
        MainForm.Show()
    End Sub

    Private Sub Export_file_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        DataGridView1.DataSource = ""
    End Sub

    Private Sub Bt_xoa_Click(sender As System.Object, e As System.EventArgs)
        DataGridView1.DataSource = ""
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        SaveFileDialog1.Title = " Save Report file"
        SaveFileDialog1.Filter = "CSV Files (*.csv*)|*.csv"
        'SaveFileDialog1.ShowDialog()

        If DataGridView1.RowCount <> 0 Then
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim exportString As String = ""
                For index = 0 To DataGridView1.ColumnCount - 1
                    exportString = exportString & DataGridView1.Columns.Item(index).Name & ","
                Next
                exportString = exportString & Chr(13) & Chr(10)
                For index = 0 To DataGridView1.RowCount - 1
                    For j = 0 To DataGridView1.ColumnCount - 1
                        exportString = exportString & DataGridView1.Rows(index).Cells(j).Value & ","
                    Next
                    exportString = exportString & Chr(13) & Chr(10)
                Next
                My.Computer.FileSystem.WriteAllText(SaveFileDialog1.FileName, exportString, True)
            End If
        Else
            MessageBox.Show("Không có dữ liệu", "Waring", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End If
    End Sub


End Class