using System;

namespace PE.Plugins.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public abstract class Validator : Attribute
    {
        #region Properties

        virtual public string Message { get; set; }

        virtual public int Order { get; set; }

        #endregion Properties

        #region Methods

        public abstract bool IsValid(object value);

        #endregion Methods
    }
}
