using System.IO;
using UnityEngine;

public static class PathUtils {
	private static string basePath;
	private static bool initialized = false;

	public static void init() {
		if (!initialized)
			basePath = Path.GetDirectoryName(Application.dataPath);
		else
			Debug.LogWarning("PathUtils already initialized");
	}

	public static string mockDataPath(string path)
	{
		return Path.Combine(assetPath("mockData"), path);
	}

	public static string assetPath(string path)
	{
		return Path.Combine(relativePath("Assets"), path);
	}

	public static string relativePath(string path)
	{
		return Path.Combine(basePath, path);
	}
}