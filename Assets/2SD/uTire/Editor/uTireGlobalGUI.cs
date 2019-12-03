namespace TSD
{
	namespace uTireEditor
	{
		using System.Collections;
		using System.Collections.Generic;
		using UnityEngine;
		using UnityEditor;

		/// <summary>
		/// Used for holding GUIStyles data
		/// </summary>
		public class uTireGlobalGUI
		{

			float big = 32;
			float bigger = 48;


			GUIStyle _toggleButtonStyleNormal;
			public GUIStyle ToggleButtonStyleNormal
			{
				get
				{
					if (_toggleButtonStyleNormal == null || true)
					{
						_toggleButtonStyleNormal = new GUIStyle("Button");
					}
					return _toggleButtonStyleNormal;
				}
			}
			GUIStyle _toggleButtonStyleToggled;
			public GUIStyle ToggleButtonStyleToggled
			{
				get
				{
					if (_toggleButtonStyleToggled == null || true)
					{
						_toggleButtonStyleToggled = new GUIStyle("Button");
						_toggleButtonStyleToggled.normal.background = _toggleButtonStyleToggled.active.background;
					}
					return _toggleButtonStyleToggled;
				}
			}


			GUIStyle _toggleBiggerButtonStyleNormal;
			public GUIStyle ToggleBiggerButtonStyleNormal
			{
				get
				{
					if (_toggleBiggerButtonStyleNormal == null || true)
					{
						_toggleBiggerButtonStyleNormal = new GUIStyle("Button");
						_toggleBiggerButtonStyleNormal.fixedHeight = big;
					}
					return _toggleBiggerButtonStyleNormal;
				}
			}


			GUIStyle _toggleBiggerButtonStyleToggled;
			public GUIStyle ToggleBiggerButtonStyleToggled
			{
				get
				{
					if (_toggleBiggerButtonStyleToggled == null || true)
					{
						_toggleBiggerButtonStyleToggled = new GUIStyle("Button");
						_toggleBiggerButtonStyleToggled.normal.background = _toggleBiggerButtonStyleToggled.active.background;
						_toggleBiggerButtonStyleToggled.fixedHeight = big;
					}
					return _toggleBiggerButtonStyleToggled;
				}
			}

			GUIStyle _biggerBtn;
			public GUIStyle biggerBtn
			{
				get
				{
					if (_biggerBtn == null)
					{
						_biggerBtn = new GUIStyle("Button");
						_biggerBtn.fixedHeight = big;
					}

					return _biggerBtn;
				}
			}

			GUIStyle _hugeBtn;
			public GUIStyle hugeBtn
			{
				get
				{
					if(_hugeBtn == null)
					{
						_hugeBtn = new GUIStyle("Button");
						_hugeBtn.fixedHeight = bigger;
					}
					return _hugeBtn;
				}
			}

			GUIStyle _toolbar;
			public GUIStyle Toolbar
			{
				get
				{
					if (_toolbar == null)
					{
						_toolbar = new GUIStyle("Toolbar");
					}
					return _toolbar;
				}
			}

			GUIStyle _toolbarBig;
			public GUIStyle ToolbarBig
			{
				get
				{
					if (_toolbarBig == null)
					{
						_toolbarBig = new GUIStyle("Toolbar");
						_toolbarBig.fixedHeight = big;
						_toolbarBig.alignment = TextAnchor.MiddleCenter;
					}
					return _toolbarBig;
				}
			}

			GUIStyle _empty;
			public GUIStyle styleEmpty
			{
				get
				{
					if (_empty == null || true) { _empty = new GUIStyle(); }
					return _empty;
				}
			}

			///////////////////////////////
			/// ///
			///////////////////////////////

			public void drawCloseBtn(EditorWindow _windowToClose)
			{
				if (GUI.Button(new Rect(Screen.width - 30, 5, 25, 25), "X"))
				{
					_windowToClose.Close();
				}
			}

			public void drawLogo(Texture logo, Texture logoText = null)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(logo);//, GUILayout.MaxHeight(maxLogoSize.y));
				if (logoText != null)
				{
					GUI.Label(GUILayoutUtility.GetLastRect(), logoText);
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}

			/// <summary>
			/// Returns a toggled\normal GUI style based on the provided bool
			/// </summary>
			/// <param name="state"></param>
			/// <returns></returns>
			public GUIStyle ToggleStyle(bool state, buttonSize _btnSize = buttonSize.normal)
			{
				switch (_btnSize)
				{
					case buttonSize.normal:
						return state ? ToggleButtonStyleToggled : ToggleButtonStyleNormal; 
					case buttonSize.big:
						return state ? ToggleBiggerButtonStyleToggled : ToggleBiggerButtonStyleNormal;
					default:
						return state ? ToggleButtonStyleToggled : ToggleButtonStyleNormal; 
				}
			}


			public enum buttonSize
			{
				normal,
				big
			}

			///////////////////////////////
			/// ///
			///////////////////////////////
			static uTireGlobalGUI m_instance;

			public static uTireGlobalGUI Instance
			{
				get
				{
					if (m_instance == null)
					{
						m_instance = new uTireGlobalGUI();
					}
					return m_instance;
				}
				
			}
		}
	}
}