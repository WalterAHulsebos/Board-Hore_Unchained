namespace Utilities.CGTK
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
	public abstract class PrefAttribute : Attribute
	{
		public virtual void Save()
		{
			
		}

		public virtual void Load()
		{
			
		}
	}
}
