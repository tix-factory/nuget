using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using TixFactory.Configuration;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder : ISqlQueryBuilder
	{
		internal const string _CountParameterName = "Count";
		internal const string _ExclusiveStartParameterName = "ExclusiveStart";
		internal const string _IsAscendingParameterName = "IsAscending";

		private readonly IDatabaseTypeParser _DatabaseTypeParser;
		private readonly Regex _ExpressionCutRegex = new Regex(@"^.*=>\s*");
		private readonly Regex _CaseInsensitiveReplacementRegex = new Regex(@"(`\w+`|@\w+)\.To(Lower|Upper)\(\)");
		private readonly Regex _EqualReplacementRegex = new Regex(@"(`\w+`|@\w+)\.Equals\((`\w+`|@\w+),?\s*(\w*)\)");
		private readonly Regex _ContainsReplacementRegex = new Regex(@"(`\w+`|@\w+)\.Contains\((`\w+`|@\w+),?\s*(\w*)\)");
		private readonly Regex _StartsWithReplacementRegex = new Regex(@"(`\w+`|@\w+)\.StartsWith\((`\w+`|@\w+),?\s*(\w*)\)");
		private readonly Regex _EndsWithReplacementRegex = new Regex(@"(`\w+`|@\w+)\.EndsWith\((`\w+`|@\w+),?\s*(\w*)\)");
		private readonly Regex _NullSwapRegex = new Regex(@"null\s*([!=]=)\s*(`\w+`|@\w+)");

		/// <summary>
		/// Initializes a new <see cref="SqlQueryBuilder"/>.
		/// </summary>
		/// <param name="databaseTypeParser">An <see cref="IDatabaseTypeParser"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="databaseTypeParser"/>
		/// </exception>
		public SqlQueryBuilder(IDatabaseTypeParser databaseTypeParser)
		{
			_DatabaseTypeParser = databaseTypeParser ?? throw new ArgumentNullException(nameof(databaseTypeParser));
		}

		/// <summary>
		/// Parses the entity column aliases from a <typeparamref name="TRow"/>.
		/// </summary>
		/// <typeparam name="TRow">The model class representing the row of a the table the query is being built for.</typeparam>
		/// <remarks>
		/// An entity column alias dictionary is the mapping a model property name -> table column name.
		/// Represented by the <see cref="DataMemberAttribute"/> on the property if the property name doesn't match the table column name.
		/// </remarks>
		/// <returns>The dictionary mapping property name -> table column name.</returns>
		internal IDictionary<string, string> GetEntityColumnAliases<TRow>()
			where TRow : class
		{
			var aliases = new Dictionary<string, string>();
			var rowProperties = typeof(TRow).GetProperties();

			foreach (var property in rowProperties)
			{
				var column = new TableColumn(property, _DatabaseTypeParser);
				aliases[property.Name] = column.Name;
			}

			return aliases;
		}

		internal string ParseWhereClause(LambdaExpression queryExpression, IDictionary<string, string> entityColumnAliases)
		{
			var expression = queryExpression?.ToString();
			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException($"{nameof(queryExpression)} must not be null or whitespace.", nameof(queryExpression));
			}

			if (entityColumnAliases == null)
			{
				throw new ArgumentNullException(nameof(entityColumnAliases));
			}

			// TODO: Regex example
			expression = _ExpressionCutRegex.Replace(queryExpression.ToString(), "");

			if (expression.Contains("\"") || expression.Contains("'") || expression.Contains("`"))
			{
				throw new ArgumentException($"Restricted character found in {nameof(queryExpression)}", nameof(queryExpression));
			}

			var expressionParameters = queryExpression.Parameters;
			var entityName = expressionParameters.First().Name;
			var parameterNames = expressionParameters.Skip(1).Select(p => p.Name).ToArray();

			expression = ReplaceEntityColumnAliases(expression, entityName, entityColumnAliases);
			expression = ReplaceParameterNames(expression, parameterNames);

			expression = _CaseInsensitiveReplacementRegex.Replace(expression, CaseInsensitiveReplacement);
			expression = _EqualReplacementRegex.Replace(expression, EqualReplacement);
			expression = _ContainsReplacementRegex.Replace(expression, ContainsReplacement);
			expression = _StartsWithReplacementRegex.Replace(expression, StartsWithReplacement);
			expression = _EndsWithReplacementRegex.Replace(expression, EndsWithReplacement);
			expression = _NullSwapRegex.Replace(expression, NullSwapReplacement);

			expression = expression
				.Replace("!= null", "IS NOT NULL")
				.Replace("== null", "IS NULL")
				.Replace("==", "=")
				.Replace("AndAlso", "AND")
				.Replace("OrElse", "OR")
				.Replace(".HasValue", " IS NOT NULL")
				.Replace(".Value", "")
				.Replace("DateTime.Now", "LOCALTIMESTAMP()")
				.Replace("DateTime.UtcNow", "UTC_Timestamp()");

			return expression;
		}

		private void ValidateWhereExpression<TRow>(LambdaExpression whereExpression, string parameterName)
			where TRow : class
		{
			if (whereExpression.ReturnType != typeof(bool))
			{
				throw new ArgumentException($"Return type of '{parameterName}' expected to be 'bool'.", parameterName);
			}

			if (!whereExpression.Parameters.Any())
			{
				throw new ArgumentException($"'{parameterName}' to have at least one parameter (the '{nameof(TRow)}', '{typeof(TRow).Name}').", parameterName);
			}

			var entityParameterCount = whereExpression.Parameters.Count(p => p.Type == typeof(TRow));
			if (entityParameterCount != 1)
			{
				throw new ArgumentException($"Exactly one '{nameof(TRow)}' ('{typeof(TRow).Name}') parameter expected in the '{parameterName}' (got {entityParameterCount}).", parameterName);
			}
		}

		private string ParseOrderBy<TRow>(OrderBy<TRow> orderBy, IDictionary<string, string> entityColumnAliases)
			where TRow : class
		{
			if (orderBy == null)
			{
				return null;
			}

			var sort = orderBy.SortOrder == SortOrder.Ascending ? "ASC" : "DESC";
			return $"`{entityColumnAliases[orderBy.Property.Name]}` {sort}";
		}

		/// <example>
		/// TODO
		/// </example>
		private string ReplaceEntityColumnAliases(string expression, string entityName, IDictionary<string, string> entityColumnAliases)
		{
			return entityColumnAliases.Aggregate(expression, (current, alias) => current.Replace($"{entityName}.{alias.Key}", $"`{alias.Value}`"));
		}

		/// <example>
		/// TODO
		/// </example>
		private string ReplaceParameterNames(string expression, ICollection<string> parameterNames)
		{
			if (parameterNames.Contains(_CountParameterName, StringComparer.OrdinalIgnoreCase))
			{
				throw new ApplicationException($"The query parameter name '@{_CountParameterName}' is reserved.");
			}

			if (parameterNames.Contains(_ExclusiveStartParameterName, StringComparer.OrdinalIgnoreCase))
			{
				throw new ApplicationException($"The query parameter name '@{_ExclusiveStartParameterName}' is reserved.");
			}

			if (parameterNames.Contains(_IsAscendingParameterName, StringComparer.OrdinalIgnoreCase))
			{
				throw new ApplicationException($"The query parameter name '@{_IsAscendingParameterName}' is reserved.");
			}

			foreach (var parameterName in parameterNames)
			{
				var replaceRegex = new Regex($"[^`]?({parameterName})[^`]?");
				expression = replaceRegex.Replace(expression, match =>
				{
					var chars = match.Value.ToCharArray();
					return chars.First() + $"@{parameterName}" + chars.Last();
				});
			}

			return expression;
		}

		/// <example>
		/// - null != hello -> hello != null
		/// - null == hello -> hello == null
		/// </example>
		private string NullSwapReplacement(Match expressionMatch)
		{
			return $"{expressionMatch.Groups[2]} {expressionMatch.Groups[1]} null";
		}

		/// <example>
		/// - hello.StartsWith(world) -> hello LIKE CONCAT(world, "%")
		/// </example>
		private string StartsWithReplacement(Match expressionMatch)
		{
			var comparer = expressionMatch.Groups[3].ToString();
			if (comparer.Contains("IgnoreCase"))
			{
				return $"LOWER({expressionMatch.Groups[1]}) LIKE CONCAT(LOWER({expressionMatch.Groups[2]}), '%')";
			}

			return $"{expressionMatch.Groups[1]} LIKE CONCAT({expressionMatch.Groups[2]}, '%')";
		}

		/// <example>
		/// - hello.EndsWith(world) -> hello LIKE CONCAT("%", world)
		/// </example>
		private string EndsWithReplacement(Match expressionMatch)
		{
			var comparer = expressionMatch.Groups[3].ToString();
			if (comparer.Contains("IgnoreCase"))
			{
				return $"LOWER({expressionMatch.Groups[1]}) LIKE CONCAT('%', LOWER({expressionMatch.Groups[2]}))";
			}

			return $"{expressionMatch.Groups[1]} LIKE CONCAT('%', {expressionMatch.Groups[2]})";
		}

		/// <example>
		/// - hello.Contains(world) -> hello LIKE CONCAT("%", world, "%")
		/// </example>
		private string ContainsReplacement(Match expressionMatch)
		{
			var comparer = expressionMatch.Groups[3].ToString();
			if (comparer.Contains("IgnoreCase"))
			{
				return $"LOWER({expressionMatch.Groups[1]}) LIKE CONCAT('%', LOWER({expressionMatch.Groups[2]}), '%')";
			}

			return $"{expressionMatch.Groups[1]} LIKE CONCAT('%', {expressionMatch.Groups[2]}, '%')";
		}

		/// <example>
		/// - hello.ToUpper() -> UPPER(hello)
		/// </example>
		private string CaseInsensitiveReplacement(Match expressionMatch)
		{
			return $"{expressionMatch.Groups[2].ToString().ToUpper()}({expressionMatch.Groups[1]})";
		}

		/// <example>
		/// - hello.Equals(world) -> hello = world
		/// - hello.Equals(world, IgnoreCase) -> LOWER(hello) = LOWER(world)
		/// </example>
		private string EqualReplacement(Match expressionMatch)
		{
			var comparer = expressionMatch.Groups[3].ToString();
			if (comparer.Contains("IgnoreCase"))
			{
				return $"LOWER({expressionMatch.Groups[1]}) = LOWER({expressionMatch.Groups[2]})";
			}

			return $"{expressionMatch.Groups[1]} = {expressionMatch.Groups[2]}";
		}

		private string CompileTemplate<T>(QueryTemplateVariables templateVariables)
			where T : new()
		{
			var sessionProperty = typeof(T).GetProperty("Session");
			var initializeMethod = typeof(T).GetMethod("Initialize");
			var compileMethod = typeof(T).GetMethod("TransformText");

			var template = new T();

			sessionProperty.SetValue(template, new Dictionary<string, object>
			{
				{ "Vars", templateVariables }
			});

			initializeMethod.Invoke(template, Array.Empty<object>());

			var compiledTemplate = (string)compileMethod.Invoke(template, Array.Empty<object>());
			return compiledTemplate.Trim();
		}

		private SqlQueryParameter TranslateParameter(ParameterExpression parameter)
		{
			var mySqlType = _DatabaseTypeParser.GetMySqlType(parameter.Type);
			var databaseTypeName = _DatabaseTypeParser.GetDatabaseTypeName(mySqlType);
			return new SqlQueryParameter(parameter.Name, databaseTypeName, length: null, parameterDirection: ParameterDirection.Input);
		}
	}
}
