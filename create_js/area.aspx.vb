
Partial Class create_js_area
    Inherits System.Web.UI.Page

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load

        CreateAirCode()
        Response.Write("OK")

    End Sub

    Private Sub CreateAirCode()
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

        Dim path As String = Server.MapPath("../js/common/area_code.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(s)
        sw.Write(s_map)
        sw.Close()
        fn.Close()


    End Sub

    Private Sub CreateWaterCode()
        Dim ary As New List(Of String)
        Dim da As New Jeff.DataAccess
        Dim sql As String = " select * from WaterBasin "
        Dim dt As Data.DataTable = da.GetDataTable(sql)
        '流域
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim r As Data.DataRow = dt.Rows(i)
            Dim v As String = "{{ ""level"": ""Air"", ""code"": ""{0}"", ""text"": ""{1}"" }} "
            v = String.Format(v, "" & r("BasinId"), "" & r("BasinName"))
            ary.Add(v)
        Next

        'stataic
        sql = " select t1.*, t2.[c_date] from AirStation t1 " &
            " join station_create_date t2 " &
            " on t1.SITENAME = t2.[name] "

        dt = da.GetDataTable(sql)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim r As Data.DataRow = dt.Rows(i)
            Dim str_d As String = ""
            If IsDate("" & r("c_date")) Then
                Dim d As DateTime = r("c_date")
                str_d = d.Year - 1911 & "/" & d.ToString("MM/dd")
            End If
            Dim v As String = "{{ ""level"": ""Station"", ""code"": ""{0}"", ""text"": ""{1}"", ""create_date"": ""{2}"" }} "
            v = String.Format(v, "" & r("NUMBER"), "" & r("SITENAME"), str_d)
            ary.Add(v)
        Next

        Dim s As String = "var area_code = [ " & vbCrLf &
            "{ ""level"": ""ALL"", ""code"": ""ALL"", ""text"": ""全國"" }, " & vbCrLf &
            String.Join("," & vbCrLf, ary.ToArray()) & vbCrLf &
            "];"


        Dim path As String = Server.MapPath("../js/common/area_code.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(s)
        sw.Close()
        fn.Close()


    End Sub


End Class
