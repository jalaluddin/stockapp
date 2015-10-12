using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace StockMarketSharedLibrary
{
    public class DataTablesAjaxRequestModel
    {
        public int iDisplayStart { get; set;}
        public int iDisplayLength { get; set;}
        public int iSortingCols { get; set; }
        public static object EmptyResult
        {
            get
            {
                return new
                {
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = (new string[] { }).ToArray()
                };
            }
        }

        public string GetSortElements(string[] columnNames)
        {
            for (int i = 0; i < iSortingCols; i++)
            {
                int colIndex = 0;
                int.TryParse(HttpContext.Current.Request["iSortCol_" + i], out colIndex);
                if (HttpContext.Current.Request["bSortable_" + colIndex] == "true")
                {
                    return string.Format("{0} {1}", columnNames[colIndex], HttpContext.Current.Request["sSortDir_" + i]);
                }
            }
            return "ID asc";
        }

        public string GetSearchText()
        {
            string searchText = HttpContext.Current.Request["sSearch"];
            return searchText;
        }

        public int GetPageIndex()
        {
            if (iDisplayLength > 0)
                return (iDisplayStart / iDisplayLength) + 1;
            else
                return 1;
        }

        public int GetPageSize()
        {
            if (iDisplayLength == 0)
                return 10;
            else
                return iDisplayLength;
        }
    }
}