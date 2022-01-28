using HP.Omnicept.Messaging.Messages;
using UnityEngine;
using UnityEngine.UI;

public class BlinksGazeVRController : MonoBehaviour
{
    public HPLabsGliaController gliaController;

    [SerializeField]
    private int avatarIndex;
    public int AvatarIndex 
    { 
        get => avatarIndex; 
        set
        {
            SetAvatarIndex(value);
        } 
    }

    public AvatarInfo[] avatarInfoArray;
    private AvatarInfo currentAvatar;

    public Button leftButton;
    public Button rightButton;

    [Header("Debug")]
    public Vector3 eyeModifier = Vector3.one;
    public GameObject testEye;

    private GameObject leftEye;
    private GameObject rightEye;
    private GameObject head;

    private Quaternion leftEyeStartRotation;
    private Quaternion rightEyeStartRotation;
    private Quaternion headStartRotation;

    private Quaternion testEyeStartRotation;

    private void Awake()
    {
        SetAvatarIndex(avatarIndex);

        leftEyeStartRotation = leftEye.transform.localRotation;
        rightEyeStartRotation = rightEye.transform.localRotation;

        testEyeStartRotation = testEye.transform.localRotation;

        headStartRotation = head.transform.rotation;
    }

    private void Start()
    {
        leftButton.onClick.AddListener(() =>
        {
            SetAvatarIndex(avatarIndex - 1);
        });

        rightButton.onClick.AddListener(() =>
        {
            SetAvatarIndex(avatarIndex + 1);
        });
    }

    private void SetAvatarIndex(int index)
    {
        index = index >= avatarInfoArray.Length ? 0 : index;
        index = index < 0 ? avatarInfoArray.Length - 1 : index;

        avatarIndex = index;

        foreach (var avatarInfo in avatarInfoArray)
            avatarInfo.model.SetActive(false);

        currentAvatar = avatarInfoArray[avatarIndex];

        currentAvatar.model.SetActive(true);
        leftEye = currentAvatar.leftEye;
        rightEye = currentAvatar.rightEye;
        head = currentAvatar.head;
    }

    private void OnValidate()
    {
        AvatarIndex = avatarIndex;
    }

    private void Update()
    {
        UpdateEyeState();
        UpdateHeadState();
    }

    private EyeTrackingData eyeData = new EyeTrackingData();
    private void UpdateEyeState()
    {
        EyeTracking eyeTracking = gliaController.lastEyeTracking;
        eyeData.From(eyeTracking);

        SetAvatarEyeState(eyeData);
    }

    private void SetAvatarEyeState(EyeTrackingData state)
    {
        if (state == null) return;

        SetBlinkState(state);
        SetGazeState(state);
    }

    private void SetBlinkState(EyeTrackingData state)
    {
        float leftBlinkValue = state.left_openness == 0 ? 100f : 0f;
        float rightBlinkValue = state.right_openness == 0 ? 100f : 0f;

        // mirroring, so left is right and vice versa
        currentAvatar.SetBlendShapeWeight("Eye_Blink_L", rightBlinkValue);
        currentAvatar.SetBlendShapeWeight("Eye_Blink_R", leftBlinkValue);
    }

    private void SetGazeState(EyeTrackingData state)
    {
        Vector3 vLeftEye = new Vector3(state.left_gaze_y * eyeModifier.x, 0f, state.left_gaze_x * eyeModifier.z);
        Vector3 vRightEye = new Vector3(state.right_gaze_y * eyeModifier.x, 0f, state.right_gaze_x * eyeModifier.z);

        // mirroring, so left is right and vice versa
        leftEye.transform.localRotation = leftEyeStartRotation * Quaternion.Euler(vRightEye * 40f);
        rightEye.transform.localRotation = rightEyeStartRotation * Quaternion.Euler(vLeftEye * 40f);

        //Debug.Log($"Left X: {state.left_gaze_x}, Y: {state.left_gaze_y}, Z: {state.left_gaze_z}");

        Vector3 vTestEye = new Vector3(0f, state.left_gaze_x, state.left_gaze_y);
        testEye.transform.localRotation = testEyeStartRotation * Quaternion.Euler(vTestEye * 40f);
    }

    HeadTrackingData headData = new HeadTrackingData();
    private void UpdateHeadState()
    {
        headData.head_rot_x = gliaController.myCamera.rotation.x;
        headData.head_rot_y = gliaController.myCamera.rotation.y;
        headData.head_rot_z = gliaController.myCamera.rotation.z;
        headData.head_rot_w = gliaController.myCamera.rotation.w;

        SetAvatarHeadState(headData);
    }

    private void SetAvatarHeadState(HeadTrackingData state)
    {
        if (state == null) return;

        SetHeadPosition(state);
        SetHeadRotation(state);
    }

    private void SetHeadPosition(HeadTrackingData state)
    {
        // nothing to do here
    }

    private void ReSetHeadRotation()
    {
        head.transform.rotation = headStartRotation;
    }

    private void SetHeadRotation(HeadTrackingData state)
    {
        Quaternion qHeadRot = new Quaternion(state.head_rot_x, state.head_rot_y, state.head_rot_z, state.head_rot_w);
        Vector3 eulerAngles = qHeadRot.eulerAngles;

        head.transform.rotation = headStartRotation * Quaternion.Euler(new Vector3(eulerAngles.x, -eulerAngles.y, -eulerAngles.z));
    }
}
