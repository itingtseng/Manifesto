<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataImport.aspx.vb" Inherits="DataImport_DataImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h4>資料匯入</h4>
        </div>

        <div>
        </div>
        <div>
            <table cellpadding="5">
                <tr>
                    <td>
                        <asp:RadioButton ID="RadioButton_air" GroupName="type" runat="server" Checked="true" />空氣資料
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_aire" runat="server">
                            <asp:ListItem>二氧化硫</asp:ListItem>
                            <asp:ListItem>懸浮微粒</asp:ListItem>
                            <asp:ListItem>臭氧</asp:ListItem>
                            <asp:ListItem>一氧化碳</asp:ListItem>
                            <asp:ListItem Enabled="false">二氧化碳</asp:ListItem>
                            <asp:ListItem>二氧化氮</asp:ListItem>
                            <asp:ListItem>非甲烷碳氫化合物</asp:ListItem>
                            <asp:ListItem>細懸浮微粒</asp:ListItem>
                            <asp:ListItem Value="細懸浮微粒_Data2">細懸浮微粒(手動測站)</asp:ListItem>
                            <asp:ListItem Value="PSI">PSI</asp:ListItem>
                            <asp:ListItem Value="PSI_COUNT_PERCENT">PSI(百分比)</asp:ListItem>
                            <asp:ListItem>鉛</asp:ListItem>
                            <asp:ListItem Enabled="false">空氣污染防制</asp:ListItem>
                            <asp:ListItem Value="Area_PM_10">空品區間(懸浮微粒)</asp:ListItem>
                            <asp:ListItem Value="Area_O3">空品區間(臭氧)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton ID="RadioButton_water" GroupName="type" runat="server" />水資料
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList_water" runat="server">
                            <asp:ListItem Value="p1_懸浮固體">懸浮固體</asp:ListItem>
                            <asp:ListItem Value="p1_溶氧(電極法)">溶氧(電極法)</asp:ListItem>
                            <asp:ListItem Value="p1_氨氮">氨氮</asp:ListItem>
                            <asp:ListItem Value="p1_RPI" Enabled="false">RPI</asp:ListItem>
                            <asp:ListItem Value="p1_生化需氧量">生化需氧量</asp:ListItem>
                            <asp:ListItem Value="p2_river_all">p2_河川水質調查</asp:ListItem>
                            <asp:ListItem Value="p3_s4_house">s4_house</asp:ListItem>
                            <asp:ListItem Value="p3_s4_water">s4_water</asp:ListItem>
                            <asp:ListItem Value="p3_s8_人口數">s8_人口數</asp:ListItem>
                            <asp:ListItem Value="p3_s8_列管家數">s8_列管家數</asp:ListItem>
                            <asp:ListItem Value="p3_s8_工業廢水削減量">s8_工業廢水削減量</asp:ListItem>
                            <asp:ListItem Value="p3_s8_工業廢水產生量">s8_工業廢水產生量</asp:ListItem>
                            <asp:ListItem Value="p3_s8_生活污水削減量">s8_生活污水削減量</asp:ListItem>
                            <asp:ListItem Value="p3_s8_生活污水產生量">s8_生活污水產生量</asp:ListItem>
                            <asp:ListItem Value="p3_s8_畜牧現有頭數">s8_畜牧現有頭數</asp:ListItem>
                            <asp:ListItem Value="p3_s8_畜牧廢水削減量">s8_畜牧廢水削減量</asp:ListItem>
                            <asp:ListItem Value="p3_s8_畜牧廢水產生量">s8_畜牧廢水產生量</asp:ListItem>
                            <asp:ListItem Value="p3_水污染防治" Enabled="false">水污染防治</asp:ListItem>
                            <asp:ListItem Value="p3_自來水" Enabled="false">自來水</asp:ListItem>
                            <asp:ListItem Value="p3_直接供水" Enabled="false">直接供水</asp:ListItem>
                            <asp:ListItem Value="p3_飲用水" Enabled="false">飲用水</asp:ListItem>
                            <asp:ListItem Value="p3_簡易自來水" Enabled="false">簡易自來水</asp:ListItem>
                            <asp:ListItem Value="p4_s5_水庫優養">s5_水庫優養</asp:ListItem>
                            <asp:ListItem Value="p4_s6_percent">海域水質監測</asp:ListItem>
                            <asp:ListItem Value="p4_s6_ph" Enabled="false">s6_ph</asp:ListItem>
                            <asp:ListItem Value="p4_s6_汞" Enabled="false">s6_汞</asp:ListItem>
                            <asp:ListItem Value="p4_s6_溶氧量" Enabled="false">s6_溶氧量</asp:ListItem>
                            <asp:ListItem Value="p4_s6_鉛" Enabled="false">s6_鉛</asp:ListItem>
                            <asp:ListItem Value="p4_s6_銅" Enabled="false">s6_銅</asp:ListItem>
                            <asp:ListItem Value="p4_s6_鋅" Enabled="false">s6_鋅</asp:ListItem>
                            <asp:ListItem Value="p4_s6_鎘" Enabled="false">s6_鎘</asp:ListItem>
                            <asp:ListItem Value="p4_s9_beach">海灘水質監測</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton ID="RadioButton_standard" GroupName="type" runat="server" />標準資料</td>
                    <td>
                        <asp:DropDownList ID="DropDownList_standard" runat="server">
                            <asp:ListItem Value="Car">汽車數量</asp:ListItem>
                            <asp:ListItem Value="Scootor">機車數量</asp:ListItem>
                            <asp:ListItem Value="COemission">一氧化碳排放標準</asp:ListItem>
                            <asp:ListItem Value="data_so2">燃料油含硫標準</asp:ListItem>
                            <asp:ListItem Value="type_data">車輛 HC,THC排放標準 / 無鉛汽油使用率 / 有鉛汽油含鉛量</asp:ListItem>
                            <asp:ListItem Value="station_create_date">測站建置時間</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <br />
            <asp:Button ID="Button2" runat="server" Text="下載資料" />
        </div>
        <div>
            <br />
            選擇檔案：<asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="Button1" runat="server" Text="上傳" />
        </div>
    </form>
</body>
</html>
