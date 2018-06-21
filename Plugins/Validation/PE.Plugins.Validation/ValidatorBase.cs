namespace PE.Plugins.Validation
{
    public abstract class ValidatorBase
    {
        #region Constructors

        public ValidatorBase(string message)
        {
            Message = message;
        }

        public ValidatorBase(string message, int order)
            : this(message)
        {
            Order = order;
        }

        #endregion Constructors

        #region Properties

        public string Message { get; private set; }

        public int Order { get; private set; }

        #endregion Properties

        #region Implementation

        public abstract bool IsValid(object value);

        #endregion Implementation
    }
}
