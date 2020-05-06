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

        '年度
        Dim da As New Jeff.DataAccess
        Dim s2 As String = " select min([data_year])  as y1, max([data_year]) as y2 from data_so2"
        Dim dt As Data.DataTable = da.GetDataTable(s2)
        Dim d_y1 As Integer = dt.Rows(0)("y1") + 1911
        Dim d_y2 As Integer = dt.Rows(0)("y2") + 1911
        Dim qd1 As New DateTime(d_y1, 1, 1)
        Dim qd2 As New DateTime(d_y2, 12, 31)

        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = (From t In dc.AirQualityMonth
                       Join t2 In dc.AirStation On t.COUNTY Equals t2.COUNTY And t.SITENAME Equals t2.SITENAME
                       Where t.ITEMNAME = "二氧化硫" And t.MONTHLYVALUE IsNot Nothing _
                           And t2.SITETYPE = "工業" _
                           And CInt(t.MONITORYEAR) >= d_y1 _
                           And CInt(t.MONITORYEAR) <= d_y2
                       Order By CInt(t.MONITORYEAR), CInt(t.MONITORMONTH)
                       Select t
                           ).ToArray()

        Dim min_ym = dt_site.First()
        Dim max_ym = dt_site.Last()
        Dim sd As New DateTime(min_ym.MONITORYEAR, min_ym.MONITORMONTH, 1)
        Dim ed As New DateTime(max_ym.MONITORYEAR, max_ym.MONITORMONTH, 1)

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

        Dim path As String = Server.MapPath("../js/so2/co_map.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn)
        sw.Write(result)
        sw.Close()
        fn.Close()

    End Sub

    Private Function GetAreaData(data() As WhitePaper.Table.AirQualityMonth,
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
                                 Where r.MONITORYEAR = data_month.Year And r.MONITORMONTH = data_month.Month _
                                 And area_list.Contains(r.COUNTY)
                                 Order By r.MONTHLYVALUE Descending
                                 Select r.MONTHLYVALUE).ToArray()

                Dim value As Double? = Nothing
                If value_ary.Count > 0 Then
                    value = value_ary.Average
                End If

                Dim str_v As String = ""
                If value Is Nothing Then
                    str_v = "null"
                Else
                    str_v = value.ToString()
                End If

                Dim js2 As String = String.Format("{{""code"":""{0}"", ""value"":{1} }}",
                                                  k, str_v)
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
        Dim s2 As String = " select min([data_year])  as y1, max([data_year]) as y2 from data_so2"
        Dim dt As Data.DataTable = da.GetDataTable(s2)
        Dim d_y1 As Integer = dt.Rows(0)("y1") + 1911
        Dim d_y2 As Integer = dt.Rows(0)("y2") + 1911
        Dim qd1 As New DateTime(d_y1, 1, 1)
        Dim qd2 As New DateTime(d_y2, 12, 31)

        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = (From t In dc.AirQualityMonth
                       Join t2 In dc.AirStation On t.COUNTY Equals t2.COUNTY And t.SITENAME Equals t2.SITENAME
                       Where t.ITEMNAME = "二氧化硫" And t.MONTHLYVALUE IsNot Nothing _
                           And t2.SITETYPE = "工業" _
                           And CInt(t.MONITORYEAR) >= d_y1 _
                           And CInt(t.MONITORYEAR) <= d_y2
                       Order By CInt(t.MONITORYEAR), CInt(t.MONITORMONTH)
                       Select t
                           ).ToArray()

        Dim min_ym = dt_site.First()
        Dim max_ym = dt_site.Last()
        Dim sd As New DateTime(min_ym.MONITORYEAR, min_ym.MONITORMONTH, 1)
        Dim ed As New DateTime(max_ym.MONITORYEAR, max_ym.MONITORMONTH, 1)

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

        '測站
        Dim sql2 As String = " select t1.*, t2.CODE,t3.sn from AirStation t1 " &
            " left join Country t2 on t1.COUNTY = t2.COUNTRY " &
            " left join AirQualityArea t3 on t3.NAME = t1.AIRQUALITYAREA "

        Dim dt_s As Data.DataTable = da.GetDataTable(sql2)
        Dim site = (From r In dt_site Select SITENAME = r.SITENAME).Distinct().ToArray()
        For i As Integer = 0 To UBound(site)
            Dim n As String = site(i)
            Dim rs() As Data.DataRow = dt_s.Select(String.Format("SITENAME = '{0}' ", n))
            Dim p2 As String = ""
            Dim sno = n
            If rs.Length > 0 Then
                sno = rs(0)("NUMBER")
                p2 = String.Format(", ""SITENAME"":""{0}"", ""x"": {1}, ""y"": {2}, ""CountyCode"": ""{3}"" ",
                                   "" & rs(0)("SITENAME"), "" & rs(0)("X"), "" & rs(0)("Y"), "" & rs(0)("CODE"))
            End If

            Dim data_site = (From r In dt_site Where r.SITENAME = n Select r).ToArray()
            js_ary.Add(get_data_json(data_site, sd, ed, "Station", sno, {}, p2))
        Next

        Dim json As String = "[" & String.Join("," & vbCrLf, js_ary.ToArray()) & "]"
        Dim result As String = "var co_chart = " & vbCrLf & json & ";"

        Dim path As String = Server.MapPath("../js/so2/co_chart.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(result)
        sw.Close()
        fn.Close()


    End Sub
    Private Function get_data_json(data() As WhitePaper.Table.AirQualityMonth,
                             StartDate As DateTime, EndDate As DateTime, level As String, area_code As String, area() As String,
                             Optional other_p As String = "") As String

        Dim m_count As Integer = DateDiff(DateInterval.Month, StartDate, EndDate)
        Dim js_time As New List(Of String)

        Dim data_area = data
        If area.Length > 0 Then
            data_area = (From r In data Where area.Contains(r.COUNTY)).ToArray()
        End If

        For i As Integer = 0 To m_count
            Dim month As DateTime = StartDate.AddMonths(i)
            Dim value As Double?
            Dim value_ary = (From r In data_area
                             Where r.MONITORYEAR = month.Year _
                                 And r.MONITORMONTH = month.Month
                             Order By r.MONTHLYVALUE
                             Select r.MONTHLYVALUE
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
        Dim s2 As String = " select min([data_year])  as y1, max([data_year]) as y2 from data_so2"
        Dim dt As Data.DataTable = da.GetDataTable(s2)
        Dim d_y1 As Integer = dt.Rows(0)("y1") + 1911
        Dim d_y2 As Integer = dt.Rows(0)("y2") + 1911
        Dim qd1 As New DateTime(d_y1, 1, 1)
        Dim qd2 As New DateTime(d_y2, 12, 31)

        Dim dc As New WhitePaper.DataClassesDataContext
        Dim dt_site = (From t In dc.AirQualityMonth
                       Join t2 In dc.AirStation On t.COUNTY Equals t2.COUNTY And t.SITENAME Equals t2.SITENAME
                       Where t.ITEMNAME = "二氧化硫" And t.MONTHLYVALUE IsNot Nothing _
                           And t2.SITETYPE = "工業" _
                           And CInt(t.MONITORYEAR) >= d_y1 _
                           And CInt(t.MONITORYEAR) <= d_y2
                       Order By CInt(t.MONITORYEAR), CInt(t.MONITORMONTH)
                       Select t
                           ).ToArray()

        Dim min_ym = dt_site.First()
        Dim max_ym = dt_site.Last()
        Dim sd As New DateTime(min_ym.MONITORYEAR, min_ym.MONITORMONTH, 1)
        Dim ed As New DateTime(max_ym.MONITORYEAR, max_ym.MONITORMONTH, 1)

        sd = New DateTime(1911 + 83, 1, 1)
        ed = New DateTime(1911 + 104, 12, 1)

        Dim dt_oil As Data.DataTable = da.GetDataTable("select * from data_so2")

        Dim j2 As String = Me.GetDriveJson("fuel", dt_oil, sd, ed)
        Dim j3 As String = Me.GetDriveJson("gasoline", dt_oil, sd, ed)
        Dim j4 As String = Me.GetDriveJson("diesel ", dt_oil, sd, ed)

        Dim result As String = "var co_oil = { " &
            """fuel"" : " & j2 & "," & vbCrLf &
            """gasoline"" : " & j2 & "," & vbCrLf &
            """diesel"" : " & j2 & vbCrLf &
            " };"

        Dim path As String = Server.MapPath("../js/so2/co_oil.js")
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
