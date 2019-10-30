using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonGames.Utilities;

using Sirenix.OdinInspector;

using JetBrains.Annotations;

#if Odin_Inspector
using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#endif

public class MainMenu : Singleton<MainMenu>
{
	#region Variables

	[SerializeField] private MultiScene multiScene = null;

	[FoldoutGroup("Credits")]
	[Sirenix.OdinInspector.ReadOnly]
	[SerializeField] private bool inCreditMenu = false;
	
	[FoldoutGroup("Credits")]
	[SerializeField] private Animator creditAnimator = null;
	
	[FoldoutGroup("Credits")]
	[SerializeField] private string creditEnableBoolName = "InMenu";
	
	#endregion

	#region Methods

	[UsedImplicitly]
	public void StartGame()
	{
		multiScene.Load();
	}

	[UsedImplicitly]
	public void ExitGame()
	{
		Application.Quit();
	}
	
	public void AnimateCredits()
	{
		inCreditMenu = !inCreditMenu;
        
		creditAnimator.SetBool(creditEnableBoolName, inCreditMenu);
	}
	
	#endregion	
}
