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

        //Get:Inputdata
        [HttpGet]
        public ActionResult InputData()
        {
            ViewBag.InterestPeriod = new SelectList(db.Period, "Id", "Name");
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

            if (model.InterestPeriod == 1)
            {
                model.FirstTerm = model.Term;
                model.SecondTerm = 0;
                model.ThirdTerm = 0;
                ModelState.Remove("FirstTerm");
            }

            if (model.InterestPeriod == 2)
            {
                model.ThirdTerm = 0;

            }

            if (model.SecondTerm + model.FirstTerm + model.ThirdTerm != model.Term)
            {
                ModelState.AddModelError(nameof(model.Term), "A kamatperiódusoknál megadott futamidő hossza nem egyezik meg a futadmidővel ");
            }

            if (ModelState.IsValid)
            {
                LoanSearchParameter data = new LoanSearchParameter
                {
                    Id = model.Id,
                    LoanAmmount = model.LoanAmmount,
                    Term = model.Term,
                    InterestPeriodId = model.InterestPeriod,
                    UserId = loggedUser,
                    InterestFirstPeriod = model.InterestFirstPeriod,
                    InterestSecondPeriod = model.InterestSecondPeriod,
                    InterestThirdPeriod = model.InterestThirdPeriod,
                    TermFirstPeriod = model.FirstTerm,
                    TermSecondPeriod = model.SecondTerm,
                    TermThirdPeriod = model.ThirdTerm,

                };

                model.CalculationTime = DateTime.Now.Date;
                data.CalculationTime = model.CalculationTime;
                db.LoanSearchParameter.Add(data);
                db.SaveChanges();
                return RedirectToAction("DataCalculation", "Loan", new { Id = data.Id });
            }
            return View(model);

        }
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

            if (inputData.InterestPeriodId == 3)
            {

                double firstPeriodSum = Sum(inputData.LoanAmmount, inputData.InterestFirstPeriod, inputData.TermFirstPeriod, inputData.Term);
                double secondPeriodSum = Sum(inputData.LoanAmmount, (double)inputData.InterestSecondPeriod, (int)inputData.TermSecondPeriod, inputData.Term);
                double thirdPeriodSum = Sum(inputData.LoanAmmount, (double)inputData.InterestThirdPeriod, (int)inputData.TermThirdPeriod, inputData.Term);
                double allPeriodSum = firstPeriodSum + secondPeriodSum + thirdPeriodSum;



                CalculationModel model = new CalculationModel
                {
                    Average = MonthlyAvg(allPeriodSum, inputData.Term),

                    Sum = allPeriodSum,

                    CalculationFirst = FirstPeriodCalculation(inputData.TermFirstPeriod, inputData.InterestFirstPeriod, inputData.LoanAmmount, inputData.Term, allPeriodSum),
                    CalculationSecond = SecondPeriodCalculation(inputData.TermSecondPeriod, inputData.TermFirstPeriod, inputData.InterestSecondPeriod, inputData.LoanAmmount, inputData.Term, allPeriodSum, firstPeriodSum),
                    CalculationThird = ThirdPeriodCalculation(inputData.TermThirdPeriod, inputData.TermSecondPeriod, inputData.InterestThirdPeriod, inputData.LoanAmmount, inputData.Term, allPeriodSum, firstPeriodSum, secondPeriodSum),
                    UserName = User.Identity.GetUserName(),
                    LoanAmmount = inputData.LoanAmmount,
                    AverageInterest = (inputData.InterestFirstPeriod + (double)inputData.InterestSecondPeriod + (double)inputData.InterestThirdPeriod) / inputData.InterestPeriodId,
                    Id = inputData.Id,
                    InterestPeriod = inputData.InterestPeriodId,
                    Term = inputData.Term,
                    CalculationTime = inputData.CalculationTime,
                    ExpireTime = inputData.CalculationTime.AddMonths(Convert.ToInt32(inputData.Term))

                };


                return View(model);
            }
            else if (inputData.InterestPeriodId == 2)
            {

                double firstPeriodSum = Sum(inputData.LoanAmmount, inputData.InterestFirstPeriod, inputData.TermFirstPeriod, inputData.Term);
                double secondPeriodSum = Sum(inputData.LoanAmmount, (double)inputData.InterestSecondPeriod, (int)inputData.TermSecondPeriod, inputData.Term);
                double allPeriodSum = firstPeriodSum + secondPeriodSum;

                CalculationModel model = new CalculationModel
                {
                    Average = MonthlyAvg(allPeriodSum, inputData.Term),

                    Sum = allPeriodSum,

                    CalculationFirst = FirstPeriodCalculation(inputData.TermFirstPeriod, inputData.InterestFirstPeriod, inputData.LoanAmmount, inputData.Term, allPeriodSum),
                    CalculationSecond = SecondPeriodCalculation(inputData.TermSecondPeriod, inputData.TermFirstPeriod, inputData.InterestSecondPeriod, inputData.LoanAmmount, inputData.Term, allPeriodSum, firstPeriodSum),
                    UserName = User.Identity.GetUserName(),
                    LoanAmmount = inputData.LoanAmmount,
                    AverageInterest = (inputData.InterestFirstPeriod + (double)inputData.InterestSecondPeriod) / (double)inputData.InterestPeriodId,
                    Id = inputData.Id,
                    InterestPeriod = inputData.InterestPeriodId,
                    Term = inputData.Term,
                    CalculationTime = inputData.CalculationTime,
                    ExpireTime = inputData.CalculationTime.AddMonths(Convert.ToInt32(inputData.Term))

                };


                return View(model);

            }

            else if (inputData.InterestPeriodId == 1)
            {

                double firstPeriodSum = Sum(inputData.LoanAmmount, inputData.InterestFirstPeriod, inputData.TermFirstPeriod, inputData.Term);


                CalculationModel model = new CalculationModel
                {
                    Average = MonthlyAvg((double)firstPeriodSum, inputData.Term),

                    Sum = firstPeriodSum,

                    CalculationFirst = FirstPeriodCalculation(inputData.TermFirstPeriod, inputData.InterestFirstPeriod, inputData.LoanAmmount, inputData.Term, (double)firstPeriodSum),
                    UserName = User.Identity.GetUserName(),
                    LoanAmmount = inputData.LoanAmmount,
                    AverageInterest = inputData.InterestFirstPeriod,
                    Id = inputData.Id,
                    InterestPeriod = inputData.InterestPeriodId,
                    Term = inputData.Term,
                    CalculationTime = inputData.CalculationTime,
                    ExpireTime = inputData.CalculationTime.AddMonths(Convert.ToInt32(inputData.Term))

                };


                return View(model);

            }
            else
            {
                return View();
            }

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
                Term = loanSearchParameter.Term,
                AverageInterest = (loanSearchParameter.InterestFirstPeriod + loanSearchParameter.InterestSecondPeriod + loanSearchParameter.InterestThirdPeriod) / loanSearchParameter.InterestPeriodId,
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


        public ActionResult CalculationSearch()
        {

            var user = User.Identity.GetUserId();

            var query = db.LoanSearchParameter.Where(log => log.UserId == user);

            //Setting the unused periodinterests to Zero
            SetPeriodInterestToZero(query);

            List<CalculationSearchModel> model = query.AsEnumerable().Select(lsp => new CalculationSearchModel()
            {

                LoanAmmount = lsp.LoanAmmount,
                Id = lsp.Id,
                CalculationTime = lsp.CalculationTime,
                InterestFirstPeriod = lsp.InterestFirstPeriod,
                InterestSecondPeriod = lsp.InterestSecondPeriod,
                InterestThirdPeriod = lsp.InterestThirdPeriod,
                Term = lsp.Term,
                AverageInterest = (lsp.InterestFirstPeriod + lsp.InterestSecondPeriod + lsp.InterestThirdPeriod) / lsp.InterestPeriodId,

            }).ToList();

            return View(model);

        }

        /// <summary>
        ///  Post CalculationSearch
        ///  Displaying previous calculations of the logged user, using filters.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        // [ValidateAntiForgeryToken]
        public PartialViewResult SearchItem(CalculationSearchModel model)
        {
            //getting the Id of the logged user.
            var loggedUser = User.Identity.GetUserId();

            // creating default model
            List<CalculationSearchModel> result = new List<CalculationSearchModel>();

            if (ModelState.IsValid)
            {
                // Selecting items where the UserId equals to current logged user Id from datatbase.
                var query = db.LoanSearchParameter.Where(log => log.UserId == loggedUser);

                //Setting the unused periodinterests to Zero
                SetPeriodInterestToZero(query);

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
                    InterestFirstPeriod = lsp.InterestFirstPeriod,
                    InterestSecondPeriod = lsp.InterestSecondPeriod,
                    InterestThirdPeriod = lsp.InterestThirdPeriod,
                    AverageInterest = (lsp.InterestFirstPeriod + lsp.InterestSecondPeriod + lsp.InterestThirdPeriod) / lsp.InterestPeriodId,
                    Term = lsp.Term,


                }).ToList();
                return PartialView(result);
            }

            return PartialView(result);
        }

        /// <summary>
        /// Method for export datas per period to EXCEL (file output).
        /// </summary>
        /// <param name="Id">LoanSearchParameter.Id</param>
        public void ExportToExcel(int? Id)
        {
            LoanSearchParameter inputData = db.LoanSearchParameter.Find(Id);

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
            DataColumn[] cols = {
                 new DataColumn(period),
                 new DataColumn(monthlyInterest),
                 new DataColumn(monthlyPrinciple),
                 new DataColumn(paid),
                 new DataColumn(remainingPrinciple)
                };

            dt.Columns.AddRange(cols);

            if (inputData.InterestPeriodId == 3)
            {

                double firstPeriodSum = Sum(inputData.LoanAmmount, inputData.InterestFirstPeriod, inputData.TermFirstPeriod, inputData.Term);
                double secondPeriodSum = Sum(inputData.LoanAmmount, (double)inputData.InterestSecondPeriod, (int)inputData.TermSecondPeriod, inputData.Term);
                double thirdPeriodSum = Sum(inputData.LoanAmmount, (double)inputData.InterestThirdPeriod, (int)inputData.TermThirdPeriod, inputData.Term);
                double allPeriodSum = firstPeriodSum + secondPeriodSum + thirdPeriodSum;


                ExcelDataModel model = new ExcelDataModel
                {
                    CalculationFirst = FirstPeriodCalculation(inputData.TermFirstPeriod, inputData.InterestFirstPeriod, inputData.LoanAmmount, inputData.Term, (double)allPeriodSum),
                    CalculationSecond = SecondPeriodCalculation(inputData.TermSecondPeriod, inputData.TermFirstPeriod, inputData.InterestSecondPeriod, inputData.LoanAmmount, inputData.Term, (double)allPeriodSum, (double)firstPeriodSum),
                    CalculationThird = ThirdPeriodCalculation(inputData.TermThirdPeriod, inputData.TermSecondPeriod, inputData.InterestThirdPeriod, inputData.LoanAmmount, inputData.Term, (double)allPeriodSum, (double)firstPeriodSum, (double)secondPeriodSum),
                };

                FirstPeriodRows(period, monthlyInterest, monthlyPrinciple, paid, remainingPrinciple, dt, model, inputData);
                SecondPeriodRows(period, monthlyInterest, monthlyPrinciple, paid, remainingPrinciple, dt, model, inputData);
                ThirdPeriodRows(period, monthlyInterest, monthlyPrinciple, paid, remainingPrinciple, dt, model, inputData);
                Construct(package, ws, dt);

            }

            else if (inputData.InterestPeriodId == 2)
            {

                double firstPeriodSum = Sum(inputData.LoanAmmount, inputData.InterestFirstPeriod, inputData.TermFirstPeriod, inputData.Term);
                double secondPeriodSum = Sum(inputData.LoanAmmount, (double)inputData.InterestSecondPeriod, (int)inputData.TermSecondPeriod, inputData.Term);
                double allPeriodSum = firstPeriodSum + secondPeriodSum;

                ExcelDataModel model = new ExcelDataModel
                {
                    CalculationFirst = FirstPeriodCalculation(inputData.TermFirstPeriod, inputData.InterestFirstPeriod, inputData.LoanAmmount, inputData.Term, (double)allPeriodSum),
                    CalculationSecond = SecondPeriodCalculation(inputData.TermSecondPeriod, inputData.TermFirstPeriod, inputData.InterestSecondPeriod, inputData.LoanAmmount, inputData.Term, (double)allPeriodSum, (double)firstPeriodSum)
                };

                FirstPeriodRows(period, monthlyInterest, monthlyPrinciple, paid, remainingPrinciple, dt, model, inputData);
                SecondPeriodRows(period, monthlyInterest, monthlyPrinciple, paid, remainingPrinciple, dt, model, inputData);
                Construct(package, ws, dt);

            }

            else if (inputData.InterestPeriodId == 1)
            {

                double firstPeriodSum = Sum(inputData.LoanAmmount, inputData.InterestFirstPeriod, inputData.TermFirstPeriod, inputData.Term);

                ExcelDataModel model = new ExcelDataModel
                {
                    CalculationFirst = FirstPeriodCalculation(inputData.TermFirstPeriod, inputData.InterestFirstPeriod, inputData.LoanAmmount, inputData.Term, (double)firstPeriodSum)
                };

                FirstPeriodRows(period, monthlyInterest, monthlyPrinciple, paid, remainingPrinciple, dt, model, inputData);
                Construct(package, ws, dt);

            }

        }
        //Method for getting the monthly average installment
        private double MonthlyAvg(double sum, double term)
        {
            double r = sum / term;
            double result = Math.Round(r, 2);
            return result;
        }

        //Method  for getting the sum of repayable amount in each period

        private double Sum(double LoanAmmount, double interest, int actualTerm, int term)

        {
            double r = (LoanAmmount * (1 + (interest / 100))) / ((double)term / (double)actualTerm);
            double result = Math.Round((double)r, 2);
            return result;
        }

        //method for construct the excel file

        private void Construct(ExcelPackage package, ExcelWorksheet ws, DataTable dt)
        {
            ws.Cells[1, 1].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
            HttpContext.Response.Clear();
            HttpContext.Response.AddHeader("", "");
            HttpContext.Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
            HttpContext.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=Export.xlsx");
            HttpContext.Response.ContentType = "application/excel";
            HttpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            HttpContext.Response.BinaryWrite(package.GetAsByteArray());
            HttpContext.Response.End();

        }

        //methods for calculating rows in the excel file

        private void FirstPeriodRows(string period, string monthlyInterest, string monthlyPrinciple, string paid, string remainingPrinciple, DataTable dt, ExcelDataModel model, LoanSearchParameter inputData)
        {

            for (int i = 0; i < inputData.TermFirstPeriod; i++)
            {
                DataRow row = dt.NewRow();
                row[period] = model.CalculationFirst.ElementAt(i).Period;//Period of term
                row[monthlyInterest] = model.CalculationFirst.ElementAt(i).InterestPaid;//Monthly paid interest
                row[monthlyPrinciple] = model.CalculationFirst.ElementAt(i).Payment;//Monthly paid ammount
                row[paid] = model.CalculationFirst.ElementAt(i).PrinciplePaid;// Already paid ammount
                row[remainingPrinciple] = model.CalculationFirst.ElementAt(i).RemainingPrincipal; // Remaining installment
                dt.Rows.Add(row);
            }

        }

        private void SecondPeriodRows(string period, string monthlyInterest, string monthlyPrinciple, string paid, string remainingPrinciple, DataTable dt, ExcelDataModel model, LoanSearchParameter inputData)
        {

            for (int i = 0; i < (inputData.TermSecondPeriod); i++)
            {
                DataRow row = dt.NewRow();
                row[period] = model.CalculationSecond.ElementAt(i).Period;//Period of term
                row[monthlyInterest] = model.CalculationSecond.ElementAt(i).InterestPaid;//Monthly paid interest
                row[monthlyPrinciple] = model.CalculationSecond.ElementAt(i).Payment;//Monthly paid ammount
                row[paid] = model.CalculationSecond.ElementAt(i).PrinciplePaid;// Already paid ammount
                row[remainingPrinciple] = model.CalculationSecond.ElementAt(i).RemainingPrincipal; // Remaining installment
                dt.Rows.Add(row);
            }

        }

        private void ThirdPeriodRows(string period, string monthlyInterest, string monthlyPrinciple, string paid, string remainingPrinciple, DataTable dt, ExcelDataModel model, LoanSearchParameter inputData)
        {

            for (int i = 0; i < (inputData.TermThirdPeriod); i++)
            {
                DataRow row = dt.NewRow();
                row[period] = model.CalculationThird.ElementAt(i).Period;//Period of term
                row[monthlyInterest] = model.CalculationThird.ElementAt(i).InterestPaid;//Monthly paid interest
                row[monthlyPrinciple] = model.CalculationThird.ElementAt(i).Payment;//Monthly paid ammount
                row[paid] = model.CalculationThird.ElementAt(i).PrinciplePaid;// Already paid ammount
                row[remainingPrinciple] = model.CalculationThird.ElementAt(i).RemainingPrincipal; // Remaining installment
                dt.Rows.Add(row);
            }

        }

        //method for Getting datas per period 1st interest period 

        private List<PeriodicCalculationFirst> FirstPeriodCalculation(int inputTermFirst, double inputInterestFirst, double loanAmmount, int term, double sumAllPeriod)
        {
            double fullInterest = (loanAmmount * (inputInterestFirst / 100));
            double periodicInterest = (fullInterest / (term / inputTermFirst)) / inputTermFirst;

            double fullmonthlyPrinciple = (loanAmmount + (loanAmmount * (inputInterestFirst / 100)));
            double periodicmonthlyPrinciple = (fullmonthlyPrinciple / (term / inputTermFirst)) / inputTermFirst;

            double paid = 0;
            double remainingPrinciple = sumAllPeriod;

            List<PeriodicCalculationFirst> FirstPeriodicCalculation = new List<PeriodicCalculationFirst>();


            for (int i = 1; i <= inputTermFirst; i++)
            {

                PeriodicCalculationFirst calculation = new PeriodicCalculationFirst();
                calculation.InterestPaid = periodicInterest;

                calculation.Payment = periodicmonthlyPrinciple;

                paid += periodicmonthlyPrinciple;

                calculation.PrinciplePaid = paid;

                remainingPrinciple = remainingPrinciple - periodicmonthlyPrinciple;

                calculation.RemainingPrincipal = remainingPrinciple;

                calculation.Period = i;

                FirstPeriodicCalculation.Add(calculation);

            }

            return FirstPeriodicCalculation;

        }

        //method for Getting datas per period 2st interest period 

        private List<PeriodicCalculationSecond> SecondPeriodCalculation(int? inputTermSecond, int inputTermFirst, double? inputInterestSecond, double loanAmmount, int term, double sumAllPeriod, double sumFirstPeriod)
        {
            double fullInterest = (loanAmmount * ((double)inputInterestSecond / 100));
            double periodicInterest = (fullInterest / (term / (double)inputTermSecond)) / (double)inputTermSecond;

            double fullmonthlyPrinciple = (loanAmmount + (loanAmmount * (double)(inputInterestSecond / 100)));
            double periodicmonthlyPrinciple = (fullmonthlyPrinciple / (term / (double)inputTermSecond)) / (double)inputTermSecond;

            double paid = sumFirstPeriod;
            double remainingPrinciple = sumAllPeriod - sumFirstPeriod;

            List<PeriodicCalculationSecond> SecondPeriodicCalculation = new List<PeriodicCalculationSecond>();


            for (int i = 1; i <= inputTermSecond; i++)
            {

                PeriodicCalculationSecond calculation = new PeriodicCalculationSecond();
                calculation.InterestPaid = periodicInterest;

                calculation.Payment = periodicmonthlyPrinciple;

                paid += periodicmonthlyPrinciple;

                calculation.PrinciplePaid = paid;

                remainingPrinciple = remainingPrinciple - periodicmonthlyPrinciple;

                calculation.RemainingPrincipal = remainingPrinciple;

                calculation.Period = i + inputTermFirst;

                SecondPeriodicCalculation.Add(calculation);

            }

            return SecondPeriodicCalculation;

        }


        //method for Getting datas per period 3st interest period 

        private List<PeriodicCalculationThird> ThirdPeriodCalculation(int? inputTermThird, int? InputTermSecond, double? inputInterestThird, double loanAmmount, int term, double sumAllPeriod, double sumFirstPeriod, double sumSecondPeriod)
        {
            double fullInterest = loanAmmount * ((double)inputInterestThird / 100);
            double periodicInterest = (fullInterest / (term / (double)inputTermThird)) / (double)inputTermThird;

            double fullmonthlyPrinciple = (loanAmmount + (loanAmmount * ((double)inputInterestThird / 100)));
            double periodicmonthlyPrinciple = (fullmonthlyPrinciple / (term / (double)inputTermThird)) / (double)inputTermThird;

            double paid = sumFirstPeriod + sumSecondPeriod;
            double remainingPrinciple = sumAllPeriod - paid;

            List<PeriodicCalculationThird> ThirdPeriodicCalculation = new List<PeriodicCalculationThird>();


            for (int i = 1; i <= inputTermThird; i++)
            {

                PeriodicCalculationThird calculation = new PeriodicCalculationThird();
                calculation.InterestPaid = periodicInterest;

                calculation.Payment = periodicmonthlyPrinciple;

                paid += periodicmonthlyPrinciple;

                calculation.PrinciplePaid = paid;

                remainingPrinciple = remainingPrinciple - periodicmonthlyPrinciple;

                calculation.RemainingPrincipal = remainingPrinciple;

                calculation.Period = i + (term - (int)inputTermThird - (int)InputTermSecond) + (int)InputTermSecond;

                ThirdPeriodicCalculation.Add(calculation);

            }

            return ThirdPeriodicCalculation;

        }
        //Setting the unused periodinterest to Zero
        private void SetPeriodInterestToZero(IQueryable<LoanSearchParameter> query)
        {

            foreach (var x in query.AsEnumerable())
            {
                if (x.InterestPeriodId == 1)
                {
                    x.InterestSecondPeriod = 0;
                    x.InterestThirdPeriod = 0;
                }
                else if (x.InterestPeriodId == 2)
                {
                    x.InterestThirdPeriod = 0;
                }
            }

        }
    }

    }







