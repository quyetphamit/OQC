Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.Data
Imports System.Data.Common
Imports System.Data.Objects
Imports System.Data.Objects.DataClasses
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic
Imports System.IO.Ports
Imports System.Data.EntityClient
Imports System.String
Imports System.Data.SqlClient

Public Class MainForm
    Public PathLog As String = "\\umc-c231\OQC share Cuong\wip111"

    Public ComRF As String = "COM6"
    Public SetBaudRateComRF As Integer = 9600
    Public SetDataBitsComRF As Integer = 8
    Public SetStopBitsComRF As Integer = 1
    Public SetParityComRF As String = "None"
    ' Com Control module
    Public ComControl As String = "COM7"
    Public SetBaudRateComControl As Integer = 9600
    Public SetDataBitsComControl As Integer = 8
    Public SetStopBitsComControl As Integer = 1
    Public SetParityComControl As String = "None"
    Public TimeAlarmSet As Integer = 20
    Public MusicEnable As Boolean = False
    Public PathMusic As String
    Public ArrayConfirm() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "K", "L"}
    Public Listcustomer() As String = {"abcd", "Toyodenso", "HondaLock", "Yokowo", "Nihon", "Canon", "Brother", "Murata", "Schneider", "Kyocera", "Fuji", "Nichicon"}
    Dim FileNameLog As String = ""
    '===================================================
    Delegate Sub SetTextCallback(ByVal [text] As String) 'Added to prevent threading errors during receiveing of data
    '======================================================================
    Public Sub deletefile(ByVal targetDirectory As String)
        Dim fileEntries As String() = Directory.GetFiles(targetDirectory)
        ' Process the list of files found in the directory.
        Dim fileName As String
        For Each fileName In fileEntries
            If fileName.Contains(".TXT") = True Then
                File.Delete(fileName)
            End If
        Next fileName
        ' Dim subdirectoryEntries As String() = Directory.GetDirectories(targetDirectory)
        ' Recurse into subdirectories of this directory.
        'Dim subdirectory As String
        'For Each subdirectory In subdirectoryEntries
        '    deletefile(subdirectory)
        'Next subdirectory
    End Sub 'ProcessDirectory
    Public Sub ProcessDirectory(ByVal targetDirectory As String)
        Dim fileEntries As String() = Directory.GetFiles(targetDirectory)
        ' Process the list of files found in the directory.
        Dim fileName As String
        For Each fileName In fileEntries
            'Dim minute As String = ""
            'If Val(Now.ToString("mm")) - 1 < 10 Then
            '    minute = "0" & Val(Now.ToString("mm")) - 1
            'Else
            '    minute = Val(Now.ToString("mm")) - 1
            'End If
            If fileName.Contains(".TXT") = True Then
                FileNameLog = fileName
                Exit Sub
            End If
        Next fileName
    End Sub 'ProcessDirectory
    Private Sub CommPortSetup()
        ' Setup Com RF
        With ComRfPort
            .PortName = ComRF
            .BaudRate = SetBaudRateComRF
            .DataBits = SetDataBitsComRF
            If SetParityComRF = "None" Then
                .Parity = Parity.None
            ElseIf SetParityComRF = "Even" Then
                .Parity = Parity.Even
            ElseIf SetParityComRF = "Odd" Then
                .Parity = Parity.Odd
            ElseIf SetParityComRF = "Mark" Then
                .Parity = Parity.Mark
            ElseIf SetParityComRF = "Space" Then
                .Parity = Parity.Space
            End If
            If SetStopBitsComRF = 0 Then
                .StopBits = StopBits.None
            ElseIf SetStopBitsComRF = 1 Then
                .StopBits = StopBits.One
            ElseIf SetStopBitsComRF = 2 Then
                .StopBits = StopBits.Two
            End If
            .Handshake = Handshake.None
            .ReceivedBytesThreshold = 1
        End With
        Try
            ComRfPort.Open()
        Catch ex As Exception
            MessageBox.Show("COM RF: " & ComRF & " not connect. Please check connect the device !", "Error device", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End
        End Try

        '---------------------------------------------
        'Setup Com Control
        With ComControlPort
            .PortName = ComControl
            .BaudRate = SetBaudRateComControl
            .DataBits = SetDataBitsComControl
            If SetParityComControl = "None" Then
                .Parity = Parity.None
            ElseIf SetParityComControl = "Even" Then
                .Parity = Parity.Even
            ElseIf SetParityComControl = "Odd" Then
                .Parity = Parity.Odd
            ElseIf SetParityComControl = "Mark" Then
                .Parity = Parity.Mark
            ElseIf SetParityComControl = "Space" Then
                .Parity = Parity.Space
            End If
            If SetStopBitsComControl = 0 Then
                .StopBits = StopBits.None
            ElseIf SetStopBitsComControl = 1 Then
                .StopBits = StopBits.One
            ElseIf SetStopBitsComControl = 2 Then
                .StopBits = StopBits.Two
            End If
            .Handshake = Handshake.None
            .ReceivedBytesThreshold = 1
        End With
        Try
            ComControlPort.Open()
        Catch ex As Exception
            MessageBox.Show("COM Control Light Alarm: " & ComControl & " not connect. Please check connect the device !", "Error device", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End
        End Try
        If ComControlPort.IsOpen = True Then ComControlPort.Write("X")
    End Sub
    Private Sub LoadFileSetting()
        Dim PathApplycation As String = Application.StartupPath.ToString
        Dim PathSetting As String = String.Format("{0}\{1}", PathApplycation, "Setting.txt")
        Dim LineFileSetting As String
        If System.IO.File.Exists(PathSetting) = True Then
            '--- doc tung gia tri setup COM RF
            LineFileSetting = ReadTextFile(PathSetting, 2) ' cai dat cong com
            ComRF = Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1)
            LineFileSetting = Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length)
            SetBaudRateComRF = Val(Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1))
            LineFileSetting = Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length)
            SetDataBitsComRF = Val(Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1))
            LineFileSetting = Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length)
            SetParityComRF = Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1)
            SetStopBitsComRF = Val(Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length))
            '---------------------------------------------------------------------------------------------------------------------------------
            '--- doc tung gia tri setup COM control
            LineFileSetting = ReadTextFile(PathSetting, 4) ' cai dat cong com
            ComControl = Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1)
            LineFileSetting = Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length)
            SetBaudRateComControl = Val(Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1))
            LineFileSetting = Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length)
            SetDataBitsComControl = Val(Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1))
            LineFileSetting = Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length)
            SetParityComControl = Mid(LineFileSetting, 1, InStr(LineFileSetting, ",", CompareMethod.Text) - 1)
            SetStopBitsComControl = Val(Mid(LineFileSetting, InStr(LineFileSetting, ",", CompareMethod.Text) + 1, LineFileSetting.Length))
            '--------------------------------------------------------------------------------
            Timer1.Interval = Val(ReadTextFile(PathSetting, 6)) ' cai dat TIMER 1
            'Timer2.Interval = Val(ReadTextFile(PathSetting, 8)) ' cai dat TIMER 2 DK DEN
            Timer3.Interval = Val(ReadTextFile(PathSetting, 10)) ' cai dat TIMER 3 DOC TIN HIEU
            TimeAlarmSet = Val(ReadTextFile(PathSetting, 12)) ' cai dat thoi gian co nhay den va coi
            If Val(ReadTextFile(PathSetting, 14)) = 1 Then
                MusicEnable = True
            Else
                MusicEnable = False
            End If
            PathMusic = PathApplycation & "\" & ReadTextFile(PathSetting, 16)
            PathLog = ReadTextFile(PathSetting, 18)
            If MusicEnable = True Then
                WindowPlayer.URL = PathMusic
                WindowPlayer.settings.volume = WindowPlayer.settings.volume.MaxValue
                WindowPlayer.Ctlcontrols.stop()
            End If

        Else
            MsgBox(" File Setting.txt in " & PathSetting & " not found")
            'End
        End If
    End Sub
    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick

        'TimeDateWorking.Text = ""
        If (Now.Hour = 20 And Now.Minute = 1 And Now.Second = 10) Or (Now.Hour = 8 And Now.Minute = 1 And Now.Second = 10) Then
            LoadDatabase()
            System.Diagnostics.Process.Start("shutdown", "-r -t 00")
        End If
        'LabelDate.Text = ""
        TimeDateWorking.Text = Now.ToString("HH:mm:ss dd/MM/yyyy")
        LabelDate.Text = Datecheck
        For index = 1 To TotalCustomer
            If AlarmCustomer(index) = True Then
                WindowPlayer.Ctlcontrols.play()
                TimeAlarmCustomer(index) = TimeAlarmCustomer(index) + 1
                If TimeAlarmCustomer(index) = TimeAlarmSet Then
                    TimeAlarmCustomer(index) = 0
                    WindowPlayer.Ctlcontrols.stop()

                    AlarmCustomer(index) = False
                    ConfirmAlarm(index) = False
                    GroupBox2.Controls("Button" & index).BackColor = Color.Transparent
                End If
            End If
        Next
        Chart1.Series("Error").Points.DataBindXY(xDailyAll, yDailyAll)
        ChartThang.Series("MonthError").Points.DataBindXY(xmonthAllPercent, yMonthAll)
        ChartNam.Series("Nam").Points.DataBindXY(xYearAll, yYearAll)
        Chart2.Series("Loi").Points.DataBindXY(xdateofMonth, ydateofMonth)
        LabelTongLoiThang.Text = TotalMonthErrorALL
    End Sub

    Private Sub MainForm_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        'If ComControlPort.IsOpen = True Then ComControlPort.Write("X")
        End
    End Sub

    Private Sub MainForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False ' tranh loi khi doc com port
        LoadFileSetting()
        LoadDatabase()
        For index = 1 To 11
            ConfirmAlarm(index) = False
            AlarmCustomer(index) = False
        Next

        For index = 1 To TotalCustomer
            GroupBox2.Controls("Button" & index).BackColor = Color.Transparent
        Next
        Chart1.Series("Error").Points.Clear()
        ChartThang.Series("MonthError").Points.Clear()
        ChartNam.Series("Nam").Points.Clear()
        CheckCaSX()
        Chart1.Series("Error").Points.DataBindXY(xDailyAll, yDailyAll)
        ChartThang.Series("MonthError").Points.DataBindXY(xMonthAll, yMonthAll)
        ChartNam.Series("Nam").Points.DataBindXY(xYearAll, yYearAll)
        Chart2.Series("Loi").Points.DataBindXY(xdateofMonth, ydateofMonth)
        ' CommPortSetup()
        Timer1.Enabled = True
        'Timer2.Enabled = True
        Timer3.Enabled = True
    End Sub


    Private Sub Timer3_Tick(sender As System.Object, e As System.EventArgs) Handles Timer3.Tick
        If Directory.Exists(PathLog) Then
            Label6.Visible = False
            ProcessDirectory(PathLog)
            If FileNameLog <> "" Then
                For index = 1 To 11
                    If FileNameLog.Contains(Listcustomer(index)) = True Then
                        File.Delete(FileNameLog)
                        'WindowPlayer.Ctlcontrols.stop()

                        GroupBox2.Controls("Button" & index).BackColor = Color.Red ' thay doi backColor de tao nhap nhay
                        InformError(index) = InformError(index) + 1 ' tang so luong loi cua khach hang thu index
                        InformCustomer(index) = InformCustomer(index) + 1 ' tang so luong loi cua khach hang ca ngay len 1
                        LoiThangArray(index) = LoiThangArray(index) + 1 ' tang so luong loi hang thang 
                        TotalMonthErrorALL = TotalMonthErrorALL + 1 ' tang tong so loi 1 thang len 1
                        AlarmCustomer(index) = True
                        yDailyAll(index - 1) = InformError(index)
                        yMonthAll(index - 1) = LoiThangArray(index)
                        xmonthAllPercent(index - 1) = xMonthAll(index - 1) & ": " & ChartThang.Series("MonthError").Label
                        loingayrecord = loingayrecord + 1
                    End If
                Next

                FileNameLog = ""
                RecordDatabase()
           
            End If
        Else
            Label6.Visible = True
        End If
    End Sub

    'Private Sub Timer2_Tick(sender As System.Object, e As System.EventArgs)
    '    For index = 1 To TotalCustomer
    '        If GroupBox2.Controls("Button" & index).BackColor = Color.Red Then
    '            If ComControlPort.IsOpen = True Then ComControlPort.Write("V") ' BAT DEN VANG
    '            'If ComControlPort.IsOpen = True Then ComControlPort.Write("F") ' TAT CHUONG
    '            GroupBox2.Controls("Button" & index).BackColor = Color.Blue
    '        ElseIf GroupBox2.Controls("Button" & index).BackColor = Color.Blue Then
    '            If ComControlPort.IsOpen = True Then ComControlPort.Write("Z") ' TAT DEN VANG
    '            ' If ComControlPort.IsOpen = True Then ComControlPort.Write("B") ' BAT CHUONG
    '            GroupBox2.Controls("Button" & index).BackColor = Color.Red
    '        End If
    '    Next
    'End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        CS_Toyodenso.Show()
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        CSHondaLock.Show()
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        CSYokowo.Show()
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        CSNihon.Show()
    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        CSCanon.Show()
    End Sub

    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click
        CSBrother.Show()
    End Sub

    Private Sub Button7_Click(sender As System.Object, e As System.EventArgs) Handles Button7.Click
        CSMurata.Show()
    End Sub

    Private Sub Button8_Click(sender As System.Object, e As System.EventArgs) Handles Button8.Click
        CSSchneider.Show()
    End Sub

    Private Sub Button9_Click(sender As System.Object, e As System.EventArgs) Handles Button9.Click
        CSKyocera.Show()
    End Sub

    Private Sub Button10_Click(sender As System.Object, e As System.EventArgs) Handles Button10.Click
        CSFuji.Show()
    End Sub

    Private Sub Button11_Click(sender As System.Object, e As System.EventArgs) Handles Button11.Click
        CSNichicon.Show()
    End Sub

    Private Sub ExportCsv_Click(sender As System.Object, e As System.EventArgs) Handles ExportCsv.Click
        Export_file.Show()
    End Sub

    Private Sub Serial_Receive_TextChanged(sender As System.Object, e As System.EventArgs)

    End Sub
End Class
