using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Taken from: https://answers.unity.com/questions/460727/how-to-serialize-dictionary-with-unity-serializati.html
/// Answer given by: https://answers.unity.com/users/456555/christophfranke123.html
/// Link to answer: http://answers.unity.com/answers/809221/view.html
/// 
/// Provides a serializable dictionary that allows values to be saved between scenes and editor mode.
/// </summary>
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver where TKey : new() where TValue : new(){
	[SerializeField]
	private List<TKey> keys = new List<TKey>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	public void OnBeforeSerialize() {
		keys.Clear();
		values.Clear();
		foreach(KeyValuePair<TKey, TValue> pair in this) {
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}
		
	public void OnAfterDeserialize() {
		this.Clear();

		for(int i = 0; i < keys.Count; i++)
			this.Add(keys[i], values[i]);
	}
}