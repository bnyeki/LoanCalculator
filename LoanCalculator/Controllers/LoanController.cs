using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LoanCalculator.Models;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;

namespace LoanCalculator.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private LoanDBEntities db = new LoanDBEntities();

        /// <summary>
        /// Making data calculations(periodic, and summary calculation) from inputdata
        /// </summary>
        /// <param name="Id">LoanSearchParamater.Id</param>
        /// <returns></returns>
        //

        [HttpGet]
        public ActionResult DataCalculation(int? Id)
        {
            LoanSearchParameter inputData = db.LoanSearchParameter.Find(Id);
            CalculationModel model = new CalculationModel
            {
                Average = MonthlyAvg(inputData.LoanAmmount, inputData.Interest, inputData.Term),

                Sum = Sum(inputData.LoanAmmount, inputData.Interest),

                Calculation = Datas(inputData.Term, inputData.Interest, inputData.LoanAmmount),

                UserName = User.Identity.GetUserName(),
                LoanAmmount = inputData.LoanAmmount,
                Interest = inputData.Interest,
                Id = inputData.Id,
                InterestPeriod = inputData.InterestPeriod,
                Term = inputData.Term,
                CalculationTime = inputData.CalculationTime,
                ExpireTime = inputData.CalculationTime.AddMonths(Convert.ToInt32(inputData.Term))

            };
            return View(model);
        }

        //Get:Inputdata
        [HttpGet]
        public ActionResult InputData()
        {

            return View();
        }

        /// <summary>
        /// POST: Inputdata
        ///  Getting input data from UI and updating database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InputData(InputDataModel model)
        {
            var loggedUser = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                LoanSearchParameter data = new LoanSearchParameter
                {
                    Id = model.Id,
                    LoanAmmount = model.LoanAmmount,
                    Term = model.Term,
                    Interest = model.Interest,
                    InterestPeriod = model.InterestPeriod

                };

                model.CalculationTime = DateTime.Now.Date;
                data.CalculationTime = model.CalculationTime;
                data.UserId = loggedUser;
                db.LoanSearchParameter.Add(data);
                db.SaveChanges();
                return RedirectToAction("DataCalculation", "Loan", new { Id = data.Id });
            }
            return View(model);


        }
        /// <summary>
        /// Calculation/Delete
        /// Displaying the selected calculation parameters
        /// </summary>
        /// <param name="Id">LoanSearchParameter</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanSearchParameter loanSearchParameter = db.LoanSearchParameter.Find(Id);

            DeleteModel model = new DeleteModel
            {
                LoanAmmount = loanSearchParameter.LoanAmmount,
                Interest = loanSearchParameter.Interest,
                Term = loanSearchParameter.Term,
                InterestPeriod = loanSearchParameter.Term,
                UserName = User.Identity.GetUserName(),
                CalculationTime = loanSearchParameter.CalculationTime.Date

            };
            if (loanSearchParameter == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        /// <summary>
        ///POST: Calculation/Delete/
        ///Deleting selected calculation, updating database
        /// </summary>
        /// <param name="Id">LoanSearchParameter.Id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            LoanSearchParameter loanSearchParameter = db.LoanSearchParameter.Find(Id);
            db.LoanSearchParameter.Remove(loanSearchParameter);
            db.SaveChanges();
            return RedirectToAction("CalculationSearch");
        }

        /// <summary>
        /// Get Calculation Search
        /// Displaying previous calculations of the user.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult CalculationSearch()
        {

            var user = User.Identity.GetUserId();

            var query = db.LoanSearchParameter.Where(log => log.UserId == user);

            List<CalculationSearchModel> model = query.AsEnumerable().Select(lsp => new CalculationSearchModel()
            {

                LoanAmmount = lsp.LoanAmmount,
                Id = lsp.Id,
                CalculationTime = lsp.CalculationTime,
                Interest = lsp.Interest,
                Term = lsp.Term,
                Average = MonthlyAvg(lsp.LoanAmmount, lsp.Interest, lsp.Term),
                Sum = Sum(lsp.LoanAmmount, lsp.Interest)


            }).ToList();


            return View(model);

        }

        /// <summary>
        ///  Post CalculationSearch
        ///  Displaying previous calculations of the logged user, using filters.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalculationSearch(CalculationSearchModel model)
        {
            //getting the Id of the logged user.
            var loggedUser = User.Identity.GetUserId();

            // creating default model
            List<CalculationSearchModel> result = new List<CalculationSearchModel>();

            if (ModelState.IsValid)
            {
                // Selecting items where the UserId equals to current logged user Id from datatbase.
                var query = db.LoanSearchParameter.Where(log => log.UserId == loggedUser);

                //Adding the items to query where the first calculation time is not null.
                if (model.Filter.CalculationTimeFrom != null)
                    query = query.Where(log => log.CalculationTime >= model.Filter.CalculationTimeFrom);

                //Adding the items to query where the last calculation time is not null.
                if (model.Filter.CalculationTimeTo != null)
                    query = query.Where(log => log.CalculationTime <= model.Filter.CalculationTimeTo);

                //Adding the items to query where the minimum LoanAmmount is not null.
                if (model.Filter.LoanAmmountFrom != null)
                    query = query.Where(log => log.LoanAmmount >= model.Filter.LoanAmmountFrom);

                //Adding the items to query where the maximum LoanAmmount is not null.
                if (model.Filter.LoanAmmountTo != null)
                    query = query.Where(log => log.LoanAmmount <= model.Filter.LoanAmmountTo);

                //Creating list for displaying datas
                result = query.AsEnumerable().Select(lsp => new CalculationSearchModel()
                {
                    LoanAmmount = lsp.LoanAmmount,
                    Id = lsp.Id,
                    CalculationTime = lsp.CalculationTime,
                    Interest = lsp.Interest,
                    Term = lsp.Term,
                    Average = MonthlyAvg(lsp.LoanAmmount, lsp.Interest, lsp.Term),
                    Sum = Sum(lsp.LoanAmmount, lsp.Interest)

                }).ToList();
                return View(result);
            }

            return View(result);
        }


        //Method for getting the monthly average installment
        private double MonthlyAvg(double LoanAmmount, double interest, double term)
        {
            double r = (LoanAmmount * (1 + (interest / 100))) / term;
            double result = Math.Round(r, 2);
            return result;
        }

        //Method  for getting the sum of repayable amount
        private double Sum(double LoanAmmount, double interest)
        {
            double r = (LoanAmmount * (1 + (interest / 100)));
            double result = Math.Round(r, 2);
            return result;
        }

        //Method for getting the datas per period
        private List<PeriodicCalculation> Datas(double inputTerm, double inputInterest, double loanAmmount)
        {

            double monthlyInterest = (loanAmmount * (inputInterest / 100)) / inputTerm;
            double monthlyPrinciple = (loanAmmount + (loanAmmount * (inputInterest / 100))) / inputTerm;
            double paid = 0;
            double remainingPrinciple = loanAmmount + (loanAmmount * (inputInterest / 100));
            List<PeriodicCalculation> PeriodicCalculation = new List<PeriodicCalculation>();


            for (int i = 1; i <= inputTerm; i++)
            {

                PeriodicCalculation calculation = new PeriodicCalculation();
                calculation.InterestPaid = monthlyInterest;

                calculation.Payment = monthlyPrinciple;

                paid += monthlyPrinciple;

                calculation.PrinciplePaid = paid;

                remainingPrinciple = remainingPrinciple - monthlyPrinciple;

                calculation.RemainingPrincipal = remainingPrinciple;

                calculation.Period = i;

                PeriodicCalculation.Add(calculation);

            }

            return PeriodicCalculation;

        }

        /// <summary>
        /// Method for export datas per period to EXCEL (file output).
        /// </summary>
        /// <param name="Id">LoanSearchParameter.Id</param>
        public void ExportToExcel(int? Id)
        {
            LoanSearchParameter inputData = db.LoanSearchParameter.Find(Id);
            CalculationModel model = new CalculationModel
            {

                Calculation = Datas(inputData.Term, inputData.Interest, inputData.LoanAmmount),

                LoanAmmount = inputData.LoanAmmount,
                Interest = inputData.Interest,
                Id = inputData.Id,
                InterestPeriod = inputData.InterestPeriod,
                Term = inputData.Term,
                UserId = inputData.UserId,
                CalculationTime = inputData.CalculationTime
            };


            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet ws = package.Workbook.Worksheets.Add("Export");
            ws.Cells.Style.Font.Size = 20;
            ws.Cells.Style.Font.Name = "Arial";
            ws.Cells.Style.Numberformat.Format = "#,##0.00";
            ws.Cells.AutoFitColumns();

           
            string period = "Hónap";
            string monthlyInterest = "Havonta fizetendő kamat";
            string monthlyPrinciple = "Havonta fizetendő részlet";
            string paid = "Kiegyenlített tartozás";
            string remainingPrinciple = "Hátralék";


            DataTable dt = new DataTable(); // Read records from database 
            DataColumn[] cols = { new DataColumn(period),
             new DataColumn(monthlyInterest),
             new DataColumn(monthlyPrinciple),
             new DataColumn(paid),
             new DataColumn(remainingPrinciple)
            };

            dt.Columns.AddRange(cols);

            for (int i = 0; i < model.Term; i++)
            {
                DataRow row = dt.NewRow();
                row[period] = model.Calculation.ElementAt(i).Period;//Period of term
                row[monthlyInterest] = model.Calculation.ElementAt(i).Payment;//Monthly paid interest
                row[monthlyPrinciple] = model.Calculation.ElementAt(i).InterestPaid;//Monthly paid ammount
                row[paid] = model.Calculation.ElementAt(i).PrinciplePaid;// Already paid ammount
                row[remainingPrinciple] = model.Calculation.ElementAt(i).RemainingPrincipal; // Remaining installment
                dt.Rows.Add(row);
            }
            ws.Cells[1, 1].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
            HttpContext.Response.Clear();
            HttpContext.Response.AddHeader("", "");
            HttpContext.Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
            HttpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=Export.xlsx");
            HttpContext.Response.ContentType = "application/text";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            HttpContext.Response.BinaryWrite(package.GetAsByteArray());
            HttpContext.Response.End();

        }



    }
}





