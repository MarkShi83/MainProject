using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MainProject.Common.Data.Helpers
{
    public interface IQueryBuilder
    {
        CompiledQuery Build<T>(T query) where T : Query;
    }

    public class QueryBuilder : IQueryBuilder
    {
        public CompiledQuery Build<T>(T query)
            where T : Query
        {
            var result = new CompiledQuery
            {
                Query = query
            };
           
            var builder = new StringBuilder();
            var countBuilder = new StringBuilder();

            var prefix = query.QueryAttribute.Prefix ?? "t1";

            ApplyReadOnlyMode(builder, countBuilder);

            ApplySelect(builder, prefix, countBuilder);

            var filterProperties = GetFilters(query).ToArray();

            ApplyFrom(query, filterProperties, prefix, builder, countBuilder);

            ApplyWhere(query, filterProperties, countBuilder, prefix, builder, result);

            result.CountSql = countBuilder.ToString();

            ApplyOrderBy(query, builder, prefix);

            ApplyPagination(query, builder, result);

            result.Sql = builder.ToString();
            return result;
        }

        private static void ApplySelect(StringBuilder builder, string prefix, StringBuilder countBuilder)
        {
            builder.AppendLine($"SELECT {prefix}.*");
            countBuilder.AppendLine("SELECT COUNT(*)");
        }

        private static void ApplyReadOnlyMode(StringBuilder builder, StringBuilder countBuilder)
        {
            const string ReadOnlyMode = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED READ ONLY;";

            builder.AppendLine(ReadOnlyMode);
            countBuilder.AppendLine(ReadOnlyMode);
        }

        private static void ApplyFrom<T>(
            T query,
            IEnumerable<KeyValuePair<PropertyInfo, QueryParameterAttribute>> filterProperties,
            string prefix,
            StringBuilder builder,
            StringBuilder countBuilder)
            where T : Query
        {
            var from = $"FROM {query.QueryAttribute.TableName} AS {prefix}";

            foreach (var (_, attribute) in filterProperties)
            {
                if (string.IsNullOrEmpty(attribute.TablesIfValueProvided))
                {
                    continue;
                }

                from = $"FROM {attribute.TablesIfValueProvided}";
                break;
            }

            builder.AppendLine(from);
            countBuilder.AppendLine(from);
        }

        private static void ApplyWhere<T>(
            T query,
            IEnumerable<KeyValuePair<PropertyInfo, QueryParameterAttribute>> filterProperties,
            StringBuilder countBuilder,
            string prefix,
            StringBuilder builder,
            CompiledQuery result)
            where T : Query
        {
            var isWhereAdded = false;
            foreach (var (property, attribute) in filterProperties)
            {
                var value = property.GetValue(query);
                var propertyType = property.PropertyType;
                var valueProperty = propertyType.GetProperty("Value");
                var operatorProperty = propertyType.GetProperty("Operator");

                var operatorValue = (QueryOperator)operatorProperty.GetValue(value);

                var parameterName = property.Name.ToLowerInvariant();
                var name = attribute.ColumnName ?? parameterName;
                var filterPrefix = !isWhereAdded ? "WHERE " : "AND ";
                var leftOperand = $"{attribute.Prefix ?? prefix}.{name}";
                var @operator = GetSqlOperator(operatorValue);
                var rightOperand = $"@{parameterName}";
                var row = $"{filterPrefix}{leftOperand}{@operator}{rightOperand}";

                builder.AppendLine(row);
                countBuilder.AppendLine(row);

                isWhereAdded = true;
                
                result.Parameters.Add(property.Name.ToLowerInvariant(), valueProperty.GetValue(value));
            }
        }

        private static string GetSqlOperator(QueryOperator @operator)
        {
            switch (@operator.Type)
            {
                case QueryOperator.OperatorType.Equal:
                    return "=";
                case QueryOperator.OperatorType.NotEqual:
                    return "!=";
                case QueryOperator.OperatorType.Greater:
                    return ">";
                case QueryOperator.OperatorType.GreaterAndEqual:
                    return ">=";
                case QueryOperator.OperatorType.Less:
                    return "<";
                case QueryOperator.OperatorType.LessAndEqual:
                    return "<=";
                case QueryOperator.OperatorType.SimilarTo:
                    return "~";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<KeyValuePair<PropertyInfo, QueryParameterAttribute>> GetFilters<T>(T query)
            where T : Query
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(c => c.PropertyType.Name == "QueryParameterValue`1").ToArray();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<QueryParameterAttribute>();

                if (attribute == null || !attribute.CanBeFiltered)
                {
                    continue;
                }

                var value = property.GetValue(query);
                var valueIsProvidedProperty = property.PropertyType.GetProperty("ValueIsProvided");

                var valueIsProvided = (bool)valueIsProvidedProperty.GetValue(value);

                if (!valueIsProvided)
                {
                    continue;
                }

                yield return new KeyValuePair<PropertyInfo, QueryParameterAttribute>(property, attribute);
            }
        }

        private static IEnumerable<Tuple<PropertyInfo, SortMethod, QueryParameterAttribute>> GetSorts<T>(T query)
            where T : Query
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(c => c.PropertyType.Name == "QueryParameterValue`1").ToArray();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<QueryParameterAttribute>();

                if (attribute == null || !attribute.CanBeSorted)
                {
                    continue;
                }

                var value = property.GetValue(query);
                var sortMethodProperty = property.PropertyType.GetProperty("SortMethod");

                var sortMethod = (SortMethod)sortMethodProperty.GetValue(value);

                if (sortMethod == SortMethod.None)
                {
                    continue;
                }

                yield return new Tuple<PropertyInfo, SortMethod, QueryParameterAttribute>(property, sortMethod, attribute);
            }
        }

        private static void ApplyOrderBy<T>(T query, StringBuilder builder, string prefix)
            where T : Query
        {
            var sortProperties = GetSorts(query);
            var isFirstItem = true;

            foreach (var (property, sortMethod, attribute) in sortProperties)
            {
                builder.Append(isFirstItem ? "ORDER BY " : ", ");
                builder.AppendLine($"{attribute.ColumnName ?? property.Name.ToLowerInvariant()} {sortMethod}");

                isFirstItem = false;
            }

            if (isFirstItem && !string.IsNullOrEmpty(query.QueryAttribute.DefaultSortColumn))
            {
                builder.AppendLine(
                    $"ORDER BY {prefix}.{query.QueryAttribute.DefaultSortColumn} {query.QueryAttribute.DefaultSortMethod}");
            }
        }

        private static void ApplyPagination<T>(T query, StringBuilder builder, CompiledQuery result)
            where T : Query
        {
            var (length, offset) = query.GetOffset();

            builder.AppendLine("LIMIT  @length");
            builder.AppendLine("OFFSET @offset");

            result.Parameters.Add("@length", length);
            result.Parameters.Add("@offset", offset);
        }
    }
}
