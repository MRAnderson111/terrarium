using UnityEngine;

/// <summary>
/// 资源加载工具类，提供静态方法用于加载各种Unity资源
/// </summary>
public static class ResourceLoadUtils
{
    /// <summary>
    /// 根据名称从Resources文件夹加载预制体
    /// </summary>
    /// <param name="objectName">对象名称</param>
    /// <param name="resourcePath">资源路径前缀，默认为"Prefabs"</param>
    /// <returns>对应的预制体，如果找不到则返回null</returns>
    public static GameObject LoadPrefabByName(string objectName, string resourcePath = "Prefabs")
    {
        if (string.IsNullOrEmpty(objectName))
        {
            Debug.LogError("ResourceLoadUtils: 对象名称不能为空");
            return null;
        }
        
        string prefabPath = $"{resourcePath}/{objectName}";
        
        // 从Resources文件夹加载预制体
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"ResourceLoadUtils: 无法从路径 {prefabPath} 加载预制体");
        }
        
        return prefab;
    }
    
    /// <summary>
    /// 根据名称从Resources文件夹加载TextAsset
    /// </summary>
    /// <param name="fileName">文件名，不包含扩展名</param>
    /// <param name="resourcePath">资源路径前缀，默认为空</param>
    /// <returns>对应的TextAsset，如果找不到则返回null</returns>
    public static TextAsset LoadTextAssetByName(string fileName, string resourcePath = "")
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("ResourceLoadUtils: 文件名不能为空");
            return null;
        }
        
        string textAssetPath = string.IsNullOrEmpty(resourcePath) ? fileName : $"{resourcePath}/{fileName}";
        
        // 从Resources文件夹加载TextAsset
        TextAsset textAsset = Resources.Load<TextAsset>(textAssetPath);
        if (textAsset == null)
        {
            Debug.LogWarning($"ResourceLoadUtils: 无法从路径 {textAssetPath} 加载TextAsset");
        }
        
        return textAsset;
    }
}