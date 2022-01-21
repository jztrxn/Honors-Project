using HP.Omnicept.Messaging.Messages;
using HP.Omnicept.Unity;
using UnityEngine;

public class SubscriptionStatus : MonoBehaviour
{
    private GliaBehaviour _gliaBehaviour;
    protected GliaBehaviour gliaBehaviour
    {
        get
        {
            if (_gliaBehaviour == null)
            {
                _gliaBehaviour = FindObjectOfType<GliaBehaviour>();
            }

            return _gliaBehaviour;
        }
    }

    [Tooltip("A message you are subscribed to")]
    public GliaMessageTypes messageToCheck = GliaMessageTypes.HeartRate;

    public GameObject rejected;
    public GameObject pending;


    private void OnEnable()
    {
        gliaBehaviour.OnSubscriptionResultListEvent.AddListener(OnSubscriptionResultList);
    }


    private void OnDisable()
    {
        gliaBehaviour?.OnSubscriptionResultListEvent.RemoveListener(OnSubscriptionResultList);
    }

    private void OnSubscriptionResultList(SubscriptionResultList subscriptionResults)
    {
        SubscriptionResult.SubscriptionResultType status = SubscriptionResult.SubscriptionResultType.Unknown;

        foreach (var subscriptionResult in subscriptionResults.SubscriptionResults)
        {
            //As we are subscirbed to all messages by default 
            if (subscriptionResult.Subscription.MessageType != (uint)messageToCheck)
            {
                continue;
            }

            if (subscriptionResult.Result != status)
            {
                Debug.Log($"{subscriptionResult.Subscription.MessageType} Changed {subscriptionResult.Result}");
            }

            status = subscriptionResult.Result;
        }

        UIStatus(status);
    }

    private void UIStatus(SubscriptionResult.SubscriptionResultType status)
    {
        switch (status)
        {
            case SubscriptionResult.SubscriptionResultType.Approved:
                pending.SetActive(false);
                rejected.SetActive(false);
                break;
            case SubscriptionResult.SubscriptionResultType.Rejected:
                pending.SetActive(false);
                rejected.SetActive(true);
                break;
            case SubscriptionResult.SubscriptionResultType.Pending:
                pending.SetActive(true);
                rejected.SetActive(false);
                break;
            default:
                pending.SetActive(false);
                rejected.SetActive(false);
                break;
        }
    }
}

public enum GliaMessageTypes
{
    HeartRate = 3,
    VSYNC = 28,
    HeartRateVariability = 36,
    EyeTracking = 9,
    CognitiveLoad = 21,
    FaceCameraImage = 37
}
