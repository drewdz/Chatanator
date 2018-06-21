using System;

namespace PE.Plugins.Validation.Validators
{
    public class MaxValueValidator : Validator
    {
        #region Constructor

        public MaxValueValidator(int maxValue)
        {
            MaxValue = maxValue;
        }

        #endregion Constructor

        #region Properties

        public int MaxValue { get; set; }

        #endregion Properties

        #region Methods

        public override bool IsValid(object value)
        {
            try
            {
                if (value == null) return false;
                int s = (int)value;
                return (MaxValue > s);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion Methods
    }
}
