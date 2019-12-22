// FROM https://blog.rsuter.com/use-t4-texttemplatingfilepreprocessor-in-net-standard-or-pcl-libraries/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TixFactory.Database.MySql.Templates
{
	internal static class T4Extensions
	{
		public static MethodInfo GetMethod(this Type type, string method, params Type[] parameters)
		{
			return type.GetRuntimeMethod(method, parameters);
		}
	}
}

namespace System.CodeDom.Compiler
{
	internal class CompilerErrorCollection : List<CompilerError>
	{
		public bool HasErrors { get; set; }
	}

	internal class CompilerError
	{
		public string ErrorText { get; set; }

		public bool IsWarning { get; set; }
	}
}

namespace System.Runtime.Remoting.Messaging
{
	internal class CallContext
	{
		public static object LogicalGetData(string variableName)
		{
			return null;
		}
	}
}