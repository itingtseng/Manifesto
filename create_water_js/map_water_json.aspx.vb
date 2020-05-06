
Partial Class create_js_map_water_json
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
        Public group_type As String = "avg"

        Public Sub New(item_name As String, file_name As String)
            Me.item_name = item_name
            Me.file_name = file_name
            Me.stie_type = stie_type
        End Sub

    End Class


    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("懸浮固體", "ss"))
        ary.Add(New class_kind_json("溶氧(電極法)", "do"))
        ary.Add(New class_kind_json("氨氮", "amm"))
        ary.Add(New class_kind_json("RPI", "rpi"))
        ary.Add(New class_kind_json("生化需氧量", "bod"))

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            create_map_json(data.item_name, data.file_name, data.data_unit)
        Next

        Response.Write("OK_MAP_" & DateTime.Now.ToString)

    End Sub

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("km_none", "type1_km_none") With {.group_type = "sum"})
        ary.Add(New class_kind_json("km_mild", "type1_km_mild") With {.group_type = "sum"})
        ary.Add(New class_kind_json("km_mod", "type1_km_mod") With {.group_type = "sum"})
        ary.Add(New class_kind_json("km_severe", "type1_km_severe") With {.group_type = "sum"})
        ary.Add(New class_kind_json("percent_none", "type1_percent_none"))
        ary.Add(New class_kind_json("percent_mild", "type1_percent_mild"))
        ary.Add(New class_kind_json("percent_mod", "type1_percent_mod"))
        ary.Add(New class_kind_json("percent_severe", "type1_percent_severe"))

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            create_map_json_type1(data.item_name, data.file_name, data.group_type)
        Next

        Response.Write("OK_" & DateTime.Now.ToString)

    End Sub

    Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("s4_house", "s4_house") With {.group_type = "sum"})
        ary.Add(New class_kind_json("s4_water", "s4_water") With {.group_type = "sum"})

        ary.Add(New class_kind_json("s8_人口數", "s8_man") With {.data_unit = class_kind_json.enum_data_unit.year, .group_type = "sum"})
        ary.Add(New class_kind_json("s8_列管家數", "s8_factory") With {.data_unit = class_kind_json.enum_data_unit.year, .group_type = "sum"})
        ary.Add(New class_kind_json("s8_工業廢水削減量", "s8_ind_d") With {.group_type = "sum", .data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s8_工業廢水產生量", "s8_ind_m") With {.group_type = "sum", .data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s8_生活污水削減量", "s8_life_d") With {.group_type = "sum", .data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s8_生活污水產生量", "s8_life_m") With {.group_type = "sum", .data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s8_畜牧現有頭數", "s8_animal_count") With {.group_type = "sum", .data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s8_畜牧廢水削減量", "s8_animal_d") With {.group_type = "sum", .data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s8_畜牧廢水產生量", "s8_animal_m") With {.group_type = "sum", .data_unit = class_kind_json.enum_data_unit.year})

        ary.Add(New class_kind_json("水污染防治", "s3_money") With {.data_unit = class_kind_json.enum_data_unit.year})

        ary.Add(New class_kind_json("自來水", "s7_rw"))
        ary.Add(New class_kind_json("直接供水", "s7_dw"))
        ary.Add(New class_kind_json("飲用水", "s7_drink"))
        ary.Add(New class_kind_json("簡易自來水", "s7_erw"))

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            create_map_json_city(data.item_name, data.file_name, data.data_unit, data.group_type)
        Next

        Response.Write("OK_MAP_" & DateTime.Now.ToString)

    End Sub

    Private Sub create_map_json(item_name As String, file_name As String, data_unit As class_kind_json.enum_data_unit)

        '年度
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select t1.* from WaterMonthValue t1 " &
                        " where t1.ITEMNAME = @ITEMNAME  "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item_name))
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

        Dim st_name As String = "all"
        Dim area_json As New List(Of String)
        area_json.Add(get_area_json("ALL", "ALL", item_name, sd, ed, data_unit))

        '流域
        'area_json.Add(get_area_json("Basin", "Basin", item_name, sd, ed, data_unit))

        Dim result = "var map_" & file_name & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim path As String = Server.MapPath("../js/WaterMap/map_" & file_name & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Sub create_map_json_type1(field As String, file_name As String, group_type As String)

        '年度
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select t1.* from RiverData_Type1 t1 " &
                        " order by convert(int, [MONITORYEAR]) "

        Dim dt_data As Data.DataTable = da.GetDataTable(sql)

        Dim min_yy = dt_data.Rows(0)("MONITORYEAR")
        Dim min_mm = 1

        Dim max_yy As Integer = dt_data.Rows(dt_data.Rows.Count - 1)("MONITORYEAR")
        Dim max_mm As Integer = 1

        Dim sd As New DateTime(min_yy, min_mm, 1)
        Dim ed As New DateTime(max_yy, max_mm, 1)

        Dim st_name As String = "all"
        Dim area_json As New List(Of String)
        area_json.Add(get_area_json_type1("ALL", "ALL", field, sd, ed, group_type))

        '流域
        'area_json.Add(get_area_json("Basin", "Basin", item_name, sd, ed, data_unit))

        Dim result = "var map_" & file_name & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim path As String = Server.MapPath("../js/WaterMap/map_" & file_name & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Function get_area_json(leval As String, obj_id As String, item As String,
                                    sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit) As String

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))

        Dim code_list() As Object = {}

        Select Case leval
            Case "ALL"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                    "  convert(varchar, t3.[BasinId]) as code " &
                    " From WaterMonthValue t1 " &
                    " left join RiverSite t2 on t1.[SITENAME] = t2.[SITENAME] " &
                    " left join WaterBasin t3 on t3.[BasinName] = t2.[Basin] " &
                    " where t1.[ITEMNAME] = @ITEMNAME " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[BasinId]  " &
                    " order by t3.[BasinId] "

                Dim sql_code As String = " select * from WaterBasin "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("BasinId")).ToArray()

            Case "Basin"

                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                    " convert(varchar, t3.[BasinId]) as code " &
                 " From WaterMonthValue t1 " &
                 " left join RiverSite t2 on t1.[SITENAME] = t2.[SiteName] " &
                 " left join WaterBasin t3 on t3.[BasinName] = t2.[Basin] " &
                 " where t1.[ITEMNAME] = @ITEMNAME " &
                 " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[BasinId] " &
                 " order by t3.[BasinId]  "

                para.Add(da.CreateParameter("AreaId", obj_id))

                Dim sql_code As String = " select * from WaterBasin "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("BasinId")).ToArray()

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

    Private Function get_area_json_type1(leval As String, obj_id As String, field As String,
                                    sd As DateTime, ed As DateTime, group_type As String) As String

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""

        Dim code_list() As Object = {}

        Select Case leval
            Case "ALL"
                sql = " select " & group_type & "(t1.[{0}]) as value, t1.MONITORYEAR, " &
                    "  convert(varchar, t3.[BasinId]) as code " &
                    " From RiverData_Type1 t1 " &
                    " left join WaterBasin t3 on t3.[BasinName] = t1.[BasinName] " &
                    " group by t1.[MONITORYEAR], t3.[BasinId]  " &
                    " order by t3.[BasinId] "

                Dim sql_code As String = " select * from WaterBasin "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("BasinId")).ToArray()

        End Select

        sql = String.Format(sql, field)

        Dim unit_count As Integer = 0
        unit_count = DateDiff(DateInterval.Year, sd, ed)

        Dim dt As Data.DataTable = da.GetDataTable(sql)

        Dim value_list As New List(Of String)
        For m As Integer = 0 To unit_count
            Dim d As DateTime = sd.AddMonths(m)

            Dim data_list As New List(Of String)
            For c As Integer = 0 To code_list.Count - 1

                Dim code As String = "" & code_list(c)
                Dim value As String = "null"
                Dim rs() As Data.DataRow = {}

                d = sd.AddYears(m)
                rs = dt.Select(String.Format("MONITORYEAR = '{0}' and code = '{1}' " _
                           , d.Year, code))

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


    Private Sub create_map_json_city(item_name As String, file_name As String,
                                     data_unit As class_kind_json.enum_data_unit, Optional group_type As String = "avg")

        '年度
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select t1.* from WaterCityMonthValue t1 " &
                        " where t1.ITEMNAME = @ITEMNAME  "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item_name))
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

        Dim st_name As String = "all"
        Dim area_json As New List(Of String)
        area_json.Add(get_area_json_city("ALL", "ALL", item_name, sd, ed, data_unit, group_type))

        '流域
        'area_json.Add(get_area_json("Basin", "Basin", item_name, sd, ed, data_unit))

        Dim result = "var map_" & file_name & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim path As String = Server.MapPath("../js/WaterMap/map_" & file_name & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Function get_area_json_city(leval As String, obj_id As String, item As String,
                                    sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit, group_type As String) As String

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))

        Dim code_list() As Object = {}

        Select Case leval
            Case "ALL"
                sql = " select {0}(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                    "  convert(varchar, t2.[CODE]) as code " &
                    " From WaterCityMonthValue t1 " &
                    " left join Country t2 on t2.[COUNTRY] = t1.[Country] " &
                    " where t1.[ITEMNAME] = @ITEMNAME " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t2.[CODE]  " &
                    " order by  t2.[CODE] "

                Dim sql_code As String = " select * from Country "
                Dim dt_code = da.GetDataTable(sql_code)
                code_list = (From r As Data.DataRow In dt_code.Rows Select n = "" & r("CODE")).ToArray()

        End Select

        sql = String.Format(sql, group_type)

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


    Protected Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        'create point
        create_point()
    End Sub

    Private Sub create_point()

        Dim da As New Jeff.DataAccess
        Dim sql As String = " select distinct type from water_point "
        Dim dt_type As Data.DataTable = da.GetDataTable(sql)

        Dim dt_point As Data.DataTable = da.GetDataTable("select * from water_point")

        Dim point_list As New List(Of String)
        For i As Integer = 0 To dt_type.Rows.Count - 1

            Dim ary As New List(Of String)
            Dim rs() As Data.DataRow = dt_point.Select(String.Format("type = '{0}' ", "" & dt_type.Rows(i)("type")))
            For k As Integer = 0 To rs.Count - 1
                Dim row As Data.DataRow = rs(k)

                Dim v As String = "{{ ""name"": ""{0}"", ""x"": {1}, ""y"": {2} }} "
                v = String.Format(v, "" & row("name"), "" & row("x"), "" & row("y"))
                ary.Add(v)
            Next

            Dim bb As String = "var " & dt_type.Rows(i)("type") & "_point = [ " & vbCrLf &
                        String.Join(", " & vbCrLf, ary.ToArray()) & vbCrLf & " ];"

            point_list.Add(bb)

        Next

        Dim ss As String = String.Join(vbCrLf, point_list.ToArray())

        Dim path As String = Server.MapPath("../js/common_water/point_list.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(ss)
        sw.Close()
        fn.Close()

        Response.Write("OK")

    End Sub

End Class
