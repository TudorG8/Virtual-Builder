using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ConnectorInformationEvent : UnityEvent<ConnectorInformation> {}

/// <summary>
/// Holds together an input and output
/// </summary>
public class ConnectorInformation {
	[SerializeField] ConnectorInput input; 
	[SerializeField] ConnectorOutput output;

	public ConnectorInput Input {
		get {
			return this.input;
		}
		set {
			input = value;
		}
	}

	public ConnectorOutput Output {
		get {
			return this.output;
		}
		set {
			output = value;
		}
	}

	public ConnectorInformation (ConnectorInput input, ConnectorOutput output){
		this.input = input;
		this.output = output;
	}	
}
