namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Executes JavaScript in the browser with basic typing helpers.
    /// </summary>
    public interface IScriptExecutor
    {
        /// <summary>Executes arbitrary script, returning a raw object (driver-specific).</summary>
        object? Execute(string script, CancellationToken ct = default, params object[] args);

        /// <summary>Executes arbitrary script and converts the result to <typeparamref name="T"/> when possible.</summary>
        T? Execute<T>(string script, CancellationToken ct = default, params object[] args);
    }
}
