using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StockMarketSharedLibrary
{
    public static class BootstrapValidationExtensions
    {
        public static MvcHtmlString BootstrapValidationSummary(this HtmlHelper htmlHelper, ActionResponse actionResponse)
        {
            StringBuilder control = new StringBuilder();
            if (actionResponse != null)
            {
                control.Append("<div");

                if (actionResponse.MessageType == ActionResponseMessageType.Error)
                    control.Append(" class='alert alert-danger' role='alert'>\r\n");
                else if (actionResponse.MessageType == ActionResponseMessageType.Info)
                    control.Append(" class='alert alert-info' role='alert'>\r\n");
                else if (actionResponse.MessageType == ActionResponseMessageType.Success)
                    control.Append(" class='alert alert-success' role='alert'>\r\n");
                else if (actionResponse.MessageType == ActionResponseMessageType.Warning)
                    control.Append(" class='alert alert-warning' role='alert'>\r\n");
                else
                    control.Append(">\r\n");

                control.Append("<a href='#' class='close' data-dismiss='alert'>&times;</a>\r\n");
                control.Append("<p>").Append(actionResponse.Message).AppendLine("</p>");
                control.Append("</div>\r\n");
            }

            return MvcHtmlString.Create(control.ToString());
        }
    }
}
