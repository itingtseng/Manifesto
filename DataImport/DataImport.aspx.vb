
Partial Class DataImport_DataImport
    Inherits System.Web.UI.Page

    Class class_kind_json
        Public Enum enum_data_unit
            month
            year
        End Enum

        Public item_name As String
        Public file_name As String
        Public stie_type() As String
        Public group_type As String = "avg"

        Public default_sd As DateTime?
        Public default_ed As DateTime?

        Public data_unit As enum_data_unit = enum_data_unit.month

        Public Sub New(item_name As String, file_name As String, stie_type() As String)
            Me.item_name = item_name
            Me.file_name = file_name
            Me.stie_type = stie_type
        End Sub

        Public Sub New(item_name As String, file_name As String)
            Me.item_name = item_name
            Me.file_name = file_name
        End Sub

        Public Sub New(item_name As String, file_name As String, group_type As String)
            Me.item_name = item_name
            Me.file_name = file_name
            Me.group_type = group_type
        End Sub

    End Class

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '下載檔案
        If Me.RadioButton_air.Checked = True Then
            Dim item As String = Me.DropDownList_aire.SelectedValue
            Dim obj_data As New AirData_p1
            Dim dt As Data.DataTable = obj_data.GetDataTable(item)
            DownloadFile(Me.DropDownList_aire.SelectedItem.Text, dt)
        ElseIf Me.RadioButton_water.Checked = True Then
            Dim item As String = Me.DropDownList_water.SelectedValue
            Dim obj_data As IWaterData = Nothing
            Dim t As String = item.Substring(0, 2)
            Select Case t
                Case "p1"
                    obj_data = New WaterData_p1
                Case "p2"
                    obj_data = New WaterData_p2
                Case "p3"
                    obj_data = New WaterData_p3
                Case "p4"
                    obj_data = New WaterData_p4
            End Select
            item = item.Remove(0, 3)
            Dim dt As Data.DataTable = obj_data.GetDataTable(item)
            DownloadFile(Me.DropDownList_water.SelectedItem.Text, dt)

        ElseIf Me.RadioButton_standard.Checked = True Then

            Dim obj_data As New Standard
            Dim item As String = Me.DropDownList_standard.SelectedValue
            Dim dt As Data.DataTable = obj_data.GetDataTable(item)
            DownloadFile(Me.DropDownList_standard.SelectedItem.Text, dt)

        End If
    End Sub

    Private Sub DownloadFile(item As String, dt As Data.DataTable)
        '下載
        SetDownloadHearder(item & ".csv")
        Response.Clear()

        Dim head As String = ""
        For i As Integer = 0 To dt.Columns.Count - 1
            head &= dt.Columns(i).ColumnName & ","
        Next

        Dim bs() As Byte = System.Text.Encoding.Default.GetBytes(head & vbCrLf)
        Response.BinaryWrite(bs)

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim body As String = ""
            For c As Integer = 0 To dt.Columns.Count - 1
                body &= "" & dt.Rows(i)(c) & ","
            Next
            bs = System.Text.Encoding.Default.GetBytes(body & vbCrLf)
            Response.BinaryWrite(bs)
        Next

        Response.End()

    End Sub

    Public Shared Sub SetDownloadHearder(FileName As String)
        Dim sContentDisposition As String = "attachment;"
        sContentDisposition &= "filename=" & HttpUtility.UrlEncode(FileName)
        HttpContext.Current.Response.AddHeader("Content-disposition", sContentDisposition)
        HttpContext.Current.Response.ContentType = "application/" &
            System.IO.Path.GetExtension(FileName).Replace(".", "")
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '上傳檔案
        If Me.RadioButton_air.Checked = True Then
            upload_air(Me.DropDownList_aire.SelectedValue)
            Jeff.JavaScript.Alert("上傳完成, 已產出檔案", Me.Page)
        ElseIf Me.RadioButton_water.Checked = True Then
            If Me.DropDownList_water.SelectedValue = "p2_river_all" Then
                Dim item() As String = {"p2_km_none", "p2_km_mild", "p2_km_mod", "p2_km_severe",
                    "p2_percent_none", "p2_percent_mild", "p2_percent_mod", "p2_percent_severe"}
                del_p2 = False
                For i As Integer = 0 To UBound(item)
                    upload_water(item(i))
                Next
            Else
                upload_water(Me.DropDownList_water.SelectedValue)
            End If
            Jeff.JavaScript.Alert("上傳完成, 已產出檔案", Me.Page)

        ElseIf Me.RadioButton_standard.Checked = True Then
            upload_standard(Me.DropDownList_standard.SelectedValue)
            Jeff.JavaScript.Alert("上傳完成, 已產出檔案", Me.Page)
        End If
    End Sub

    Private Function parse_file() As Data.DataTable

        Dim sr As New System.IO.StreamReader(Me.FileUpload1.FileContent, System.Text.Encoding.Default)
        Dim index As Integer = 0
        Dim dt As New Data.DataTable

        Dim col_len As Integer = 0
        Dim line As String = ""

        While sr.Peek > -1

            line = sr.ReadLine
            index += 1
            If line.Trim() = "" Then Continue While

            If index = 1 Then
                'head
                Dim head() As String = line.Split(",")
                For i As Integer = 0 To UBound(head)
                    Dim n As String = head(i)

                    Dim col_type As System.Type = System.Type.GetType("System.String")
                    If (n.Trim() = "") Then Continue For
                    Select Case n.ToUpper()
                        Case "MONTHLYVALUE"
                            col_type = System.Type.GetType("System.Double")
                        Case "MONITORYEAR", "MONITORMONTH", "ID"
                            col_type = System.Type.GetType("System.Int32")
                    End Select
                    dt.Columns.Add(n, col_type)
                Next
                col_len = dt.Columns.Count
            Else
                'body
                If line.Trim = "" Then Continue While
                Dim body() As String = line.Split(",")
                Dim row As Data.DataRow = dt.NewRow
                Dim has_data As Boolean = False
                For c As Integer = 0 To col_len - 1
                    If c < body.Length Then
                        Try
                            If body(c).Trim() <> "" Then has_data = True
                            row(c) = body(c).Replace("__", ",")
                        Catch ex As Exception
                            row(c) = DBNull.Value
                        End Try
                    End If
                Next

                '全空白不加入
                If has_data = True Then dt.Rows.Add(row)
            End If

        End While

        Return dt

    End Function

    Public Function GetAirItem(item As String) As class_kind_json

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("二氧化硫", "so2", {"工業", "一般", "交通"}))
        ary.Add(New class_kind_json("懸浮微粒", "pm10", {"工業", "一般", "交通"}))
        ary.Add(New class_kind_json("臭氧", "o3", {"工業", "一般", "交通"}))
        ary.Add(New class_kind_json("一氧化碳", "co", {"工業", "一般", "交通"}))
        ary.Add(New class_kind_json("二氧化碳", "co2", {"工業", "一般", "交通"}))
        ary.Add(New class_kind_json("二氧化氮", "no2", {"工業", "一般", "交通"}) _
              With {.default_sd = New DateTime(83 + 1911, 1, 1)})
        ary.Add(New class_kind_json("非甲烷碳氫化合物", "NMHC", {"一般", "交通"}))
        ary.Add(New class_kind_json("細懸浮微粒", "pm25", {"工業", "一般", "交通"}) _
            With {.data_unit = class_kind_json.enum_data_unit.year, .default_sd = New DateTime(94 + 1911, 1, 1)})
        ary.Add(New class_kind_json("PSI", "psi", {"工業", "一般", "交通"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("PSI_COUNT_PERCENT", "psi_count", {"工業", "一般", "交通"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("鉛", "pb", {""}))
        ary.Add(New class_kind_json("空氣污染防制", "air_city_month", {""}) _
                With {.data_unit = class_kind_json.enum_data_unit.year, .default_sd = New DateTime(2001, 1, 1)})
        ary.Add(New class_kind_json("細懸浮微粒_Data2", "pm25_d2", {""}) _
            With {.default_sd = New DateTime(94 + 1911, 1, 1)})
        ary.Add(New class_kind_json("Area_PM_10", "Area_PM_10", {"1", "2", "3"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})
        ary.Add(New class_kind_json("Area_O3", "Area_O3", {"1", "2", "3"}) _
                With {.data_unit = class_kind_json.enum_data_unit.year})

        Dim result = (From r In ary Where r.item_name = item Select r).SingleOrDefault()
        Return result

    End Function

    Public Function GetWaterItem(item As String) As class_kind_json

        Dim ary As New List(Of class_kind_json)
        ary.Add(New class_kind_json("懸浮固體", "ss"))
        ary.Add(New class_kind_json("溶氧(電極法)", "do"))
        ary.Add(New class_kind_json("氨氮", "amm"))
        ary.Add(New class_kind_json("RPI", "rpi"))
        ary.Add(New class_kind_json("生化需氧量", "bod"))

        ary.Add(New class_kind_json("km_none", "type1_km_none") With {.group_type = "sum"})
        ary.Add(New class_kind_json("km_mild", "type1_km_mild") With {.group_type = "sum"})
        ary.Add(New class_kind_json("km_mod", "type1_km_mod") With {.group_type = "sum"})
        ary.Add(New class_kind_json("km_severe", "type1_km_severe") With {.group_type = "sum"})
        ary.Add(New class_kind_json("percent_none", "type1_percent_none"))
        ary.Add(New class_kind_json("percent_mild", "type1_percent_mild"))
        ary.Add(New class_kind_json("percent_mod", "type1_percent_mod"))
        ary.Add(New class_kind_json("percent_severe", "type1_percent_severe"))

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

        Dim result = (From r In ary Where r.item_name = item Select r).SingleOrDefault()
        Return result

    End Function

    Private del_p2 As Boolean = False
    Private Sub upload_water(item As String)

        'Dim obj As New WaterData_p1
        'obj.create_data("", "", "", Nothing, Nothing, class_kind_json.enum_data_unit.year)

        Dim head() As String = {}
        Dim t As String = item.Substring(0, 2)
        item = item.Remove(0, 3)

        Dim obj_data As IWaterData = Nothing

        '刪除資料
        Dim da As New Jeff.DataAccess
        Dim sql As String = ""
        Dim table_name As String = ""
        Select Case t.ToLower()
            Case "p1"
                sql = " delete from WaterMonthValue where ITEMNAME = @ITEMNAME "
                Dim p_item As Object = da.CreateParameter("ITEMNAME", item)
                da.ExecNonQuery(sql, p_item)

                table_name = "WaterMonthValue"
                obj_data = New WaterData_p1
            Case "p2"
                If del_p2 = False Then
                    sql = " delete from RiverData_Type1 "
                    da.ExecNonQuery(sql)
                End If
                table_name = "RiverData_Type1"
                obj_data = New WaterData_p2
            Case "p3", "p4"
                sql = " delete from WaterCityMonthValue where ITEMNAME = @ITEMNAME "
                Dim p_item As Object = da.CreateParameter("ITEMNAME", item)
                da.ExecNonQuery(sql, p_item)

                table_name = "WaterCityMonthValue"
                If t = "p3" Then
                    obj_data = New WaterData_p3
                Else
                    obj_data = New WaterData_p4
                End If
        End Select

        Dim dt As Data.DataTable = Me.parse_file()
        Try
            If del_p2 = False Or t.ToLower() <> "p2" Then
                da.BulkCopy(table_name, dt)
                del_p2 = True
            End If
        Catch ex As Exception
            Jeff.JavaScript.Alert("儲存資料失敗(write db Error)", Me.Page)
            Response.Write(ex.ToString())
            Exit Sub
        End Try

        Dim water_item = Me.GetWaterItem(item)
        Dim dir_path As String = Server.MapPath("../js/WaterChart/")
        Dim chart As String = obj_data.create_data(water_item.item_name, dir_path, water_item.file_name,
                            water_item.default_sd, water_item.default_ed, water_item.data_unit, water_item.group_type)

        Response.Write("已產出 chart js file " & chart & "<br />")

        dir_path = Server.MapPath("../js/WaterMap/")
        Dim map As String = obj_data.create_map_json(water_item.item_name, dir_path, water_item.file_name,
                             water_item.data_unit, water_item.group_type)

        Response.Write("已產出 map js file " & chart & "<br />")

    End Sub

    Private Sub upload_air(item As String)

        Dim head() As String = {"COUNTY", "SITENAME", "ITEMNAME", "MONITORYEAR",
            "MONITORMONTH", "MONTHLYVALUE"}

        Dim dt As Data.DataTable = Me.parse_file()

        'check head
        Try
            For i As Integer = 0 To UBound(head)
                If dt.Columns(i).ColumnName.ToUpper() <> head(i) Then
                    Throw New Exception("head Error")
                End If
            Next
        Catch ex As Exception
            Jeff.JavaScript.Alert("表頭錯誤(head Error)", Me.Page)
            Exit Sub
        End Try

        Dim da As New Jeff.DataAccess
        Dim sql As String = " delete from AirQualityMonth where ITEMNAME = @ITEMNAME "
        Dim p_item As Object = da.CreateParameter("ITEMNAME", item)
        Try
            da.ExecNonQuery(sql, p_item)
            da.BulkCopy("AirQualityMonth", dt)
        Catch ex As Exception
            Jeff.JavaScript.Alert("儲存資料失敗(write db Error)", Me.Page)
            Response.Write(ex.Message)
            Exit Sub
        End Try

        Dim air_item = Me.GetAirItem(item)
        Dim air As New AirData_p1
        Dim dir_path As String = Server.MapPath("../js/AirQualityChart/")

        'chart js
        For k As Integer = 0 To air_item.stie_type.Count - 1
            Dim expport As String = ""
            If item.StartsWith("Area_") Then
                expport = air.create_data_area(air_item.item_name, dir_path, air_item.file_name, air_item.stie_type(k),
                   air_item.default_sd, air_item.default_ed, air_item.data_unit)
            Else
                expport = air.create_data(air_item.item_name, dir_path, air_item.file_name, air_item.stie_type(k),
                        air_item.default_sd, air_item.default_ed, air_item.data_unit)
            End If


            Response.Write("已產出 chart js file " & expport & "<br />")
        Next

        'map js
        dir_path = Server.MapPath("../js/AirQualityMap/")
        For k As Integer = 0 To air_item.stie_type.Count - 1
            Dim map_file As String = ""
            If item.StartsWith("Area_") Then
                map_file = air.create_map_json_air_area(air_item.item_name, dir_path, air_item.file_name, air_item.stie_type(k),
                      air_item.default_sd, air_item.default_ed, air_item.data_unit)
            Else
                map_file = air.create_map_json(air_item.item_name, dir_path, air_item.file_name, air_item.stie_type(k),
                       air_item.default_sd, air_item.default_ed, air_item.data_unit)
            End If

            Response.Write("已產出 map js file " & map_file & "<br />")
        Next

    End Sub

    Private Sub upload_standard(item As String)

        Dim dt As Data.DataTable = Me.parse_file()
        Dim obj As New Standard
        Try
            Select Case item.ToLower()
                Case "Car".ToLower()
                    obj.UploadCar(dt)
                Case "Scootor".ToLower()
                    obj.UploadScootor(dt)
                Case "COemission".ToLower()
                    obj.UploadCOemission(dt)
                Case "data_so2"
                    obj.UploadSO2(dt)
                Case "type_data".ToLower()
                    obj.UploadTypeData(dt)
                Case "station_create_date".ToLower()
                    obj.UploadStation(dt)
            End Select
        Catch ex As Exception
            Jeff.JavaScript.Alert("儲存資料失敗(write db Error)", Me.Page)
            Response.Write(ex.ToString())
            Exit Sub
        End Try

        Dim js_path As String = ""
        Select Case item.ToLower()
            Case "Car".ToLower()
                js_path = obj.GetCar()
            Case "Scootor".ToLower()
                js_path = obj.GetScootor()
            Case "COemission".ToLower()
                js_path = obj.GetCOemission()
            Case "data_so2".ToLower()
                js_path = obj.GetSO2()
            Case "type_data".ToLower()
                js_path = obj.GetTypeData()
            Case "station_create_date".ToLower()
                js_path = obj.GetStation()
        End Select

        Response.Write("已產出 js file " & js_path & "<br />")

    End Sub

End Class
