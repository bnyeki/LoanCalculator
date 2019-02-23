using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoanCalculator.Models
{
    public class CalculationModel
    {

        [Display(Name = "Azonosító")]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Hitelösszeg")]
        public double LoanAmmount { get; set; }

        [Display(Name = "Futamidő")]
        public int Term { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "ÁtlagosKamat")]
        public double AverageInterest { get; set; }

        [Display(Name = "Kamatperiódus")]
        public int InterestPeriod { get; set; }

        [Display(Name = "Kalkuláció Ideje")]
        [DataType(DataType.Date)]
        public DateTime CalculationTime { get; set; }

        [Display(Name = "Lejárat Dátuma")]
        [DataType(DataType.Date)]
        public DateTime ExpireTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Havi átlagos fizetendő összeg")]
        public double Average { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Összesen visszafizetendő összeg")]
        public double Sum { get; set; }

        public string UserId { get; set; }


        public List<PeriodicCalculationFirst> CalculationFirst { get; set; }
        public List<PeriodicCalculationSecond> CalculationSecond { get; set; }
        public List<PeriodicCalculationThird> CalculationThird { get; set; }


        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }


    }
}