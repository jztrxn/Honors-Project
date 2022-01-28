using HP.Omnicept;
using HP.Omnicept.Errors;
using HP.Omnicept.Messaging;
using HP.Omnicept.Messaging.Messages;
using HP.Omnicept.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HPLabsGliaController : MonoBehaviour
{
    public HPLabsGliaBehaivour glia;

    public Transform myCamera;

    public EyeTracking lastEyeTracking { get; private set; }

    public string defaultProcessName { get; } = "BlinksGazeDemo";

    private void Awake()
    {
        glia.MessageReceived += OnGliaMessageReceived;
        glia.OnConnect.AddListener(OnGliaConnectListener);
        glia.OnDisconnect.AddListener(OnGliaDisconnectListener);
        glia.OnConnectionFailure.AddListener(OnGliaConnectionFailure);
    }

    private void OnDestroy()
    {
        if (glia != null)
        {
            glia.OnConnect.RemoveListener(OnGliaConnectListener);
            glia.OnDisconnect.RemoveListener(OnGliaDisconnectListener);
            glia.OnConnectionFailure.RemoveListener(OnGliaConnectionFailure);
            glia.MessageReceived -= OnGliaMessageReceived;
        }
    }

    private void OnGliaConnectListener(GliaBehaviour arg0)
    {
        DoGliaConnectStuff();
    }

    private void OnGliaConnectionFailure(ClientHandshakeError arg0)
    {
        Debug.LogError($"Connection to Glia failed: {arg0.Message}");
    }

    private void OnGliaDisconnectListener(string arg0)
    {
        Debug.LogError($"Disconnected from Glia: {arg0}");
    }

    public void SendMessage(string message)
    {
        glia.SendByteMessage(message);
    }

    private void DoGliaConnectStuff()
    {
        Debug.Log("Initializing message subscriptions");
        InitSubscription();
        UpdateSubscriptions("eyetracker", true);

        SendMessage($"iam {defaultProcessName}");
    }

    private void OnGliaMessageReceived(MonoBehaviour go, ITransportMessage msg)
    {
        try
        {
            if (msg != null)
            {
                switch (msg.Header.MessageType)
                {
                    case MessageTypes.ABI_MESSAGE_BYTE_MESSAGE:
                        //ByteMessage command = glia.GliaClient.Connection.Build<ByteMessage>(msg);
                        //HandleCommand(command.Message);
                        break;

                    default:
                        string status = RecordMessage(msg);
                        if (status != null)
                        {
                            SendMessage(status);
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error from Glia: {ex.Message}");
        }
    }

    public string RecordMessage(ITransportMessage msg)
    {
        string status = null;
        uint mtype = msg.Header.MessageType;

        switch (mtype)
        {
            case MessageTypes.ABI_MESSAGE_EYE_TRACKING:
                {
                    //Debug.Log($"RecordMessage - received: ABI_MESSAGE_EYE_TRACKING");
                    lastEyeTracking = glia.GliaClient.Connection.Build<EyeTracking>(msg);
                    break;
                }
        }
        return status;
    }

    List<uint> subs = null;
    private void InitSubscription()
    {
        subs = new List<uint>();
        lock (subs)
        {
            subs.Add(MessageTypes.ABI_MESSAGE_BYTE_MESSAGE);
        }
    }

    private void AddSubscription(uint mtype)
    {
        lock (subs)
        {
            if (!subs.Contains(mtype))
            {
                subs.Add(mtype);
            }
        }
    }

    private void RemoveSubscription(uint mtype)
    {
        lock (subs)
        {
            if (subs.Contains(mtype))
            {
                subs.Remove(mtype);
            }
        }
    }

    private void SubscribeMessages()
    {
        SubscriptionBuilder msg = new SubscriptionBuilder();
        lock (subs)
        {
            foreach (var mtype in subs)
            {
                msg.Add(mtype, null, null, null, null);
            }
        }
        if (glia != null && glia.GliaClient != null)
            glia.GliaClient.Connection.Send<SubscriptionList>(msg.Build());
    }

    private bool UpdateSubscriptions(string device, bool turnOn)
    {
        var mtypes = new List<uint>();
        switch (device)
        {
            case "eyetracker":
                mtypes.Add(MessageTypes.ABI_MESSAGE_EYE_TRACKING);
                //mtypes.Add(MessageTypes.ABI_MESSAGE_EYE_TRACKING_FRAME);
                //mtypes.Add(MessageTypes.ABI_MESSAGE_EYE_PUPILLOMETRY);
                break;
            case "scenecolor":
                mtypes.Add(MessageTypes.ABI_MESSAGE_SCENE_COLOR);
                mtypes.Add(MessageTypes.ABI_MESSAGE_SCENE_COLOR_FRAME);
                break;
            case "ppg":
                mtypes.Add(MessageTypes.ABI_MESSAGE_PPG);
                mtypes.Add(MessageTypes.ABI_MESSAGE_PPG_FRAME);
                break;
            case "emg":
                mtypes.Add(MessageTypes.ABI_MESSAGE_FACIAL_EMG);
                mtypes.Add(MessageTypes.ABI_MESSAGE_FACIAL_EMG_FRAME);
                break;
            case "imu":
                mtypes.Add(MessageTypes.ABI_MESSAGE_IMU);
                mtypes.Add(MessageTypes.ABI_MESSAGE_IMU_FRAME);
                break;
            case "camera_image":
                mtypes.Add(MessageTypes.ABI_MESSAGE_CAMERA_IMAGE);
                break;
            default:
                return false;
        }

        foreach (var mtype in mtypes)
        {
            if (turnOn)
                AddSubscription(mtype);
            else
                RemoveSubscription(mtype);
        }

        if (mtypes.Count > 0)
            SubscribeMessages();

        return true;
    }
}
