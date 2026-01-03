using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the conversion of modifiers into stats.
/// </summary>
public class DataModifierConverter : Singleton<DataModifierConverter> {
	void Awake() {
		InitiateSingleton ();
	}

	/// <summary>
	/// Adds a list of modifiers to a stat if they have the same stat name.
	/// </summary>
	public void AddListToStat(FloatStat stat, List<Modifier> modifiers) {
		for (int i = 0; i < modifiers.Count; i++) {
			Modifier modifier = modifiers [i];

			if (stat.Name == modifier.Name) {
				stat.Modifiers.Add (modifier);
			}
		}

		stat.Recalculate ();
	}

	/// <summary>
	/// Adds a list of modifiers to a list of stats, with the option of clearing all modifiers from the stats.
	/// </summary>
	public void AddModifiersToStats(List<Modifier> modifiers, bool clearStats, List<FloatStat> stats) {
		if (clearStats) {
			for (int i = 0; i < stats.Count; i++) {
				stats [i].Modifiers.Clear ();
			}
		}

		for (int i = 0; i < stats.Count; i++) {
			AddListToStat (stats [i], modifiers);
		}
	}
}
