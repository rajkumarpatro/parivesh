﻿using Dapper;
using PDFReader.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace PDFReader.Controllers
{
    public class CentralController : Controller
    {
        // GET: Dashboard
        public async Task<ActionResult> Index()
        {

            var guid = Guid.NewGuid();

            DataTable dt = new DataTable();
            dt.Columns.Add("WebGUID", typeof(Guid));

            dt.Rows.Add(guid);


            DataRow[] drList = dt.Select("WebGUID = '" + guid.ToString() + "'");
            var xx = dt.Select("WebGUID = '" + guid.ToString() + "'");

            CentralModel ob = new CentralModel();

            return View("Central", ob);
        }

        [HttpGet]
        public async Task<ActionResult> LoadCentralData()
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                try
                {
                    string str_query = @"SELECT dbo.parivesh_central.Record_ID, dbo.VIEW_LATEST_CENTRAL.Proposal_No, dbo.parivesh_central.MOEFCC_File_No, dbo.parivesh_central.Project_Name, dbo.parivesh_central.Company, dbo.parivesh_central.Proposal_Status, 
                                        dbo.parivesh_central.Location, dbo.parivesh_central.Important_Dates, dbo.parivesh_central.Category, dbo.parivesh_central.Company_or_Proponent, dbo.parivesh_central.Type_of_project, dbo.parivesh_central.Attached_Files, 
                                        dbo.parivesh_central.Acknowlegment, dbo.parivesh_central.Pagination, dbo.parivesh_central.input_company_name, dbo.parivesh_central.subsidiary_name, dbo.parivesh_central.DateTimeStamp
                                        FROM dbo.parivesh_central INNER JOIN
                                        dbo.VIEW_LATEST_CENTRAL ON dbo.parivesh_central.Record_ID = dbo.VIEW_LATEST_CENTRAL.Record_ID order by dbo.parivesh_central.DateTimeStamp desc";
                    var data = Json(new { data = await db.QueryAsync<CentralModel>(str_query, commandType: CommandType.Text) }, JsonRequestBehavior.AllowGet);
                    return data;
                }
                catch (Exception ee)
                {
                    CentralModel ob = new CentralModel();
                    return View("Central", ob);
                }
            }
        }
    }
}