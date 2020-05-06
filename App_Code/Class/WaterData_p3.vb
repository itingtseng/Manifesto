Imports System.Data
Imports Microsoft.VisualBasic

Public Class WaterData_p3
    Implements IWaterData

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

    Public Function GetDataTable(item As String) As Data.DataTable Implements IWaterData.GetDataTable


        Dim da As New Jeff.DataAccess
        Dim sql As String = " select * from WaterCityMonthValue " &
                " where ITEMNAME = @ITEMNAME " &
                " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH]) "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql, para.ToArray())

        da.Dispose()
        Return dt_data

    End Function

    Public Function create_data(ITEM_NAME As String, Dir As String, FileName As String,
                            start_date As DateTime?, end_date As DateTime?,
                                data_unit As IWaterData.class_kind_json.enum_data_unit, Optional gropu_type As String = "avg") As String Implements IWaterData.create_data

        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim sql As String = " select t1.* from WaterCityMonthValue t1 " &
                        " where t1.ITEMNAME = @ITEMNAME  "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", ITEM_NAME))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql_order, para.ToArray())
        If dt_data.Rows.Count = 0 Then Return ITEM_NAME & ", 無資料"

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
        json_data.AddRange(get_area_json("ALL2", ITEM_NAME, sd, ed, data_unit, gropu_type))
        json_data.AddRange(get_area_json("ALL", ITEM_NAME, sd, ed, data_unit, gropu_type))
        json_data.AddRange(get_area_json("City", ITEM_NAME, sd, ed, data_unit, gropu_type))

        Dim json As String = "[" & String.Join("," & vbCrLf, json_data.ToArray()) & "]"
        Dim result As String = "var chart_" & FileName & "_" & st_name & " = " & vbCrLf & json & ";"

        Dim file_name As String = "chart_" & FileName & "_" & st_name & ".js"
        Dim path As String = System.IO.Path.Combine(Dir, file_name)
        'Dim path As String = Server.MapPath("../js/WaterChart/chart_" & FileName & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        da.Dispose()

        Return path

    End Function
    Private Function get_area_json(leval As String, item As String,
                                    sd As DateTime, ed As DateTime,
                                   data_unit As IWaterData.class_kind_json.enum_data_unit, Optional group_type As String = "avg") As String() _
                                   Implements IWaterData.get_area_json


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

        da.Dispose()
        Return json.ToArray()

    End Function


    Private Function GetTimeText(year As Integer, month As Integer) As String
        Dim y1 As New DateTime(1970, 1, 1)
        Dim y2 As New DateTime(year, month, 1)
        Dim s As Int64 = DateDiff(DateInterval.Second, y1, y2) * 1000
        Return s.ToString()
    End Function


    Public Function create_map_json(item_name As String, Dir As String, FileName As String,
                                    data_unit As IWaterData.class_kind_json.enum_data_unit, Optional group_type As String = "avg") As String _
        Implements IWaterData.create_map_json

        '年度
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select t1.* from WaterCityMonthValue t1 " &
                        " where t1.ITEMNAME = @ITEMNAME  "

        Dim sql_order As String = sql &
            " order by convert(int, [MONITORYEAR]), convert(int, [MONITORMONTH])  "

        Dim para As New List(Of Object)
        para.Add(da.CreateParameter("ITEMNAME", item_name))
        Dim dt_data As Data.DataTable = da.GetDataTable(sql_order, para.ToArray())
        If dt_data.Rows.Count = 0 Then Return item_name & ", 無資料"

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
        area_json.Add(get_map_json("ALL", "ALL", item_name, sd, ed, data_unit, group_type))

        '流域
        'area_json.Add(get_area_json("Basin", "Basin", item_name, sd, ed, data_unit))

        Dim result = "var map_" & FileName & "_" & st_name & " = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim file_name As String = "map_" & FileName & "_" & st_name & ".js"
        Dim path As String = System.IO.Path.Combine(Dir, file_name)
        'Dim path As String = Server.MapPath("../js/WaterMap/map_" & file_name & "_" & st_name & ".js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

        da.Dispose()
        Return path

    End Function

    Private Function get_map_json(leval As String, obj_id As String, item As String,
                                    sd As DateTime, ed As DateTime,
                                  data_unit As IWaterData.class_kind_json.enum_data_unit, Optional group_type As String = "avg") As String _
                                    Implements IWaterData.get_map_json

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

        da.Dispose()
        Return text

    End Function


End Class
