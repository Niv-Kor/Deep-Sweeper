using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DifficultyButton : MonoBehaviour, IPointerClickHandler
{
    #region Exposed Editor Parameters
    [Header("Settings")]
    [Tooltip("The sorting order of the button within the set.")]
    [SerializeField] private int placementOrder;

    [Header("Timing")]
    [Tooltip("The time it takes the button the scale up or down.")]
    [SerializeField] private float scaleTime;

    [Tooltip("The time it takes the button to translate position.")]
    [SerializeField] private float moveTime;
    #endregion

    #region Class Members
    private RectTransform rect;
    private Coroutine scaleCoroutine;
    private Coroutine moveCoroutine;
    #endregion

    #region Events
    /// <param int>The placement order of the button</param>
    public event UnityAction<int> ClickedEvent;
    #endregion

    #region Properties
    public int PlacementOrder { get => placementOrder; }
    #endregion

    private void Start() {
        this.rect = GetComponent<RectTransform>();
    }

    /// <inheritdoc/>
    public void OnPointerClick(PointerEventData ev) {
        ClickedEvent?.Invoke(PlacementOrder);
    }

    /// <see cref="Scale(float)"/>
    /// <param name="time">Scale animation time</param>
    private IEnumerator ScaleButton(float scale, float time) {
        float originScale = rect.localScale.x;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            float step = Mathf.Lerp(originScale, scale, timer / time);
            rect.localScale = Vector3.one * step;
            yield return null;
        }
    }

    /// <summary>
    /// Set the scale of the button.
    /// </summary>
    /// <param name="scale">The new cubed scale</param>
    /// <param name="instant">True to scale the button instantly</param>
    public void Scale(float scale, bool instant = false) {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        float time = instant ? 0 : scaleTime;
        scaleCoroutine = StartCoroutine(ScaleButton(scale, time));
    }

    /// <see cref="Translate(Vector2)"/>
    /// <param name="time">Translation animation time</param>
    private IEnumerator TranslateButton(Vector2 position, float time) {
        Vector2 originPos = rect.localPosition;
        float timer = 0;

        while (timer <= time) {
            timer += Time.deltaTime;
            rect.localPosition = Vector3.Lerp(originPos, position, timer / time);
            yield return null;
        }
    }

    /// <summary>
    /// Move the button by a fixed position.
    /// </summary>
    /// <param name="position">The delta position by which to translate the button</param>
    /// <param name="instant">True to translate the button instantly</param>
    public void Translate(Vector2 position, bool instant = false) {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        float time = instant ? 0 : moveTime;
        moveCoroutine = StartCoroutine(TranslateButton(position, time));
    }
}