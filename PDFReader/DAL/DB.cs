using Dapper;
using PDFReader.Model;
using PDFReader.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PDFReader
{
    public class DB
    {
        #region PDF reader

        public static ReportResult GetPrcoessedReport(string FinancialYear)
        {
            ReportResult reportResult = new ReportResult();
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                reportResult.Processed = db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where ProcessedDate is not null and FinancialYear = '{FinancialYear}'", commandType: CommandType.Text).Result.ToList();
                reportResult.UnProcessed = db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where ProcessedDate is null and FinancialYear = '{FinancialYear}'", commandType: CommandType.Text).Result.ToList();
            }

            return reportResult;
        }

        public static int totalAnnouncmentCount(string dtFrom, string dtTo)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                return db.ExecuteScalar<int>($"SELECT COUNT(ANN_ID) FROM TBL_ANNOUNCEMENT WHERE CONVERT(VARCHAR(10),NEWS_SUBMISSION_DATE,105) IN ('{dtFrom}','{dtTo}')", commandType: CommandType.Text);
            }
        }
        public static DateTime GetLastAnnDateTime(string dtFrom, string dtTo)
        {

            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                return db.ExecuteScalar<DateTime>($"select top(1) DT_TM from TBL_ANNOUNCEMENT WHERE CONVERT(VARCHAR(10),NEWS_SUBMISSION_DATE, 105) IN ('{dtFrom}','{dtTo}') order by NEWS_SUBMISSION_DATE desc", commandType: CommandType.Text);
            }

        }

        public static async Task<IEnumerable<string>> GetFinancialYears()
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                return await db.QueryAsync<string>($"select distinct(FinancialYear) from tbl_AnnualReports", commandType: CommandType.Text);
            }
        }

        public static IEnumerable<KeywordResult> GetSearchedReport(string FinancialYear)
        {
            //type 1 - processed, 2 - unprocessed, 3 - Contains Img, 4 - Non Img
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                return db.QueryAsync<KeywordResult>(
                    $"sELECT PageText, ar.ID ReportId, ar.CompanyName, ar.Url " +
                    ", ar.FinancialYear, fk.PDFPageNumber, fk.FoundKeywords, TotalPages, Skipped, ann.news_subject, FORMAT (ann.NEWS_SUBMISSION_DATE , 'dd/MM/yyyy HH::mm:ss', 'en-us') NEWS_SUBMISSION_DATE, ann.Company_Id " +
                    "FROM dbo.tbl_AnnualReports ar " +
                    $"INNER JOIN dbo.Tbl_FoundKeywords fk ON ar.ID = fk.ReportID  and ar.FinancialYear = '{FinancialYear}' " +
                    "left join TBL_ANNOUNCEMENT ann on ann.ann_id = ar.annid " +
                    "and processeddate is not null and isnull(isdeleted, 0) = 0 order by CompanyName",
                   commandType: CommandType.Text).Result;


                //For LocalHost Run
                //return db.QueryAsync<KeywordResult>(
                //    $"sELECT PageText, ar.ID ReportId, ar.CompanyName, ar.Url " +
                //    ", ar.FinancialYear, fk.PDFPageNumber, fk.FoundKeywords, TotalPages, Skipped, ann.news_subject, ann.NEWS_SUBMISSION_DATE, ann.Company_Id " +
                //    "FROM dbo.tbl_AnnualReports ar " +
                //    $"INNER JOIN dbo.Tbl_FoundKeywords fk ON ar.ID = fk.ReportID  and ar.FinancialYear = '{FinancialYear}' " +
                //    $"and convert(date, processedDate, 103) between  convert(date, '{dtFrm}', 103)  and convert(date, '{dtTo}', 103) " +
                //    "left join TBL_ANNOUNCEMENT ann on ann.ann_id = ar.annid " +
                //    "and processeddate is not null and isnull(isdeleted, 0) = 0 order by CompanyName",
                //   commandType: CommandType.Text).Result;
            }
        }

        public static IEnumerable<KeywordResult> GetSearchedReport(string FinancialYear, DateTime dtFrm, DateTime dtTo)
        {
            string str_alert_condition = "";
            if (FinancialYear != "All Alert")
            {
                str_alert_condition = " and ar.FinancialYear = '" + FinancialYear + "'";
            }

            //type 1 - processed, 2 - unprocessed, 3 - Contains Img, 4 - Non Img
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                //return db.QueryAsync<KeywordResult>(
                //    $"sELECT PageText, ar.ID ReportId, ar.CompanyName, ar.Url " +
                //    ", ar.FinancialYear, fk.PDFPageNumber, fk.FoundKeywords, TotalPages, Skipped, ann.news_subject, ann.NEWS_SUBMISSION_DATE, ann.Company_Id " +
                //    "FROM dbo.tbl_AnnualReports ar " +
                //    $"INNER JOIN dbo.Tbl_FoundKeywords fk ON ar.ID = fk.ReportID  and ar.FinancialYear = '{FinancialYear}' " +
                //    $"and convert(date, processedDate) between  convert(date, '{dtFrm}')  and convert(date, '{dtTo}') " +
                //    "left join TBL_ANNOUNCEMENT ann on ann.ann_id = ar.annid " +
                //    "and processeddate is not null and isnull(isdeleted, 0) = 0 order by CompanyName",
                //   commandType: CommandType.Text).Result;

                //For LocalHost Run

                string frm = dtFrm.ToString("yyyy/MM/dd");
                string to = dtTo.ToString("yyyy/MM/dd");

                return db.QueryAsync<KeywordResult>(
                    $"sELECT PageText, ar.ID ReportId, ar.CompanyName, ar.Url " +
                    ", ar.FinancialYear, fk.PDFPageNumber, fk.FoundKeywords, TotalPages, Skipped, ann.news_subject, ann.NEWS_SUBMISSION_DATE, ann.Company_Id " +
                    "FROM dbo.tbl_AnnualReports ar " +
                    $"INNER JOIN dbo.Tbl_FoundKeywords fk ON ar.ID = fk.ReportID  " + str_alert_condition +
                    $"and convert(date, insertdate) between '{frm}'  and '{to}' " +
                    "left join TBL_ANNOUNCEMENT ann on ann.ann_id = ar.annid " +
                    " and isnull(isdeleted, 0) = 0 order by CompanyName",
                   commandType: CommandType.Text).Result;
            }
        }

        public static IEnumerable<KeywordResult> GetSearchedReportNyAnnId(string FinancialYear, string annIds)
        {
            //type 1 - processed, 2 - unprocessed, 3 - Contains Img, 4 - Non Img
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                return db.QueryAsync<KeywordResult>(
                     $"sELECT PageText, ar.ID ReportId, ar.CompanyName, ar.Url " +
                     ", ar.FinancialYear, fk.PDFPageNumber, fk.FoundKeywords, TotalPages, Skipped, ann.news_subject, ann.NEWS_SUBMISSION_DATE, ann.Company_Id " +
                     "FROM dbo.tbl_AnnualReports ar " +
                     $"inner join Split_Strings('{annIds}',',') t on t.item = ar.annid " +
                     $"INNER JOIN dbo.Tbl_FoundKeywords fk ON ar.ID = fk.ReportID  and ar.FinancialYear = '{FinancialYear}' " +
                     "left join TBL_ANNOUNCEMENT ann on ann.ann_id = ar.annid " +
                     "order by CompanyName",
                    commandType: CommandType.Text).Result;
            }
        }
        public static IEnumerable<ReportModel> GetReports(string year, DateTime date)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                db.Open();

                return db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where FinancialYear = '{year}' and convert(date, '{date.ToString("yyyy/MM/dd")}') = insertdate", commandType: CommandType.Text).Result.ToList();
                //return db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where FinancialYear = '{year}'", commandType: CommandType.Text).Result.ToList();
            }
        }
        public static IEnumerable<ReportModel> GetReportsByAnnId(string year, string annIds)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                db.Open();

                return db.QueryAsync<ReportModel>
                ($"select * from tbl_AnnualReports ar left join tbl_foundkeywords fk on ar.ID=fk.ReportID inner join Split_Strings('{annIds}', ',') t on t.item = ar.annid and FinancialYear = '{year}' where fk.ReportID is null and processeddate is null ", commandType: CommandType.Text).Result.ToList();
                //return db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where FinancialYear = '{year}'", commandType: CommandType.Text).Result.ToList();
            }
        }

        public static List<ReportModel> insertCompaniesData(string alertName)
        {
            using (var connection = new SqlConnection(Connection.MyConnection()))
            {
                connection.Open();

                return connection.Query<ReportModel>("sp_InsertCompanies",
                    new
                    {
                        @alertname = alertName
                    }, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public static async Task InsertFoundKeywords(List<FetchedKeywords> list, int TotalPages, bool IsPDF_Skipped, int ReportId)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                db.Open();

                var dt = new DataTable();
                dt.Columns.Add("ReportID");
                dt.Columns.Add("PDFPageNumber");
                dt.Columns.Add("foundKeywords");
                dt.Columns.Add("pagetext");

                foreach (var item in list)
                {
                    dt.Rows.Add(item.ReportID, item.PDFPageNumber, item.FoundKeywords, item.PageText);
                }

                await db.QueryAsync("sp_InsertFOundKeywords",
                    new
                    {
                        @keywords = dt.AsTableValuedParameter("SearchedKeywordsType"),
                        @TotalPages = TotalPages,
                        @IsSkipped = IsPDF_Skipped,
                        @ReportId = ReportId
                    }, commandType: CommandType.StoredProcedure);
            }
        }

        public async static Task UpdateReportAsProcessed(int reportId)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                db.Open();
                var xx = await db.ExecuteScalarAsync("update [tbl_AnnualReports] set ProcessedDate = getdate() Where id = @reportid",
                    new { @reportid = reportId }, commandType: CommandType.Text);
            }
        }

        public static async Task DeleteReport(string FinancialYear)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                await db.ExecuteAsync(
                   $"UPDATE tbl_AnnualReports set ProcessedDate = null, totalpages = 0, skipped = 0 where FinancialYear='{FinancialYear}'",
                    commandType: CommandType.Text);

                await db.ExecuteAsync(
                  $"update fk set Isdeleted = 1 from Tbl_FoundKeywords fk inner join tbl_AnnualReports ar on fk.ReportID = ar.ID and ar.FinancialYear = '{FinancialYear}'",
                   commandType: CommandType.Text);
            }
        }

        public static SkippedCount GetSkippedCount(string year)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                db.Open();

                var x1 = db.ExecuteScalarAsync<int>($"select count(*) from tbl_AnnualReports where Skipped = 1 and TotalPages>0 and FinancialYear= '{year}'", commandType: CommandType.Text).Result;
                var x2 = db.ExecuteScalarAsync<int>($"select count(*) from tbl_AnnualReports where Skipped = 1 and TotalPages=0 and FinancialYear= '{year}'", commandType: CommandType.Text).Result;
                return new SkippedCount { ImgSkipped = x2, NonImgSkipped = x1 };
                //return db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where FinancialYear = '{year}'", commandType: CommandType.Text).Result.ToList();
            }
        }

        public static IEnumerable<ReportModel> GetUnproccesedRpt(string year, int type)
        {
            using (IDbConnection db = new SqlConnection(Connection.MyConnection()))
            {
                db.Open();

                if (type == 4)
                    return db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where Skipped = 1 and TotalPages>0 and FinancialYear= '{year}'", commandType: CommandType.Text).Result;

                if (type == 3)
                    return db.QueryAsync<ReportModel>($"select * from tbl_AnnualReports where Skipped = 1 and TotalPages=0 and FinancialYear= '{year}'", commandType: CommandType.Text).Result;

                if (type == 2)
                    return db.QueryAsync<ReportModel>($"Select * from tbl_AnnualReports where FinancialYear = '{year}' and isnull(processeddate,'')=''", commandType: CommandType.Text).Result;
            }

            return new List<ReportModel>();
        }

        public static void DeleteCompany(string Yr)
        {
            using (var connection = new SqlConnection(Connection.MyConnection()))
            {
                connection.Open();
                connection.ExecuteScalar($"delete fk from Tbl_FoundKeywords fk inner join tbl_AnnualReports ar on fk.reportid = ar.id and ar.FinancialYear = '{Yr}'", commandType: CommandType.Text);
                connection.ExecuteScalar($"delete from tbl_AnnualReports where FinancialYear = '{Yr}'", commandType: CommandType.Text);
            }
        }

        #endregion PDF reader

    }
}