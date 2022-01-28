using UnityEngine;
using HP.Omnicept;
using HP.Omnicept.Unity;
using HP.Omnicept.Messaging;
using HP.Omnicept.Messaging.Messages;

public class HPLabsGliaBehaivour : GliaBehaviour
{
    public Glia GliaClient
    {
        get { return m_gliaClient; }
    }

    public delegate void MessageHandler(MonoBehaviour go, ITransportMessage msg);
    public event MessageHandler MessageReceived;

    public void SendByteMessage(string message)
    {
        //Debug.Log("Send message is being called.");
        if (m_gliaClient != null)
        {
            var msg = new ByteMessage("*", message);
            m_gliaClient.Connection.Send<ByteMessage>(msg);
        }
        else
        {
            Debug.LogError("Lost outgoing message: " + message);
        }
    }

    GliaSettings settings = null;
    string applicationName;

    public GliaSettings LoadSettings(string applicationName = null)
    {
        if (settings != null) return settings;
        if (applicationName == null)
        {
            applicationName = "GliaClient";
        }
        this.applicationName = applicationName;
        settings = Resources.Load<GliaSettings>("GliaSettings");

        if (settings == null)
        {
            //Use Defaults
            settings = ScriptableObject.CreateInstance<GliaSettings>();
        }
        return settings;
    }

    public override void HandleMessage(ITransportMessage msg)
    {
        MessageReceived?.Invoke(this, msg);
        base.HandleMessage(msg);
    }
}
