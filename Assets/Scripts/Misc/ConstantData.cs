using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Constants")]
public class ConstantData : ScriptableObject {
	public LayerMask groundLayers;

	public string mainMenuScene;
}
