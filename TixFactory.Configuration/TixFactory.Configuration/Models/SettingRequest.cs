namespace TixFactory.Configuration
{
	/// <summary>
	/// A request for getting a raw setting value.
	/// </summary>
	public struct SettingRequest
	{
		/// <summary>
		/// The settings group.
		/// </summary>
		public string Group { get; internal set; }
		
		/// <summary>
		/// The setting name.
		/// </summary>
		public string Name { get; internal set; }
	}
}
