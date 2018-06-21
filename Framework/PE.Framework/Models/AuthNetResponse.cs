using System;
namespace PE.Framework.Models
{
    public class AuthNetResponse
    {
        public AuthNetResponse(bool twoFa = false,
		                       bool success = true,
		                      string errorMessage = "")
        {
			this.TwoFa = twoFa;
			this.Success = success;
			this.ErrorMessage = errorMessage;
        }

		public bool TwoFa { get; set; }
        public bool Success { get; set; }
		public string ErrorMessage { get; set; }
    }
}
