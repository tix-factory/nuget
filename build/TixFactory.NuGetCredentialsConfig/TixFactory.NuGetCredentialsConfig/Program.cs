using System;
using System.IO;

namespace TixFactory.NuGetCredentialsConfig
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var configFile = args[0];
			var githubToken = args[1];

			var fileName = Path.GetFileName(configFile);
			if(!File.Exists(configFile) || fileName != "nuget.config")
			{
				Console.WriteLine("Invalid config file.");
				Environment.Exit(1);
				return;
			}

			var fileText = File.ReadAllText(configFile);
			fileText = fileText.Replace("GITHUB_TOKEN", githubToken);

			File.WriteAllText(configFile, fileText);
		}
	}
}
