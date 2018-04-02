using System.Collections;
using UnityEngine;

public class DragonController : MonoBehaviour 
{
    public float WalkSpeed;
    public float RunSpeed;
    public float FlyForwardSpeed;
    public float FlyDownSpeed;

    private playerControl dragon;
    public DragonState StartState;
    private void Start()
    {
        dragon = GetComponent<playerControl>();

        FaceRandomGroundDirection(); // so they're not all facing the same direciton when you load the game
        State = StartState;
    }

    public enum DragonState
    {
        idle, walking, running, sleeping, flying,
    }

    private DragonState TrueState;
    private DragonState State
    {
        get { return TrueState; }
        set
        {
            if (CurrentBehaviorsRoutine != null) { StopCoroutine(CurrentBehaviorsRoutine); }
            TrueState = value;

            switch (value)
            {
                case DragonState.idle:
                    CurrentBehaviorsRoutine = StartCoroutine(IdleBehaviors());
                    break;

                case DragonState.walking:
                    CurrentBehaviorsRoutine = StartCoroutine(WalkBehaviors());
                    break;

                case DragonState.running:
                    CurrentBehaviorsRoutine = StartCoroutine(RunBehaviors());
                    break;

                case DragonState.flying:
                    CurrentBehaviorsRoutine = StartCoroutine(FlyBehaviors());
                    break;

                case DragonState.sleeping:
                    CurrentBehaviorsRoutine = StartCoroutine(SleepBehaviors());
                    break;
            }
        }
    }

    private Coroutine CurrentBehaviorsRoutine;

    private IEnumerator IdleBehaviors()
    {
        // find a fun way to start being idle
        DoRandomNonFlyingAnimation();

        yield return new WaitForSeconds(Random.Range(3f, 7f));

        State = (DragonState)Random.Range(0, 5);
    }

    private IEnumerator WalkBehaviors()
    {
        DoRandomNonFlyingAnimation();
        yield return new WaitForSeconds(3);
        FaceRandomGroundDirection();

        float TimeWalking = 0;
        dragon.Walk();
        while (true)
        {
            yield return new WaitForEndOfFrame();
            transform.position += transform.forward * WalkSpeed * Time.deltaTime;

            TimeWalking += Time.deltaTime;
            if(TimeWalking > 4f) // time the animation takes
            {
                int WhatToDoNext = Random.Range(0, 20);
                if (WhatToDoNext <= 4)
                {
                    State = (DragonState)Random.Range(0, 5);
                    yield return null;
                }
                else
                {
                    dragon.Walk();
                    TimeWalking = 0;
                }
            }
        }
    }

    private IEnumerator RunBehaviors()
    {
        DoRandomNonFlyingAnimation();
        yield return new WaitForSeconds(3);
        FaceRandomGroundDirection();

        float TimeRunning = 0;
        dragon.Run();
        while (true)
        {
            yield return new WaitForEndOfFrame();
            transform.position += (transform.forward * RunSpeed * Time.deltaTime);

            TimeRunning += Time.deltaTime;
            if (TimeRunning > 2.3f) // time the animation takes
            {
                int WhatToDoNext = Random.Range(0, 20);
                if (WhatToDoNext <= 4)
                {
                    State = (DragonState)Random.Range(0, 5);
                    yield return null;
                }
                else
                {
                    dragon.Run();
                    TimeRunning = 0;
                }
            }
        }
    }

    private IEnumerator FlyBehaviors()
    {
        if (transform.position.y < 1) // if we're on the ground, take off
        {
            dragon.TakeOff();
            yield return new WaitForSeconds(5); // time the animation takes
        }

        if(Random.Range(0, 4) == 2)
        {
            dragon.FlyFlameAttack();
            yield return new WaitForSeconds(3);
        }

        bool FlyingUp = FaceRandomSkyDirectionAndReturnTrueIfFlyingUp();

        if (FlyingUp)
        {
            dragon.FlyForward();
            float FlyTime = Random.Range(10, 30);
            float FlyTimeSoFar = 0;

            while (true)
            {
                yield return new WaitForEndOfFrame();
                transform.position += (transform.forward * FlyForwardSpeed * Time.deltaTime);

                FlyTimeSoFar += Time.deltaTime;
                if(FlyTimeSoFar >= FlyTime)
                {
                    State = DragonState.flying;
                    yield return null;
                    yield break;
                }
            }
        }
        else
        {
            float TimeGliding = 0;
            dragon.FlyGlide();
            while (true)
            {
                yield return new WaitForEndOfFrame();
                transform.position += (transform.forward * FlyDownSpeed * Time.deltaTime);

                TimeGliding += Time.deltaTime;
                if (TimeGliding > 3f) // time the animation takes
                {
                    dragon.FlyGlide();
                    TimeGliding = 0;
                }

                if (transform.position.y <= HeightToStartLandingAt)
                {
                    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                    transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    dragon.Land();
                    yield return new WaitForSeconds(4); // time the animation takes

                    State = (DragonState)Random.Range(0, 5);
                    yield return null;
                    yield break; // is this needed to stop the routine??
                }
            }
        }
    }

    public float HeightToStartLandingAt = 30;

    private IEnumerator SleepBehaviors()
    {
        dragon.Sleep();
        yield return new WaitForSeconds(5);
        State = (DragonState)Random.Range(0, 5);
    }

    private void DoRandomNonFlyingAnimation()
    {
        int WhatToDo = Random.Range(0, 5);
        if (WhatToDo == 0) { dragon.Scream(); }
        else if (WhatToDo == 1) { dragon.BasicAttack(); }
        else if (WhatToDo == 2) { dragon.ClawAttack(); }
        else if (WhatToDo == 3) { dragon.FlameAttack(); }

        // there is a chance of nothing happening!
    }

    public Vector3 Home;
    private void Awake()
    {
        Home = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public float DistanceBeyondWhichToComeHome;
    public float HomeSpread;
    private void FaceRandomGroundDirection()
    {
        if (Vector3.Distance(Home, new Vector3(transform.position.x, 0, transform.position.z)) <= DistanceBeyondWhichToComeHome)
        {
            transform.eulerAngles = new Vector3(0, Random.Range(0, 360f), 0);
        }
        else
        {
            transform.LookAt(Home + new Vector3(Random.Range(-HomeSpread, HomeSpread), transform.position.y, Random.Range(-HomeSpread, HomeSpread)));
        }
    }

    private bool FaceRandomSkyDirectionAndReturnTrueIfFlyingUp()
    {
        FaceRandomGroundDirection();

        // if we're below 30 meters, don't fly down ever, fly up
        if (transform.position.y < 30)
        {
            FaceUpByRandomAmount();
            return true;
        }
        // if we're above 300 meters, don't fly up ever, fly down
        else if (transform.position.y > 300)
        {
            FaceDownByRandomAmount();
            return false;
        }
        // otherwise choose randomly
        else
        {
            if (Random.Range(0, 10) > 4) { FaceUpByRandomAmount(); return true; }
            else { FaceDownByRandomAmount(); return false; }
        }
    }

    public float MinUpAngle;
    public float MaxUpAngle;
    public float MinDownAngle;
    public float MaxDownAngle;
    private void FaceUpByRandomAmount()
    {
        transform.eulerAngles += new Vector3(Random.Range(MinUpAngle, MaxUpAngle), 0, 0);
    }

    private void FaceDownByRandomAmount()
    {
        transform.eulerAngles += new Vector3(Random.Range(MinDownAngle, MaxDownAngle), 0, 0);
    }
}