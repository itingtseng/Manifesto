Imports Microsoft.VisualBasic

Public Class AirData_p1

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


    Public Function GetDataTable(item As String) As Data.DataTable

        Dim da As New Jeff.DataAccess
        Dim sql As String = " select * from AirQualityMonth " &
                " where ITEMNAME = @ITEMNAME " &
                " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH]) "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql, para.ToArray())

        da.Dispose()
        Return dt_data

    End Function

    Public Function create_data(ITEM_NAME As String, Dir As String, FileName As String, SITE_TYPE As String,
                              start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit) As String

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
        If dt_data.Rows.Count = 0 Then Return ITEM_NAME & " - " & SITE_TYPE & ", 無資料"

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

        Dim file_name As String = "chart_" & FileName & "_" & st_name & ".js"
        Dim path As String = System.IO.Path.Combine(Dir, file_name)
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        da.Dispose()
        Return path

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

        Dim dt_station As Data.DataTable = da.GetDataTable(" select t1.*, t2.CODE as c_code From AirStation t1 " &
                                                           " left join country_area t2 on t1.COUNTY = t2.COUNTRY ")

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
            Dim X As String = "-1"
            Dim Y As String = "-1"
            Dim SITENAME As String = ""

            If leval = "Station" Then
                If IsNumeric(code_list(c)) Then
                    Dim row_s = dt_station.Select(String.Format("NUMBER = '{0}' ", "" & code_list(c)))
                    If row_s.Length > 0 Then
                        X = "" & row_s(0)("X")
                        Y = "" & row_s(0)("Y")
                        SITENAME = "" & row_s(0)("SITENAME")
                        c_code = "" & row_s(0)("c_code")
                    End If
                End If
            End If

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
                    'If leval = "Station" Then
                    '    c_code = "" & rs(0)("c_code")
                    '    X = "" & rs(0)("X")
                    '    Y = "" & rs(0)("Y")
                    '    SITENAME = "" & rs(0)("SITENAME")
                    'End If
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

        da.Dispose()
        Return json.ToArray()

    End Function

    Public Function create_map_json(item_name As String, Dir As String, FileName As String, site_type As String,
                                     start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit) As String

        '年度
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select t1.* from AirQualityMonth t1 " &
                        " left join AirStation t2 on  t1.COUNTY = t2.COUNTY And t1.SITENAME = t2.SITENAME " &
                        " where t1.ITEMNAME = @ITEMNAME and (t2.SITETYPE = @SITETYPE or @SITETYPE = '') "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item_name))
        para.Add(da.CreateParameter("SITETYPE", site_type))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql_order, para.ToArray())
        If dt_data.Rows.Count = 0 Then Return item_name & " - " & site_type & ", 無資料"

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
        Dim ed As New DateTime(max_yy, max_mm, 1)

        If start_date IsNot Nothing Then sd = start_date
        If end_date IsNot Nothing Then ed = end_date

        Dim st_name As String = ""
        Select Case site_type
            Case "工業"
                st_name = "industry"
            Case "交通"
                st_name = "traffic"
            Case "一般"
                st_name = "normal"
            Case ""
                st_name = "all"
        End Select

        Dim area_json As New List(Of String)
        area_json.Add(get_map_json("ALL", "ALL", item_name, site_type, sd, ed, data_unit))

        'city
        area_json.Add(get_map_json("City", "City", item_name, site_type, sd, ed, data_unit))

        Dim result = "var map_" & FileName & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim file_name As String = "map_" & FileName & "_" & st_name & ".js"
        Dim path As String = System.IO.Path.Combine(Dir, file_name)

        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

        da.Dispose()
        Return path

    End Function

    Private Function get_map_json(leval As String, obj_id As String, item As String,
                                   site As String, sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit) As String

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))
        para.Add(da.CreateParameter("SITETYPE", site))

        Dim code_list() As Object = {}

        Select Case leval
            Case "ALL"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                    "  convert(varchar, t3.[AreaId]) as code " &
                    " From AirQualityMonth t1 " &
                    " left join AirStation t2 on t1.[SITENAME] = t2.[SITENAME] " &
                    " left join country_area t3 on t3.[COUNTRY] = t1.[COUNTY] " &
                    " where t1.[ITEMNAME] = @ITEMNAME and  ( t2.[SITETYPE] = @SITETYPE or @SITETYPE = '' ) " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[AreaId]  " &
                    " order by t3.[AreaId] "

                Dim sql_code As String = " select * from AirQualityArea "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("sn")).ToArray()

            Case "City"

                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                    " convert(varchar, t3.[CODE]) as code " &
                 " From AirQualityMonth t1 " &
                 " left join AirStation t2 on t1.[SITENAME] = t2.[SITENAME] " &
                 " left join country_area t3 on t3.[COUNTRY] = t1.[COUNTY] " &
                 " where t1.[ITEMNAME] = @ITEMNAME and  ( t2.[SITETYPE] = @SITETYPE or @SITETYPE = '' ) " &
                 " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[CODE] " &
                 " order by t3.[CODE]  "

                para.Add(da.CreateParameter("AreaId", obj_id))

                Dim sql_code As String = " select * from country_area "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("CODE")).ToArray()

        End Select

        Dim unit_count As Integer = 0
        Select Case data_unit
            Case class_kind_json.enum_data_unit.month
                unit_count = DateDiff(DateInterval.Month, sd, ed)
            Case class_kind_json.enum_data_unit.year
                unit_count = DateDiff(DateInterval.Year, sd, ed)
        End Select

        Dim dt As Data.DataTable = da.GetDataTable(sql, para.ToArray())

        Dim value_list As New List(Of String)
        For m As Integer = 0 To unit_count
            Dim d As DateTime = sd.AddMonths(m)

            Dim data_list As New List(Of String)
            For c As Integer = 0 To code_list.Count - 1

                Dim code As String = "" & code_list(c)
                Dim value As String = "null"
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

                If rs.Count > 0 Then
                    If IsNumeric(rs(0)("value")) Then
                        value = "" & rs(0)("value")
                    End If
                End If

                Dim s As String = "{{""code"":""{0}"", ""value"":{1} }}"
                If value.Trim() = "" Then value = "null"
                s = String.Format(s, code, value)
                data_list.Add(s)

            Next

            Dim data As String = "[" & String.Join(",", data_list.ToArray()) & "]"
            Dim y As String = "{{""year"": ""{0}"", ""data"": {1} }}"
            Dim ym As String = d.Year & "_" & d.Month.ToString("00")
            y = String.Format(y, ym, data)
            value_list.Add(y)
        Next

        Dim text As String = String.Format("{{ ""level"": ""{0}"", ""area"": ""{1}"", ""value"":[ ",
                                           leval, obj_id)

        text &= vbCrLf &
            String.Join("," & vbCrLf, value_list.ToArray()) & vbCrLf &
            "] }"

        da.Dispose()
        Return text

    End Function

    ''' <summary>
    ''' 空品區
    ''' </summary>
    ''' <param name="ITEM_NAME"></param>
    ''' <param name="FileName"></param>
    ''' <param name="SITE_TYPE"></param>
    ''' <param name="start_date"></param>
    ''' <param name="end_date"></param>
    ''' <param name="data_unit"></param>
    Public Function create_data_area(ITEM_NAME As String, Dir As String, FileName As String, SITE_TYPE As String,
                              start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit) As String

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
        If dt_data.Rows.Count = 0 Then Return ITEM_NAME & " - " & SITE_TYPE & ", 無資料"

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

        Dim file_name As String = "chart_" & FileName & "_" & st_name & ".js"
        Dim path As String = System.IO.Path.Combine(Dir, file_name)
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        Return path

    End Function


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

    Public Function create_map_json_air_area(item_name As String, Dir As String, FileName As String, site_type As String,
                                     start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit) As String

        '年度
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select t1.* from AirQualityMonth t1 " &
                        " left join AirStation t2 on  t1.COUNTY = t2.COUNTY And t1.SITENAME = t2.SITENAME " &
                        " where t1.ITEMNAME = @ITEMNAME and (t2.SITETYPE = @SITETYPE or @SITETYPE = '') "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item_name))
        para.Add(da.CreateParameter("SITETYPE", site_type))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql_order, para.ToArray())
        If dt_data.Rows.Count = 0 Then Return item_name & " - " & site_type & ", 無資料"

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
        Dim ed As New DateTime(max_yy, max_mm, 1)

        Dim st_name As String = ""
        Select Case site_type
            Case "工業"
                st_name = "industry"
            Case "交通"
                st_name = "traffic"
            Case "一般"
                st_name = "normal"
            Case ""
                st_name = "all"
        End Select

        Dim area_json As New List(Of String)

        'city
        area_json.Add(get_area_json_area_air("City", "City", item_name, site_type, sd, ed, data_unit))
        area_json.Add(get_area_json_area_air("Area", "Area", item_name, site_type, sd, ed, data_unit))

        Dim result = "var map_" & FileName & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim file_name As String = "map_" & FileName & "_" & st_name & ".js"
        Dim path As String = System.IO.Path.Combine(Dir, file_name)

        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

        da.Dispose()
        Return path

    End Function

    Private Function get_area_json_area_air(leval As String, obj_id As String, item As String,
                                   site As String, sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit) As String

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))

        Dim code_list() As Object = {}

        Select Case leval
            Case "City"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                    "  convert(varchar, t3.[CODE]) as code " &
                    " From AirQualityMonth t1 " &
                    " left join country_area t3 on replace(t3.COUNTRY, '臺', '台') = replace(t1.[COUNTY], '臺', '台') " &
                    " where t1.[ITEMNAME] = @ITEMNAME and t3.COUNTRY is not null " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[CODE]  " &
                    " order by t3.[CODE] "

                Dim sql_code As String = " select * from country_area "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("CODE")).ToArray()

            Case "Area"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                    "  convert(varchar, t3.[AirAreaid]) as code " &
                    " From AirQualityMonth t1 " &
                    " left join AirArea t3 on t3.[AirAreaName] = t1.[COUNTY] " &
                    " where t1.[ITEMNAME] = @ITEMNAME and t3.AirAreaid is not null " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[AirAreaid]  " &
                    " order by t3.[AirAreaid] "

                Dim sql_code As String = " select * from AirArea "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("AirAreaid")).ToArray()

        End Select

        Dim unit_count As Integer = 0
        Select Case data_unit
            Case class_kind_json.enum_data_unit.month
                unit_count = DateDiff(DateInterval.Month, sd, ed)
            Case class_kind_json.enum_data_unit.year
                unit_count = DateDiff(DateInterval.Year, sd, ed)
        End Select

        Dim dt As Data.DataTable = da.GetDataTable(sql, para.ToArray())

        Dim value_list As New List(Of String)
        For m As Integer = 0 To unit_count
            Dim d As DateTime = sd.AddMonths(m)

            Dim data_list As New List(Of String)
            For c As Integer = 0 To code_list.Count - 1

                Dim code As String = "" & code_list(c)
                Dim value As String = "null"
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

                If rs.Count > 0 Then
                    If IsNumeric(rs(0)("value")) Then
                        value = "" & rs(0)("value")
                    End If
                End If

                Dim s As String = "{{""code"":""{0}"", ""value"":{1} }}"
                If value.Trim() = "" Then value = "null"
                s = String.Format(s, code, value)
                data_list.Add(s)

            Next

            Dim data As String = "[" & String.Join(",", data_list.ToArray()) & "]"
            Dim y As String = "{{""year"": ""{0}"", ""data"": {1} }}"
            Dim ym As String = d.Year & "_" & d.Month.ToString("00")
            y = String.Format(y, ym, data)
            value_list.Add(y)
        Next

        Dim text As String = String.Format("{{ ""level"": ""{0}"", ""area"": ""{1}"", ""value"":[ ",
                                           leval, obj_id)

        text &= vbCrLf &
            String.Join("," & vbCrLf, value_list.ToArray()) & vbCrLf &
            "] }"

        Return text

    End Function







End Class
