using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace PE.Plugins.Validation
{
    public delegate object GetDataDelegate();

    public delegate string CreateHashCallback(string value);

    public interface IValidationService
    {
        bool Validate(object sender);

        bool Validate(object sender, Dictionary<string, GetDataDelegate> callbacks);

        bool Validate(string invalidSuffix, object sender);

        bool Validate<T>(object sender, Expression<Func<T>> property);

        bool Validate<T, TInvalid>(object sender, Expression<Func<T>> property, Expression<Func<TInvalid>> invalid);

        bool Validate(object sender, [CallerMemberName] string property = "");

        bool Validate<T>(object sender, Expression<Func<T>> property, GetDataDelegate getDataCallback);

        string CreateHash(string value);
    }
}
