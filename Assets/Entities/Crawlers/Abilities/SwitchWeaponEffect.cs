using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/SwitchWeaponEffect")]
public class SwitchWeaponEffect : AbstractEffect
{
    public string weaponName = "";
	public float switchtime = .6f;

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Enable(character, calledByLocalPlayer, calledByServer);

        _SwitchWeaponEffect comp = character.gameObject.AddComponent<_SwitchWeaponEffect>();
		comp._Initialize(weaponName, switchtime);
    }

    public override void Disable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Disable(character, calledByLocalPlayer, calledByServer);

        Destroy(character.gameObject.GetComponent<_SwitchWeaponEffect>());
    }

    [RequireComponent(typeof(GenericCharacter))]
    private class _SwitchWeaponEffect : MonoBehaviour
    {
		float rand = 5;
		float ejectionSpeed = 7;
        BasicAttack oldWeapon = null;
		float switchtimee; 
		public void _Initialize(string weaponName, float _switchtime)
        {
            var character = gameObject.GetComponent<GenericCharacter>();
			switchtimee = _switchtime;
			var animator = GetComponentInChildren<Animator> ();
			if (animator != null)
				animator.SetTrigger ("Draw");

			var animatorSync = GetComponentInChildren<AnimatorSync> ();
			if (animatorSync != null) {
				animatorSync.leftHand = false;
				animatorSync.rightHand = false;
			}

			oldWeapon = character.basicAttack;
			oldWeapon.gameObject.SetActive(false);
			if (oldWeapon.dropWeaponPrefab != null) {
				var rig = Instantiate (oldWeapon.dropWeaponPrefab, oldWeapon.gameObject.transform.position, oldWeapon.gameObject.transform.rotation).GetComponent<Rigidbody> ();  
				transform.Rotate (0, 0, Random.Range (-rand, rand));
				if (rig != null) {
					rig.velocity = new Vector3 (Random.Range (-rand, rand), ejectionSpeed, Random.Range (-rand, rand));
					rig.angularVelocity = new Vector3 (ejectionSpeed + Random.Range (-rand, rand), ejectionSpeed + Random.Range (-rand, rand), ejectionSpeed + Random.Range (-rand, rand));
				}
			}
			StartCoroutine (draw (character, weaponName));

        }

        private void OnDisable()
        {
            var character = gameObject.GetComponent<GenericCharacter>();

            GameObject weaponInstance = character.basicAttack.gameObject;

			var animatorSync = GetComponentInChildren<AnimatorSync> ();
			if (animatorSync != null) {
				animatorSync.leftHand = true;
				animatorSync.rightHand = true;
			}

            weaponInstance.SetActive(false);

			if (character.basicAttack.dropWeaponPrefab != null) {
				var rig = Instantiate (character.basicAttack.dropWeaponPrefab, character.basicAttack.gameObject.transform.position, character.basicAttack.gameObject.transform.rotation).GetComponent<Rigidbody> ();
				transform.Rotate (0,0,Random.Range(-rand, rand));
				if (rig != null) {
					rig.velocity = new Vector3 (Random.Range(-rand, rand),ejectionSpeed, Random.Range(-rand, rand));
					rig.angularVelocity = new Vector3 (ejectionSpeed+Random.Range(-rand, rand),ejectionSpeed+Random.Range(-rand, rand),ejectionSpeed+Random.Range(-rand, rand));
				}
			}

            oldWeapon.gameObject.SetActive(true);
            character.basicAttack = oldWeapon;
        }
		IEnumerator draw(GenericCharacter character, string weaponName)
		{
			yield return new WaitForSeconds (switchtimee);
			GameObject weaponInstance = transform.Find(weaponName).gameObject;
			weaponInstance.SetActive(true);
			character.basicAttack = weaponInstance.GetComponent<BasicAttack>();
		}
    }
}
