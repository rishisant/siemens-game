using System.Collections.Generic;
using UnityEngine;

public class CosmeticHandler : MonoBehaviour
{
	// List of chest prefabs that have different appearances
	[SerializeField] private RuntimeAnimatorController[] chestAnimatorControllers;

	// Function to get a chest prefab by index or name (depending on your selection criteria)
	public RuntimeAnimatorController GetChestController(int index)
	{
		if (index >= 0 && index < chestAnimatorControllers.Length)
		{
			return chestAnimatorControllers[index];
		}
		return null;
	}

	// Optionally, you can create a method that finds a prefab by name
	public RuntimeAnimatorController GetChestControllerByName(string name)
	{
		foreach (var prefab in chestAnimatorControllers)
		{
			if (prefab.name == name)
			{
				return prefab;
			}
		}
		return null;
	}

	public int ChestAnimControllerLenght()
	{
		return chestAnimatorControllers.Length;
	}
}
