using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides calculation methods for stats.
/// </summary>
public class CalculationMethods : MonoBehaviour {
	public delegate float Method(float startingBase, List<Modifier>  modifiers);

	/// <summary>
	/// Base method for stats. The formula is the following:
	/// (startingBase + baseTotal) * additiveTotal * moreTotal;
	/// moreTotal are multiplied against each other, while additive are just added up.
	/// </summary>
	/// <returns>The total of the calculation.</returns>
	public static Method SimpleMethod() {
		return (startingBase, modifiers) => {
			float baseTotal     = 0;
			float additiveTotal = 1;
			float moreTotal     = 1;

			foreach(Modifier mod in modifiers) {
				switch (mod.Type) {
				case ModifierType.Base:
					baseTotal += mod.Amount;
					break;
				case ModifierType.Increased:
					additiveTotal += mod.Amount / 100f;
					break;
				case ModifierType.More:
					moreTotal *= (1 + mod.Amount / 100f);
					break;
				}
			}

			baseTotal     = Mathf.Clamp(baseTotal, 0, float.MaxValue);
			additiveTotal = Mathf.Clamp(additiveTotal, 0.1f, float.MaxValue);
			moreTotal     = Mathf.Clamp(moreTotal, 0.1f, float.MaxValue);

			return (startingBase + baseTotal) * additiveTotal * moreTotal;
		};
	}
}
