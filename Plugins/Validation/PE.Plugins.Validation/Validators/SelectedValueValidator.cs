namespace PE.Plugins.Validation.Validators
{
    public class SelectedValueValidator : Validator
    {
        #region Constructor

        public SelectedValueValidator(string defaultValue)
        {
            DefaultValue = defaultValue;
        }

        #endregion Constructor

        #region Fields

        private string DefaultValue { get; set; }

        #endregion Fields

        #region Implementation

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            
            string v = value.ToString();

            if (string.IsNullOrEmpty(v)) return false;

            if (v == "[ -- Please Select -- ]") return false;
            if (v == "[Please Select]") return false;
            if (v == "Select Option...") return false;

            //  check if the value is the default
            return (v.Equals(DefaultValue)) ? false : true;
        }

        #endregion Implementation
    }
}
