using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Tanttinator.ModularUI;

public class TileInfoUI : MonoBehaviour
{
    [SerializeField] TMP_Text infoText = default;
    [SerializeField] Hidable hidable = default;

    static TileInfoUI instance;

    public static void Show(TileData state)
    {
        instance.infoText.text = state != null? state.ToString() : "Undiscovered";
        instance.hidable.Show();
    }

    public static void Hide()
    {
        instance.hidable.Hide();
    }

    private void Awake()
    {
        instance = this;
    }
}
