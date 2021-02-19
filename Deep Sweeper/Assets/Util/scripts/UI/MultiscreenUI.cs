using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiscreenUI : MonoBehaviour
{
    #region Exposed Editor Parameters
    [Header("Settings")]
    [Tooltip("The first screen that should be presented.")]
    [SerializeField] private ScreenLayout defaultScreen;

    [Header("Timing")]
    [Tooltip("The time it takes a screen to fade in or out.")]
    [SerializeField] private float fadeTime = 1;

    [Tooltip("The percentage of the current screen's fade in time, "
           + "after which the next screen will start fading in.")]
    [SerializeField] [Range(0f, 1f)] private float screenAppearAfter = 0;
    #endregion

    #region Class Members
    private List<UIScreen> screens;
    private UIScreen currenScreen;
    private bool firstSwitch;
    #endregion

    #region Properties
    public bool IsSwitching { get; private set; }
    #endregion

    private void Start() {
        this.screens = new List<UIScreen>(GetComponentsInChildren<UIScreen>());
        this.IsSwitching = false;
        this.currenScreen = GetScreenByID(defaultScreen);
        this.firstSwitch = true;

        InitScreens();
        firstSwitch = false;
    }

    /// <summary>
    /// Turn off all screens that are currently showing, but shouldn't.
    /// Also, turn the default starting screen on.
    /// </summary>
    private void InitScreens() {
        foreach (UIScreen screen in screens) {
            if (screen != currenScreen) {
                if (screen.IsPresent)
                    screen.FadeScreen(false, 0);
            }
            else screen.FadeScreen(true, 0);
        }
    }

    /// <summary>
    /// Get the actual screen object by its screen type ID.
    /// </summary>
    /// <param name="ID">The enum value of the screen</param>
    private UIScreen GetScreenByID(ScreenLayout ID) {
        return screens.Find(x => x.ID == ID);
    }

    /// <param name="origin">The current screen</param>
    /// <param name="target">The screen that should replace the current one</param>
    /// <returns>True if the two screens are allowed to switch.</returns>
    private bool ShouldSwitch(UIScreen origin, UIScreen target) {
        return target != null && (origin != target || firstSwitch) && !IsSwitching;
    }

    /// <summary>
    /// Replace two screens while managing their timing correctly.
    /// </summary>
    /// <param name="origin">The current screen</param>
    /// <param name="target">The screen that should replace the current one</param>
    /// <param name="instant">True to instantly switch between the two screen</param>
    private IEnumerator ManageSwitch(UIScreen origin, UIScreen target, bool instant = false) {
        float time = instant ? 0 : fadeTime;
        float pause = screenAppearAfter * time;
        IsSwitching = true;

        if (!origin.ChildScreensID.Contains(target.ID)) origin.FadeScreen(false, time);
        if (pause > 0) yield return new WaitForSeconds(pause);
        target.FadeScreen(true, time);
        IsSwitching = false;

        currenScreen = target;
    }

    /// <summary>
    /// Close the current screen and turn another screen on.
    /// </summary>
    /// <param name="targetScreen">The screen to turn on</param>
    /// <param name="instant">True to instantly switch between the two screen</param>
    public void SwitchScreens(ScreenLayout targetScreen, bool instant = false) {
        UIScreen nextScreen = GetScreenByID(targetScreen);

        if (ShouldSwitch(currenScreen, nextScreen))
            StartCoroutine(ManageSwitch(currenScreen, nextScreen, instant));
    }
}