using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanCalculator.Models
{
    public class ExcelDataModel
    {
        public List<PeriodicCalculationFirst> CalculationFirst { get; set; }
        public List<PeriodicCalculationSecond> CalculationSecond { get; set; }
        public List<PeriodicCalculationThird> CalculationThird { get; set; }
    }
}