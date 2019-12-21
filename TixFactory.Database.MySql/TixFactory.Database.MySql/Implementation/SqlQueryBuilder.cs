﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using TixFactory.Configuration;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public class SqlQueryBuilder : ISqlQueryBuilder
	{
		private const string _CountParameterName = "Count";
		private readonly Regex _ExpressionCutRegex = new Regex(@"^.*=>\s*");
		private readonly Regex _CaseInsensitiveReplacementRegex = new Regex(@"(`\w+`|@\w+)\.To(Lower|Upper)\(\)");
		private readonly Regex _EqualReplacementRegex = new Regex(@"(`\w+`|@\w+)\.Equals\((`\w+`|@\w+),?\s*(\w*)\)");
		private readonly Regex _ContainsReplacementRegex = new Regex(@"(`\w+`|@\w+)\.Contains\((`\w+`|@\w+)\)");
		private readonly Regex _StartsWithReplacementRegex = new Regex(@"(`\w+`|@\w+)\.StartsWith\((`\w+`|@\w+)\)");
		private readonly Regex _EndsWithReplacementRegex = new Regex(@"(`\w+`|@\w+)\.EndsWith\((`\w+`|@\w+)\)");
		private readonly Regex _NullSwapRegex = new Regex(@"null\s*([!=]=)\s*(`\w+`|@\w+)");

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow}(string,string,OrderBy{TRow})"/>
		public string BuildSelectTopQuery<TRow>(string databaseName, string tableName, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			return BuildSelectAllQuery(
				databaseName, 
				tableName,
				whereClause: null,
				orderByStatement: ParseOrderBy(orderBy, entityColumnAliases));
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow}(string,string,System.Linq.Expressions.Expression{System.Func{TRow,bool}},OrderBy{TRow})"/>
		public string BuildSelectTopQuery<TRow>(string databaseName, string tableName, Expression<Func<TRow, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause<TRow>(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1}"/>
		public string BuildSelectTopQuery<TRow, TP1>(string databaseName, string tableName, Expression<Func<TRow, TP1, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause<TRow>(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2}"/>
		public string BuildSelectTopQuery<TRow, TP1, TP2>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause<TRow>(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3}"/>
		public string BuildSelectTopQuery<TRow, TP1, TP2, TP3>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause<TRow>(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3,TP4}"/>
		public string BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause<TRow>(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement);
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildSelectTopQuery{TRow,TP1,TP2,TP3,TP4,TP5}"/>
		public string BuildSelectTopQuery<TRow, TP1, TP2, TP3, TP4, TP5>(string databaseName, string tableName, Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> whereExpression, OrderBy<TRow> orderBy = null)
			where TRow : class
		{
			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause<TRow>(whereExpression, whereExpression.Parameters, entityColumnAliases);
			var orderByStatement = ParseOrderBy(orderBy, entityColumnAliases);

			return BuildSelectAllQuery(
				databaseName,
				tableName,
				whereClause,
				orderByStatement);
		}

		private string BuildSelectAllQuery(string databaseName, string tableName, string whereClause, string orderByStatement)
		{
			var query = $"SELECT *\n\tFROM `{databaseName}`.`{tableName}`";

			if (!string.IsNullOrWhiteSpace(whereClause))
			{
				query += $"\n\tWHERE {whereClause}";
			}

			if (!string.IsNullOrWhiteSpace(orderByStatement))
			{
				query += $"\n\tORDER BY {orderByStatement}";
			}

			query += $"\n\tLIMIT @{_CountParameterName}";

			return query;
		}

		private string ParseOrderBy<TRow>(OrderBy<TRow> orderBy, IDictionary<string, string> entityColumnAliases)
			where TRow : class
		{
			if (orderBy == null)
			{
				return null;
			}

			var sort = orderBy.SortOrder == SortOrder.Ascending ? "ASC" : "DESC";
			return $"`{entityColumnAliases[orderBy.PropertyName]}` {sort}";
		}

		private string ParseWhereClause<TRow>(Expression queryExpression, IReadOnlyCollection<ParameterExpression> expressionParameters, IDictionary<string, string> entityColumnAliases)
			where TRow : class
		{
			// TODO: Regex example
			var expression = _ExpressionCutRegex.Replace(queryExpression.ToString(), "");

			var entityName = expressionParameters.First().Name;
			var parameterNames = expressionParameters.Skip(1).Select(p => p.Name).ToArray();

			return ParseWhereClause(expression, entityName, parameterNames, entityColumnAliases);
		}

		private string ParseWhereClause(string expression, string entityName, ICollection<string> parameterNames, IDictionary<string, string> entityColumnAliases)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException($"{nameof(expression)} must not be null or whitespace.", nameof(expression));
			}

			if (string.IsNullOrWhiteSpace(entityName))
			{
				throw new ArgumentException($"{nameof(entityName)} must not be null or whitespace.", nameof(entityName));
			}

			if (entityColumnAliases == null)
			{
				throw new ArgumentNullException(nameof(entityColumnAliases));
			}

			if (expression.Contains("\"") || expression.Contains("'") || expression.Contains("`"))
			{
				throw new ArgumentException($"Restricted character found in {nameof(expression)}", nameof(expression));
			}

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
			return $"{expressionMatch.Groups[1]} LIKE CONCAT({expressionMatch.Groups[2]}, \"%\")";
		}

		/// <example>
		/// - hello.EndsWith(world) -> hello LIKE CONCAT("%", world)
		/// </example>
		private string EndsWithReplacement(Match expressionMatch)
		{
			return $"{expressionMatch.Groups[1]} LIKE CONCAT(\"%\", {expressionMatch.Groups[2]})";
		}

		/// <example>
		/// - hello.Contains(world) -> hello LIKE CONCAT("%", world, "%")
		/// </example>
		private string ContainsReplacement(Match expressionMatch)
		{
			return $"{expressionMatch.Groups[1]} LIKE CONCAT(\"%\", {expressionMatch.Groups[2]}, \"%\")";
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

		/// <summary>
		/// Parses the entity column aliases from a <typeparamref name="TRow"/>.
		/// </summary>
		/// <typeparam name="TRow">The model class representing the row of a the table the query is being built for.</typeparam>
		/// <remarks>
		/// An entity column alias dictionary is the mapping a model property name -> table column name.
		/// Represented by the <see cref="DataMemberAttribute"/> on the property if the property name doesn't match the table column name.
		/// </remarks>
		/// <returns>The dictionary mapping property name -> table column name.</returns>
		private IDictionary<string, string> GetEntityColumnAliases<TRow>()
			where TRow : class
		{
			var aliases = new Dictionary<string, string>();
			var rowProperties = typeof(TRow).GetProperties();

			foreach (var property in rowProperties)
			{
				var dataMember = property.GetCustomAttribute<DataMemberAttribute>();
				if (string.IsNullOrWhiteSpace(dataMember?.Name))
				{
					aliases[property.Name] = property.Name;
				}
				else
				{
					aliases[property.Name] = dataMember.Name;
				}
			}

			return aliases;
		}
	}
}
