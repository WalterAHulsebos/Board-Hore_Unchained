//      
//   ^\.-
//  c====ɔ   Crafted with <3 by Nate Tessman
//   L__J    nate@madgvox.com
// 
//				 Edited with <3 by Walter Haynes
//

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.Extensions;
using UnityEngine.SceneManagement;
    
#if ODIN_INSPECTOR
    
using ScriptableObject = Sirenix.OdinInspector.SerializedScriptableObject;
    
#endif

using Utilities.CGTK;

public class MultiScene : ScriptableObject
{
	[Serializable]
	public struct SceneInfo
	{
		public SceneReference asset;
		public bool loadScene;

		public SceneInfo(SceneReference asset, bool loadScene = true)
		{
			this.asset = asset;
			this.loadScene = loadScene;
		}
	}
	
	public SceneReference activeScene;
	public List<SceneInfo> sceneAssets = new List<SceneInfo>();
	
	public void Load()//(LoadSceneMode mode = LoadSceneMode.Additive)
	{
		//sceneAssets.For(sceneInfo => SceneManager.LoadScene(sceneInfo.asset, LoadSceneMode.Additive));

		for (int index = 0; index < sceneAssets.Count; index++)
		{
			SceneInfo sceneInfo = sceneAssets[index];
			
			SceneManager.LoadSceneAsync(sceneInfo.asset, (index == 0)? LoadSceneMode.Single : LoadSceneMode.Additive);
		}
	}
}