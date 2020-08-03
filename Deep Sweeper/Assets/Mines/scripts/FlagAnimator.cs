using UnityEngine;

public class FlagAnimator : MonoBehaviour
{
    public MeshFilter[] meshCache;
    [HideInInspector]
    public Transform meshCached;
    public Transform meshContainerFBX;
    public float playSpeed = 1f;
    public float playSpeedRandom = 0f;
    public bool randomSpeedLoop;
    float currentSpeed;
    [HideInInspector]
    public float currentFrame;
    [HideInInspector]
    public int meshCacheCount;
    [HideInInspector]
    public MeshFilter meshFilter;
    [HideInInspector]
    public Renderer rendererComponent;
    public float updateInterval = 0.05f;
    
    public bool randomRotateX;
    public bool randomRotateY;
    public bool randomRotateZ;
    public bool randomStartFrame = true;
    public bool randomRotateLoop;
    public bool loop = true;
    public bool pingPong;
    public bool playOnAwake = true;
    public Vector2 randomStartDelay = new Vector2(0.0f,0.0f);
    float startDelay;
    float startDelayCounter;
    
    public static float updateSeed;
    
    bool pingPongToggle;
    
    public Transform transformCache;
    public float delta;
    
    public void Start() {
    	this.transformCache = transform;
    	CheckIfMeshHasChanged();
    	startDelay = UnityEngine.Random.Range(randomStartDelay.x, randomStartDelay.y);
    	updateSeed += .0005f;

    	if (playOnAwake) Invoke("Play", updateInterval+updateSeed);
    	if (updateSeed >= updateInterval) updateSeed = .0f;
    	if (rendererComponent == null) GetRequiredComponents();
    }
    
    public void Play() {
    	CancelInvoke();

    	if(randomStartFrame) currentFrame = meshCacheCount*UnityEngine.Random.value;
    	else currentFrame = .0f;
    	
    	meshFilter.sharedMesh = meshCache[(int)currentFrame].sharedMesh;		
    	this.enabled = true;	
    	RandomizePlaySpeed();	
    	RandomRotate();
    }
    
    public void RandomRotate() {
    	if (randomRotateX) {
            Quaternion tmp_cs1 = transformCache.localRotation;
            Vector3 tmp_cs2 = tmp_cs1.eulerAngles;
            tmp_cs2.x = (float)UnityEngine.Random.Range(0, 360);
            tmp_cs1.eulerAngles = tmp_cs2;
            transformCache.localRotation = tmp_cs1;
        }

    	if (randomRotateY) {
            Quaternion tmp_cs3 = transformCache.localRotation;
            Vector3 tmp_cs4 = tmp_cs3.eulerAngles;
            tmp_cs4.y = (float) UnityEngine.Random.Range(0, 360);
            tmp_cs3.eulerAngles = tmp_cs4;
            transformCache.localRotation = tmp_cs3;
        }

    	if (randomRotateZ) {
            Quaternion tmp_cs5 = transformCache.localRotation;
            Vector3 tmp_cs6 = tmp_cs5.eulerAngles;
            tmp_cs6.z = (float) UnityEngine.Random.Range(0, 360);
            tmp_cs5.eulerAngles = tmp_cs6;
            transformCache.localRotation = tmp_cs5;
        }
    }
    
    public void GetRequiredComponents() {
    	rendererComponent = GetComponent<Renderer>();
    }
    
    public void RandomizePlaySpeed() {
    	if (playSpeedRandom > 0)
    	    currentSpeed = UnityEngine.Random.Range(playSpeed-playSpeedRandom, playSpeed+playSpeedRandom);
    	else currentSpeed = playSpeed;
    }
    
    public void FillCacheArray() {
    	GetRequiredComponents();
    	if (transformCache == null)	transformCache = transform;
    	meshFilter = transformCache.GetComponent<MeshFilter>();
    	meshCacheCount = meshContainerFBX.childCount;
    	meshCached = meshContainerFBX;
    	meshCache = new MeshFilter[meshCacheCount];

    	for(int i = 0; i < meshCacheCount; i++)
    		meshCache[i] = meshContainerFBX.GetChild(i).GetComponent<MeshFilter>();

    	currentFrame = meshCacheCount*UnityEngine.Random.value;	
    	meshFilter.sharedMesh = meshCache[(int) currentFrame].sharedMesh;
    }
    
    public void CheckIfMeshHasChanged(){
    	if(meshCached != meshContainerFBX){  
    	    if(meshContainerFBX!=null)
    			FillCacheArray();
    	}
    }
    
    public void Update() {
    	delta = Time.deltaTime;	
    	startDelayCounter += delta;		

    	if(startDelayCounter > startDelay) {
    		rendererComponent.enabled = true;
    		Animate();	
    	}

    	if (this.enabled) return;
    	else rendererComponent.enabled = false;
    }
    
    public bool PingPongFrame(){	
    	if(pingPongToggle) currentFrame+= currentSpeed*delta;
    	else currentFrame-= currentSpeed*delta;	

    	if (currentFrame <= 0) {
    		currentFrame = .0f;
    		pingPongToggle = true;
    		return true;
    	}

    	if (currentFrame >= meshCacheCount) {
    		pingPongToggle = false;
    		currentFrame = meshCacheCount - 1;
    		return true;
    	}
    	return false;
    }
    
    public bool NextFrame(){
    	currentFrame+= currentSpeed * delta;

    	if (currentFrame > meshCacheCount + 1) {
    		currentFrame = 0f;
    		if (!loop) this.enabled = false;
    		return true;
    	}

    	else if (currentFrame >= meshCacheCount) {
    		currentFrame = meshCacheCount - currentFrame;
    		if (!loop) this.enabled = false;
    		return true;
    	}

    	return false;
    }
    
    public void RandomizePropertiesAfterLoop() {
    	if (randomSpeedLoop) RandomizePlaySpeed();
    	if (randomRotateLoop) RandomRotate();
    }
    
    public void Animate() {
    	if (rendererComponent.isVisible) {
    		if (pingPong && PingPongFrame()) RandomizePropertiesAfterLoop();
    		else if (!pingPong && NextFrame()) RandomizePropertiesAfterLoop();

    		meshFilter.sharedMesh = meshCache[(int) currentFrame].sharedMesh;		
    	}
    }
}