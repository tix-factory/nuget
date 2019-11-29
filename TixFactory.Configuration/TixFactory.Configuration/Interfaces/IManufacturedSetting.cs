namespace TixFactory.Configuration
{
	/// <summary>
	/// A manufactured setting is a setting which has a value that comes from a factory.
	/// </summary>
	/// <typeparam name="T">The setting value type.</typeparam>
	public interface IManufacturedSetting<T> : ISetting<T>
	{
		/// <summary>
		/// Checks the <see cref="ISetting{T}.Value"/> for the setting value from the factory.
		/// </summary>
		void Refresh();
	}
}
