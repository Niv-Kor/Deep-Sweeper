public class MainMenuScreen : UIScreen
{
    public void GotoCampaignScreen() {
        UI.SwitchScreens(ScreenLayout.Campaign);
    }

    /// <inheritdoc/>
    protected override void OnScreenUp(UIScreen prevScreen) {}

    /// <inheritdoc/>
    protected override void OnScreenOff(UIScreen nextScreen) {}
}