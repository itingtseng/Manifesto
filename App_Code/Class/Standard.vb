Imports Microsoft.VisualBasic

Public Class Standard

    Private Function GetTimeText(year As Integer, month As Integer) As String
        Dim y1 As New DateTime(1970, 1, 1)
        Dim y2 As New DateTime(year, month, 1)
        Dim s As Int64 = DateDiff(DateInterval.Second, y1, y2) * 1000
        Return s.ToString()
    End Function

    Private Function GetDbl(value As String) As Double
        If IsNumeric(value) Then
            Return CDbl(value)
        Else
            Return 0
        End If
    End Function

    Public Function GetDataTable(item As String) As Data.DataTable

        Dim table As String = ""
        Select Case item.ToLower()
            Case "Car".ToLower()
                table = "Car"
            Case "Scootor".ToLower()
                table = "Scootor"
            Case "COemission".ToLower()
                table = "dbo.COemission"
            Case "data_so2".ToLower()
                table = "data_so2"
            Case Else
                table = item
        End Select

        Dim da As New Jeff.DataAccess
        Dim sql As String = " select * from " & table
        Dim dt_data As Data.DataTable = da.GetDataTable(sql)

        If item = "type_data" Then
            For i As Integer = 0 To dt_data.Rows.Count - 1
                dt_data.Rows(i)("type") = dt_data.Rows(i)("type").ToString().Replace(",", "__")
            Next
        End If

        da.Dispose()
        Return dt_data

    End Function

    Public Sub UploadCar(dt As System.Data.DataTable)

        'drop table

        Dim da As New Jeff.DataAccess
        Dim sql As String = " drop table Car "
        Try
            da.ExecNonQuery(sql)
        Catch ex As Exception

        End Try

        'create table
        Dim field As New List(Of String)
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim n As String = dt.Columns(i).ColumnName
            If IsNumeric(n.Substring(0, 4)) Then
                field.Add(String.Format("[{0}] float NULL", n, ""))
            Else
                field.Add(String.Format("[{0}] nvarchar(255) NULL", n, ""))
            End If
        Next
        sql = " CREATE TABLE Car ( " & String.Join(",", field) & " ) "
        da.ExecNonQuery(sql)

        da.BulkCopy("Car", dt)


    End Sub

    Public Sub UploadScootor(dt As System.Data.DataTable)

        'drop table

        Dim da As New Jeff.DataAccess
        Dim sql As String = " drop table Scootor "
        Try
            da.ExecNonQuery(sql)
        Catch ex As Exception

        End Try

        'create table
        Dim field As New List(Of String)
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim n As String = dt.Columns(i).ColumnName
            If IsNumeric(n.Substring(0, 4)) Then
                field.Add(String.Format("[{0}] float NULL", n, ""))
            Else
                field.Add(String.Format("[{0}] nvarchar(255) NULL", n, ""))
            End If
        Next
        sql = " CREATE TABLE Scootor ( " & String.Join(",", field) & " ) "
        da.ExecNonQuery(sql)

        da.BulkCopy("Scootor", dt)


    End Sub

    Public Sub UploadCOemission(dt As System.Data.DataTable)

        'drop table

        Dim da As New Jeff.DataAccess
        Dim sql As String = " drop table COemission "
        Try
            da.ExecNonQuery(sql)
        Catch ex As Exception

        End Try

        'create table
        Dim field As New List(Of String)
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim n As String = dt.Columns(i).ColumnName
            If IsNumeric(n.Substring(0, 4)) Then
                field.Add(String.Format("[{0}] float NULL", n, ""))
            Else
                field.Add(String.Format("[{0}] nvarchar(255) NULL", n, ""))
            End If
        Next
        sql = " CREATE TABLE COemission ( " & String.Join(",", field) & " ) "
        da.ExecNonQuery(sql)

        da.BulkCopy("COemission", dt)

    End Sub

    Public Sub UploadSO2(dt As System.Data.DataTable)

        'delete data
        Dim da As New Jeff.DataAccess
        Dim sql As String = "  truncate table data_so2 "
        Try
            da.ExecNonQuery(sql)
        Catch ex As Exception

        End Try

        da.BulkCopy("data_so2", dt)

    End Sub

    Public Sub UploadTypeData(dt As System.Data.DataTable)

        'delete data
        Dim da As New Jeff.DataAccess
        Dim sql As String = "  truncate table type_data "
        Try
            da.ExecNonQuery(sql)
        Catch ex As Exception

        End Try

        For i As Integer = 0 To dt.Rows.Count - 1
            If ("" & dt.Rows(i)("value")).ToString().Trim() = "" Then
                dt.Rows(i)("value") = DBNull.Value
            End If
        Next

        da.BulkCopy("type_data", dt)

    End Sub

    Public Sub UploadStation(dt As System.Data.DataTable)

        'delete data
        Dim da As New Jeff.DataAccess
        Dim sql As String = "  truncate table station_create_date "
        Try
            da.ExecNonQuery(sql)
        Catch ex As Exception

        End Try

        For i As Integer = 0 To dt.Rows.Count - 1
            If IsDate("" & dt.Rows(i)("c_date")) = False Then
                dt.Rows(i)("c_date") = DBNull.Value
            End If
        Next

        da.BulkCopy("station_create_date", dt)

    End Sub

    Public Function GetCar() As String
        Return create_driver_data("Car")
    End Function

    Public Function GetScootor() As String
        Return create_driver_data("Scootor")
    End Function

    Private Function create_driver_data(table As String) As String

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = da.GetDataTable("select * from " & table)

        '找出最大年
        Dim year As Integer = 0
        For i As Integer = 0 To dt_site.Columns.Count - 1
            Dim n = dt_site.Columns(i).ColumnName
            If IsNumeric(n.Substring(0, 4)) Then
                Dim f_y As Integer = n.Substring(0, 4)
                If f_y > year Then year = f_y
            End If
        Next

        Dim sd As New DateTime(1988, 1, 1)
        Dim ed As New DateTime(year, 12, 1)

        Dim city_code As Data.DataTable = da.GetDataTable("select * from Country")

        Dim area_c As New System.Collections.Generic.Dictionary(Of String, String())
        area_c.Add("1", {"宜蘭縣"}) '宜蘭空品區
        area_c.Add("2", {"彰化縣", "南投縣", "臺中市"}) '中部空品區   
        area_c.Add("3", {"基隆市", "臺北市", "新北市", "桃園市"}) '北部空品區
        area_c.Add("4", {"新竹市", "苗栗縣", "新竹縣"}) '竹苗空品區
        area_c.Add("5", {"臺東縣", "花蓮縣"}) '花東空品區
        area_c.Add("6", {"屏東縣", "高雄市"}) '高屏空品區
        area_c.Add("7", {"雲林縣", "臺南市", "嘉義市", "嘉義縣"}) '雲嘉南空品區
        area_c.Add("8", {"澎湖縣"}) '澎湖縣
        area_c.Add("9", {"澎湖縣"}) '金門縣
        area_c.Add("10", {"澎湖縣"}) '連江縣

        Dim js_ary As New List(Of String)
        js_ary.Add(get_driver_json(dt_site, sd, ed, "ALL2", "ALL2", {})) '全台
        js_ary.Add(get_driver_json(dt_site, sd, ed, "ALL", "ALL", {})) '全台

        '空品區
        For Each k In area_c.Keys
            js_ary.Add(get_driver_json(dt_site, sd, ed, "Air", k, area_c(k)))
        Next

        '縣市
        For Each city In area_c
            For Each c In city.Value
                Dim row() = city_code.Select("COUNTRY = '" & c & "'")
                If row.Length > 0 Then
                    js_ary.Add(get_driver_json(dt_site, sd, ed, "City", row(0)("CODE"), {c})) '全台
                End If
            Next
        Next

        Dim json As String = "[" & String.Join("," & vbCrLf, js_ary.ToArray()) & "]"
        Dim result As String = "var " & table & "_count = " & vbCrLf & json & ";"

        Dim path As String = HttpContext.Current.Server.MapPath("../js/driver/" & table & "_count.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        Return path

    End Function

    Private Function get_driver_json(data As Data.DataTable,
                             StartDate As DateTime, EndDate As DateTime, level As String, area_code As String, area() As String,
                             Optional other_p As String = "") As String

        Dim m_count As Integer = DateDiff(DateInterval.Month, StartDate, EndDate)
        Dim js_time As New List(Of String)

        Dim data_area() As Data.DataRow
        If area.Length > 0 Then
            data_area = (From r As Data.DataRow In data.Rows Where area.Contains(r("COUNTRY"))).ToArray()
        Else
            data_area = (From r As Data.DataRow In data.Rows Select r).ToArray()
        End If

        For i As Integer = 0 To m_count
            Dim month As DateTime = StartDate.AddMonths(i)
            Dim value As Double?
            Dim col_name As String = month.Year & "Y"
            Dim value_ary = (From r In data_area
                             Select CInt(r(col_name))
                             ).ToArray()

            If value_ary.Count > 0 Then
                value = value_ary.Sum()
            End If

            Dim str_value As String = "null"
            If value IsNot Nothing Then
                Dim v As Double = value
                str_value = v.ToString()
            End If

            Dim s As String = String.Format("[{0}, {1}]",
                                             Me.GetTimeText(month.Year, month.Month), str_value)
            js_time.Add(s)
        Next

        Dim j2 As String = String.Join(", ", js_time.ToArray())
        Dim json As String = String.Format("{{""level"":""{2}"", ""area"":""{0}""" & other_p & ", ""time_value"":[{1}] }}",
                                           area_code, j2, level)

        Return json
    End Function

    Public Function GetCOemission() As String

        Dim dc As New WhitePaper.DataClassesDataContext
        Dim da As New Jeff.DataAccess

        '找出年份
        Dim sql As String = " select * from COemission "
        Dim dt = da.GetDataTable(sql)
        Dim year = Me.GetYearRange(dt)

        Dim min_ym = year.MinYear
        Dim max_ym = year.MaxYear
        Dim sd As New DateTime(year.MinYear, 1, 1)
        Dim ed As New DateTime(year.MaxYear, 12, 1)

        Dim j2 As String = Me.GetCOJson("Scootor", dt, sd, ed)
        Dim j3 As String = Me.GetCOJson("Car", dt, sd, ed)
        Dim j4 As String = Me.GetCOJson("Diesel", dt, sd, ed)

        Dim result As String = "var co_drive = { " &
            """Scootor"" : " & j2 & "," & vbCrLf &
            """Car"" : " & j3 & "," & vbCrLf &
            """Diesel"" : " & j4 & "," & vbCrLf & " };"

        Dim path As String = HttpContext.Current.Server.MapPath("../js/air/co_drive.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        Return path

    End Function

    Public Structure strcYearRange
        Public MaxYear As Integer?
        Public MinYear As Integer?
    End Structure

    Private Function GetYearRange(dt As System.Data.DataTable) As strcYearRange

        Dim find As Boolean = False
        Dim result As New strcYearRange

        For i As Integer = 0 To dt.Columns.Count - 1
            Dim n As String = dt.Columns(i).ColumnName
            If n.Length = 5 AndAlso IsNumeric(n.Substring(0, 4)) Then
                Dim year As Integer = CInt(n.Substring(0, 4))
                If (find = False) Then
                    result.MaxYear = year
                    result.MinYear = year
                    find = True
                Else
                    If year > result.MaxYear Then result.MaxYear = year
                    If year < result.MinYear Then result.MinYear = year
                End If
            End If
        Next

        Return result

    End Function


    Private Function GetCOJson(type As String, dt As System.Data.DataTable,
                 StartDate As DateTime, EndDate As DateTime) As String

        Dim row = dt.Select(String.Format("TYPE = '{0}' ", type))
        Dim regex As New System.Text.RegularExpressions.Regex("^_\d{4}Y$")
        Dim value As New List(Of String)

        Dim MinYear As Integer = StartDate.Year
        Dim MaxYear As Integer = EndDate.Year
        Dim MinMonth As Integer = StartDate.Month
        Dim MaxMonth As Integer = EndDate.Month

        For i As Integer = MinYear To MaxYear
            Dim v As String = "" & row(0)(i & "Y")
            For m As Integer = 1 To 12
                Dim s As String = Me.GetTimeText(i, m)
                value.Add(String.Format("[{0},{1}]", s, v))
            Next
        Next

        Dim json As String = "[" & String.Join(",", value.ToArray()) & "]"
        Return json

    End Function

    Public Function GetSO2() As String

        '年度
        Dim da As New Jeff.DataAccess
        Dim s2 As String = " select min([data_year])  as y1, max([data_year]) as y2 from data_so2 where type = 'gasoline'"
        Dim dt As Data.DataTable = da.GetDataTable(s2)
        Dim d_y1 As Integer = dt.Rows(0)("y1") + 1911
        Dim d_y2 As Integer = dt.Rows(0)("y2") + 1911
        Dim qd1 As New DateTime(d_y1, 1, 1)
        Dim qd2 As New DateTime(d_y2, 12, 31)

        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = (From t In dc.AirQualityMonth
                       Join t2 In dc.AirStation On t.COUNTY Equals t2.COUNTY And t.SITENAME Equals t2.SITENAME
                       Where t.ITEMNAME = "二氧化硫" And t.MONTHLYVALUE IsNot Nothing _
                           And t2.SITETYPE = "交通" _
                           And CInt(t.MONITORYEAR) >= d_y1 _
                           And CInt(t.MONITORYEAR) <= d_y2
                       Order By CInt(t.MONITORYEAR), CInt(t.MONITORMONTH)
                       Select t
                           ).ToArray()

        Dim min_ym = dt_site.First()
        Dim max_ym = dt_site.Last()
        Dim sd As New DateTime(min_ym.MONITORYEAR, min_ym.MONITORMONTH, 1)
        Dim ed As New DateTime(max_ym.MONITORYEAR, max_ym.MONITORMONTH, 1)

        Dim dt_oil As Data.DataTable = da.GetDataTable("select * from data_so2")

        Dim j2 As String = Me.GetSO2Json("fuel", dt_oil, sd, ed)
        Dim j3 As String = Me.GetSO2Json("gasoline", dt_oil, sd, ed)
        Dim j4 As String = Me.GetSO2Json("diesel ", dt_oil, sd, ed)

        Dim result As String = "var co_oil = { " &
            """fuel"" : " & j2 & "," & vbCrLf &
            """gasoline"" : " & j3 & "," & vbCrLf &
            """diesel"" : " & j4 & vbCrLf &
            " };"

        Dim path As String = HttpContext.Current.Server.MapPath("../js/so2_p2/co_oil.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        Dim path2 As String = HttpContext.Current.Server.MapPath("../js/so2/co_oil.js")
        System.IO.File.Copy(path, path2, True)

        Return path & "<br/>" & path2

    End Function

    Private Function GetSO2Json(type As String, data As Data.DataTable,
                 StartDate As DateTime, EndDate As DateTime) As String

        Dim value As New List(Of String)
        Dim MinYear As Integer = StartDate.Year
        Dim MaxYear As Integer = EndDate.Year
        Dim MinMonth As Integer = StartDate.Month
        Dim MaxMonth As Integer = EndDate.Month

        Dim row() As Data.DataRow = data.Select(String.Format("type = '{0}' ", type))

        For i As Integer = 0 To row.Count - 1
            Dim y As Integer = row(i)("data_year") + 1911
            Dim v As Decimal = GetDbl("" & row(i)("value"))

            If y < MinYear Or y > MaxYear Then Continue For

            Dim m1 As Integer = 1, m2 As Integer = 12
            If y = MinYear Then m1 = MinMonth
            If y = MaxYear Then m2 = MaxMonth

            For m As Integer = m1 To m2
                Dim s As String = Me.GetTimeText(y, m)
                value.Add(String.Format("[{0},{1}]", s, v.ToString()))
            Next
        Next

        Dim json As String = "[" & String.Join(",", value.ToArray()) & "]"
        Return json

    End Function

    Public Function GetTypeData() As String

        '年度
        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext

        Dim data As New Dictionary(Of String, String)
        data.Add("gas_v1", "汽油車HC,THC排放標準")
        data.Add("gas_v2", "汽油車HC+NOx,NMHC+NOx排放標準")
        data.Add("gas_v3", "汽油車NOX排放標準")

        data.Add("feul_v1", "柴油車HC,THC排放標準")
        data.Add("feul_v2", "柴油車HC+NOx,NMHC+NOx排放標準")
        data.Add("feul_v3", "柴油車NOX排放標準")

        data.Add("moto_v1", "機車HC,THC排放標準")
        data.Add("moto_v2", "機車HC+NOx,NMHC+NOx排放標準")
        data.Add("moto_v3", "機車NOX排放標準")

        Dim dt_oil As Data.DataTable = da.GetDataTable("select * from type_data ")

        Dim j2 As String = Me.GetTypeDataJson("有鉛汽油含鉛量", dt_oil)
        Dim j3 As String = Me.GetTypeDataJson("無鉛汽油使用率", dt_oil)

        Dim result As String = "var co_oil = { " &
            """gas_has"" : " & j2 & "," & vbCrLf &
            """gas_no"" : " & j3 &
            " };"

        Dim path_gas As String = HttpContext.Current.Server.MapPath("../js/gasoline/co_oil.js")
        Dim fn As New System.IO.FileStream(path_gas, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        'ed = New DateTime(105 + 1911, 11, 1)
        Dim json As New List(Of String)
        For i As Integer = 0 To data.Count - 1
            Dim key As String = data.Keys(i)
            Dim type As String = data(key)

            Dim text As String = Me.GetTypeDataJson(type, dt_oil)
            json.Add("""" & key & """ : " & text)
        Next

        Dim result_ss As String = "var co_oil = { " &
           String.Join("," & vbCrLf, json.ToArray()) & vbCr &
            " };"

        Dim path As String = HttpContext.Current.Server.MapPath("../js/s3/co_oil.js")
        Dim fn2 As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw2 As New System.IO.StreamWriter(fn2, System.Text.Encoding.UTF8)
        sw2.Write(result_ss)
        sw2.Close()
        fn2.Close()

        Dim path2 As String = HttpContext.Current.Server.MapPath("../js/s3/co_oil.js")

        Return path_gas & "<br/>" & path

    End Function

    Private Function GetTypeDataJson(type As String, data As Data.DataTable) As String

        Dim value As New List(Of String)
        Dim row() As Data.DataRow = data.Select(String.Format("type = '{0}' ", type), "data_year")

        For i As Integer = 0 To row.Count - 1
            Dim y As Integer = row(i)("data_year") + 1911
            Dim v As Double = GetDbl("" & row(i)("value"))
            Dim m1 As Integer = 1, m2 As Integer = 12
            For m As Integer = m1 To m2
                Dim s As String = Me.GetTimeText(y, m)
                value.Add(String.Format("[{0},{1}]", s, v))
            Next
        Next

        Dim json As String = "[" & String.Join(",", value.ToArray()) & "]"
        Return json

    End Function

    Public Function GetStation() As String
        Dim ary As New List(Of String)
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select * from AirQualityArea "
        Dim dt As Data.DataTable = da.GetDataTable(sql)
        '空品區
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim r As Data.DataRow = dt.Rows(i)
            Dim v As String = "{{ ""level"": ""Air"", ""code"": ""{0}"", ""text"": ""{1}"" }} "
            v = String.Format(v, "" & r("sn"), "" & r("NAME"))
            ary.Add(v)
        Next

        'city
        sql = " select * from country_area  "
        dt = da.GetDataTable(sql)
        Dim map_ary As New List(Of String)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim r As Data.DataRow = dt.Rows(i)
            Dim v As String = "{{ ""level"": ""City"", ""code"": ""{0}"", ""text"": ""{1}"", ""AreaId"": ""{2}"" }} "
            v = String.Format(v, "" & r("CODE"), "" & r("COUNTRY"), "" & r("AreaId"))
            ary.Add(v)

            Dim v2 As String = "{{ ""code"": ""{0}"", ""value"": {1}, ""drilldown"": ""{0}"" }}"
            v2 = String.Format(v2, "" & r("CODE"), 0)
            map_ary.Add(v2)
        Next

        'city map 

        'stataic
        sql = " Select t1.*, t2.[c_date] from AirStation t1 " &
            " join station_create_date t2 " &
            " On t1.SITENAME = t2.[name] "

        dt = da.GetDataTable(sql)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim r As Data.DataRow = dt.Rows(i)
            Dim str_d As String = ""
            If IsDate("" & r("c_date")) Then
                Dim d As DateTime = r("c_date")
                str_d = d.Year - 1911 & "/" & d.ToString("MM/dd")
            End If
            Dim v As String = "{{ ""level"": ""Station"", ""code"": ""{0}"", ""text"": ""{1}"", ""create_date"": ""{2}"", ""year"": ""{3}"", ""x"": ""{4}"", ""y"": ""{5}"" }} "
            v = String.Format(v, "" & r("NUMBER"), "" & r("SITENAME"), str_d, r("YEAR"), r("X"), r("Y"))
            ary.Add(v)
        Next

        Dim s As String = "var area_code = [ " & vbCrLf &
            "{ ""level"": ""ALL"", ""code"": ""ALL"", ""text"": ""全國"" }, " & vbCrLf &
            String.Join("," & vbCrLf, ary.ToArray()) & vbCrLf &
            "];" & vbCrLf

        Dim s_map As String = "var map_city_value = [ " & vbCrLf &
            String.Join("," & vbCrLf, map_ary.ToArray()) & " ];" & vbCrLf

        Dim path As String = HttpContext.Current.Server.MapPath("../js/common/area_code.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(s)
        sw.Write(s_map)
        sw.Close()
        fn.Close()

        Return path

    End Function


End Class
