using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour, IPointerClickHandler
{
    #region Exposed Editor Parameters
    [Tooltip("The tint of the buttons when selected.")]
    [SerializeField] private Color selectedTint;

    [SerializeField] private float tintAnimationTime = .5f;
    #endregion

    #region Constants
    private static readonly Color UNSELECTED_TINT = Color.white;
    #endregion

    #region Class Members
    private RectTransform rect;
    private ParticleSystem FX;
    private Image image;
    #endregion

    #region Events
    /// <param DifficultyButton>The invoking DifficultyButton component</param>
    public event UnityAction<DifficultyButton> ClickedEvent;
    #endregion

    #region Properties
    public Vector2 ActualSize {
        get {
            if (rect == null) return Vector2.zero;
            else {
                Vector2 scale = rect.localScale;
                Vector2 size = rect.sizeDelta;
                return Vector2.Scale(size, scale);
            }
        }
    }
    #endregion

    private void Start() {
        this.rect = GetComponent<RectTransform>();
        this.FX = GetComponentInChildren<ParticleSystem>();
        this.image = GetComponent<Image>();
    }

    /// <inheritdoc/>
    public void OnPointerClick(PointerEventData ev) {
        ClickedEvent?.Invoke(this);
    }

    /// <summary>
    /// Change the tint of the image.
    /// </summary>
    /// <param name="tint">New tint color</param>
    /// <param name="time">The time it takes to complete the transition</param>
    /// <returns></returns>
    private IEnumerator ChangeTint(Color tint, float time) {
        Color srcTint = image.color;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            image.color = Color.Lerp(srcTint, tint, timer / time);
            yield return null;
        }
    }

    /// <summary>
    /// Mark this button as selected.
    /// </summary>
    /// <param name="flag">True to select or false to deselect</param>
    /// <param name="instant">True to instantly select the button without animation</param>
    public void Select(bool flag, bool instant = false) {
        Color imageTint = flag ? selectedTint : UNSELECTED_TINT;
        float tintTime = instant ? 0 : tintAnimationTime;
        StopAllCoroutines();
        StartCoroutine(ChangeTint(imageTint, tintTime));

        if (flag) FX.Play();
        else {
            FX.Clear();
            FX.Stop();
        }
    }
}