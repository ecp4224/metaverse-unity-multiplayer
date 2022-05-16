using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using StarterAssets;

[RequireComponent(typeof(PlayerInput))]
public class CameraChange : MonoBehaviour
{

    private StarterAssetsInputActions playerControls;

    public GameObject ThirdCam;
    public GameObject FirstCam;
    public bool thirdPersonView = true;

    private void Awake()
    {
        playerControls = new StarterAssetsInputActions();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void StartCamera()
    {
        StartCoroutine(CamChange());
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControls.Player.Camera.WasPressedThisFrame())
        {
            Debug.Log("Camera Toggle...");
            thirdPersonView = !thirdPersonView;
            StartCoroutine(CamChange());
        }

    }

    private Quaternion ogHead;
    public static Quaternion headRotation = Quaternion.Euler(95.49402f, 27.67101f, 116.934f);
    
    IEnumerator CamChange()
    {
        if (Player.IsUsingVR())
        {
            yield return new WaitForSeconds(0.01f);
        }
        else
        {
            if (!thirdPersonView)
            {
                Debug.Log("Got POS: " + Player.LocalPlayer.FPSTarget.position);
                FirstCam.transform.SetPositionAndRotation(Player.LocalPlayer.FPSTarget.position, Player.LocalPlayer.FPSTarget.rotation);
                
                ogHead = Player.LocalPlayer.head.transform.rotation;
                Player.LocalPlayer.head.transform.rotation = headRotation;
                
                Player.LocalPlayer.CmdShowWrist();
            }
            else
            {
                Player.LocalPlayer.head.transform.rotation = ogHead;
                Player.LocalPlayer.CmdHideWrist();
            }
            
            ThirdCam.SetActive(thirdPersonView);
            FirstCam.SetActive(!thirdPersonView);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
