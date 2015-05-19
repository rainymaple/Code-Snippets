using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PCG.Skovision.Business.Connector;

namespace PCG.Skovision.Web.Admin
{
    public partial class Connector : Page
    {
        private readonly ConnectorBll _connectorBll;
        public ExcelImportResult ImportResult { get; set; }

        public List<InvalidMessage> InvalidMessages
        {
            get { return Session["InvalidMessage"] as List<InvalidMessage>; }
            set { Session["InvalidMessage"] = value; }
        }

        public Connector()
        {
            _connectorBll = new ConnectorBll();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearErrorMessages();
            }
            if (InvalidMessages == null || !InvalidMessages.Any())
            {
                FeedbackList.Items.Clear();
                GridViewError.Visible = false;
                btnExportExcel.Visible = false;
            }
        }
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            ClearErrorMessages();

            if (FileUploadControl.HasFile)
            {
                try
                {
                    var extension = Path.GetExtension(FileUploadControl.FileName);
                    if (extension != null)
                    {
                        var fileExt = extension.ToLower();
                        if (fileExt != ".xls" && fileExt != ".xlsx")
                        {
                            throw new Exception(Localize("VAL_ONLY_EXCEL_ALLOWED"));
                        }
                    }
                    else
                    {
                        throw new Exception(Localize("VAL_FAILED_TO_READ_UPLOADED_FILE"));
                    }
                    var watch = Stopwatch.StartNew();

                    // set the upload file template type and organization name type
                    _connectorBll.FileTemplate = GetTemplateType();
                    _connectorBll.OrganizationNameType = GetOuNameType();

                    var importSucceeded = ImportAndValidateExcelFile();
                    watch.Stop();
                    var elapsed = watch.Elapsed;
                    var message = string.Format(Localize("MSG_VALIDATION_TIME"), elapsed.Minutes, elapsed.Seconds);
                    if (importSucceeded)
                    {
                        ClearErrorMessages();
                        watch = Stopwatch.StartNew();
                        message += RunDbProcess();
                        watch.Stop();
                        var elapsed2 = watch.Elapsed;
                        message += string.Format(Localize("MSG_EXECUTION_TIME"), elapsed2.Minutes, elapsed2.Seconds);
                        ShowFeedback(message);
                    }
                    else
                    {
                        ShowFeedback(message);
                    }
                }
                catch (Exception ex)
                {
                    ShowFeedback(ex.Message);
                }
                finally
                {
                    FileUploadControl.Attributes.Clear();
                }
            }
            else
            {
                ShowFeedback(Localize("VAL_SPECIFY_A_FILE"));
            }
        }

        protected void ExportExcel_Click(object sender, EventArgs e)
        {
            Session["PostID"] = "1001";
            ViewState["PostID"] = Session["PostID"].ToString();

            // Save the Excel spreadsheet to a MemoryStream and return it to the client
            if (InvalidMessages == null)
            {
                return;
            }
            using (var exportData = _connectorBll.ExportExcel(InvalidMessages))
            {
                var saveAsFileName = string.Format("ValidationMessage-{0:d}.xls", DateTime.Now).Replace("/", "-");
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                Response.Clear();
                Response.BinaryWrite(exportData.GetBuffer());
                Response.End();
            }
        }
        protected void GridViewError_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewError.DataSource = InvalidMessages;
            GridViewError.PageIndex = e.NewPageIndex;
            GridViewError.DataBind();
        }
        private string RunDbProcess()
        {
            return _connectorBll.RunDbProcess(GetTemplateType(), ImportResult, GetOuNameType().ToString(),
                GetActionForIndicator(), SkovisionUserSession.AccountUserId);
        }

        private FileTemplate GetTemplateType()
        {
            if (Rd_Tpl_Import.Checked)
            {
                return FileTemplate.Import;
            }
            return FileTemplate.Update;
        }
        private OrganizationNameType GetOuNameType()
        {
            var orgNameType = OrganizationNameType.FullName;
            if (Rd_Org_ShortName.Checked)
            {
                orgNameType = OrganizationNameType.ShortName;
            }
            else if (Rd_Org_Code.Checked)
            {
                orgNameType = OrganizationNameType.Code;
            }
            return orgNameType;
        }

        private ActionForIndicator GetActionForIndicator()
        {
            if (Rd_Matched_Replace.Checked)
            {
                return ActionForIndicator.Replace;
            }
            return ActionForIndicator.Discard;
        }


        private bool ImportAndValidateExcelFile()
        {
            // validate the excel file data
            ImportResult = _connectorBll.ImportAndValidateExcel(string.Empty, FileUploadControl.FileBytes);
            var validateMsgs = ImportResult.ExcelValidateMessage;

            if (validateMsgs.Any())
            {
                CreateHtmlErrorMessage(validateMsgs);
                return false;
            }
            // validate against business rules
            ImportResult = _connectorBll.ValidateBusinessRules(ImportResult);
            validateMsgs = ImportResult.BusinessValidateMessage;

            if (validateMsgs.Any())
            {
                CreateHtmlErrorMessage(validateMsgs);
                return false;
            }
            return true;
        }

        private void CreateHtmlErrorMessage(List<InvalidMessage> validateMsgs)
        {
            ShowFeedback(Localize("ERROR_PROMPT"));
            if (validateMsgs.Any())
            {
                InvalidMessages = validateMsgs;
                GridViewError.DataSource = validateMsgs;
                GridViewError.DataBind();
                GridViewError.Visible = true;
                btnExportExcel.Visible = true;
            }
            else
            {
                ClearErrorMessages();
                GridViewError.Visible = false;
                btnExportExcel.Visible = false;
            }
        }

        private void ShowFeedback(string message)
        {
            var messages = message.Split('.').Where(c => c.Trim() != string.Empty);
            FeedbackList.DataSource = messages;
            FeedbackList.DataBind();
        }

        private void ClearErrorMessages()
        {
            InvalidMessages = null;
            FeedbackList.Items.Clear();
            GridViewError.Visible = false;
            btnExportExcel.Visible = false;
        }

        private string Localize(string key)
        {
            return (string)GetLocalResourceObject(key);
        }
        private bool IsRefresh()
        {
            if (!IsPostBack)
            {
                Session["PostID"] = "1001";
                ViewState["PostID"] = Session["PostID"].ToString();
                //First Load
                return false;
            }
            if (ViewState["PostID"].ToString() == Session["PostID"].ToString())
            {

                Session["PostID"] = (Convert.ToInt16(Session["PostID"]) + 1).ToString();

                ViewState["PostID"] = Session["PostID"].ToString();
                //Postback
                return false;

            }
            ViewState["PostID"] = Session["PostID"].ToString();
            //refreshed
            return true;
        }

        protected void FileTemplateChanged(object sender, EventArgs e)
        {
            //ClearErrorMessages();
        }

        protected void ButtonRollbackImport_Click(object sender, EventArgs e)
        {
            ClearErrorMessages();
            ShowFeedback(_connectorBll.Rollback(FileTemplate.Import));
        }
        protected void ButtonRollbackUpdate_Click(object sender, EventArgs e)
        {
            ClearErrorMessages();
            ShowFeedback(_connectorBll.Rollback(FileTemplate.Update));
        }

    }
}