namespace PE.Plugins.Validation.Validators
{
    public class MinValueValidator : Validator
    {
        #region Constructor

        public MinValueValidator(int minValue)
        {
            MinValue = minValue;
        }

        #endregion Constructor

        #region Properties

        public int MinValue { get; set; }

        #endregion Properties

        #region Methods

        public override bool IsValid(object value)
        {
            try
            {
                if (value == null) return false;
                //  cater for string, int? and int
                int v = 0;
                if (value is int)
                {
                    v = (int)value;
                }
                else if (value is int?)
                {
                    v = ((int?)value).Value;
                }
                else
                {
                    v = int.Parse(value.ToString());
                }
                return (v >= MinValue);
            }
            catch
            {
                return false;
            }
        }

        #endregion Methods
    }
}
