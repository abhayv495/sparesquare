#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Prevents the legacy Parse compatibility assembly from being referenced by Editor assemblies.
/// Unity 6 provides these framework types itself, so the duplicate DLL breaks editor compilation.
/// </summary>
public sealed class UnityCompatEditorExclusion : AssetPostprocessor
{
    private const string CompatibilityAssembly = "Assets/Parse/Plugins/Unity.Compat.dll";

    private void OnPreprocessAsset()
    {
        if (assetPath != CompatibilityAssembly)
        {
            return;
        }

        var importer = (PluginImporter)assetImporter;
        importer.SetCompatibleWithEditor(false);
    }
}
#endif
