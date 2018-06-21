using System;
namespace PE.Framework.Models
{
    public class AuthConfirmNetResponse
    {
		public AuthConfirmNetResponse(bool success = true,
                              string errorMessage = "")
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }
        
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
