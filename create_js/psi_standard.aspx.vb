
Partial Class create_js_psi_standard
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        CreatePSI_Standard_json()
        Response.Write("OK" & Now.ToString())
    End Sub

    Private Sub CreatePSI_Standard_json()
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select * from PSI_standard order by [date_year], [data_month] "
        Dim dt As Data.DataTable = da.GetDataTable(sql)

        Dim min_ym = dt.Rows(0)
        Dim max_ym = dt.Rows(dt.Rows.Count - 1)
        Dim sd As New DateTime(min_ym("date_year") + 1911, min_ym("data_month"), 1)
        Dim ed As New DateTime(max_ym("date_year") + 1911, max_ym("data_month"), 1)

        Dim dt_area As Data.DataTable = da.GetDataTable("select * from AirQualityArea ")

        '全國
        Dim js_ary As New List(Of String)
        js_ary.Add(get_data_json(dt, sd, ed, "ALL2", "ALL2", "全國"))
        js_ary.Add(get_data_json(dt, sd, ed, "ALL", "ALL", "全國"))

        '空品區
        'For k As Integer = 0 To dt_area.Rows.Count - 1
        '    Dim row = dt_area.Rows(k)
        '    js_ary.Add(get_data_json(dt, sd, ed, "Air", row("sn"), row("NAME")))
        'Next

        js_ary.Add(get_data_json(dt, sd, ed, "Air", "3", "北部空品區"))
        js_ary.Add(get_data_json(dt, sd, ed, "Air", "4", "竹苗空品區"))
        js_ary.Add(get_data_json(dt, sd, ed, "Air", "2", "中部空品區"))
        js_ary.Add(get_data_json(dt, sd, ed, "Air", "7", "雲嘉南空品區"))
        js_ary.Add(get_data_json(dt, sd, ed, "Air", "6", "高屏空品區"))
        js_ary.Add(get_data_json(dt, sd, ed, "Air", "5", "花東空品區"))
        js_ary.Add(get_data_json(dt, sd, ed, "Air", "1", "宜蘭空品區"))

        Dim json As String = "[" & String.Join("," & vbCrLf, js_ary.ToArray()) & "]"
        Dim result As String = "var psi_standard = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/air_s10/psi_standard.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Function get_data_json(data As Data.DataTable,
                             StartDate As DateTime, EndDate As DateTime, level As String, area As String, field As String) As String

        Dim m_count As Integer = DateDiff(DateInterval.Month, StartDate, EndDate)
        Dim js_time As New List(Of String)

        For i As Integer = 0 To m_count
            Dim month As DateTime = StartDate.AddMonths(i)
            Dim value As Double?
            Dim value_ary = data.Select(String.Format("date_year = '{0}' and data_month = '{1}' ", month.Year - 1911, month.Month))

            If value_ary.Count > 0 Then
                value = CDbl(value_ary(0)(field).ToString().Replace("%", ""))
            End If

            Dim str_value As String = "null"
            If value IsNot Nothing Then
                str_value = value.ToString()
            End If

            Dim s As String = String.Format("[{0},{1}]",
                                             Me.GetTimeText(month.Year, month.Month), str_value)
            js_time.Add(s)
        Next

        Dim j2 As String = String.Join(",", js_time.ToArray())
        Dim json As String = String.Format("{{""level"":""{2}"", ""area"":""{0}""" & ", ""time_value"":[{1}] }}",
                                           area, j2, level)

        Return json
    End Function

    Private Function GetTimeText(year As Integer, month As Integer) As String
        Dim y1 As New DateTime(1970, 1, 1)
        Dim y2 As New DateTime(year, month, 1)
        Dim s As Int64 = DateDiff(DateInterval.Second, y1, y2) * 1000
        Return s.ToString()
    End Function

End Class
