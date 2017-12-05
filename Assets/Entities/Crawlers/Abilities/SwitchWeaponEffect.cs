using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Effects/SwitchWeaponEffect")]
public class SwitchWeaponEffect : AbstractEffect
{
    public string weaponName = "";

    public override void Enable(
        GenericCharacter character,
        bool calledByLocalPlayer,
        bool calledByServer)
    {
        base.Enable(character, calledByLocalPlayer, calledByServer);

        _SwitchWeaponEffect comp = character.gameObject.AddComponent<_SwitchWeaponEffect>();
        comp._Initialize(weaponName);
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
        BasicAttack oldWeapon = null;
        public void _Initialize(string weaponName)
        {
            var character = gameObject.GetComponent<GenericCharacter>();

            oldWeapon = character.basicAttack;
            oldWeapon.gameObject.SetActive(false);

            GameObject weaponInstance = transform.Find(weaponName).gameObject;
            weaponInstance.SetActive(true);
            character.basicAttack = weaponInstance.GetComponent<BasicAttack>();
        }

        private void OnDisable()
        {
            var character = gameObject.GetComponent<GenericCharacter>();

            GameObject weaponInstance = character.basicAttack.gameObject;
            weaponInstance.SetActive(false);

            oldWeapon.gameObject.SetActive(true);
            character.basicAttack = oldWeapon;
        }
    }
}
