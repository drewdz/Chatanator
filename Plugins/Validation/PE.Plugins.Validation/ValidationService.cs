using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PE.Plugins.Validation
{
    public class ValidationService : IValidationService
    {
        #region Fields

        private readonly CreateHashCallback _CreateHash;

        #endregion Fields

        #region Constructors

        public ValidationService(ValidationConfig config)
        {
            _CreateHash = config.CreateHash;
        }

        #endregion Constructors

        #region Methods

        virtual public bool Validate(object sender)
        {
            //  find all properties with the attribute
            var properties = sender.GetType().GetRuntimeProperties();
            bool valid = true;
            foreach (PropertyInfo property in properties)
            {
                //  find a property for the invalid message
                PropertyInfo invalid = sender.GetType().GetRuntimeProperty(property.Name + "Invalid");
                //  validate
                if (invalid != null)
                {
                    valid = ValidateProperty(sender, property, invalid, null) && valid;
                }
            }
            return valid;
        }

        virtual public bool Validate(object sender, Dictionary<string, GetDataDelegate> callbacks)
        {
            //  find all properties with the attribute
            var properties = sender.GetType().GetRuntimeProperties();
            bool valid = true;
            foreach (PropertyInfo property in properties)
            {
                //  find a property for the invalid message
                PropertyInfo invalid = sender.GetType().GetRuntimeProperty(property.Name + "Invalid");
                //  validate
                if (invalid != null)
                {
                    var callback = (callbacks.ContainsKey(property.Name)) ? callbacks[property.Name] : null;
                    valid = ValidateProperty(sender, property, invalid, callback) && valid;
                }
            }
            return valid;
        }

        virtual public bool Validate(string invalidSuffix, object sender)
        {
            //  find all properties with the attribute
            var properties = sender.GetType().GetRuntimeProperties();
            bool valid = true;
            foreach (PropertyInfo property in properties)
            {
                //  find a property for the invalid message
                PropertyInfo invalid = sender.GetType().GetRuntimeProperty(property.Name + invalidSuffix);
                //  validate
                if (invalid != null)
                {
                    valid = valid && ValidateProperty(sender, property, invalid, null);
                }
            }
            return valid;
        }

        virtual public bool Validate<T>(object sender, Expression<Func<T>> property)
        {
            PropertyInfo p = GetProperty(sender, property);
            return ValidateProperty(sender, p, sender.GetType().GetRuntimeProperty(p.Name + "Invalid"), null);
        }

        virtual public bool Validate<T, TInvalid>(object sender, Expression<Func<T>> property, Expression<Func<TInvalid>> invalid)
        {
            return ValidateProperty(sender, GetProperty(sender, property), GetProperty(sender, invalid), null);
        }

        virtual public bool Validate(object sender, [CallerMemberName] string property = "")
        {
            Type type = sender.GetType();
            return ValidateProperty(sender, type.GetRuntimeProperty(property), type.GetRuntimeProperty(property + "Invalid"), null);
        }

        virtual public bool Validate<T>(object sender, Expression<Func<T>> property, GetDataDelegate getDataCallback)
        {
            PropertyInfo p = GetProperty(sender, property);
            return ValidateProperty(sender, p, sender.GetType().GetRuntimeProperty(p.Name + "Invalid"), getDataCallback);
        }

        private bool ValidateProperty(object sender, PropertyInfo property, PropertyInfo invalid, GetDataDelegate getDataCallback)
        {
            //  find the validator attribute           
            var attributes = property.GetCustomAttributes<Validator>(true);
            var sortedAttributes = attributes.OrderBy(si => si.Order).ToList();
            foreach (Attribute attribute in sortedAttributes)
            {
                if (!ValidateProperty(sender, property, attribute, invalid, getDataCallback)) return false;
            }
            return true;
        }

        private bool ValidateProperty(object sender, PropertyInfo property, Attribute attribute, PropertyInfo invalid, GetDataDelegate getDataCallback)
        {
            //  reset the invalid message
            invalid.SetValue(sender, string.Empty);

            Validator validatorAttribute = attribute as Validator;
            //  instantiate the validator
            if (validatorAttribute == null) throw new Exception(string.Format("Could not construct validator for attribute. {0}", attribute));
            try
            {
                if (!validatorAttribute.IsValid(property.GetValue(sender)))
                {
                    //  update the invalid property with the message
                    invalid.SetValue(sender, validatorAttribute.Message);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("*** ValidationService.ValidateProperty ({0}) - Exception: {1}", property.Name, ex));
                //  update the invalid property with the message
                invalid.SetValue(sender, validatorAttribute.Message);
                return false;
            }
        }

        private PropertyInfo GetProperty<T>(object sender, Expression<Func<T>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            Type type = sender.GetType();
            return type.GetRuntimeProperty(body.Member.Name);
        }

        public string CreateHash(string value)
        {
            if (_CreateHash == null) throw new ArgumentNullException("No callback found to create the hash.");
            return _CreateHash(value);
        }

        #endregion Methods
    }
}
