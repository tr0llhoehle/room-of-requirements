using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    enum State
    {
        INITIAL,
        DISCLAIMER,
        SCENE,
        LOST_USER,
        FINISHED
    }

    public static double MAX_FACE_AGE = 30.0; // seconds
    public static double MAX_SESSION_LENGTH = 150; // seconds, 2.5min
    public static double MAX_LOST_LENGTH = 20; // seconds
    public static double MAX_GESTURE_AGE = 30.0;

    private ulong sessionStart = 0; // Unix timestamp
    private ulong lostStart = 0; // Unix timestamp
    private State state = State.INITIAL;
    private TextMesh centerText = null;
    private Transform disclaimerTransform = null;
    private SpriteRenderer handsUp = null;
    private SpriteRenderer leaving = null;
    private SpriteRenderer loading = null;

    private void Start()
    {
        centerText = GameObject.Find("CenterText").GetComponent<TextMesh>();
        disclaimerTransform = GameObject.Find("DisclaimerText").GetComponent<Transform>();
        handsUp = GameObject.Find("HandsUp").GetComponent<SpriteRenderer>();
        leaving = GameObject.Find("Leaving").GetComponent<SpriteRenderer>();
        loading = GameObject.Find("Loading").GetComponent<SpriteRenderer>();

        handsUp.color = new Color(1f, 1f, 1f, 0);
        leaving.color = new Color(1f, 1f, 1f, 0);
        loading.color = new Color(1f, 1f, 1f, 0);

    }

    private ulong GetUnixNow()
    {
        return (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }

    private double GetFaceAge()
    {
        double age = (GetUnixNow() - GameModel.Instance.faceData.time) / 1000.0;
        return age;
    }

    private double GetGestureAge()
    {
        double age = (GetUnixNow() - GameModel.Instance.gestureData.time) / 1000.0;
        return age;
    }

    private double GetSessionLength()
    {
        double age = (GetUnixNow() - sessionStart) / 1000.0;
        return age;
    }

    private double GetLostLength()
    {
        double age = (GetUnixNow() - lostStart) / 1000.0;
        return age;
    }

    private bool HasFace()
    {
        if (GameModel.Instance.faceData != null)
        {
            if (GetFaceAge() < MAX_FACE_AGE)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasGesture()
    {
        if (GameModel.Instance.gestureData != null)
        {
            if (GetGestureAge() < MAX_GESTURE_AGE)
            {
                return true;
            }
        }

        return false;
    }

    private State UpdateInitial()
    {
        loading.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time, 1));

        if (HasFace())
        {
            GameModel.Instance.agreed = false;
            loading.color = new Color(1f, 1f, 1f, 0);
            return State.DISCLAIMER;
        }
        return State.INITIAL;
    }

    private State UpdateDisclaimer()
    {
        if (!HasFace())
        {
            disclaimerTransform.transform.position = new Vector3(0f, -4.31f, 3.037f);
            handsUp.color = new Color(1f, 1f, 1f, 0);

            return State.INITIAL;
        }

        if (HasGesture())
        {
            Debug.Log(string.Format("gesture: {0}", GameModel.Instance.gestureData.type));
            GameModel.Instance.agreed = GameModel.Instance.gestureData.type == 1; // HandsAboveHead
        }
        else
        {
            Debug.Log("No gesture data");
        }

        if (GameModel.Instance.agreed)
        {
            sessionStart = GetUnixNow();
            disclaimerTransform.transform.position = new Vector3(0f, -4.31f, 3.037f);
            handsUp.color = new Color(1f, 1f, 1f, 0);
            return State.SCENE;
        }

        disclaimerTransform.Translate(Vector3.up * Time.deltaTime * 2, Space.World);
        handsUp.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time, 1));
        return State.DISCLAIMER;
    }

    private State UpdateScene()
    {
        if (!HasFace())
        {
            lostStart = GetUnixNow();
            return State.LOST_USER;
        }

        if (GetSessionLength() > MAX_SESSION_LENGTH)
        {
            return State.FINISHED;
        }

        return State.SCENE;
    }

    private State UpdateLostUser()
    {
        if (HasFace())
        {
            loading.color = new Color(1f, 1f, 1f, 0);
            return State.SCENE;
        }

        if (GetLostLength() > MAX_LOST_LENGTH)
        {
            loading.color = new Color(1f, 1f, 1f, 0);
            return State.INITIAL;
        }

        loading.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time, 1));

        return State.LOST_USER;
    }

    private State UpdateFinished()
    {
        if (!HasFace())
        {
            leaving.color = new Color(1f, 1f, 1f, 0);
            return State.INITIAL;
        }

        leaving.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time, 1));

        return State.FINISHED;
    }

    private void Update()
    {
        switch (state)
        {
            case State.INITIAL:
                state = UpdateInitial();
                break;
            case State.DISCLAIMER:
                state = UpdateDisclaimer();
                break;
            case State.LOST_USER:
                state = UpdateLostUser();
                break;
            case State.SCENE:
                state = UpdateScene();
                break;
            case State.FINISHED:
                state = UpdateFinished();
                break;
            default:
                break;
        }
    }
}
