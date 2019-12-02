namespace TSD
{
	namespace uTireRuntime
	{
		using UnityEngine;

		public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
		{
			static T m_instance;
			public static T Instance
			{
				get
				{
					if (m_instance == null)
					{
						m_instance = (T)FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[Singleton] Something went really wrong " +
								" - there should never be more than 1 singleton!" +
								" Reopening the scene might fix it.");
							return m_instance;
						}

						if (m_instance == null)
						{
							GameObject singleton = new GameObject();
							m_instance = singleton.AddComponent<T>();
							singleton.name = "(singleton) " + typeof(T).ToString();


							/*Debug.Log("[Singleton] An instance of " + typeof(T) +
								" is needed in the scene, so '" + singleton +
								"' was created."); */
						}
					}

					return m_instance;
				}
			}

			protected void Awake()
			{
				if (m_instance == null) m_instance = this as T;
				else if (m_instance != this) Destroy(this);
			}
		}
	}
}