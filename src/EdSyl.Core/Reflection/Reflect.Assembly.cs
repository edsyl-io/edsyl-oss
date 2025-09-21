namespace EdSyl.Reflection;

public static partial class Reflect
{
    /// <summary> Find a loaded assembly by the name. </summary>
    /// <param name="domain">Application domain with loaded assemblies.</param>
    /// <param name="assemblyName">Assembly to search for.</param>
    /// <returns>Assembly instance on success; null if not found.</returns>
    public static Assembly? GetAssemblyByName(this AppDomain domain, string assemblyName)
        => Array.Find(domain.GetAssemblies(), t => t.GetName().Name == assemblyName);
}
