using Assets.Scripts;
using UnityEditor;

public class BuildBundler
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(UnityConstants.PathToAssetBundles, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}