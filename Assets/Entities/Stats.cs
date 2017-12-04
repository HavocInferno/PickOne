using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Stats that belong to any GenericCharacter.
/// </summary>
[RequireComponent(typeof(GenericCharacter))]
public class Stats : NetworkBehaviour
{
    /// <summary>
    /// Attribute that has max value and current value, such as Health, Energy, e.t.c.
    /// To each of the attributes, its corresponding UI visualisation can be assigned.
    /// 
    /// Note, that for simplicity attributes always contain Health,
    /// and shortcut properties are provided for it.
    /// </summary>
    [System.Serializable]
    public class Attribute
    {
        [SerializeField]
        float _max = 100.0f;

        float _value = 100.0f;

        public string name = "";
        public RectTransform bar;

        public float Value
        {
            get { return _value; }
            set { _value = value; _Update(); }
        }

        public float Max
        {
            get { return _max; }
            set { _max = value; _Update(); }
        }

        protected void _Update()
        {
            _value = Mathf.Min(_value, _max);
            if (bar) bar.sizeDelta = new Vector2(100.0f * _value / _max, bar.sizeDelta.y);
        }
    }
    public Dictionary<string, Attribute> attributes = new Dictionary<string, Attribute>();
    [SerializeField]
    private List<Attribute> _attributesList = new List<Attribute>();

    public float attackMultiplier = 1.0f;
    public float defenseMultiplier = 1.0f;

    public bool destroyOnDeath;
    public GameObject deathEffect;

    private GenericCharacter _character;

    private void Reset()
    {
        _attributesList.Add(new Attribute
        {
            name = "Health"
        });
    }

    //
    // Getters and setters
    //

    public float Health
    {
        get { return GetAttributeValue("Health"); }
        set { SetAttributeValue("Health", value); }
    }

    public float MaxHealth
    {
        get { return GetAttributeMax("Health"); }
        set { SetAttributeMax("Health", value); }
    }

    public void SetAttributeValue(string name, float value)
    {
        Debug.Assert(isServer, "Only server can change character stats!");
        if (isServer)
        {
            attributes[name].Value = value;
            RpcSetAttributeValue(name, value);
        }
    }

    public float GetAttributeValue(string name)
    {
        return attributes[name].Value;
    }

    [ClientRpc]
    void RpcSetAttributeValue(string name, float value)
    {
        if (!isServer)
            attributes[name].Value = value;
    }

    void SetAttributeMax(string name, float max)
    {
        Debug.Assert(isServer, "Only server can change character stats!");
        if (isServer)
            RpcSetAttributeMax(name, max);
    }

    public float GetAttributeMax(string name)
    {
        return attributes[name].Value;
    }

    [ClientRpc]
    void RpcSetAttributeMax(string name, float value)
    {
        attributes[name].Max = value;
    }

    public bool HasAttribute(string name)
    {
        return attributes.ContainsKey(name);
    }

    //
    // Logic
    //

    public override void OnStartServer()
    {
        _character = gameObject.GetComponent<GenericCharacter>();
        foreach (var attribute in _attributesList)
        {
            attributes.Add(attribute.name, attribute);
            attribute.Value = attribute.Max;
        }
    }

    // Damage handling, supposed to be entirely server-side for core data.
    public void Hit(
        float baseAmount,
        GenericCharacter attacker,
        Vector3 hitPoint,
        Vector3 hitDirection)
    {
        Stats attackerStats = attacker.GetComponent<Stats>();
        float amount = baseAmount * defenseMultiplier;
        if (attackerStats != null) amount *= attackerStats.attackMultiplier;

        if (isServer)
            Health -= amount;
        _character.OnReceiveDamage(amount, attacker, hitPoint, hitDirection);
        attacker.OnMakeDamage(amount, _character, hitPoint, hitDirection);

        if (!isServer) return;

        if (Health <= 0.0f)
        {
            gameObject.GetComponent<GenericCharacter>().isDead = true;

            if (deathEffect != null)
            {
                var ded = Instantiate(
                    deathEffect,
                    hitPoint,
                    Quaternion.LookRotation(hitDirection));

                NetworkServer.Spawn(ded);

                // Destroy the effect after 2.15 seconds
                Destroy(ded, 2.15f);
            }
        }
    }

    [ClientRpc]
    void RpcDie()
    {
        gameObject.GetComponentInChildren<Crawler>().isDead = true;
    }
}