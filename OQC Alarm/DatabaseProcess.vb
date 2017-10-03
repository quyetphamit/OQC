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
Module DatabaseProcess
    Public TotalCustomer As Integer = 11
    Public InformError(12) As Integer
    Public InformCustomer(12) As Integer
    Public Customer(12) As String
    Public AlarmCustomer(12) As Boolean
    Public ConfirmAlarm(12) As Boolean
    Public TimeAlarmCustomer(12) As Integer
    Public LoiThangArray(12) As Integer
    Public LoiNamArray(13) As Integer
    Public Datecheck As String = Now.Date.ToString("yyyyMMdd")
    Public Shiftcheck As Boolean = True
    'Public xDailyAll = {"Toyo", "HondaLock", "Yokowo", "Nihon", "Canon", "Brother", "Murata", "Schneider", "Kyocera", "Fuji", "Nichicon"}
    'Public xDailyAll = {"Toy", "HLV", "Yok", "Nih", "Can", "Bro", "Mur", "Sch", "Kyo", "Fuj", "Nic"}
    Public xDailyAll = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}
    Public yDailyAll = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    Public xmonthAllPercent = {"Toyodenso", "HondaLock", "Yokowo", "Nihon", "Canon", "Brother", "Murata", "Schneider", "Kyocera", "Fuji", "Nichicon"}
    Public xMonthAll = {"Toyodenso", "HondaLock", "Yokowo", "Nihon", "Canon", "Brother", "Murata", "Schneider", "Kyocera", "Fuji", "Nichicon"}
    Public yMonthAll = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    Public xdateofMonth(30) As String
    Public ydateofMonth(30) As Integer
    Public xYearAll = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}
    Public yYearAll = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    Public TotalMonthErrorALL As Integer = 0
    Public TimeMonthLoad As String = ""
    Public loingayrecord As Integer = 0
    Public Sub FormatDateOfMonth(ByVal year As Integer, ByVal month As Integer)
        Select Case month
            Case 1, 3, 5, 7, 8, 10, 12
                ReDim xdateofMonth(30)
                ReDim ydateofMonth(30)
            Case 4, 6, 9, 11
                ReDim xdateofMonth(29)
                ReDim ydateofMonth(29)
            Case 2
                If (((year Mod 4 = 0) And (year Mod 100 <> 0) Or (year Mod 400 = 0))) Then
                    ReDim xdateofMonth(28)
                    ReDim ydateofMonth(28)
                Else
                    ReDim xdateofMonth(27)
                    ReDim ydateofMonth(27)
                End If
        End Select
        For index = 1 To xdateofMonth.Length
            xdateofMonth(index - 1) = index
            ydateofMonth(index - 1) = 0
        Next
    End Sub
    Public Sub CheckCaSX()
        If Now.Hour >= 8 And Now.Hour <= 19 Then
            Shiftcheck = True
            MainForm.LabelShift.Text = "DayShift"
        Else
            Shiftcheck = False
            MainForm.LabelShift.Text = "NightShift"
        End If
    End Sub
    Public Sub FormatNgayCasx()
        CheckCaSX()
        If Shiftcheck = True Then
            Datecheck = Now.Date.ToString("yyyyMMdd")
            FormatDateOfMonth(Now.Year, Now.Month)
            TimeMonthLoad = Now.Year & Now.Month.ToString().PadLeft(2, "0"c)
        Else
            If Now.Hour >= 0 And Now.Hour <= 7 Then
                If Now.Day > 1 Then
                    Datecheck = Now.Year & Now.Month.ToString().PadLeft(2, "0"c) & (Now.Day - 1).ToString.PadLeft(2, "0"c)
                    FormatDateOfMonth(Now.Year, Now.Month)
                    TimeMonthLoad = Now.Year & (Now.Month).ToString().PadLeft(2, "0"c)
                Else
                    Select Case Now.Month - 1
                        Case 1
                            Datecheck = Now.Year - 1 & "1231"
                            FormatDateOfMonth(Now.Year - 1, 12)
                        Case 3, 5, 7, 8, 10, 12
                            Datecheck = Now.Year & (Now.Month - 1).ToString().PadLeft(2, "0"c) & "31"
                            TimeMonthLoad = Now.Year & (Now.Month - 1).ToString().PadLeft(2, "0"c)
                            FormatDateOfMonth(Now.Year, Now.Month - 1)
                        Case 4, 6, 9, 11
                            TimeMonthLoad = Now.Year & (Now.Month - 1).ToString().PadLeft(2, "0"c)
                            Datecheck = Now.Year & (Now.Month - 1).ToString().PadLeft(2, "0"c) & "30"
                            FormatDateOfMonth(Now.Year, Now.Month - 1)
                        Case 2
                            TimeMonthLoad = Now.Year & "02"
                            If (((Now.Year Mod 4 = 0) And (Now.Year Mod 100 <> 0) Or (Now.Year Mod 400 = 0))) Then
                                Datecheck = Now.Year & "0229"
                                FormatDateOfMonth(Now.Year, 2)
                            Else
                                Datecheck = Now.Year & "0228"
                                FormatDateOfMonth(Now.Year, 2)

                            End If
                    End Select
                End If
            Else
                Datecheck = Now.Date.ToString("yyyyMMdd")
                FormatDateOfMonth(Now.Year, Now.Month)
                TimeMonthLoad = Now.Year & Now.Month.ToString().PadLeft(2, "0"c)
            End If
            MainForm.LabelDate.Text = Datecheck

        End If
    End Sub
    Public Sub Tracedate(ByVal Monthsearch As String, ByVal Getdocument As Integer)
        Using connect As New Database1Entities1
            If Getdocument = 1 Then
                Dim Load = (From s In connect.InformationErrors Where s.DateProduction.Contains(Monthsearch)).ToList
                If Load.Count <> 0 Then
                    Export_file.DataGridView1.DataSource = Load
                End If
            ElseIf Getdocument = 2 Then
                Dim Load = (From s In connect.ErrrorOfMonths Where s.DateOfMonth.Contains(Monthsearch)).ToList
                If Load.Count <> 0 Then
                    Export_file.DataGridView1.DataSource = Load
                End If
            End If
        End Using
    End Sub
    Public Sub LoadDateofMonth()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim Loi1ngay = (From s In connect.ErrrorOfMonths Where s.DateOfMonth.Contains(Datecheck)).FirstOrDefault
            If Loi1ngay Is Nothing Then
                Loi1ngay = New ErrrorOfMonth
                Loi1ngay.DateOfMonth = Datecheck
                Loi1ngay.TotalError = 0
                loingayrecord = 0
                connect.ErrrorOfMonths.AddObject(Loi1ngay)
                connect.SaveChanges()
            End If
            Dim DateError = (From s In connect.ErrrorOfMonths Where s.DateOfMonth.Contains(TimeMonthLoad)).ToList
            If DateError.Count > 0 Then
                For Each ngay In DateError
                    If ngay.DateOfMonth = Datecheck Then loingayrecord = ngay.TotalError
                    ydateofMonth(Val(Mid(ngay.DateOfMonth, 7, 2)) - 1) = ngay.TotalError
                Next
            End If
        End Using
    End Sub
    Public Sub LoadDatabase()
        FormatNgayCasx()
        LoadDailydata()
        LoadMonthdata()
        LoadDateofMonth()
        LoadYeardata()
        LoadToyo() ' load loi tung khach hang
        LoadHondaLock()
        LoadYokowo()
        LoadNihon()
        LoadCanon()
        LoadBrother()
        LoadMurata()
        LoadSchneider()
        LoadKyocera()
        LoadFuji()
        LoadNichicon()
    End Sub
    Public Sub RecordDatabase()
        FormatNgayCasx() ' dinh dang ngay san xua theo ca ngay va dem
        RecordDailyAll() ' ghi loi thong ke hang ngay tat ca khach hang
        RecordMonthAll() ' Ghi du lieu vao loi hang thang
        RecordYear() ' Ghi du lieu vao loi hang nam
        RecordDateofMonth()
        RecordDailyToyodenso() ' ghi loi tung khach hang
        RecordDailyHondaLock()
        RecordDailyYokowo()
        RecordDailyNihon()
        RecordDailyCanon()
        RecordDailyBrother()
        RecordDailyMurata()
        RecordDailySchneider()
        RecordDailyKyocera()
        RecordDailyFuji()
        RecordDailyNichicon()
    End Sub
    Public Sub LoadDailydata()
        Using connect As New Database1Entities1
            Dim DateError = (From s In connect.InformationErrors Where s.DateProduction = Datecheck And s.DayShift = Shiftcheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New InformationError
                DateError.DateProduction = Datecheck
                DateError.DayShift = Shiftcheck
                DateError.DateErrorToyodenso = 0
                DateError.DateErrorHondaLock = 0
                DateError.DateErrorYokowo = 0
                DateError.DateErrorNihondesan = 0
                DateError.DateErrorCanon = 0
                DateError.DateErrorBrother = 0
                DateError.DateErrorMurata = 0
                DateError.DateErrorSchneider = 0
                DateError.DateErrorKyocera = 0
                DateError.DateErrorFujixerox = 0
                DateError.DateErrorNichicon = 0
                connect.InformationErrors.AddObject(DateError)
                connect.SaveChanges()
                For index = 1 To TotalCustomer
                    InformError(index) = 0
                Next
            Else
                yDailyAll(0) = DateError.DateErrorToyodenso
                yDailyAll(1) = DateError.DateErrorHondaLock
                yDailyAll(2) = DateError.DateErrorYokowo
                yDailyAll(3) = DateError.DateErrorNihondesan
                yDailyAll(4) = DateError.DateErrorCanon
                yDailyAll(5) = DateError.DateErrorBrother
                yDailyAll(6) = DateError.DateErrorMurata
                yDailyAll(7) = DateError.DateErrorSchneider
                yDailyAll(8) = DateError.DateErrorKyocera
                yDailyAll(9) = DateError.DateErrorFujixerox
                yDailyAll(10) = DateError.DateErrorNichicon
                For index = 1 To TotalCustomer
                    InformError(index) = yDailyAll(index - 1)
                Next
            End If
        End Using
    End Sub
    Public Sub LoadMonthdata()

        Using connect As New Database1Entities1

            Dim LoiThang = (From s In connect.LoiHangThangs Where s.NamThang = Mid(Datecheck, 1, 6)).FirstOrDefault
            If LoiThang Is Nothing Then
                LoiThang = New LoiHangThang
                LoiThang.NamThang = Mid(Datecheck, 1, 6)
                LoiThang.TotalError = 0
                LoiThang.ErrorToyodenso = 0
                LoiThang.ErrorHondaLock = 0
                LoiThang.ErrorYokowo = 0
                LoiThang.ErrorNihondesan = 0
                LoiThang.ErrorCanon = 0
                LoiThang.ErrorBrother = 0
                LoiThang.ErrorMurata = 0
                LoiThang.ErrorSchneider = 0
                LoiThang.ErrorKyocera = 0
                LoiThang.ErrorFujixerox = 0
                LoiThang.ErrorNichicon = 0
                For index = 0 To TotalCustomer
                    LoiThangArray(index) = 0
                Next
                TotalMonthErrorALL = 0
                connect.LoiHangThangs.AddObject(LoiThang)
                connect.SaveChanges()
            Else
                yMonthAll(0) = LoiThang.ErrorToyodenso
                yMonthAll(1) = LoiThang.ErrorHondaLock
                yMonthAll(2) = LoiThang.ErrorYokowo
                yMonthAll(3) = LoiThang.ErrorNihondesan
                yMonthAll(4) = LoiThang.ErrorCanon
                yMonthAll(5) = LoiThang.ErrorBrother
                yMonthAll(6) = LoiThang.ErrorMurata
                yMonthAll(7) = LoiThang.ErrorSchneider
                yMonthAll(8) = LoiThang.ErrorKyocera
                yMonthAll(9) = LoiThang.ErrorFujixerox
                yMonthAll(10) = LoiThang.ErrorNichicon
                For index = 1 To TotalCustomer
                    TotalMonthErrorALL = LoiThang.TotalError
                    LoiThangArray(index) = yMonthAll(index - 1)
                    xmonthAllPercent(index - 1) = xMonthAll(index - 1) & ": " & MainForm.ChartThang.Series("MonthError").Label
                Next
            End If
        End Using
    End Sub

    Public Sub LoadYeardata()
        Using connect As New Database1Entities1
            Dim LoiNam = (From s In connect.LoiHangNams Where s.Nam = Now.Year).FirstOrDefault
            If LoiNam Is Nothing Then
                LoiNam = New LoiHangNam
                LoiNam.Nam = Now.Year
                LoiNam.T1 = 0
                LoiNam.T2 = 0
                LoiNam.T3 = 0
                LoiNam.T4 = 0
                LoiNam.T5 = 0
                LoiNam.T6 = 0
                LoiNam.T7 = 0
                LoiNam.T8 = 0
                LoiNam.T9 = 0
                LoiNam.T10 = 0
                LoiNam.T11 = 0
                LoiNam.T12 = 0
                For index = 0 To TotalCustomer
                    LoiNamArray(index) = 0
                Next
                connect.LoiHangNams.AddObject(LoiNam)
                connect.SaveChanges()
            Else
                yYearAll(0) = LoiNam.T1
                yYearAll(1) = LoiNam.T2
                yYearAll(2) = LoiNam.T3
                yYearAll(3) = LoiNam.T4
                yYearAll(4) = LoiNam.T5
                yYearAll(5) = LoiNam.T6
                yYearAll(6) = LoiNam.T7
                yYearAll(7) = LoiNam.T8
                yYearAll(8) = LoiNam.T9
                yYearAll(9) = LoiNam.T10
                yYearAll(10) = LoiNam.T11
                yYearAll(11) = LoiNam.T12
                For index = 1 To 12
                    LoiNamArray(index) = yYearAll(index - 1)
                Next
            End If
        End Using
    End Sub

    Public Sub LoadToyo()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.TOYODENSOes Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New TOYODENSO
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.TOYODENSOes.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(1) = 0
            Else
                InformCustomer(1) = DateError.ErrorTotal
            End If
        End Using
    End Sub
    Public Sub LoadHondaLock()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.HondaLocks Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New HondaLock
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.HondaLocks.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(2) = 0
            Else
                InformCustomer(2) = DateError.ErrorTotal
            End If
        End Using
    End Sub
    Public Sub LoadYokowo()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Yokowoes Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Yokowo
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Yokowoes.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(3) = 0
            Else
                InformCustomer(3) = DateError.ErrorTotal

            End If
        End Using
    End Sub
    Public Sub LoadNihon()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Nihondesans Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Nihondesan
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Nihondesans.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(4) = 0
            Else
                InformCustomer(4) = DateError.ErrorTotal

            End If
        End Using
    End Sub
    Public Sub LoadCanon()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Canons Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Canon
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Canons.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(5) = 0
            Else
                InformCustomer(5) = DateError.ErrorTotal
            End If
        End Using
    End Sub
    Public Sub LoadBrother()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Brothers Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Brother
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Brothers.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(6) = 0
            Else
                InformCustomer(6) = DateError.ErrorTotal

            End If
        End Using
    End Sub
    Public Sub LoadMurata()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Muratas Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Murata
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Muratas.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(7) = 0
            Else
                InformCustomer(7) = DateError.ErrorTotal
            End If
        End Using
    End Sub
    Public Sub LoadSchneider()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Schneiders Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Schneider
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Schneiders.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(8) = 0
            Else
                InformCustomer(8) = DateError.ErrorTotal
            End If
        End Using
    End Sub
    Public Sub LoadKyocera()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Kyoceras Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Kyocera
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Kyoceras.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(9) = 0
            Else
                InformCustomer(9) = DateError.ErrorTotal

            End If
        End Using
    End Sub
    Public Sub LoadFuji()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Fujixeroxes Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Fujixerox
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Fujixeroxes.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(10) = 0
            Else
                InformCustomer(10) = DateError.ErrorTotal
            End If
        End Using
    End Sub
    Public Sub LoadNichicon()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Nichicons Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Nichicon
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                connect.Nichicons.AddObject(DateError)
                connect.SaveChanges()
                InformCustomer(11) = 0
            Else
                InformCustomer(11) = DateError.ErrorTotal
            End If
        End Using
    End Sub
    '\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    Public Sub RecordDailyAll()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.InformationErrors Where s.DateProduction = Datecheck And s.DayShift = Shiftcheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New InformationError
                DateError.DateProduction = Datecheck
                DateError.DayShift = Shiftcheck
                DateError.DateErrorToyodenso = 0
                DateError.DateErrorHondaLock = 0
                DateError.DateErrorYokowo = 0
                DateError.DateErrorNihondesan = 0
                DateError.DateErrorCanon = 0
                DateError.DateErrorBrother = 0
                DateError.DateErrorMurata = 0
                DateError.DateErrorSchneider = 0
                DateError.DateErrorKyocera = 0
                DateError.DateErrorFujixerox = 0
                DateError.DateErrorNichicon = 0
                connect.InformationErrors.AddObject(DateError)
                connect.SaveChanges()
                For index = 1 To TotalCustomer
                    yDailyAll(index - 1) = 0
                    InformError(index) = 0
                Next
            Else
                DateError.DateErrorToyodenso = InformError(1)
                DateError.DateErrorHondaLock = InformError(2)
                DateError.DateErrorYokowo = InformError(3)
                DateError.DateErrorNihondesan = InformError(4)
                DateError.DateErrorCanon = InformError(5)
                DateError.DateErrorBrother = InformError(6)
                DateError.DateErrorMurata = InformError(7)
                DateError.DateErrorSchneider = InformError(8)
                DateError.DateErrorKyocera = InformError(9)
                DateError.DateErrorFujixerox = InformError(10)
                DateError.DateErrorNichicon = InformError(11)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDateofMonth() ' Ghi du lieu tung ngay cua thang 
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim Loi1ngay = (From s In connect.ErrrorOfMonths Where s.DateOfMonth.Contains(Datecheck)).FirstOrDefault
            If Loi1ngay Is Nothing Then
                Loi1ngay = New ErrrorOfMonth
                Loi1ngay.DateOfMonth = Datecheck
                Loi1ngay.TotalError = 0
                loingayrecord = 0
                connect.ErrrorOfMonths.AddObject(Loi1ngay)
                connect.SaveChanges()
            End If
            Dim Loingay = (From s In connect.ErrrorOfMonths Where s.DateOfMonth.Contains(TimeMonthLoad)).ToList
            If Loingay.Count > 0 Then
                For Each ngay In Loingay
                    If ngay.DateOfMonth = Datecheck Then ngay.TotalError = loingayrecord
                    ydateofMonth(Val(Mid(ngay.DateOfMonth, 7, 2)) - 1) = ngay.TotalError
                Next
            End If
            connect.SaveChanges()
        End Using
    End Sub
    Public Sub RecordMonthAll() ' Ghi du lieu hang thang 
        ' ghi du lieu cua tung ngay toan bo khach hang
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim LoiThang = (From s In connect.LoiHangThangs Where s.NamThang = Mid(Datecheck, 1, 6)).FirstOrDefault
            If LoiThang Is Nothing Then
                LoiThang = New LoiHangThang
                LoiThang.NamThang = Mid(Datecheck, 1, 6)
                LoiThang.TotalError = 0
                LoiThang.ErrorToyodenso = 0
                LoiThang.ErrorHondaLock = 0
                LoiThang.ErrorYokowo = 0
                LoiThang.ErrorNihondesan = 0
                LoiThang.ErrorCanon = 0
                LoiThang.ErrorBrother = 0
                LoiThang.ErrorMurata = 0
                LoiThang.ErrorSchneider = 0
                LoiThang.ErrorKyocera = 0
                LoiThang.ErrorFujixerox = 0
                LoiThang.ErrorNichicon = 0
                TotalMonthErrorALL = 0
                For index = 0 To TotalCustomer
                    LoiThangArray(index) = 0
                Next
                connect.LoiHangThangs.AddObject(LoiThang)
                connect.SaveChanges()
            Else
                LoiThang.TotalError = TotalMonthErrorALL
                LoiThang.ErrorToyodenso = LoiThangArray(1)
                LoiThang.ErrorHondaLock = LoiThangArray(2)
                LoiThang.ErrorYokowo = LoiThangArray(3)
                LoiThang.ErrorNihondesan = LoiThangArray(4)
                LoiThang.ErrorCanon = LoiThangArray(5)
                LoiThang.ErrorBrother = LoiThangArray(6)
                LoiThang.ErrorMurata = LoiThangArray(7)
                LoiThang.ErrorSchneider = LoiThangArray(8)
                LoiThang.ErrorKyocera = LoiThangArray(9)
                LoiThang.ErrorFujixerox = LoiThangArray(10)
                LoiThang.ErrorNichicon = LoiThangArray(11)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordYear()        ' Ghi du lieu hang thang 
        ' ghi du lieu cua tung ngay toan bo khach hang
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim LoiNam = (From s In connect.LoiHangNams Where s.Nam = Now.Year).FirstOrDefault
            If LoiNam Is Nothing Then
                LoiNam = New LoiHangNam
                LoiNam.Nam = Now.Year
                LoiNam.T1 = 1
                LoiNam.T2 = 0
                LoiNam.T3 = 0
                LoiNam.T4 = 0
                LoiNam.T5 = 0
                LoiNam.T6 = 0
                LoiNam.T7 = 0
                LoiNam.T8 = 0
                LoiNam.T9 = 0
                LoiNam.T10 = 0
                LoiNam.T11 = 0
                LoiNam.T12 = 0
                TotalMonthErrorALL = 0
                For index = 1 To 12
                    LoiNamArray(index) = 0
                Next
                connect.LoiHangNams.AddObject(LoiNam)
                connect.SaveChanges()
            Else
                For index = 1 To 12
                    If index = Val(Now.Month) Then
                        LoiNamArray(index) = TotalMonthErrorALL
                    End If
                    yYearAll(index - 1) = LoiNamArray(index)
                Next
                LoiNam.T1 = LoiNamArray(1)
                LoiNam.T2 = LoiNamArray(2)
                LoiNam.T3 = LoiNamArray(3)
                LoiNam.T4 = LoiNamArray(4)
                LoiNam.T5 = LoiNamArray(5)
                LoiNam.T6 = LoiNamArray(6)
                LoiNam.T7 = LoiNamArray(7)
                LoiNam.T8 = LoiNamArray(8)
                LoiNam.T9 = LoiNamArray(9)
                LoiNam.T10 = LoiNamArray(10)
                LoiNam.T11 = LoiNamArray(11)
                LoiNam.T12 = LoiNamArray(12)
                connect.SaveChanges()
            End If
        End Using
    End Sub

    Public Sub RecordDailyToyodenso()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.TOYODENSOes Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New TOYODENSO
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(1) = 0
                connect.TOYODENSOes.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(1)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyHondaLock()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.HondaLocks Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New HondaLock
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(2) = 0
                connect.HondaLocks.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(2)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyYokowo()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Yokowoes Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Yokowo
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(3) = 0
                connect.Yokowoes.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(3)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyNihon()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Nihondesans Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Nihondesan
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(4) = 0
                connect.Nihondesans.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(4)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyCanon()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Canons Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Canon
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(5) = 0
                connect.Canons.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(5)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyBrother()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Brothers Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Brother
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(6) = 0
                connect.Brothers.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(6)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyMurata()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Muratas Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Murata
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(7) = 0
                connect.Muratas.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(7)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailySchneider()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Schneiders Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Schneider
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(8) = 0
                connect.Schneiders.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(8)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyKyocera()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Kyoceras Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Kyocera
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(9) = 0
                connect.Kyoceras.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(9)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyFuji()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Fujixeroxes Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Fujixerox
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(10) = 0
                connect.Fujixeroxes.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(10)
                connect.SaveChanges()
            End If
        End Using
    End Sub
    Public Sub RecordDailyNichicon()
        Using connect As New Database1Entities1
            ' ghi du lieu cua tung ngay toan bo khach hang
            Dim DateError = (From s In connect.Nichicons Where s.DateProduction = Datecheck).FirstOrDefault
            If DateError Is Nothing Then
                DateError = New Nichicon
                DateError.DateProduction = Datecheck
                DateError.ErrorTotal = 0
                InformCustomer(11) = 0
                connect.Nichicons.AddObject(DateError)
                connect.SaveChanges()
            Else
                DateError.ErrorTotal = InformCustomer(11)
                connect.SaveChanges()
            End If
        End Using
    End Sub
End Module
