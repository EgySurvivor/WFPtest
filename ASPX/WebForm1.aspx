<%@ Page Title="" Language="C#" MasterPageFile="~/ASPX/Site1.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WFPtest.ASPX.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" DataSourceID="SqlDataSource1" DataTextField="nUserID" DataValueField="nUserID">
</asp:DropDownList>
<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:BioStarConnectionString %>" SelectCommand="SELECT DISTINCT [nUserID], [nEventIdn] FROM [TB_EVENT_LOG] WHERE ([nUserID] &gt; @nUserID) ORDER BY [nUserID]">
    <SelectParameters>
        <asp:Parameter DefaultValue="2000" Name="nUserID" Type="Int32" />
    </SelectParameters>
</asp:SqlDataSource>
    <br />
    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" DataSourceID="StaffData" DataTextField="EMAIL_ADDRESS" DataValueField="EMP_ID">
    </asp:DropDownList>
    <asp:SqlDataSource ID="StaffData" runat="server" ConnectionString="<%$ ConnectionStrings:WFPConnectionString %>" SelectCommand="SELECT EMP_ID, EMAIL_ADDRESS FROM EMPLOYEES WHERE (EMAIL_ADDRESS IS NOT NULL AND EMAIL_ADDRESS &lt;&gt; '') ORDER BY EMAIL_ADDRESS"></asp:SqlDataSource>
    <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="White" BorderWidth="1px" Font-Names="Verdana" Font-Size="9pt" ForeColor="Black" Height="190px" NextPrevFormat="FullMonth" Width="350px">
        <DayHeaderStyle Font-Bold="True" Font-Size="8pt" />
        <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" VerticalAlign="Bottom" />
        <OtherMonthDayStyle ForeColor="#999999" />
        <SelectedDayStyle BackColor="#333399" ForeColor="White" />
        <TitleStyle BackColor="White" BorderColor="Black" BorderWidth="4px" Font-Bold="True" Font-Size="12pt" ForeColor="#333399" />
        <TodayDayStyle BackColor="#CCCCCC" />
    </asp:Calendar>
    <br />
    <asp:Calendar ID="Calendar2" runat="server" BackColor="White" BorderColor="White" BorderWidth="1px" Font-Names="Verdana" Font-Size="9pt" ForeColor="Black" Height="190px" NextPrevFormat="FullMonth" Width="350px">
        <DayHeaderStyle Font-Bold="True" Font-Size="8pt" />
        <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" VerticalAlign="Bottom" />
        <OtherMonthDayStyle ForeColor="#999999" />
        <SelectedDayStyle BackColor="#333399" ForeColor="White" />
        <TitleStyle BackColor="White" BorderColor="Black" BorderWidth="4px" Font-Bold="True" Font-Size="12pt" ForeColor="#333399" />
        <TodayDayStyle BackColor="#CCCCCC" />
    </asp:Calendar>
    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource2" PageSize="60">
        <Columns>
            <asp:BoundField DataField="nEventLogIdn" HeaderText="nEventLogIdn" InsertVisible="False" ReadOnly="True" SortExpression="nEventLogIdn" />
            <asp:BoundField DataField="nUserID" HeaderText="nUserID" SortExpression="nUserID" />
            <asp:BoundField DataField="date" HeaderText="Date" ReadOnly="True" SortExpression="date" />
            <asp:BoundField DataField="nReaderIdn" HeaderText="nReaderIdn" SortExpression="nReaderIdn" />
            <asp:TemplateField HeaderText="In/Out" SortExpression="nReaderIdn">  <ItemTemplate>
                <%# ReturnText(Eval("nReaderIdn")) %>
            </ItemTemplate> </asp:TemplateField>
            <asp:TemplateField HeaderText="Office" ><ItemTemplate>
                <%# ReturnTextOffice(Eval("nReaderIdn")) %>
            </ItemTemplate></asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:BioStarConnectionString %>" SelectCommand="SELECT nEventLogIdn, nUserID, date, nReaderIdn FROM TB_EVENT_LOG WHERE (nUserID = @nUserID) AND (date &gt;= @date) AND (nTNAEvent = @nTNAEvent) AND (date &lt;= @date2)">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList2" Name="nUserID" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="Calendar1" Name="date" PropertyName="SelectedDate" Type="DateTime" />
            <asp:Parameter DefaultValue="1" Name="nTNAEvent" Type="Int16" />
            <asp:ControlParameter ControlID="Calendar2" Name="date2" PropertyName="SelectedDate" Type="DateTime" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
</asp:Content>
