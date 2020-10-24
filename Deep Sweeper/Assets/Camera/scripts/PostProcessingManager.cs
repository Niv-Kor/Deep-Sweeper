using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    public ScreenSpaceReflections ScreenSpaceReflections { get; private set; }
    public ChromaticAberration ChromaticAberration { get; private set; }
    public AmbientOcclusion AmbientOcclusion { get; private set; }
    public LensDistortion LensDistortion { get; private set; }
    public AutoExposure AutoExposure { get; private set; }
    public ColorGrading ColorGrading { get; private set; }
    public DepthOfField DepthOfField { get; private set; }
    public MotionBlur MotionBlur { get; private set; }
    public Vignette Vignette { get; private set; }
    public Bloom Bloom { get; private set; }
    public Grain Grain { get; private set; }

    private void Start() {
        PostProcessVolume volume = GetComponent<PostProcessVolume>();

        //extract settings
        volume.profile.TryGetSettings(out ScreenSpaceReflections screenSpaceReflections);
        volume.profile.TryGetSettings(out ChromaticAberration chromaticAberration);
        volume.profile.TryGetSettings(out AmbientOcclusion ambientOcclusion);
        volume.profile.TryGetSettings(out LensDistortion lensDistortion);
        volume.profile.TryGetSettings(out AutoExposure autoExposure);
        volume.profile.TryGetSettings(out ColorGrading colorGrading);
        volume.profile.TryGetSettings(out DepthOfField depthOfField);
        volume.profile.TryGetSettings(out MotionBlur motionBlur);
        volume.profile.TryGetSettings(out Vignette vignette);
        volume.profile.TryGetSettings(out Bloom bloom);
        volume.profile.TryGetSettings(out Grain grain);

        //assign to properties
        this.ScreenSpaceReflections = screenSpaceReflections;
        this.ChromaticAberration = chromaticAberration;
        this.AmbientOcclusion = ambientOcclusion;
        this.LensDistortion = lensDistortion;
        this.AutoExposure = autoExposure;
        this.ColorGrading = colorGrading;
        this.DepthOfField = depthOfField;
        this.MotionBlur = motionBlur;
        this.Vignette = vignette;
        this.Bloom = bloom;
        this.Grain = grain;
    }
}