using System;
using System.Linq;
using System.Reflection;

namespace MainProject.Common.Data.Helpers
{
    public interface IQueryParameterParser
    {
        T Parse<T>(string[] filter, string[] sort) where T : Query, new();
    }

    public class QueryParameterParser : IQueryParameterParser
    {
        public T Parse<T>(string[] filter, string[] sort) where T : Query, new()
        {
            var result = new T();

            if (filter == null)
            {
                return result;
            }

            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            result.QueryAttribute = type.GetCustomAttribute<QueryAttribute>();

            if (result.QueryAttribute == null)
            {
                result.Errors.Add("Query attribute has not been provided!");
                return result;
            }
            
            var queryParameterAttributeType = typeof(QueryParameterAttribute);
            foreach (var property in properties)
            {
                var attribute = (QueryParameterAttribute)property
                    .GetCustomAttributes(queryParameterAttributeType, true)
                    .FirstOrDefault();

                if (attribute == null)
                {
                    continue;
                }

                var name = $"[{property.Name}]".ToLowerInvariant();
                var filterParameter = filter.FirstOrDefault(c => !string.IsNullOrEmpty(c) && c.ToLowerInvariant().StartsWith(name));
                var sortParameter = sort?.FirstOrDefault(c => !string.IsNullOrEmpty(c) && c.ToLowerInvariant().StartsWith(name));

                var currentValue = property.GetValue(result) ??
                                   Activator.CreateInstance(property.PropertyType);
                property.SetValue(result, currentValue);
                
                var sortMethodProperty = property.PropertyType.GetProperty("SortMethod");

                if (sortParameter != null)
                {
                    var sortSegments = sortParameter.Split('=');
                    if (sortSegments.Length != 2)
                    {
                        result.Errors.Add($"{name} has invalid sort value!");
                    }
                    else
                    {
                        var sortMethod = Enum.Parse(typeof(SortMethod), sortSegments[1], true);
                        sortMethodProperty.SetValue(currentValue, sortMethod);
                    }
                }

                if (filterParameter == null)
                {
                    if (attribute.IsMandatory)
                    {
                        result.Errors.Add($"{name} is not optional.");
                    }

                    continue;
                }

                // Only supports equality right now!
                var @operator = GetOperator(filterParameter, name);

                if (@operator == null)
                {
                    result.Errors.Add($"{name} has invalid operator.");
                    continue;
                }

                var providedValue = filterParameter.Remove(0, name.Length + 2);
                if (string.IsNullOrEmpty(providedValue))
                {
                    result.Errors.Add($"{name} has invalid value!");
                    continue;
                }

                var valueProperty = property.PropertyType.GetProperty("Value");
                
                if (!TryConvertChangeType(providedValue, valueProperty, out var value))
                {
                    result.Errors.Add($"{name} has invalid value!");
                }

                var operatorProperty = property.PropertyType.GetProperty("Operator");
                operatorProperty.SetValue(currentValue, @operator);

                var valueIsProvidedProperty = property.PropertyType.GetProperty("ValueIsProvided");

                valueIsProvidedProperty.SetValue(currentValue, true);
                valueProperty.SetValue(currentValue, value);
            }

            result.Validate();
            result.IsPopulated = true;

            return result;
        }

        private static QueryOperator GetOperator(string queryStringParameter, string name)
        {
            var replaced = queryStringParameter.ToLowerInvariant().Replace(name.ToLowerInvariant(), string.Empty);

            if (string.IsNullOrEmpty(replaced))
            {
                return null;
            }

            var @operator = replaced.Substring(0, 2);

            return QueryOperator.Operators.FirstOrDefault(c => c.Identifier == @operator);
        }

        private static bool TryConvertChangeType(string input, PropertyInfo valueProperty, out object value)
        {
            try
            {
                var valueType = valueProperty.PropertyType;

                var typeName = valueType.IsGenericType ? valueType.GenericTypeArguments[0].FullName : valueType.FullName;

                if (valueType.IsEnum)
                {
                    value = Enum.Parse(valueType, input, true);
                    return true;
                }

                switch (typeName)
                {
                    case "System.Guid":
                        value = Guid.Parse(input);
                        break;

                    case "System.DateTime":
                        value = DateTime.Parse(input);
                        break;

                    default:
                        value = Convert.ChangeType(input, valueType);
                        break;
                }
                
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}
