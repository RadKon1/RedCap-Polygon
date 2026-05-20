using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private PlayerStats _playerStats;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private List<Image> _ankhIcons;

    private void Update()
    {
        if (_playerStats != null && _coinText != null)
        {
            _coinText.text = _playerStats.NumberOfCoins.ToString();
        }
    }

    public void UpdateHealthUI(int currentLives)
    {
        for (int i = 0; i < _ankhIcons.Count; i++)
        {
            if (i < currentLives)
            {
                _ankhIcons[i].enabled = true;
            }
            else
            {
                _ankhIcons[i].enabled = false;
            }
        }
    }
}
