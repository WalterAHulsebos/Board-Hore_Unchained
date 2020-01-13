using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using CommonGames.Utilities.Extensions;

public static class MultiSceneLoader
{
	public static void LoadMultiScene(MultiScene multiScene, LoadSceneMode mode = LoadSceneMode.Single)
		=> multiScene.sceneAssets.For(sceneInfo => SceneManager.LoadScene(sceneInfo.asset, mode));
}