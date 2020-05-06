
Partial Class create_js_chart_json
    Inherits System.Web.UI.Page

    Class class_kind_json

        Public Enum enum_data_unit
            month
            year
        End Enum

        Public item_name As String
        Public file_name As String
        Public stie_type() As String

        Public default_sd As DateTime?
        Public default_ed As DateTime?

        Public data_unit As enum_data_unit = enum_data_unit.month

        Public Sub New(item_name As String, file_name As String, stie_type() As String)
            Me.item_name = item_name
            Me.file_name = file_name
            Me.stie_type = stie_type
        End Sub

    End Class


    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim ary As New List(Of class_kind_json)
        'ary.Add(New class_kind_json("二氧化硫", "so2", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("懸浮微粒", "pm10", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("臭氧", "o3", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("一氧化碳", "co", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("二氧化碳", "co2", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("二氧化氮", "no2", {"工業", "一般", "交通"}) _
        '      With {.default_sd = New DateTime(83 + 1911, 1, 1)})
        'ary.Add(New class_kind_json("非甲烷碳氫化合物", "NMHC", {"一般", "交通"}))

        ary.Add(New class_kind_json("細懸浮微粒", "pm25", {"工業", "一般", "交通"}) _
            With {.data_unit = class_kind_json.enum_data_unit.year, .default_sd = New DateTime(94 + 1911, 1, 1)})

        ary.Add(New class_kind_json("PSI", "psi", {"工業", "一般", "交通"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})

        ary.Add(New class_kind_json("PSI_COUNT_PERCENT", "psi_count", {"工業", "一般", "交通"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})

        'ary.Add(New class_kind_json("臭氧", "o3", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("鉛", "pb", {""}))

        'ary.Add(New class_kind_json("空氣污染防制", "air_city_month", {""}) _
        '        With {.data_unit = class_kind_json.enum_data_unit.year, .default_sd = New DateTime(2001, 1, 1)})

        'ary.Add(New class_kind_json("細懸浮微粒_Data2", "pm25_d2", {""}) _
        '    With {.default_sd = New DateTime(94 + 1911, 1, 1)})

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            For k As Integer = 0 To data.stie_type.Count - 1
                create_data(data.item_name, data.file_name, data.stie_type(k),
                            data.default_sd, data.default_ed, data.data_unit)
            Next
        Next

        Response.Write("OK_" & DateTime.Now.ToString)

    End Sub

    Private Sub create_data(ITEM_NAME As String, FileName As String, SITE_TYPE As String,
                            start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit)

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim sql As String = " select t1.* from AirQualityMonth t1 " &
                        " left join AirStation t2 on  t1.COUNTY = t2.COUNTY And t1.SITENAME = t2.SITENAME " &
                        " where t1.ITEMNAME = @ITEMNAME and (t2.SITETYPE = @SITETYPE or @SITETYPE = '') "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", ITEM_NAME))
        para.Add(da.CreateParameter("SITETYPE", SITE_TYPE))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql_order, para.ToArray())
        If dt_data.Rows.Count = 0 Then Exit Sub

        Dim min_yy = dt_data.Rows(0)("MONITORYEAR")
        Dim min_mm = 1
        If data_unit = class_kind_json.enum_data_unit.month Then
            min_mm = dt_data.Rows(0)("MONITORMONTH")
        End If

        Dim max_yy As Integer = dt_data.Rows(dt_data.Rows.Count - 1)("MONITORYEAR")
        Dim max_mm As Integer = 1
        If data_unit = class_kind_json.enum_data_unit.month Then
            max_mm = dt_data.Rows(dt_data.Rows.Count - 1)("MONITORMONTH")
        End If

        Dim sd As New DateTime(min_yy, min_mm, 1)
        Dim d_83 As New DateTime(83 + 1911, 1, 1)
        If sd < d_83 Then
            sd = New DateTime(83 + 1911, 1, 1)
        End If
        Dim ed As New DateTime(max_yy, max_mm, 1)

        If start_date IsNot Nothing Then sd = start_date
        If end_date IsNot Nothing Then ed = end_date

        Dim city_code As Data.DataTable = da.GetDataTable("select * from Country")

        Dim st_name As String = ""
        Select Case SITE_TYPE
            Case "工業"
                st_name = "industry"
            Case "交通"
                st_name = "traffic"
            Case "一般"
                st_name = "normal"
            Case ""
                st_name = "all"
        End Select


        Dim json_data As New List(Of String)
        json_data.AddRange(get_area_json("ALL2", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))
        json_data.AddRange(get_area_json("ALL", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))
        json_data.AddRange(get_area_json("Air", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))
        json_data.AddRange(get_area_json("City", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))
        json_data.AddRange(get_area_json("Station", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))

        Dim json As String = "[" & String.Join("," & vbCrLf, json_data.ToArray()) & "]"
        Dim result As String = "var chart_" & FileName & "_" & st_name & " = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/AirQualityChart/chart_" & FileName & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()


    End Sub

    Private Function get_data_json(data As Data.DataTable,
                             StartDate As DateTime, EndDate As DateTime, level As String, area_code As String, area() As String,
                             Optional other_p As String = "") As String

        Dim m_count As Integer = DateDiff(DateInterval.Month, StartDate, EndDate)
        Dim js_time As New List(Of String)

        Dim data_area() As Data.DataRow = data.Select()
        If area.Length > 0 Then
            data_area = (From r As Data.DataRow In data.Rows
                         Where area.Contains("" & r("COUNTY"))).ToArray()
        End If

        For i As Integer = 0 To m_count
            Dim month As DateTime = StartDate.AddMonths(i)
            Dim value As Double?
            Dim value_ary = (From r In data_area
                             Where r("MONITORYEAR") = month.Year _
                                 And r("MONITORMONTH") = month.Month _
                                 And "" & r("MONTHLYVALUE") <> ""
                             Order By r("MONTHLYVALUE")
                             Select CDbl(r("MONTHLYVALUE"))
                             ).ToArray()

            If value_ary.Count > 0 Then
                value = value_ary.Average
            End If

            Dim str_value As String = "null"
            If value IsNot Nothing Then
                Dim v As Double = value
                str_value = Math.Round(v, 4).ToString()
            End If

            Dim s As String = String.Format("[{0},{1}]",
                                             Me.GetTimeText(month.Year, month.Month), str_value)
            js_time.Add(s)
        Next

        Dim j2 As String = String.Join(",", js_time.ToArray())
        Dim json As String = String.Format("{{""level"":""{2}"", ""area"":""{0}""" & other_p & ", ""time_value"":[{1}] }}",
                                           area_code, j2, level)

        Return json
    End Function

    Private Function GetTimeText(year As Integer, month As Integer) As String
        Dim y1 As New DateTime(1970, 1, 1)
        Dim y2 As New DateTime(year, month, 1)
        Dim s As Int64 = DateDiff(DateInterval.Second, y1, y2) * 1000
        Return s.ToString()
    End Function

    Private Function get_area_json(leval As String, item As String,
                                   site As String, sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit) As String()

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))
        para.Add(da.CreateParameter("SITETYPE", site))

        Select Case leval
            Case "ALL", "ALL2"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, 'ALL' as code " &
                    " From AirQualityMonth t1 " &
                    " left join AirStation t2 on t1.[SITENAME] = t2.[SITENAME] " &
                    " left join country_area t3 on t3.[COUNTRY] = t1.[COUNTY] " &
                    " where t1.[ITEMNAME] = @ITEMNAME and  ( t2.[SITETYPE] = @SITETYPE or @SITETYPE = '' ) " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH]  "

            Case "Air"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH,  " &
                    "convert(varchar, t3.[AreaId]) as code " &
                    " From AirQualityMonth t1 " &
                    " left join AirStation t2 on t1.[SITENAME] = t2.[SITENAME] " &
                    " left join country_area t3 on t3.[COUNTRY] = t1.[COUNTY] " &
                    " where t1.[ITEMNAME] = @ITEMNAME and  ( t2.[SITETYPE] = @SITETYPE or @SITETYPE = '' ) " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[AreaId]  "

            Case "City"

                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                 " convert(varchar, t3.[CODE]) as code " &
                 " From AirQualityMonth t1 " &
                 " left join AirStation t2 on t1.[SITENAME] = t2.[SITENAME] " &
                 " left join country_area t3 on t3.[COUNTRY] = t1.[COUNTY] " &
                 " where t1.[ITEMNAME] = @ITEMNAME and  ( t2.[SITETYPE] = @SITETYPE or @SITETYPE = '' ) " &
                 " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[CODE] "

            Case "Station"

                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                 "  convert( varchar, t2.[NUMBER]) as code, t3.CODE as c_code, t2.X, t2.Y, t2.SITENAME " &
                 " From AirQualityMonth t1 " &
                 " left join AirStation t2 on t1.[SITENAME] = t2.[SITENAME] " &
                 " left join country_area t3 on t3.[COUNTRY] = t1.[COUNTY] " &
                 " where t1.[ITEMNAME] = @ITEMNAME and  ( t2.[SITETYPE] = @SITETYPE or @SITETYPE = '' ) " &
                 " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t2.[NUMBER], t3.[CODE], t2.X, t2.Y, t2.SITENAME "

        End Select

        Dim unit_count As Integer = 0
        Select Case data_unit
            Case class_kind_json.enum_data_unit.month
                unit_count = DateDiff(DateInterval.Month, sd, ed)
            Case class_kind_json.enum_data_unit.year
                unit_count = DateDiff(DateInterval.Year, sd, ed)
        End Select

        Dim dt As Data.DataTable = da.GetDataTable(sql, para.ToArray())

        Dim code_list = (From r As Data.DataRow In dt.Rows Select code = "" & r("code")).Distinct().ToArray()
        Dim json As New List(Of String)

        For c As Integer = 0 To UBound(code_list)
            Dim code As String = "" & code_list(c)
            Dim c_code As String = ""
            Dim X As String = ""
            Dim Y As String = ""
            Dim SITENAME As String = ""

            Dim time_value As New List(Of String)
            For m As Integer = 0 To unit_count

                Dim d As DateTime
                Dim rs() As Data.DataRow = {}
                Select Case data_unit
                    Case class_kind_json.enum_data_unit.month
                        d = sd.AddMonths(m)
                        rs = dt.Select(String.Format("MONITORYEAR = '{0}' and MONITORMONTH = '{1}' and code = '{2}' " _
                             , d.Year, d.Month, code))
                    Case class_kind_json.enum_data_unit.year
                        d = sd.AddYears(m)
                        rs = dt.Select(String.Format("MONITORYEAR = '{0}' and code = '{1}' " _
                           , d.Year, code))
                End Select

                Dim time As String = Me.GetTimeText(d.Year, d.Month)
                Dim value As String = "null"

                If rs.Length > 0 Then
                    If IsNumeric(rs(0)("value")) Then
                        value = "" & rs(0)("value")
                    End If
                    If leval = "Station" Then
                        c_code = "" & rs(0)("c_code")
                        X = "" & rs(0)("X")
                        Y = "" & rs(0)("Y")
                        SITENAME = "" & rs(0)("SITENAME")
                    End If
                Else
                    X = -1
                    Y = -1
                End If

                Dim t As String = String.Format("[{0}, {1}]", time, value)
                time_value.Add(t)
            Next

            If leval = "ALL2" Then
                code = "ALL2"
            End If

            Dim other As String = ""
            If leval = "Station" Then
                other = String.Format(", ""SITENAME"":""{0}"", ""x"": {1}, ""y"": {2}, ""CountyCode"": ""{3}"" ",
                                   "" & SITENAME, "" & X, "" & Y, "" & c_code)
            End If

            Dim s As String = String.Format("{{""level"":""{0}"", ""area"":""{1}"" " & other & " ,""time_value"": [",
                                             leval, code)

            s &= String.Join(",", time_value.ToArray()) & "] }"
            json.Add(s)

        Next

        Return json.ToArray()

    End Function

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '測站數量
        create_air_station()

        Response.Write("OK")
    End Sub

    Private Sub create_air_station()

        Dim da As New Jeff.DataAccess
        Dim sql As String = " select count(*) sc, year from AirStation where Year is not null  group by year order by year "
        Dim dt As Data.DataTable = da.GetDataTable(sql)

        Dim min_y As Integer = dt.Rows(0)("Year")
        Dim max_y As Integer = dt.Rows(dt.Rows.Count - 1)("Year")

        Dim unit_count As Integer = max_y - min_y - 1

        Dim code_list = {"ALL"}
        Dim json As New List(Of String)

        For c As Integer = 0 To UBound(code_list)
            Dim code As String = "" & code_list(c)
            Dim c_code As String = ""
            Dim X As String = ""
            Dim Y As String = ""
            Dim SITENAME As String = ""

            Dim time_value As New List(Of String)
            For m As Integer = 0 To unit_count

                Dim cy As Integer = min_y + m
                Dim s_count As Integer = dt.Compute("sum(sc)", "year <=" & cy)
                Dim d As New DateTime(cy + 1911, 1, 1)

                Dim time As String = Me.GetTimeText(d.Year, d.Month)
                Dim value As String = s_count

                Dim t As String = String.Format("[{0}, {1}]", time, value)
                time_value.Add(t)
            Next

            Dim other As String = ""

            Dim s As String = String.Format("{{""level"":""{0}"", ""area"":""{1}"" " & other & " ,""time_value"": [",
                                             "ALL", code)

            s &= String.Join(",", time_value.ToArray()) & "] }"
            json.Add(s)

        Next

        Dim ss As String = "[" & String.Join("," & vbCrLf, json.ToArray()) & "]"
        Dim result As String = "var chart_air_station_count  = " & vbCrLf & ss & ";"

        Dim path As String = Server.MapPath("../js/AirQualityChart/chart_air_station_count.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()



    End Sub

    Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("Area_PM_10", "Area_PM_10", {"1", "2", "3"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("Area_O3", "Area_O3", {"1", "2", "3"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            For k As Integer = 0 To data.stie_type.Count - 1
                create_data_area(data.item_name, data.file_name, data.stie_type(k),
                            data.default_sd, data.default_ed, data.data_unit)
            Next
        Next

        Response.Write("OK_" & DateTime.Now.ToString)

    End Sub

    Private Sub create_data_area(ITEM_NAME As String, FileName As String, SITE_TYPE As String,
                            start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit)

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim sql As String = " select t1.* from AirQualityMonth t1 " &
                        " left join AirStation t2 on  t1.COUNTY = t2.COUNTY And t1.SITENAME = t2.SITENAME " &
                        " where t1.ITEMNAME = @ITEMNAME "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", ITEM_NAME))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql_order, para.ToArray())
        If dt_data.Rows.Count = 0 Then Exit Sub

        Dim min_yy = dt_data.Rows(0)("MONITORYEAR")
        Dim min_mm = 1
        If data_unit = class_kind_json.enum_data_unit.month Then
            min_mm = dt_data.Rows(0)("MONITORMONTH")
        End If

        Dim max_yy As Integer = dt_data.Rows(dt_data.Rows.Count - 1)("MONITORYEAR")
        Dim max_mm As Integer = 1
        If data_unit = class_kind_json.enum_data_unit.month Then
            max_mm = dt_data.Rows(dt_data.Rows.Count - 1)("MONITORMONTH")
        End If

        Dim sd As New DateTime(min_yy, min_mm, 1)
        Dim d_83 As New DateTime(83 + 1911, 1, 1)
        If sd < d_83 Then
            sd = New DateTime(83 + 1911, 1, 1)
        End If
        Dim ed As New DateTime(max_yy, max_mm, 1)

        If start_date IsNot Nothing Then sd = start_date
        If end_date IsNot Nothing Then ed = end_date

        Dim city_code As Data.DataTable = da.GetDataTable("select * from Country")

        Dim st_name As String = SITE_TYPE
        'Select Case SITE_TYPE
        '    Case "工業"
        '        st_name = "industry"
        '    Case "交通"
        '        st_name = "traffic"
        '    Case "一般"
        '        st_name = "normal"
        '    Case ""
        '        st_name = "all"
        'End Select


        Dim json_data As New List(Of String)
        'json_data.AddRange(get_area_json_area("ALL2", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))
        json_data.AddRange(get_area_json_area("ALL", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))
        'json_data.AddRange(get_area_json_area("City", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))
        'json_data.AddRange(get_area_json_area("Area", ITEM_NAME, SITE_TYPE, sd, ed, data_unit))

        Dim json As String = "[" & String.Join("," & vbCrLf, json_data.ToArray()) & "]"
        Dim result As String = "var chart_" & FileName & "_" & st_name & " = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/AirQualityChart/chart_" & FileName & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()


    End Sub


    Private Function get_area_json_area(leval As String, item As String,
                                   MONTHLYVALUE As String, sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit) As String()

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))
        para.Add(da.CreateParameter("MONTHLYVALUE", MONTHLYVALUE))

        Select Case leval
            Case "ALL", "ALL2"
                sql = " select count(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, 'ALL' as code " &
                    " From AirQualityMonth t1 " &
                    " where t1.[ITEMNAME] = @ITEMNAME and  t1.MONTHLYVALUE = @MONTHLYVALUE " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH]  "

                'Case "City"

                '    sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                '     " convert(varchar, t3.[CODE]) as code " &
                '     " From AirQualityMonth t1 " &
                '     " left join country_area t3 on replace(t1.[COUNTY], '臺', '台') = replace(t1.[COUNTY], '臺', '台') " &
                '     " where t1.[ITEMNAME] = @ITEMNAME " &
                '     " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[CODE] "

                'Case "Area"

                '    sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                '      " convert(varchar, t3.[AirAreaid]) as code " &
                '     " From AirQualityMonth t1 " &
                '     " left join AirArea t3 on t3.[AirAreaName] = t1.[COUNTY] " &
                '     " where t1.[ITEMNAME] = @ITEMNAME " &
                '      " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[AirAreaid] "

        End Select

        Dim unit_count As Integer = 0
        Select Case data_unit
            Case class_kind_json.enum_data_unit.month
                unit_count = DateDiff(DateInterval.Month, sd, ed)
            Case class_kind_json.enum_data_unit.year
                unit_count = DateDiff(DateInterval.Year, sd, ed)
        End Select

        Dim dt As Data.DataTable = da.GetDataTable(sql, para.ToArray())

        Dim code_list = (From r As Data.DataRow In dt.Rows Select code = "" & r("code")).Distinct().ToArray()
        Dim json As New List(Of String)

        For c As Integer = 0 To UBound(code_list)
            Dim code As String = "" & code_list(c)
            Dim c_code As String = ""
            Dim X As String = ""
            Dim Y As String = ""
            Dim SITENAME As String = ""

            Dim time_value As New List(Of String)
            For m As Integer = 0 To unit_count

                Dim d As DateTime
                Dim rs() As Data.DataRow = {}
                Select Case data_unit
                    Case class_kind_json.enum_data_unit.month
                        d = sd.AddMonths(m)
                        rs = dt.Select(String.Format("MONITORYEAR = '{0}' and MONITORMONTH = '{1}' and code = '{2}' " _
                             , d.Year, d.Month, code))
                    Case class_kind_json.enum_data_unit.year
                        d = sd.AddYears(m)
                        rs = dt.Select(String.Format("MONITORYEAR = '{0}' and code = '{1}' " _
                           , d.Year, code))
                End Select

                Dim time As String = Me.GetTimeText(d.Year, d.Month)
                Dim value As String = "null"

                If rs.Length > 0 Then
                    If IsNumeric(rs(0)("value")) Then
                        value = Math.Ceiling(CDbl("" & rs(0)("value")))
                    End If
                    If leval = "Station" Then
                        c_code = "" & rs(0)("c_code")
                        X = "" & rs(0)("X")
                        Y = "" & rs(0)("Y")
                        SITENAME = "" & rs(0)("SITENAME")
                    End If
                Else
                    X = -1
                    Y = -1
                End If

                Dim t As String = String.Format("[{0}, {1}]", time, value)
                time_value.Add(t)
            Next

            If leval = "ALL2" Then
                code = "ALL2"
            End If

            Dim other As String = ""
            If leval = "Station" Then
                other = String.Format(", ""SITENAME"":""{0}"", ""x"": {1}, ""y"": {2}, ""CountyCode"": ""{3}"" ",
                                   "" & SITENAME, "" & X, "" & Y, "" & c_code)
            End If

            Dim s As String = String.Format("{{""level"":""{0}"", ""area"":""{1}"" " & other & " ,""time_value"": [",
                                             leval, code)

            s &= String.Join(",", time_value.ToArray()) & "] }"
            json.Add(s)

        Next

        Return json.ToArray()

    End Function




End Class
