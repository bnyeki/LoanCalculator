using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoanCalculator.Models
{
    public class CalculationSearchModel
    {
        
        [Display(Name = "Azonosító")]
        public int Id { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Hitelösszeg")]
        public double LoanAmmount { get; set; }
        
        [Display(Name = "Futamidő")]
        public int Term { get; set; }
        
        [Display(Name = "Kamat")]
        public double Interest { get; set; }
        
        [Display(Name = "Kamatperiódus")]
        public double InterestPeriod { get; set; }
        
        [Display(Name = "Kalkuláció Ideje")]
        [DataType(DataType.Date)]
        public DateTime CalculationTime { get; set; }
        public string UserId { get; set; }
        
        public List<PeriodicCalculation> Calculation { get; set; }
        public ReturnFilter Filter { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Havi átlagos fizetendő összeg")]
        public double Average { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Összesen visszafizetendő összeg")]
        public double Sum { get; set; }

        
    }

    public class ReturnFilter
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Dátum")]
        public DateTime? CalculationTimeFrom { get; set; }

       // [Display(Name = "Befejező dátum")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CalculationTimeTo { get; set; }
        [Display(Name = "Összeg")]
        public double? LoanAmmountFrom { get; set; }
        //[Display(Name = "Maximum Összeg")]
        public double? LoanAmmountTo { get; set; }

    }

   







}