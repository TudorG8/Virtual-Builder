using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input awaits for a connection signal from an <see cref="ConnectorOutput">Output</see>.
/// </summary>
[System.Serializable]
public class ConnectorInput : MonoBehaviour {
    [Header("Object References")]
    [Tooltip("Reference to the main component this output belongs to.")]
	[SerializeField] ItemComponent component;
	
	[Header("Read only")]
	[Tooltip("Whether this component has been connected to an output yet.")]
	[SerializeField][ReadOnly] bool connected;
	[Tooltip("The output currently connected to this element.")]
	[SerializeField] ConnectorOutput connectedTo;

	public bool Connected {
		get {
			return this.connected;
		}
		set {
			connected = value;
		}
	}

	public ItemComponent Component {
		get { 
		    return component; 
		}
	}

	public ConnectorOutput ConnectedTo {
		get {
			return this.connectedTo;
		}
		set {
			connectedTo = value;
		}
	}
}
