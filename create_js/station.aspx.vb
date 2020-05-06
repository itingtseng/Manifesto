
Partial Class create_js_station
    Inherits System.Web.UI.Page

    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load

        Dim da As New Jeff.DataAccess
        Dim sql As String = " select t1.*, t2.CODE,t3.sn from AirStation t1 " &
            " left join Country t2 on t1.COUNTY = t2.COUNTRY " &
            " left join AirQualityArea t3 on t3.NAME = t1.AIRQUALITYAREA "
        Dim dt As Data.DataTable = da.GetDataTable(sql)

        Dim f As New List(Of String)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim r As Data.DataRow = dt.Rows(i)

            Dim s As String = "{{ ""type"": ""Feature"", ""properties"": {{ ""COUNTY_CODE"":""{2}"", ""SITENAME"":""{3}"", ""AirSn"": ""{4}"", ""sno"": ""{5}"" }}, " &
                """geometry"":{{ ""type"": ""MultiPolygon"", ""coordinates"": [ [ [ [{0}, {1}] ] ] ] }} }}"

            Dim v As String = String.Format(s,
                                            r("X"), r("Y"), "" & r("CODE"), "" & r("SITENAME"), "" & r("sn"), "" & r("NUMBER"))

            f.Add(v)
        Next

        Dim json As String = "var geo_station = { " & vbCrLf &
                """type"" :  ""FeatureCollection"", " & vbCr &
                """crs"": { ""type"": ""name"", ""properties"": { ""name"": ""urn:ogc:def:crs:OGC:1.3:CRS84"" } }, " & vbCrLf & vbCrLf &
                """features"":[ " & vbCrLf &
                String.Join("," & vbCrLf, f.ToArray()) &
                 "]" & vbCrLf & "};"

        Dim path As String = Server.MapPath("../Geojson/station.geojson.js")
        Dim fn As New System.IO.FileStream(path, IO.FileMode.Create)
        Dim sw As New System.IO.StreamWriter(fn, System.Text.Encoding.UTF8)
        sw.Write(json)
        sw.Close()
        fn.Close()

        Response.Write("OK")

    End Sub


End Class
