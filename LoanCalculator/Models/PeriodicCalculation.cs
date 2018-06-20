using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
/// <summary>
/// This model is represent the datas for the periodic calculation
/// </summary>
namespace LoanCalculator.Models
{
    public class PeriodicCalculation
    {
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Havonta fizetendő kamat")]
        public double InterestPaid { get; set; }
        [Display(Name = "Hónap")]
        public int Period { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Havonta fizetendő részlet")]
        public double Payment { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Hátralévő törlesztőrészlet")]
        public double RemainingPrincipal { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Kiegyenlített tartozás")]
        public double PrinciplePaid { get; set; }

        
    }
}