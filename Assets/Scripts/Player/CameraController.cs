using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance; // Singleton instance
    public Transform target;
    public Camera mainCamera, openMapCamera;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the GameObject persists between scenes
        }
        else
        {
            Destroy(gameObject); // If another instance already exists, destroy this one
        }
    }

    private void Start()
    {
         // Ensure CinemachineVirtualCamera is active
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera component not found!");
        }
        else if (!virtualCamera.enabled)
        {
            Debug.LogError("CinemachineVirtualCamera is disabled! Enable it in the Inspector.");
        }
    }

    private void Update()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            SetFollowTarget(playerObject.transform);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = target;
        }
        else
        {
            Debug.LogError("virtualCamera is null!");
        }
    }
}

