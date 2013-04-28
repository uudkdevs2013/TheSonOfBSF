using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	
	[SerializeField]float fireRate;								// Weapon fire rate in seconds
	float endFireTime;											// Timer to fire rate
	[SerializeField]float reloadDelay;							// Reload time in seconds
	float endReloadTime;										// Timer to reloading
	[SerializeField]int clipSize;								// Weapon clip size
	[SerializeField]int clipTotal;								// The numbers of clips in a new Weapon
	int clipCurrent;											// How many bullets are in the current clip
	bool isFiring;								
	bool isReloading;
	
	void Update () {
		if (clipCurrent == 0)
			Reload();
	}
	
	// Fire the weapon
	public bool Fire() {
		// Update the weapon firing status
		if (Time.time > endFireTime)
			isFiring = false;
		// If the weapon is not in a delay between bullets or reloading, fire
		if (!isFiring && !isReloading) {
			clipCurrent--;
			isFiring = true;
			endFireTime = Time.time + fireRate;
			return true;
		}
		// If it is in a delay between bullets or reloading, don't fire
		else
			return false;
	}
	
	// Reloads the current weapon
	bool Reload () {
		// If there are clips left, reload
		if (clipTotal != 0) {
			if (!isReloading) {
				reloadDelay = Time.time + endReloadTime;
				isReloading = true;
			}
			else {
				if (Time.time > endReloadTime){
					clipCurrent = clipSize;
					clipTotal--;
					return true;
				}
			}
		}
		return false;
	}
}
