namespace System.Runtime.CompilerServices
{
    // Shim para compiladores que soportan 'init' pero apuntan a marcos antiguos
    // (por ejemplo .NET Framework 4.7.1 / Unity). No tocar si su plataforma ya lo define.
    internal static class IsExternalInit { }
}