

$(document).ready(function ()
{
	var interestPeriod = 0;
	
	$("#inputInterest").change(function () {
		
		interestPeriod = $("#inputInterest").val();

		if (interestPeriod == 1)
		{
			document.getElementById("first").style.display = "block"
			document.getElementById("second").style.display = "none"
			document.getElementById("third").style.display = "none"

			document.getElementById("firstTerm").style.display = "none"
			document.getElementById("secondTerm").style.display = "none"
			document.getElementById("thirdTerm").style.display = "none"
		}



		else if (interestPeriod == 2)
		{
			document.getElementById("first").style.display = "block"
			document.getElementById("second").style.display = "block"
			document.getElementById("third").style.display = "none"

			document.getElementById("firstTerm").style.display = "block"
			document.getElementById("secondTerm").style.display = "block"
			document.getElementById("thirdTerm").style.display = "none"
			
		}

		else if (interestPeriod == 3)
		{
			document.getElementById("first").style.display = "block"
			document.getElementById("second").style.display = "block"
			document.getElementById("third").style.display = "block"

			document.getElementById("firstTerm").style.display = "block"
			document.getElementById("secondTerm").style.display = "block"
			document.getElementById("thirdTerm").style.display = "block"
		}
		else
		{
			document.getElementById("first").style.display = "block"
			document.getElementById("second").style.display = "none"
			document.getElementById("third").style.display = "none"

			document.getElementById("firstTerm").style.display = "none"
			document.getElementById("secondTerm").style.display = "none"
			document.getElementById("thirdTerm").style.display = "none"
		}


		$.validator.methods.number = function (value, element) {
			return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)?(?:,\d+)?$/.test(value);
		}

	});

});

