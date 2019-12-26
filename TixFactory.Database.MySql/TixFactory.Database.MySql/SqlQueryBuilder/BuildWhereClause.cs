using System;
using System.Linq.Expressions;

namespace TixFactory.Database.MySql
{
	/// <inheritdoc cref="ISqlQueryBuilder"/>
	public partial class SqlQueryBuilder
	{
		/// <inheritdoc cref="ISqlQueryBuilder.BuildWhereClause{TRow}"/>
		public LambdaExpression BuildWhereClause<TRow>(Expression<Func<TRow, bool>> expression)
			where TRow : class
		{
			return expression;
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildWhereClause{TRow,TP1}"/>
		public LambdaExpression BuildWhereClause<TRow, TP1>(Expression<Func<TRow, TP1, bool>> expression)
			where TRow : class
		{
			return expression;
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildWhereClause{TRow,TP1,TP2}"/>
		public LambdaExpression BuildWhereClause<TRow, TP1, TP2>(Expression<Func<TRow, TP1, TP2, bool>> expression)
			where TRow : class
		{
			return expression;
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildWhereClause{TRow,TP1,TP2,TP3}"/>
		public LambdaExpression BuildWhereClause<TRow, TP1, TP2, TP3>(Expression<Func<TRow, TP1, TP2, TP3, bool>> expression)
			where TRow : class
		{
			return expression;
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildWhereClause{TRow,TP1,TP2,TP3,TP4}"/>
		public LambdaExpression BuildWhereClause<TRow, TP1, TP2, TP3, TP4>(Expression<Func<TRow, TP1, TP2, TP3, TP4, bool>> expression) 
			where TRow : class
		{
			return expression;
		}

		/// <inheritdoc cref="ISqlQueryBuilder.BuildWhereClause{TRow,TP1,TP2,TP3,TP4,TP5}"/>
		public LambdaExpression BuildWhereClause<TRow, TP1, TP2, TP3, TP4, TP5>(Expression<Func<TRow, TP1, TP2, TP3, TP4, TP5, bool>> expression) 
			where TRow : class
		{
			return expression;
		}
	}
}
