Imports Microsoft.VisualBasic

Namespace Lotto.DataAccess
    ''' <summary>
    ''' 資料表回傳物件
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataResult
        Public RecordCount As Integer = 0
        Public PageSize As Integer = 0
        Public ReadOnly Property PageCount As Integer
            Get
                Dim count As Integer = 0
                'total page
                If Me.PageSize > 0 Then
                    count = Math.Ceiling(Me.RecordCount / Me.PageSize)
                End If
                Return count
            End Get
        End Property

        Public Table As Data.DataTable = Nothing

        Public Property PageIndex As Integer
            Get
                '判斷分頁 Index 不能大於總頁數
                If (Me._pageIndex > Me.PageCount) Then PageIndex = Me.PageCount
                If (Me._pageIndex < 1) Then PageIndex = 1

                Return _pageIndex
            End Get
            Set(value As Integer)
                Me._pageIndex = value
            End Set
        End Property

        Private _pageIndex As Integer = 1

        Public Sub New(pageSize As Integer, rowCount As Integer, pageIndex As Integer)
            Me.PageSize = pageSize
            Me.RecordCount = rowCount
            Me.PageIndex = pageIndex
        End Sub

        Public Sub New(pageSize As Integer)
            Me.PageSize = pageSize
        End Sub
    End Class
End Namespace

