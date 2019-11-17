namespace FileCabinetApp.Configuration
{
    /// <summary>
    /// Work ConfigurationFilecabinet.
    /// </summary>
    public class ConfigurationFilecabinet
    {
        /// <summary>
        /// Gets or sets the Default validator.
        /// </summary>
        /// <value>The Default validator.</value>
        public ValidationConfiguration Default { get; set; }

        /// <summary>
        /// Gets or sets the Custom validator.
        /// </summary>
        /// <value>The Custom validator.</value>
        public ValidationConfiguration Custom { get; set; }
    }
}
