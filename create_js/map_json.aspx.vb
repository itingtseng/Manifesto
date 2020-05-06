
Partial Class create_js_map_json
    Inherits System.Web.UI.Page

    Class class_kind_json
        Public Enum enum_data_unit
            month
            year
        End Enum

        Public item_name As String
        Public file_name As String
        Public stie_type() As String
        Public data_unit As enum_data_unit = enum_data_unit.month
        Public default_sd As DateTime?
        Public default_ed As DateTime?

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
        'ary.Add(New class_kind_json("二氧化氮", "no2", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("非甲烷碳氫化合物", "NMHC", {"一般", "交通"}))
        'ary.Add(New class_kind_json("細懸浮微粒", "pm25", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("臭氧", "o3", {"工業", "一般", "交通"}))
        'ary.Add(New class_kind_json("鉛", "pb", {""}))
        'ary.Add(New class_kind_json("PSI", "psi", {"工業", "一般", "交通"}))

        'ary.Add(New class_kind_json("PSI_COUNT_PERCENT", "psi_count", {"工業", "一般", "交通"}))

        'ary.Add(New class_kind_json("空氣污染防制", "air_city_month", {""}) _
        '        With {.data_unit = class_kind_json.enum_data_unit.year})

        ary.Add(New class_kind_json("細懸浮微粒", "pm25", {"工業", "一般", "交通"}) _
            With {.data_unit = class_kind_json.enum_data_unit.year, .default_sd = New DateTime(94 + 1911, 1, 1)})

        ary.Add(New class_kind_json("PSI", "psi", {"工業", "一般", "交通"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})

        ary.Add(New class_kind_json("PSI_COUNT_PERCENT", "psi_count", {"工業", "一般", "交通"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            For k As Integer = 0 To data.stie_type.Count - 1
                create_map_json(data.item_name, data.file_name, data.stie_type(k), data.data_unit)
            Next
        Next

        Response.Write("OK_MAP_" & DateTime.Now.ToString)

    End Sub

    Private Sub create_map_json(item_name As String, file_name As String, site_type As String, data_unit As class_kind_json.enum_data_unit)

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
        area_json.Add(get_area_json("ALL", "ALL", item_name, site_type, sd, ed, data_unit))

        'city
        area_json.Add(get_area_json("City", "City", item_name, site_type, sd, ed, data_unit))

        Dim result = "var map_" & file_name & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim path As String = Server.MapPath("../js/AirQualityMap/map_" & file_name & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Function get_area_json(leval As String, obj_id As String, item As String,
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

        Return text

    End Function

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("Area_PM_10", "Area_PM_10", {""}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("Area_O3", "Area_O3", {""}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            For k As Integer = 0 To data.stie_type.Count - 1
                create_map_json_air_area(data.item_name, data.file_name, data.stie_type(k), data.data_unit)
            Next
        Next

        Response.Write("OK_" & DateTime.Now.ToString)



    End Sub

    Private Sub create_map_json_air_area(item_name As String, file_name As String, site_type As String, data_unit As class_kind_json.enum_data_unit)

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

        Dim result = "var map_" & file_name & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim path As String = Server.MapPath("../js/AirQualityMap/map_" & file_name & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

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
        Response.Write(dt.Rows.Count)

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
