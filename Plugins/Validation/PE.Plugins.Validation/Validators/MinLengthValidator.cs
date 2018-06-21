using System;

namespace PE.Plugins.Validation.Validators
{
    public class MinLengthValidator : Validator
    {
        #region Constructors

        public MinLengthValidator(int length)
        {
            Length = length;
        }

        #endregion Constructors

        #region Properties

        public int Length { get; set; }

        #endregion Properties

        #region Methods

        public override bool IsValid(object value)
        {
            //  value should be a string
            if (value == null) return false;
            if (!(value is string)) throw new Exception("Minimum length validation can only be performed on a string value.");

            string s = (string)value;
            return !(string.IsNullOrEmpty(s) || (s.Length < Length));
        }

        #endregion Methods
    }
}
