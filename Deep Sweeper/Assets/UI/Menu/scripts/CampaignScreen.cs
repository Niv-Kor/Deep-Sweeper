using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignScreen : UIScreen
{
    public void StartGame(int levelIndex) {

    }

    public void PreviousButton() {
        UI.SwitchScreens(ScreenLayout.MainMenu);
    }
}