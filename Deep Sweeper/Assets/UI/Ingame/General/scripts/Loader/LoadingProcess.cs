using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class LoadingProcess
{
    public struct LoadingStage
    {
        public string Title;
        public UnityAction Action;
    }

    #region Class Members
    private Queue<LoadingStage> stages;
    #endregion

    #region Properties
    public int StageCount => stages.Count;
    public bool UsingLoader { get; private set; }
    public string StageTitle => (stages.Count > 0) ? stages.Peek().Title : null;
    #endregion

    /// <param name="enableLoader">True to enable a loader bar while completing the process</param>
    public LoadingProcess(bool enableLoader) {
        this.stages = new Queue<LoadingStage>();
        this.UsingLoader = enableLoader;
    }

    /// <summary>
    /// Add an invokable stage to the process.
    /// </summary>
    /// <param name="action">The stage's algorithm</param>
    /// <param name="title">A short description of the stage</param>
    public void Enroll(UnityAction action, string title = "") {
        LoadingStage stage;
        stage.Title = title;
        stage.Action = action;
        stages.Enqueue(stage);
    }

    /// <summary>
    /// Execute the first stage in the queueu (FIFO).
    /// </summary>
    /// <returns>True if a stage has been executed successfully.</returns>
    public bool ExecuteStage() {
        if (stages.Count == 0) return false;

        try {
            LoadingStage stage = stages.Dequeue();
            stage.Action?.Invoke();
            return true;
        }
        catch (Exception) { return false; }
    }
}