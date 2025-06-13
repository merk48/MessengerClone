using System.Linq.Expressions;

namespace MessengerClone.Service.Features.General.Extentions
{
    public static class FilterExpressionHelper<T>
    {
        public static Expression<Func<T, bool>> GetFilter(string filter)
        {
            var parts = filter.Split(new string[] { ">=", "<=", "!=", "=", ">", "<" }, StringSplitOptions.None);

            if (parts.Length != 2) throw new ArgumentException("Invalid filter format");

            string property = parts[0].Trim();
            string operatorSymbol = filter.Substring(property.Length, filter.Length - property.Length - parts[1].Length).Trim();
            string value = parts[1].Trim();

            var filterExpression = GetFilterExpression(property, operatorSymbol, value);

            return filterExpression;
        }

        private static Expression<Func<T, bool>> GetFilterExpression(string property, string operatorSymbol, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyExpression = Expression.Property(parameter, property);
            var valueExpression = Expression.Constant(Convert.ChangeType(value, propertyExpression.Type));

            Expression comparison = operatorSymbol switch
            {
                ">" => Expression.GreaterThan(propertyExpression, valueExpression),
                "<" => Expression.LessThan(propertyExpression, valueExpression),
                ">=" => Expression.GreaterThanOrEqual(propertyExpression, valueExpression),
                "<=" => Expression.LessThanOrEqual(propertyExpression, valueExpression),
                "=" => Expression.Equal(propertyExpression, valueExpression),
                _ => throw new NotSupportedException($"Operator {operatorSymbol} is not supported")
            };

            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }
    }
}
