namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Executes JavaScript in the browser with basic typing helpers.
    /// </summary>
    public interface IScriptExecutor
    {
        /// <summary>
        /// Executes an arbitrary JavaScript snippet in the context of the current page,  
        /// returning the raw result object as provided by the browser (driver-specific format).
        /// </summary>
        object Execute(string script, CancellationToken ct = default, params object[] args);

        /// <summary>
        /// Executes an arbitrary JavaScript snippet in the context of the current page,  
        /// attempting to convert the returned value to <typeparamref name="T"/> when possible.
        /// </summary>
        T Execute<T>(string script, CancellationToken ct = default, params object[] args);
    }
}
