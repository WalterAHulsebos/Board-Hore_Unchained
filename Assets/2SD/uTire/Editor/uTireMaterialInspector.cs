namespace TSD.uTireEditor
{
	using UnityEditor;

	public class uTireMaterialInspector : ShaderGUI
	{

		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			EditorGUILayout.HelpBox("Use the editor window to change the material properties! Why? Because a ton of magic happening on the CPU side when you set up the material and due its complexity an EditorWindow makes much more sense. If you change a variable you would have to change 3 others as well to get the desired result, brr. It's better this way, I promise.", MessageType.Info);
		}
	}
}