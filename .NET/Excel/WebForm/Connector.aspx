<%@ Page Title="" Language="C#" MasterPageFile="~/MainPages/NavGlobalMaster.master" AutoEventWireup="true"
    Inherits="PCG.Skovision.Web.Admin.Connector" CodeBehind="Connector.aspx.cs" %>


<asp:Content runat="server" ContentPlaceHolderID="PageTitle">
    <asp:Literal runat="server" Text="Connector" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPHContent" runat="Server">
    <asp:ScriptManager ID="ScriptManagerConnector" runat="server">
    </asp:ScriptManager>

    <style type="text/css">
        .radio-input {
            margin-right: 10px;
        }

        #validationPanel td {
            padding-left: 0.5em;
        }

        .element_hide {
            display: none;
        }

        .file_upload {
            margin-right: 30px;
            border: 1px solid #deecf9;
            width: 460px;
            padding: 3px;
        }

        .file_uploader {
            /*border-style: solid;
            border-width: 1px;
            border-color: #EEEEEE;*/
            border-radius: 2px;
            -webkit-box-shadow: 1px 1px 1px 1px #9d9999;
            -ms-box-shadow: 1px 1px 1px 1px #9d9999;
            box-shadow: 1px 1px 1px 1px #9d9999;
            padding: 10px 20px;
            width: 630px;
        }

        .file_upload_button {
        }

        input[type=submit] {
            width: 120px;
        }
    </style>
    <table id="TableContent" style="width: 100%; height: 100%;">

        <tr>
            <td style="width: 100%; height: 100%;" valign="top" style="font-family: Verdana; font-size: 10pt">
                <table style="width: 100%; height: 100%;">
                    <tr>
                        <td>
                            <div style="margin-left: 10px">
                                <table width="60%">
                                    <tr>
                                        <td style="height: 15px"></td>
                                    </tr>
                                    <tr>
                                        <td class="Title" style="font-family: Verdana">
                                            <asp:Literal ID="lblTemplate" runat="server" Text="<%$Resources:FILE_TEMPLATE %>" /></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <hr />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span class="radio-input">
                                                <asp:RadioButton ID="Rd_Tpl_Import" runat="server" GroupName="RadioBtnTemplates" Checked="True"
                                                    EnableViewState="False" OnCheckedChanged="FileTemplateChanged" />
                                                <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="<%$ Resources:IMPORT_RESULT%>" />
                                            </span>
                                            <span class="radio-input">
                                                <asp:RadioButton ID="Rd_Tpl_Update" runat="server" GroupName="RadioBtnTemplates" Checked="false"
                                                    EnableViewState="False" OnCheckedChanged="FileTemplateChanged" />
                                                <asp:Label ID="Label2" Font-Bold="true" runat="server" Text="<%$ Resources:UPDATE_INDECATOR%>" />
                                            </span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 15px"></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table style="width: 400px;">
                                                <tr>
                                                    <td class="Title" style="font-family: Verdana">
                                                        <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:ORGANIZATION_OPTIONS %>" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <hr />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span class="radio-input">
                                                            <asp:RadioButton ID="Rd_Org_FullName" runat="server" GroupName="Rd_Org" Checked="True" />
                                                            <asp:Label ID="BriefPlanOverviewReportLbl" Font-Bold="true" runat="server" Text="<%$ Resources:ORGANIZATION_FULL_NAME%>" />
                                                        </span>
                                                        <span class="radio-input">
                                                            <asp:RadioButton ID="Rd_Org_ShortName" runat="server" GroupName="Rd_Org" Checked="False" />
                                                            <asp:Label ID="PlanOverviewReportLbl" Font-Bold="true" runat="server" Text="<%$ Resources:ORGANIZATION_SHORT_NAME%>" />
                                                        </span>
                                                        <span class="radio-input">
                                                            <asp:RadioButton ID="Rd_Org_Code" runat="server" GroupName="Rd_Org" Checked="false" />
                                                            <asp:Label ID="ScoreCardReportLbl" Font-Bold="true" runat="server" Text="<%$ Resources:ORGANIZATION_CODE%>" />
                                                        </span>

                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <asp:Panel ID="ActionExisting" runat="server">
                                                <table style="width: 400px;">
                                                    <tr>
                                                        <td style="height: 15px"></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="Title" style="font-family: Verdana">
                                                            <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:ACTION_MATCHED %>" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <hr />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <div class="radio-input">
                                                                <asp:RadioButton ID="Rd_Matched_Replace" runat="server" GroupName="RadioBtnMatched" Checked="True" />
                                                                <asp:Label ID="Label3" Font-Bold="true" runat="server" Text="<%$ Resources:ACTION_REPLACE%>" />
                                                            </div>
                                                            <div class="radio-input">
                                                                <asp:RadioButton ID="Rd_Matched_Ignore" runat="server" GroupName="RadioBtnMatched" Checked="false" />
                                                                <asp:Label ID="Label4" Font-Bold="true" runat="server" Text="<%$ Resources:ACTION_IGNORE%>" />
                                                            </div>

                                                        </td>
                                                    </tr>
                                                </table>

                                            </asp:Panel>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="height: 15px"></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table style="width: 400px;">
                                                <tr>

                                                    <td class="Title" style="font-family: Verdana">
                                                        <asp:Literal ID="Literal3" runat="server" Text="<%$Resources:ROLLBACK_PROMPT %>" />

                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <hr />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div>
                                                            <asp:Button runat="server" ID="ButtonRollback" Text="<%$ Resources:ROLLBACK%>" />
                                                            <asp:Button runat="server" ID="ButtonRollbackImport" Text="Rollback Import"
                                                                CssClass="element_hide" OnClick="ButtonRollbackImport_Click" />
                                                            <asp:Button runat="server" ID="ButtonRollbackUpdate" Text="Rollback Update"
                                                                CssClass="element_hide" OnClick="ButtonRollbackUpdate_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>

                                </table>
                            </div>

                        </td>
                    </tr>
                    <tr>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div style="margin-left: 20px; margin-top: 10px;width: 60%;">
                    <div class="Title" style=" margin-bottom: 12px;font-family: Verdana">
                            <asp:Literal ID="Literal4" runat="server" Text="<%$Resources:UPLOAD_TITLE %>" />
                        <hr />
                    </div>
                    <div class="file_uploader">
                        <asp:FileUpload ID="FileUploadControl" runat="server" EnableViewState="False" CssClass="file_upload" />
                        <div style="display: inline;" class="file_upload_button">
                            <asp:Button runat="server" ID="UploadButton" Text="<%$Resources:UPLOAD %>" OnClick="UploadButton_Click" />
                        </div>
                    </div>
                    <br />
                    <br />
                    <div id="validationPanel">
                        <br />
                        <asp:BulletedList ID="FeedbackList" BulletStyle="Disc" Font-Bold="True" ForeColor="DarkBlue"
                            EnableViewState="False" runat="server">
                        </asp:BulletedList>
                        <br />
                        <asp:Button runat="server" ID="btnExportExcel" Text="<%$Resources:EXPORT_TO_EXCEL %>" OnClick="ExportExcel_Click" />
                        <br />
                        <br />
                        <div id="error_area">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="GridViewError" runat="server"
                                        AllowPaging="True"
                                        PageSize="5"
                                        OnPageIndexChanging="GridViewError_PageIndexChanging"
                                        EnableViewState="False"
                                        CellPadding="0"
                                        ForeColor="#333333" HorizontalAlign="Center"
                                        UseAccessibleHeader="False" Width="100%"
                                        AutoGenerateColumns="False">
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="False" ForeColor="White" />
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" VerticalAlign="Middle" Height="25px" />
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="False" ForeColor="#333333" />
                                        <PagerStyle CssClass="TopMenu" ForeColor="Black" HorizontalAlign="Center" />
                                        <HeaderStyle CssClass="GridHeader" Font-Bold="False" Height="25px" />
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                        <Columns>
                                            <asp:BoundField DataField="RowNumber" HeaderText="Row" ReadOnly="True" />
                                            <asp:BoundField DataField="ColumnName" HeaderText="Column" ReadOnly="True" />
                                            <asp:BoundField DataField="InvalidData" HeaderText="Invalid Data" ReadOnly="True" />
                                            <asp:BoundField DataField="Message" HeaderText="Message" ReadOnly="True" />
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        /* page variables */
        var _isImport = true;
        var _confirmRollbackImport = '<%=GetLocalResourceObject("CONFIRM_ROLLBACK_IMPORT") %>';
        var _confirmRollbackUpdate = '<%=GetLocalResourceObject("CONFIRM_ROLLBACK_UPDATE") %>';
        var $radionImport = $('#<%=Rd_Tpl_Import.ClientID %>');
        var $radioUpdate = $('#<%=Rd_Tpl_Update.ClientID %>');
        var $actionPanel = $('#<%=ActionExisting.ClientID %>');
        var $btnRollbackImport = $('#<%=ButtonRollbackImport.ClientID %>');
        var $btnRollbackUpdate = $('#<%=ButtonRollbackUpdate.ClientID %>');
        var $btnRollback = $('#<%=ButtonRollback.ClientID %>');

        /* document.ready */
        $(function () {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initIE);
            initStatus();
            initTemplateChangeEvent();
            initFileUploadEvent();
            initRollbackEvent();
            initIE();
        });


        //#region initial functions

        function initStatus() {
            if ($radionImport.prop('checked') === true) {
                // Import
                _isImport = true;
                getRollbackStatus('ROLLBACKIMPORT_CHECK');
                $actionPanel.show();
            } else {
                // Update
                _isImport = false;
                getRollbackStatus('ROLLBACKUPDATE_CHECK');
                $actionPanel.hide();
            }
        }

        function initTemplateChangeEvent() {
            $radionImport.on('change', function () {
                $('#validationPanel').hide();
                if ($(this).prop('checked') === true) {
                    // Import
                    _isImport = true;
                    getRollbackStatus('ROLLBACKIMPORT_CHECK');
                    $actionPanel.show();
                }
            });
            $radioUpdate.change(function () {
                $('#validationPanel').hide();
                if ($(this).prop('checked') === true) {
                    // Update
                    _isImport = false;
                    getRollbackStatus('ROLLBACKUPDATE_CHECK');
                    $actionPanel.hide();
                }
            });
        }

        function initFileUploadEvent() {
            $("input:file").on('change', function () {
                $('#validationPanel').hide();
            });
        }

        function initRollbackEvent() {
            $btnRollback.on('click', function (event) {
                event.preventDefault();
                if ($(this).is("[disabled]")) {
                    return;
                }
                var method = _isImport ? 'ROLLBACKIMPORT' : 'ROLLBACKUPDATE';
                var confirmMessage = _isImport ? _confirmRollbackImport : _confirmRollbackUpdate;
                if (confirm(confirmMessage)) {
                    rollback(method);
                } else {
                    return;
                }
            });
        }

        function initIE() {
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");
            if (msie > 0) {
                $("#error_area table").css('border-collapse', 'separate');
                $(".file_uploader").css({ 'border-style': 'solid', 'border-width': '1px', 'border-color': '#CCD1BD' });
            }
        }
        //#endregion

        //#region action functions

        function getRollbackStatus(method) {
            enableRollback(false);
            jQuery.ajax({
                type: 'GET',
                cache: false,
                url: 'Connector.ashx',
                data: 'MethodName=' + method
            })
                .done(function (data) {
                    var enable = data.data;
                    enableRollback(enable);
                }).fail(function (data) {
                    enableRollback(false);
                }).always(function (date) {
                });
        }

        function rollback(method) {
            if (method === 'ROLLBACKIMPORT') {
                $btnRollbackImport.click();
            } else if (method === 'ROLLBACKUPDATE') {
                $btnRollbackUpdate.click();
            }
        }

        function enableRollback(enable) {
            $btnRollback.prop('disabled', !enable);
        }
        //#endregion

    </script>
</asp:Content>
