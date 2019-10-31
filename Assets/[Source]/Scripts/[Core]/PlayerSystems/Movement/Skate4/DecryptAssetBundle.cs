using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DecryptAssetBundle : MonoBehaviour
{
	public string targetAsset = "";

	private AssetBundle asset;

	public DecryptAssetBundle()
	{
	}

	private void LoadAsset(AssetBundle _asset)
	{
		string[] allScenePaths = _asset.GetAllScenePaths();
		for (int i = 0; i < (int)allScenePaths.Length; i++)
		{
			Debug.LogError(allScenePaths[i]);
		}
		SceneManager.LoadScene(Path.GetFileNameWithoutExtension(allScenePaths[0]), LoadSceneMode.Single);
	}

	private void Start()
	{
		string str = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "/SkaterXL/Maps");
		asset = AssetBundle.LoadFromFile(Path.Combine(str, targetAsset));
		if (asset == null)
		{
			Debug.LogError("Could Not Load AssetBundle");
			return;
		}
		LoadAsset(asset);
	}
}