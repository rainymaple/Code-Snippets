using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PCG.Skovision.Business.Connector;

namespace PCG.Skovision.Web.Admin
{

    public class Connector1 : IHttpHandler
    {
        private readonly ConnectorBll _connectorBll;
        private readonly object _responseTrue;
        private readonly object _responseFalse;

        public Connector1()
        {
            _responseTrue = new { data = true };
            _responseFalse = new { data = false };
            _connectorBll = new ConnectorBll();
        }
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";
            var method = context.Request.QueryString["MethodName"];
            if (string.IsNullOrEmpty(method))
            {
                context.Response.Write(JsonConvert.SerializeObject(_responseFalse));
                return;
            }
            method = method.Trim().ToUpper();

            object status;
            switch (method)
            {
                case "ROLLBACKIMPORT_CHECK":
                    status = CheckRollbackImportStatus() ? _responseTrue : _responseFalse;
                    context.Response.Write(JsonConvert.SerializeObject(status));
                    break;
                case "ROLLBACKUPDATE_CHECK":
                    status = CheckRollbackUpdateStatus() ? _responseTrue : _responseFalse;
                    context.Response.Write(JsonConvert.SerializeObject(status));
                    break;
                default:
                    context.Response.Write(JsonConvert.SerializeObject(_responseFalse));
                    break;
            }

        }

        private bool CheckRollbackImportStatus()
        {
            return _connectorBll.CheckRollbackImportStatus();
        }
        private bool CheckRollbackUpdateStatus()
        {
            return _connectorBll.CheckRollbackUpdateStatus();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}