using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SharedProperty.NETStandard.Extensions
{
    public static class TypeExtension
    {
        private const string implicitOperatorMethodName = "op_Implicit";

        public static bool CanImplicitOperatingConvert(this Type type, Type targetType)
        {
            if (type.canImplicitOperatingConvertTo(targetType))
            {
                return true;
            }
            if (targetType.canImplicitOperatingConvertTo(targetType))
            {
                return true;
            }
            return false;
        }

        private static IEnumerable<MethodInfo> getImplicitOperatorMethods(this Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (method.IsSpecialName == false)
                {
                    continue;
                }
                if (method.Name != implicitOperatorMethodName)
                {
                    continue;
                }

                yield return method;
            }
        }

        private static bool canImplicitOperatingConvertTo(this Type type, Type targetType)
        {
            foreach (var operatorMethod in type.getImplicitOperatorMethods())
            {
                if (operatorMethod.ReturnType == targetType)
                {
                    return true;
                }
            }
            return false;
        }

        public static Func<IProperty, T> CreatePropertyConvertAndGetValueDelegate<T>(this Type sourceType)
        {
            ParameterExpression property = Expression.Parameter(typeof(IProperty), "property");
            MemberExpression value = Expression.Property(property, nameof(IProperty.Value));
            UnaryExpression castedValue = Expression.Convert(value, sourceType);
            UnaryExpression convert = Expression.Convert(castedValue, typeof(T));
            return Expression.Lambda<Func<IProperty, T>>(convert, property).Compile();
        }
    }
}
