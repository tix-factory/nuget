﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TixFactory.Database.MySql.Templates;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery"/>
		public ISqlQuery BuildCountQuery(string databaseName, string tableName)
		{
			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause: null,
				expressionParameters: Array.Empty<ParameterExpression>());
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildCountQuery{TRow}"/>
		public ISqlQuery BuildCountQuery<TRow>(string databaseName, string tableName, LambdaExpression whereExpression)
			where TRow : class
		{
			ValidateWhereExpression<TRow>(whereExpression, nameof(whereExpression));

			var entityColumnAliases = GetEntityColumnAliases<TRow>();
			var whereClause = ParseWhereClause(whereExpression, entityColumnAliases);

			return BuildCountQuery(
				databaseName,
				tableName,
				whereClause,
				whereExpression.Parameters);
		}

		private ISqlQuery BuildCountQuery(string databaseName, string tableName, string whereClause, IReadOnlyCollection<ParameterExpression> expressionParameters)
		{
			var templateVariables = new CountQueryVariables
			{
				DatabaseName = databaseName,
				TableName = tableName,
				WhereClause = whereClause
			};

			var query = CompileTemplate<CountQuery>(templateVariables);
			var parameters = expressionParameters.Skip(1).Select(TranslateParameter).ToList();

			return new SqlQuery(query, parameters);
		}
	}
}
