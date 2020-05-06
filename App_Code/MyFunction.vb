Imports Microsoft.VisualBasic

Namespace Jeff
    Public Class JavaScript
        Public Shared Sub Alert(ByVal msg As String, ByVal page As System.Web.UI.Page, Optional ByVal jsKey As String = "_jeff_js_alert")
            page.ClientScript.RegisterStartupScript(page.GetType(), jsKey, _
                                                    String.Format("alert('{0}');", msg.Replace("'", "\'")), True)
        End Sub

        Public Shared Sub AjaxAlert(ByVal msg As String, ByVal page As System.Web.UI.Page, Optional ByVal jsKey As String = "_jeff_js_alert")
            ScriptManager.RegisterStartupScript(page, page.GetType(), jsKey, _
                                                    String.Format("alert('{0}');", msg.Replace("'", "\'")), True)
        End Sub

        ''' <summary>
        ''' 增加 JS 的code 在網頁的下方
        ''' </summary>
        ''' <param name="code"></param>
        ''' <param name="page"></param>
        ''' <param name="jsKey"></param>
        ''' <remarks></remarks>
        Public Shared Sub AddJSCode(ByVal code As String, ByVal page As System.Web.UI.Page, Optional ByVal jsKey As String = "_jeff_js_addCode")
            page.ClientScript.RegisterStartupScript(page.GetType, jsKey, code, True)
        End Sub

        Public Shared Sub AddAjaxJSCode(ByVal code As String, ByVal page As System.Web.UI.Page, Optional ByVal jsKey As String = "_jeff_js_addCode")
            ScriptManager.RegisterStartupScript(page, page.GetType, jsKey, code, True)
        End Sub

        ''' <summary>
        ''' Javascript 特殊字元轉換
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Escape(ByVal text As String) As String
            text = text.Replace("\", "\\").Replace("'", "\'")
            Return text
        End Function
    End Class

    Public Class File
        ''' <summary>
        '''  傳回檔案大小格式
        ''' </summary>
        ''' <param name="size"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetFileSizeString(ByVal size As Object) As String
            If IsNothing(size) Then Return "-- KB"
            If IsDBNull(size) Then Return "-- KB"
            If size > 1024 * 1000 Then
                If size > 1024 * 1024 * 1000 Then
                    'GB
                    Return (CDbl(size) / 1024 / 1024 / 1024).ToString("#,##0.00") & " GB"
                Else
                    'MB
                    Return (CDbl(size) / 1024 / 1024).ToString("#,##0.00") & " MB"
                End If
            Else
                Return (CDbl(size) / 1024).ToString("#,##0.##") & " KB"
            End If
        End Function

        ''' <summary>
        ''' 傳回不重複的檔案路徑
        ''' </summary>
        ''' <param name="filePath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetUniqueFilePath(ByVal filePath As String) As String
            Dim name As String = IO.Path.GetFileNameWithoutExtension(filePath)
            Dim ext As String = IO.Path.GetExtension(filePath)
            Dim dir As String = IO.Path.GetDirectoryName(filePath)
            Dim sum As Integer = 0
            While True
                If IO.File.Exists(filePath) = False Then
                    Return filePath
                End If
                filePath = IO.Path.Combine(dir, name & "(" & sum + 1 & ")" & ext)
                sum += 1
                If sum >= 50 Then
                    Throw New Exception("無法建立檔案路徑:" & filePath)
                End If
            End While
            Return ""
        End Function
    End Class

    Public Class Json
        Public Shared Function DataTableToJson(ByVal dataTable As Data.DataTable, Optional ByVal Sort As String = "") As String
            Dim el As New List(Of String)
            Dim sort_rows() As Data.DataRow = dataTable.Select("", Sort)
            For row As Integer = 0 To sort_rows.Length - 1
                Dim attrib As New List(Of String)
                For col As Integer = 0 To dataTable.Columns.Count - 1
                    Dim value As String = HttpUtility.HtmlEncode("" & sort_rows(row)(col)).Replace("\", "\\").Replace("""", "\""")
                    value = value.Replace(vbCrLf, vbCr).Replace(vbCr, "\n")
                    attrib.Add("""" & dataTable.Columns(col).ColumnName & """:""" & value & """")
                Next
                el.Add("{" & String.Join(",", attrib.ToArray) & "}")
            Next
            Return "[" & String.Join(",", el.ToArray) & "]"
        End Function
    End Class


End Namespace

Namespace Jeff.UI
    Public Class Control

        ''' <summary>
        ''' 設定 CheckBoxList 勾選的項目
        ''' </summary>
        ''' <param name="checkBoxList"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Shared Sub SetCheckBoxListSelectValue(ByRef checkBoxList As CheckBoxList, ByVal value() As String)
            For i = 0 To value.Length - 1
                Dim t As String = value(i)
                Dim item As ListItem = checkBoxList.Items.FindByValue(t)
                If (item IsNot Nothing) Then item.Selected = True
            Next
        End Sub

        ''' <summary>
        ''' 設定 DropdownList
        ''' </summary>
        ''' <param name="objDropdown"></param>
        ''' <param name="selectValue"></param>
        ''' <remarks></remarks>
        Public Shared Sub SetDropdownListValue(ByRef objDropdown As DropDownList, ByVal selectValue As String)
            objDropdown.SelectedIndex = -1
            Dim item As ListItem = objDropdown.Items.FindByValue(selectValue)
            If (item IsNot Nothing) Then
                item.Selected = True
            End If
        End Sub

        ''' <summary>
        ''' 設定 RadioButtonList
        ''' </summary>
        ''' <param name="objRadioList"></param>
        ''' <param name="selectValue"></param>
        ''' <remarks></remarks>
        Public Shared Sub SetRadioListValue(ByRef objRadioList As RadioButtonList, ByVal selectValue As String)
            objRadioList.SelectedIndex = -1
            Dim item As ListItem = objRadioList.Items.FindByValue(selectValue)
            If (item IsNot Nothing) Then
                item.Selected = True
            End If
        End Sub

        Public Shared Sub SetListboxValue(ByRef objListbox As ListBox, ByVal value() As String)            
            objListbox.SelectedIndex = -1
            objListbox.ClearSelection()
            For i = 0 To value.Length - 1
                Dim t As String = value(i)
                Dim item As ListItem = objListbox.Items.FindByValue(t)
                If (item IsNot Nothing) Then item.Selected = True
            Next
        End Sub

        Public Shared Sub SetGridRowValue(Row As GridViewRow, ControlName As String, Value As String)
            Dim obj As Object = Row.FindControl(ControlName)
            If obj IsNot Nothing Then
                Select Case obj.GetType().Name
                    Case "TextBox"
                        CType(obj, TextBox).Text = Value
                    Case "RadioButtonList"
                        SetRadioListValue(obj, Value)
                    Case "DropDownList"
                        SetDropdownListValue(obj, Value)
                End Select
            End If
        End Sub

        Public Shared Function GetGridRowValue(Row As GridViewRow, ControlName As String) As String
            Dim obj As Object = Row.FindControl(ControlName)
            If obj IsNot Nothing Then
                Select Case obj.GetType().Name
                    Case "TextBox"
                        Return CType(obj, TextBox).Text
                    Case "RadioButtonList"
                        Return CType(obj, RadioButtonList).SelectedValue
                    Case "DropDownList"
                        Return CType(obj, DropDownList).SelectedValue
                    Case Else
                        Return ""
                End Select
            End If
            Return ""
        End Function
    End Class



    Public Class Export
        ''' <summary>
        ''' GridView 轉 CSV 格式
        ''' </summary>
        ''' <param name="gv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GridViewToCSV(ByRef gv As GridView) As String
            If gv.Rows.Count = 0 Then Return ""
            Dim csv As New System.Text.StringBuilder
            'head
            For i = 0 To gv.HeaderRow.Cells.Count - 1
                csv.Append(gv.HeaderRow.Cells(i).Text & ",")
            Next
            csv.Append(vbCrLf)

            'content
            For i = 0 To gv.Rows.Count - 1
                For k = 0 To gv.Rows(i).Cells.Count - 1
                    csv.Append(gv.Rows(i).Cells(k).Text & ",")
                Next
                csv.Append(vbCrLf)
            Next
            Return csv.ToString()
        End Function

    End Class


End Namespace
