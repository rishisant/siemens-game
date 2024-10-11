using UnityEngine;

public class CosmeticHandler : MonoBehaviour
{
	// List of chest prefabs that have different appearances
	[SerializeField] private RuntimeAnimatorController[] chestAnimatorControllers;
	[SerializeField] private RuntimeAnimatorController[] legAnimatorControllers;
	[SerializeField] private RuntimeAnimatorController[] shoeAnimatorControllers;
	[SerializeField] private RuntimeAnimatorController[] hatAnimatorControllers;

	// Function to get a chest prefab by index or name (depending on your selection criteria)
	public RuntimeAnimatorController GetChestController(int index)
	{
		if (index >= 0 && index < chestAnimatorControllers.Length)
		{
			return chestAnimatorControllers[index];
		}
		return null;
	}
	public RuntimeAnimatorController GetLegController(int index)
	{
		if (index >= 0 && index < legAnimatorControllers.Length)
		{
			return legAnimatorControllers[index];
		}
		return null;
	}


	public RuntimeAnimatorController GetShoeController(int index)
	{
		if (index >= 0 && index < shoeAnimatorControllers.Length)
		{
			return shoeAnimatorControllers[index];
		}
		return null;
	}


	public RuntimeAnimatorController GetHatController(int index)
	{
		if (index >= 0 && index < hatAnimatorControllers.Length)
		{
			return hatAnimatorControllers[index];
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
	public int LegAnimControllerLenght()
	{
		return legAnimatorControllers.Length;
	}


	public int ShoeControllerLenght()
	{
		return shoeAnimatorControllers.Length;
	}

	public int HatAnimControllerLenght()
	{
		return hatAnimatorControllers.Length;
	}

}
