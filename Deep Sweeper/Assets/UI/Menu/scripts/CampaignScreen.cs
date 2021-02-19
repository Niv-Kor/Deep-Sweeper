using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignScreen : UIScreen
{
    /// <summary>
    /// Activat when the previous button is clicked.
    /// </summary>
    public void PreviousButton() {
        UI.SwitchScreens(ScreenLayout.MainMenu);
    }
}