using UnityEngine;
using System.Collections;

public class Done_DestroyByBoundary : MonoBehaviour
{
	void OnTriggerExit (Collider other) 
	{
		if(other.gameObject.tag != "Player") //non-player objects are destroyed when they exit the boundary
		Destroy(other.gameObject);
	}
}