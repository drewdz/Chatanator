namespace PE.Plugins.Validation.Validators
{
    public class RequiredValidator : Validator
    {
        #region Fields

        public string DefaultValue { get; set; }

        #endregion Fields

        #region Implementation

        public override bool IsValid(object value)
        {
            if (value == null) return false;

            string v = value as string;

            if (string.IsNullOrEmpty(v)) return false;

            //  check if the value is the default
            return (v.Equals(DefaultValue)) ? false : true;
        }

        #endregion Implementation
    }
}
