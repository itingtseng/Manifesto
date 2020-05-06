Partial Class create_js_create_js_co
    Inherits System.Web.UI.Page

    Private Function GetDbl(value As String) As Double
        If IsNumeric(value) Then
            Return CDbl(value)
        Else
            Return 0
        End If
    End Function

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        create_all()

        Response.Write("creae ok _" & Now.ToString())
    End Sub

    ''' <summary>
    ''' 建立全台空品區
    ''' </summary>
    Private Sub create_all()

        '鉛

        '年度
        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = (From t In dc.data_pb
                       Where t.d_value IsNot Nothing
                       Order By CInt(t.d_year), CInt(t.d_month)
                       Select t
                           ).ToArray()

        Dim min_ym = dt_site.First()
        Dim max_ym = dt_site.Last()
        Dim sd As New DateTime(min_ym.d_year, min_ym.d_month, 1)
        Dim ed As New DateTime(max_ym.d_year, max_ym.d_month, 1)

        Dim sql As String = " select [COUNTRY], [CODE] from Country "
        Dim dt_country As Data.DataTable = da.GetDataTable(sql)

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

        Dim area_json As New List(Of String)

        area_json.Add(GetAreaData(dt_site, sd, ed, "ALL", area_c))

        For Each c In area_c
            Dim a As String = c.Key
            Dim col As New System.Collections.Generic.Dictionary(Of String, String())
            Dim c_code As String = ""
            For Each k In c.Value
                Dim row() As Data.DataRow = dt_country.Select(String.Format("COUNTRY = '{0}'", k))
                Dim code As String = ""
                If row.Length > 0 Then
                    code = "" & row(0)("CODE")
                End If
                col.Add(code, {k})
            Next
            '空投區
            area_json.Add(GetAreaData(dt_site, sd, ed, a, col))
        Next

        '單一區
        For Each c In area_c
            For Each k In c.Value
                Dim col As New System.Collections.Generic.Dictionary(Of String, String())
                Dim row() As Data.DataRow = dt_country.Select(String.Format("COUNTRY = '{0}'", k))
                Dim code As String = ""
                If row.Length > 0 Then
                    code = "" & row(0)("CODE")
                    '空投區
                    col.Add(code, {k})
                    area_json.Add(GetAreaData(dt_site, sd, ed, code, col))
                End If
            Next
        Next

        Dim result = "var co_map = [ " & vbCrLf &
            String.Join("," & vbCrLf, area_json.ToArray()) & " ];"

        Dim path As String = Server.MapPath("../js/gasoline/co_map.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Function GetAreaData(data() As WhitePaper.Table.data_pb,
                             StartDate As DateTime, EndDate As DateTime,
                             area_code As String, area As System.Collections.Generic.Dictionary(Of String, String())) As String

        Dim month_count As Integer = DateDiff(DateInterval.Month, StartDate, EndDate)
        Dim ary_year As New List(Of String)

        For i As Integer = 0 To month_count

            Dim data_month As DateTime = StartDate.AddMonths(i)
            Dim ary_area As New List(Of String)
            For Each k In area.Keys
                Dim area_list() As String = area(k)
                Dim value_ary = (From r In data
                                 Where r.d_year = data_month.Year And r.d_month = data_month.Month _
                                 And area_list.Contains(r.city)
                                 Order By r.d_value Descending
                                 Select r.d_value).ToArray()

                Dim value As Double = 0
                Dim str_value As String = "null"
                If value_ary.Count > 0 Then
                    value = value_ary.Average
                    str_value = value.ToString("#,##0.00")
                End If

                Dim js2 As String = String.Format("{{""code"":""{0}"", ""value"":{1} }}",
                                                  k, str_value)
                ary_area.Add(js2)
            Next

            Dim js As String = String.Format("{{""year"": ""{0}"", ""data"": [{1}] }}",
                                              data_month.ToString("yyyy_MM"),
                                              String.Join(",", ary_area.ToArray())
                                              )
            ary_year.Add(js)
        Next

        Dim result = "{ ""area"": """ & area_code & """, ""value"":[ " & vbCrLf &
            String.Join("," & vbCrLf, ary_year.ToArray()) & vbCrLf & "] }"

        Return result

    End Function

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '資料的 js file

        create_data()
        Response.Write("creae  data ok _" & Now.ToString())
    End Sub
    Private Sub create_data()

        '年度
        Dim da As New Jeff.DataAccess

        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = (From t In dc.data_pb
                       Where t.d_value IsNot Nothing
                       Order By CInt(t.d_year), CInt(t.d_month)
                       Select t
                           ).ToArray()

        Dim min_ym = dt_site.First()
        Dim max_ym = dt_site.Last()
        Dim sd As New DateTime(min_ym.d_year, min_ym.d_month, 1)
        Dim ed As New DateTime(max_ym.d_year, max_ym.d_month, 1)

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
        js_ary.Add(get_data_json(dt_site, sd, ed, "ALL2", "ALL2", {})) '全台
        js_ary.Add(get_data_json(dt_site, sd, ed, "ALL", "ALL", {})) '全台

        '空品區
        For Each k In area_c.Keys
            js_ary.Add(get_data_json(dt_site, sd, ed, "Air", k, area_c(k)))
        Next

        '縣市
        For Each city In area_c
            For Each c In city.Value
                Dim row() = city_code.Select("COUNTRY = '" & c & "'")
                If row.Length > 0 Then
                    js_ary.Add(get_data_json(dt_site, sd, ed, "City", row(0)("CODE"), {c})) '全台
                End If
            Next
        Next

        Dim json As String = "[" & String.Join("," & vbCrLf, js_ary.ToArray()) & "]"
        Dim result As String = "var co_chart = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/gasoline/co_chart.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()


    End Sub

    Private Function get_data_json(data() As WhitePaper.Table.data_pb,
                             StartDate As DateTime, EndDate As DateTime, level As String, area_code As String, area() As String,
                             Optional other_p As String = "") As String

        Dim m_count As Integer = DateDiff(DateInterval.Month, StartDate, EndDate)
        Dim js_time As New List(Of String)

        Dim data_area = data
        If area.Length > 0 Then
            data_area = (From r In data Where area.Contains(r.city)).ToArray()
        End If

        For i As Integer = 0 To m_count
            Dim month As DateTime = StartDate.AddMonths(i)
            Dim value As Double?
            Dim value_ary = (From r In data_area
                             Where r.d_year = month.Year _
                                 And r.d_month = month.Month
                             Order By r.d_value
                             Select r.d_value
                             ).ToArray()

            If value_ary.Count > 0 Then
                value = value_ary.Average
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

    Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        '年度
        Dim da As New Jeff.DataAccess
        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = (From t In dc.data_pb
                       Where t.d_value IsNot Nothing
                       Order By CInt(t.d_year), CInt(t.d_month)
                       Select t
                           ).ToArray()

        Dim min_ym = dt_site.First()
        Dim max_ym = dt_site.Last()
        Dim sd As New DateTime(min_ym.d_year, min_ym.d_month, 1)
        Dim ed As New DateTime(max_ym.d_year, max_ym.d_month, 1)

        sd = New DateTime(78 + 1911, 1, 1)
        ed = New DateTime(104 + 1911, 12, 1)

        'Dim dt As Data.DataTable = da.GetDataTable(s2)
        'Dim d_y1 As Integer = dt.Rows(0)("y1") + 1911
        'Dim d_y2 As Integer = dt.Rows(0)("y2") + 1911
        'Dim qd1 As New DateTime(d_y1, 1, 1)
        'Dim qd2 As New DateTime(d_y2, 12, 31)

        'Dim sd As DateTime = qd1
        'Dim ed As DateTime = qd2

        Dim dt_oil As Data.DataTable = da.GetDataTable("select * from type_data where type in ('無鉛汽油使用率', '有鉛汽油含鉛量') ")

        Dim j2 As String = Me.GetDriveJson("有鉛汽油含鉛量", dt_oil, sd, New DateTime(104 + 1911, 1, 1))
        Dim j3 As String = Me.GetDriveJson("無鉛汽油使用率", dt_oil, sd, ed)

        Dim result As String = "var co_oil = { " &
            """gas_has"" : " & j2 & "," & vbCrLf &
            """gas_no"" : " & j3 &
            " };"

        Dim path As String = Server.MapPath("../js/gasoline/co_oil.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()

        Response.Write("ok _ drive _" & Now.ToString())

    End Sub

    Private Function GetDriveJson(type As String, data As Data.DataTable,
                 StartDate As DateTime, EndDate As DateTime) As String

        Dim value As New List(Of String)
        Dim MinYear As Integer = StartDate.Year
        Dim MaxYear As Integer = EndDate.Year
        Dim MinMonth As Integer = StartDate.Month
        Dim MaxMonth As Integer = EndDate.Month

        Dim row() As Data.DataRow = data.Select(String.Format("type = '{0}' ", type))

        Response.Write(row.Count)

        For i As Integer = 0 To row.Count - 1
            Dim y As Integer = row(i)("data_year") + 1911
            Dim v As Double = GetDbl("" & row(i)("value"))

            If y < MinYear Or y > MaxYear Then Continue For

            Dim m1 As Integer = 1, m2 As Integer = 12
            If y = MinYear Then m1 = MinMonth
            If y = MaxYear Then m2 = MaxMonth

            For m As Integer = m1 To m2
                Dim s As String = Me.GetTimeText(y, m)
                value.Add(String.Format("[{0},{1}]", s, v))
            Next
        Next

        Dim json As String = "[" & String.Join(",", value.ToArray()) & "]"
        Return json

    End Function



End Class
