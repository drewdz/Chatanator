using System.Text.RegularExpressions;

namespace PE.Plugins.Validation.Validators
{
    public class RegularExpressionValidator : Validator
    {
        #region Properties

        private string _Pattern = string.Empty;
        public string Pattern
        {
            get { return _Pattern; }
            set { _Pattern = value; }
        }

        #endregion Properties

        #region Implementation

        public override bool IsValid(object o)
        {
            if (string.IsNullOrEmpty(Pattern)) throw new System.Exception("No pattern found to evaluate.");

            if (string.IsNullOrEmpty(o as string)) return false;
            string value = o.ToString().Trim();

            if (!Regex.IsMatch(value, Pattern))
            {
                return false;
            }

            return true;
        }

        #endregion Implementation
    }
}
