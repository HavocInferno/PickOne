using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamStatusPanel : MonoBehaviour
{
    public GameObject elementPrefab;
    public float disappearSpeed = 400f;

    [Tooltip("The percentage of health at which the color palette switches.")]
    [Range(0.0f, 1.0f)]
    public float threshold = 0.5f;
    public Color fullHealthColor = Color.green;
    public Color thresholdHealthColor = Color.yellow;
    public Color zeroHealthColor = Color.red;
    public Sprite deadIcon;

    [SerializeField]
    private Dictionary<Stats, GameObject> _elements =
        new Dictionary<Stats, GameObject>();

    /// <summary>
    /// Used to add a player whose status will be tracked.
    /// </summary>
    /// <param name="player"></param>
    public void Register(Crawler player)
    {
        var playerStats = player.GetComponent<Stats>();

        GameObject newTracker = Instantiate(elementPrefab, transform);
        newTracker.GetComponent<Image>().color = _GetColourFromHealth(playerStats);
        
        // Set crawler icon
        Image icon = newTracker.transform.Find("PlayerIcon").GetComponent<Image>();
        icon.sprite = player.icon;

        if (icon.sprite != null)
            icon.color = new Color(0, 0, 0, 0.9f);
        else
            icon.color = new Color(0, 0, 0, 0);

        // Set player name
        var playerName = newTracker.GetComponentInChildren<Text>();
        if (playerName) playerName.text = player.pName;

        _elements.Add(playerStats, newTracker);
    }

    void Update ()
    {
        foreach (var playerStat in _elements.Keys)
        {
            // Update background colour depending on remaining health
            _elements[playerStat].GetComponent<Image>().color =
                _GetColourFromHealth(playerStat);

            // If the player has died then change the icon
            if (playerStat.gameObject.GetComponent<Crawler>().isDead && deadIcon)
            {
                _elements[playerStat].GetComponentInChildren<Image>().sprite = deadIcon;
            }
        }
    }

    Color _GetColourFromHealth(Stats stats)
    {
        var thresholdVal = stats.MaxHealth * threshold;

        if (stats.Health > thresholdVal)
        {
            return Color.Lerp(thresholdHealthColor, fullHealthColor,
                (stats.Health - thresholdVal) / thresholdVal);
        }
        else
        {
            return Color.Lerp(zeroHealthColor, thresholdHealthColor, stats.Health / thresholdVal);
        }
    }
}
