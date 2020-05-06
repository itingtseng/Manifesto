Imports Microsoft.VisualBasic

Public Interface IWaterData

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

    Function GetDataTable(item As String) As Data.DataTable

    Function create_data(ITEM_NAME As String, Dir As String, FileName As String,
                            start_date As DateTime?, end_date As DateTime?,
                         data_unit As class_kind_json.enum_data_unit, Optional gropu_type As String = "avg") As String

    Function get_area_json(leval As String, item As String,
                                    sd As DateTime, ed As DateTime,
                           data_unit As class_kind_json.enum_data_unit, Optional gropu_type As String = "avg") As String()

    Function create_map_json(item_name As String, Dir As String, FileName As String,
                             data_unit As class_kind_json.enum_data_unit, Optional gropu_type As String = "avg") As String

    Function get_map_json(leval As String, obj_id As String, item As String,
                                    sd As DateTime, ed As DateTime,
                          data_unit As class_kind_json.enum_data_unit, Optional gropu_type As String = "avg") As String


End Interface
