using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoanCalculator.Models
{
    public class InputDataModel
    {
        [Required]
        [Display(Name = "Azonosító")]
        public int Id { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(1, int.MaxValue, ErrorMessage = "Az összeg nem lehet negatív vagy nulla!")]
        [Display(Name = "Hitelösszeg")]
        public double LoanAmmount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A futamidő csak pozitív egész szám lehet!")]
        [Display(Name = "Futamidő")]
        public int Term { get; set; }

        [Required]
        [Range(1, 3)]
        [Display(Name = "Kamatperiódus")]
        public int InterestPeriod { get; set; }

        [Required]
        [Display(Name = "Kalkuláció Ideje")]
        [DataType(DataType.Date)]
        public DateTime CalculationTime { get; set; }

        
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(1, int.MaxValue, ErrorMessage = "A kamat csak pozitív szám lehet!")]
        public double InterestFirstPeriod { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(1, int.MaxValue, ErrorMessage = "A kamat csak pozitív szám lehet!")]
        public double? InterestSecondPeriod { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Range(1, int.MaxValue, ErrorMessage = "A kamat csak pozitív szám lehet!")]
        public double? InterestThirdPeriod { get; set; }

       
        [Range(1, int.MaxValue, ErrorMessage = "A futamidő csak pozitív egész szám lehet!")]
        public int  FirstTerm { get; set; }

       
        [Range(1, int.MaxValue, ErrorMessage = "A futamidő csak pozitív egész szám lehet!")]
        public int?  SecondTerm { get; set; }

       
        [Range(1, int.MaxValue, ErrorMessage = "A futamidő csak pozitív egész szám lehet!")]
        public int? ThirdTerm { get; set; }
        
        public string UserId { get; set; }




    }
}