namespace TixFactory.Configuration
{
    /// <summary>
    /// Represents a setting that can be changed.
    /// </summary>
    /// <typeparam name="T">The setting type.</typeparam>
    public interface ISetting<T> : IReadOnlySetting<T>
    {
        /// <summary>
        /// The setting value.
        /// </summary>
        new T Value { get; set; }
    }
}
