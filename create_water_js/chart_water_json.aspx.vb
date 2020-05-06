
Partial Class create_js_chart_water_json
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
            create_data(data.item_name, data.file_name,
                            data.default_sd, data.default_ed, data.data_unit)
        Next

        Response.Write("OK_" & DateTime.Now.ToString)

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
            create_data_type1(data.item_name, data.file_name, data.group_type)
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

        ary.Add(New class_kind_json("水污染防治", "s3_money") _
                With {.data_unit = class_kind_json.enum_data_unit.year, .default_sd = New DateTime(2001, 1, 1)})

        ary.Add(New class_kind_json("自來水", "s7_rw"))
        ary.Add(New class_kind_json("直接供水", "s7_dw"))
        ary.Add(New class_kind_json("飲用水", "s7_drink"))
        ary.Add(New class_kind_json("簡易自來水", "s7_erw"))


        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            create_data_city(data.item_name, data.file_name,
                            data.default_sd, data.default_ed, data.data_unit, data.group_type)
        Next

        Response.Write("OK_" & DateTime.Now.ToString)

    End Sub

    Protected Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("s5_水庫優養", "s5_reservoir") With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s6_percent", "s6_percent") With {.data_unit = class_kind_json.enum_data_unit.year})

        ary.Add(New class_kind_json("s6_ph", "s6_ph") With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s6_汞", "s6_hg") With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s6_溶氧量", "s6_do") With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s6_鉛", "s6_pb") With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s6_銅", "s6_cu") With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s6_鋅", "s6_zn") With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("s6_鎘", "s6_cd") With {.data_unit = class_kind_json.enum_data_unit.year})

        ary.Add(New class_kind_json("s9_beach", "s9_beach") With {.data_unit = class_kind_json.enum_data_unit.year})

        For i As Integer = 0 To ary.Count - 1
            Dim data = ary(i)
            create_data_point(data.item_name, data.file_name,
                            data.default_sd, data.default_ed, data.data_unit)
        Next

        Response.Write("OK_" & DateTime.Now.ToString)

    End Sub

    Private Sub create_data_type1(field As String, FileName As String, group_type As String)

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
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

        Dim json_data As New List(Of String)
        json_data.AddRange(get_area_json_type1("ALL2", field, sd, ed, group_type))
        json_data.AddRange(get_area_json_type1("ALL", field, sd, ed, group_type))
        json_data.AddRange(get_area_json_type1("Air", field, sd, ed, group_type))

        Dim json As String = "[" & String.Join("," & vbCrLf, json_data.ToArray()) & "]"
        Dim result As String = "var chart_" & FileName & "_" & st_name & " = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/WaterChart/chart_" & FileName & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Sub create_data(ITEM_NAME As String, FileName As String,
                            start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit)

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim sql As String = " select t1.* from WaterMonthValue t1 " &
                        " where t1.ITEMNAME = @ITEMNAME  "

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
        'Dim d_83 As New DateTime(83 + 1911, 1, 1)
        'If sd < d_83 Then
        '    sd = New DateTime(83 + 1911, 1, 1)
        'End If
        Dim ed As New DateTime(max_yy, max_mm, 1)

        If start_date IsNot Nothing Then sd = start_date
        If end_date IsNot Nothing Then ed = end_date

        Dim city_code As Data.DataTable = da.GetDataTable("select * from Country")

        Dim st_name As String = "all"

        Dim json_data As New List(Of String)
        json_data.AddRange(get_area_json("ALL2", ITEM_NAME, sd, ed, data_unit))
        json_data.AddRange(get_area_json("ALL", ITEM_NAME, sd, ed, data_unit))
        json_data.AddRange(get_area_json("Air", ITEM_NAME, sd, ed, data_unit))
        json_data.AddRange(get_area_json("Station", ITEM_NAME, sd, ed, data_unit))

        Dim json As String = "[" & String.Join("," & vbCrLf, json_data.ToArray()) & "]"
        Dim result As String = "var chart_" & FileName & "_" & st_name & " = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/WaterChart/chart_" & FileName & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()


    End Sub

    Private Sub create_data_city(ITEM_NAME As String, FileName As String,
                            start_date As DateTime?, end_date As DateTime?,
                                 data_unit As class_kind_json.enum_data_unit, Optional gropu_type As String = "avg")

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim sql As String = " select t1.* from WaterCityMonthValue t1 " &
                        " where t1.ITEMNAME = @ITEMNAME  "

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
        'Dim d_83 As New DateTime(83 + 1911, 1, 1)
        'If sd < d_83 Then
        '    sd = New DateTime(83 + 1911, 1, 1)
        'End If
        Dim ed As New DateTime(max_yy, max_mm, 1)

        If start_date IsNot Nothing Then sd = start_date
        If end_date IsNot Nothing Then ed = end_date

        Dim city_code As Data.DataTable = da.GetDataTable("select * from Country")

        Dim st_name As String = "all"

        Dim json_data As New List(Of String)
        json_data.AddRange(get_area_json_city("ALL2", ITEM_NAME, sd, ed, data_unit, gropu_type))
        json_data.AddRange(get_area_json_city("ALL", ITEM_NAME, sd, ed, data_unit, gropu_type))
        json_data.AddRange(get_area_json_city("City", ITEM_NAME, sd, ed, data_unit, gropu_type))

        Dim json As String = "[" & String.Join("," & vbCrLf, json_data.ToArray()) & "]"
        Dim result As String = "var chart_" & FileName & "_" & st_name & " = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/WaterChart/chart_" & FileName & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()


    End Sub

    Private Sub create_data_point(ITEM_NAME As String, FileName As String,
                            start_date As DateTime?, end_date As DateTime?, data_unit As class_kind_json.enum_data_unit)

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim sql As String = " select t1.* from WaterCityMonthValue t1 " &
                        " where t1.ITEMNAME = @ITEMNAME  "

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
        'Dim d_83 As New DateTime(83 + 1911, 1, 1)
        'If sd < d_83 Then
        '    sd = New DateTime(83 + 1911, 1, 1)
        'End If
        Dim ed As New DateTime(max_yy, max_mm, 1)

        If start_date IsNot Nothing Then sd = start_date
        If end_date IsNot Nothing Then ed = end_date

        Dim city_code As Data.DataTable = da.GetDataTable("select * from Country")

        Dim st_name As String = "all"

        Dim json_data As New List(Of String)
        json_data.AddRange(get_area_json_point("ALL2", ITEM_NAME, sd, ed, data_unit))
        json_data.AddRange(get_area_json_point("ALL", ITEM_NAME, sd, ed, data_unit))
        json_data.AddRange(get_area_json_point("Station", ITEM_NAME, sd, ed, data_unit))

        Dim json As String = "[" & String.Join("," & vbCrLf, json_data.ToArray()) & "]"
        Dim result As String = "var chart_" & FileName & "_" & st_name & " = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/WaterChart/chart_" & FileName & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()


    End Sub


    Private Function GetTimeText(year As Integer, month As Integer) As String
        Dim y1 As New DateTime(1970, 1, 1)
        Dim y2 As New DateTime(year, month, 1)
        Dim s As Int64 = DateDiff(DateInterval.Second, y1, y2) * 1000
        Return s.ToString()
    End Function

    Private Function get_area_json(leval As String, item As String,
                                    sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit) As String()

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))

        Select Case leval
            Case "ALL", "ALL2"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, 'ALL' as code " &
                    " From WaterMonthValue t1 " &
                    " left join country_basin t3 on t3.[COUNTRY] = t1.[COUNTY] " &
                    " where t1.[ITEMNAME] = @ITEMNAME  " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH]  "

            Case "Air"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH,  " &
                    "convert(varchar, t3.[BasinId]) as code " &
                    " From WaterMonthValue t1 " &
                    " left join RiverSite t2 on t2.SiteName = t1.SITENAME " &
                    " left join WaterBasin t3 on t3.[BasinName] = t2.[Basin] " &
                    " where t1.[ITEMNAME] = @ITEMNAME " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t3.[BasinId]  "

            Case "Station"

                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, " &
                 " convert(varchar, t2.[NUMBER]) as code, t3.BasinId, t2.[SiteName], t2.X, t2.Y " &
                 " From WaterMonthValue t1 " &
                 " join RiverSite t2 on t2.[SiteName] = t1.[SITENAME] " &
                 " join WaterBasin t3 on t3.[BasinName] = t2.Basin " &
                 " where t1.[ITEMNAME] = @ITEMNAME " &
                 " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t2.[NUMBER], t3.BasinId, t2.[SiteName], t2.X, t2.Y "

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

            If leval = "Station" Then
                Dim rs() As Data.DataRow = {}
                rs = dt.Select(String.Format("code = '{0}' ", code))

                c_code = "" & rs(0)("BasinId")
                X = "" & rs(0)("X")
                Y = "" & rs(0)("Y")
                SITENAME = "" & rs(0)("SITENAME")
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
                End If

                Dim t As String = String.Format("[{0}, {1}]", time, value)
                time_value.Add(t)
            Next

            If leval = "ALL2" Then
                code = "ALL2"
            End If

            If X = "" Then
                X = -1
                Y = -1
            End If

            Dim other As String = ""
            If leval = "Station" Then
                other = String.Format(", ""SITENAME"":""{0}"", ""x"": {1}, ""y"": {2}, ""BasinId"": ""{3}"" ",
                                   "" & SITENAME, "" & X, "" & Y, "" & c_code)
            End If

            Dim s As String = String.Format("{{""level"":""{0}"", ""area"":""{1}"" " & other & " ,""time_value"": [",
                                             leval, code)

            s &= String.Join(",", time_value.ToArray()) & "] }"
            json.Add(s)

        Next

        Return json.ToArray()

    End Function

    Private Function get_area_json_type1(leval As String, field As String,
                                    sd As DateTime, ed As DateTime, group_type As String) As String()

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""

        Select Case leval
            Case "ALL", "ALL2"
                sql = " select " & group_type & "(t1.[{0}]) as value, t1.MONITORYEAR, 'ALL' as code " &
                    " From RiverData_Type1 t1 " &
                    " group by t1.[MONITORYEAR]  "

            Case "Air"
                sql = " select " & group_type & "(t1.[{0}]) as value, t1.MONITORYEAR,  " &
                    " convert(varchar, t3.[BasinId]) as code " &
                    " From RiverData_Type1 t1 " &
                    " left join WaterBasin t3 on t3.[BasinName] = t1.[BasinName] " &
                    " group by t1.[MONITORYEAR], t3.[BasinId]  "

        End Select

        sql = String.Format(sql, field)

        Dim unit_count As Integer = 0
        unit_count = DateDiff(DateInterval.Year, sd, ed)

        Dim dt As Data.DataTable = da.GetDataTable(sql)

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

                Dim d As DateTime = sd.AddYears(m)
                Dim rs() As Data.DataRow = {}
                rs = dt.Select(String.Format("MONITORYEAR = '{0}' and code = '{1}' " _
                           , d.Year, code))

                Dim time As String = Me.GetTimeText(d.Year, d.Month)
                Dim value As String = "null"

                If rs.Length > 0 Then
                    If IsNumeric(rs(0)("value")) Then
                        value = "" & rs(0)("value")
                    End If
                    If leval = "Station" Then
                        c_code = "" & rs(0)("BasinId")
                        X = "" & rs(0)("X")
                        Y = "" & rs(0)("Y")
                        SITENAME = "" & rs(0)("SITENAME")
                    End If
                End If

                Dim t As String = String.Format("[{0}, {1}]", time, value)
                time_value.Add(t)
            Next

            If leval = "ALL2" Then
                code = "ALL2"
            End If

            If X = "" Then
                X = -1
                Y = -1
            End If

            Dim other As String = ""
            If leval = "Station" Then
                other = String.Format(", ""SITENAME"":""{0}"", ""x"": {1}, ""y"": {2}, ""BasinId"": ""{3}"" ",
                                   "" & SITENAME, "" & X, "" & Y, "" & c_code)
            End If

            Dim s As String = String.Format("{{""level"":""{0}"", ""area"":""{1}"" " & other & " ,""time_value"": [",
                                             leval, code)

            s &= String.Join(",", time_value.ToArray()) & "] }"
            json.Add(s)

        Next

        Return json.ToArray()

    End Function

    Private Function get_area_json_city(leval As String, item As String,
                                    sd As DateTime, ed As DateTime,
                                    data_unit As class_kind_json.enum_data_unit, group_type As String) As String()

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))

        Select Case leval
            Case "ALL", "ALL2"
                sql = " select {0}(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, 'ALL' as code " &
                    " From WaterCityMonthValue t1 " &
                    " where t1.[ITEMNAME] = @ITEMNAME  " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH]  "

            Case "City"
                sql = " select {0}(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH,  " &
                    "convert(varchar, t2.[CODE]) as code " &
                    " From WaterCityMonthValue t1 " &
                    " left join Country t2 on t1.Country = t2.COUNTRY " &
                    " where t1.[ITEMNAME] = @ITEMNAME " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], t2.[CODE]  "

        End Select

        Dim unit_count As Integer = 0
        Select Case data_unit
            Case class_kind_json.enum_data_unit.month
                unit_count = DateDiff(DateInterval.Month, sd, ed)
            Case class_kind_json.enum_data_unit.year
                unit_count = DateDiff(DateInterval.Year, sd, ed)
        End Select

        sql = String.Format(sql, group_type)

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
                        c_code = "" & rs(0)("BasinId")
                        X = "" & rs(0)("X")
                        Y = "" & rs(0)("Y")
                        SITENAME = "" & rs(0)("SITENAME")
                    End If
                End If

                Dim t As String = String.Format("[{0}, {1}]", time, value)
                time_value.Add(t)
            Next

            If leval = "ALL2" Then
                code = "ALL2"
            End If

            If X = "" Then
                X = -1 : Y = -1
            End If

            Dim other As String = ""
            If leval = "Station" Then
                other = String.Format(", ""SITENAME"":""{0}"", ""x"": {1}, ""y"": {2}, ""BasinId"": ""{3}"" ",
                                   "" & SITENAME, "" & X, "" & Y, "" & c_code)
            End If

            Dim s As String = String.Format("{{""level"":""{0}"", ""area"":""{1}"" " & other & " ,""time_value"": [",
                                             leval, code)

            s &= String.Join(",", time_value.ToArray()) & "] }"
            json.Add(s)

        Next

        Return json.ToArray()

    End Function

    Private Function get_area_json_point(leval As String, item As String,
                                    sd As DateTime, ed As DateTime, data_unit As class_kind_json.enum_data_unit) As String()

        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))

        Select Case leval
            Case "ALL", "ALL2"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH, 'ALL' as code " &
                    " From WaterCityMonthValue t1 " &
                    " where t1.[ITEMNAME] = @ITEMNAME  " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH]  "

            Case "Station"
                sql = " select avg(t1.[MONTHLYVALUE]) as value, t1.MONITORYEAR, t1.MONITORMONTH,  " &
                    " Country as code " &
                    " From WaterCityMonthValue t1 " &
                    " where t1.[ITEMNAME] = @ITEMNAME " &
                    " group by t1.[MONITORYEAR], t1.[MONITORMONTH], Country  "

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
                    'If leval = "Station" Then
                    '    c_code = "" & rs(0)("BasinId")
                    '    X = "" & rs(0)("X")
                    '    Y = "" & rs(0)("Y")
                    '    SITENAME = "" & rs(0)("SITENAME")
                    'End If
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
            'If leval = "Station" Then
            '    other = String.Format(", ""SITENAME"":""{0}"", ""x"": {1}, ""y"": {2}, ""BasinId"": ""{3}"" ",
            '                       "" & SITENAME, "" & X, "" & Y, "" & c_code)
            'End If

            Dim s As String = String.Format("{{""level"":""{0}"", ""area"":""{1}"" " & other & " ,""time_value"": [",
                                             leval, code)

            s &= String.Join(",", time_value.ToArray()) & "] }"
            json.Add(s)

        Next

        Return json.ToArray()

    End Function



End Class
