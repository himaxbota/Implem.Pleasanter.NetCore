﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelContext
    {
        public StringBuilder LogBuilder;
        public ExpandoObject UserData;
        public ErrorData ErrorData;
        public List<Message> Messages;
        public readonly ServerScriptModelContextServerScript ServerScript;
        public readonly Forms Forms;
        public readonly string FormStringRaw;
        public readonly string FormString;
        public readonly bool Ajax;
        public readonly bool Mobile;
        public readonly string ApplicationPath;
        public readonly string AbsoluteUri;
        public readonly string AbsolutePath;
        public readonly string Url;
        public readonly string UrlReferrer;
        public readonly string Controller;
        public readonly string Query;
        public readonly string Action;
        public readonly int TenantId;
        public readonly long SiteId;
        public readonly long Id;
        public readonly IList<int> Groups;
        public readonly string TenantTitle;
        public readonly string SiteTitle;
        public readonly string RecordTitle;
        public readonly int DeptId;
        public readonly int UserId;
        public readonly string LoginId;
        public readonly string Language;
        public readonly string TimeZoneInfo;
        public readonly bool HasPrivilege;
        public readonly decimal ApiVersion;
        public readonly string ApiRequestBody;
        public readonly string RequestDataString;
        public readonly string ContentType;
        public readonly string ControlId;

        public ServerScriptModelContext(
            Context context,
            StringBuilder logBuilder,
            ExpandoObject userData,
            List<Message> messages,
            ErrorData errorData,
            string formStringRaw,
            string formString,
            bool ajax,
            bool mobile,
            string applicationPath,
            string absoluteUri,
            string absolutePath,
            string url,
            string urlReferrer,
            string controller,
            string query,
            string action,
            int tenantId,
            long siteId,
            long id,
            IEnumerable<int> groupIds,
            string tenantTitle,
            string siteTitle,
            string recordTitle,
            int deptId,
            int userId,
            string loginId,
            string language,
            string timeZoneInfo,
            bool hasPrivilege,
            decimal apiVersion,
            string apiRequestBody,
            string requestDataString,
            string contentType,
            bool onTesting,
            long scriptDepth,
            string controlId)
        {
            LogBuilder = logBuilder;
            UserData = userData;
            Messages = messages;
            ErrorData = errorData;
            ServerScript = new ServerScriptModelContextServerScript(
                onTesting: onTesting,
                scriptDepth: scriptDepth);
            Forms = context.Forms;
            FormStringRaw = formStringRaw;
            FormString = formString;
            Ajax = ajax;
            Mobile = mobile;
            ApplicationPath = applicationPath;
            AbsoluteUri = absoluteUri;
            AbsolutePath = absolutePath;
            Url = url;
            UrlReferrer = urlReferrer;
            Controller = controller;
            Query = query;
            Action = action;
            TenantId = tenantId;
            SiteId = siteId;
            Id = id;
            Groups = groupIds?.ToArray() ?? new int[0];
            TenantTitle = tenantTitle;
            SiteTitle = siteTitle;
            RecordTitle = recordTitle;
            DeptId = deptId;
            UserId = userId;
            LoginId = loginId;
            Language = language;
            TimeZoneInfo = timeZoneInfo;
            HasPrivilege = hasPrivilege;
            ApiVersion = apiVersion;
            ApiRequestBody = apiRequestBody;
            RequestDataString = requestDataString;
            ContentType = contentType;
            ControlId = controlId;
        }

        public void Log(object log)
        {
            LogBuilder.AppendLine(log?.ToString() ?? string.Empty);
        }

        public void Error(string message)
        {
            ErrorData.Type = General.Error.Types.CustomError;
            ErrorData.Data = message.ToSingleArray();
        }

        public void AddMessage(string text, string css = "alert-information")
        {
            Messages.Add(new Message()
            {
                Text = text,
                Css = css
            });
        }
    }
}