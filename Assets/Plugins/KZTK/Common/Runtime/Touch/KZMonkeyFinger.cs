using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KZMonkeyFinger : MonoBehaviour
{
    private static int uid = 2000;
    private int _fingerId;
    private Vector2 _position = Vector2.zero;
    private Vector2 _lastPosition = Vector2.zero;
    private bool _isDown;
    private bool _wasDown;
    private bool _isMoving;
    public bool isMoving { get { return isMoving; } }
    private float _speed;
    private bool _noisy = false;
    private List<Event> events = new List<Event>();

    public void Awake()
    {
        _fingerId = uid++;
    }

    #region actions

    public void PressDown()
    {
        events.Add(new PressDownEvent(this));
    }
    public void PressDown(Vector2 position)
    {
        JumpTo(position);
        PressDown();
    }
    public void PressDownImpl()
    {
        _isDown = true;
        if (_noisy) Debug.Log("down"); //TODO Down and Up
    }


    public void LiftUp()
    {
        events.Add(new LiftUpEvent(this));
    }
    public void LiftUpImpl()
    {
        _isDown = false;
        if (_noisy) Debug.Log("up");
    }

    public void Click()
    {
        PressDown();
        LiftUp();
    }

    public void Click(Vector2 position)
    {
        JumpTo(position);
        Click();
    }

    public void JumpTo(Vector2 position)
    {
        events.Add(new JumpToEvent(this, position));
    }
    public void JumpToImpl(Vector2 position)
    {
        _position = position;
        if (_noisy) Debug.Log("set to "+position);
    }

    public void MoveTo(Vector2 position, float speed)
    {
        events.Add(new MoveToEvent(this, position, speed));
    }
    public void MoveToImpl(Vector2 position, float speed = 100)
    {
        _speed = speed;
        StopCoroutine("Move");
        if (_noisy) Debug.Log("moving from " + _position + " to " + position);
        StartCoroutine("Move", position);
        
    }

    private IEnumerator Move(Vector2 destination)
    {
        _isMoving = true;
        
        while (true)
        {
            Vector2 orientation = destination - _position;
            float distanceToGo = orientation.magnitude;
            float capacityThisFrame = _speed * Time.deltaTime;
            Vector2 delta = orientation * (capacityThisFrame / distanceToGo);
            if (distanceToGo > capacityThisFrame)
            {
                _lastPosition = _position;
                _position += delta;
                //Debug.Log("micro move from " + _lastPosition + " to " + _position);
                yield return null;
            }
            else
            {
                _lastPosition = _position;
                _position = destination;
                break;
            }

        }
        _isMoving = false;
    }
    #endregion

    public void Update()
    {
        if (events.Count > 0)
        {
            foreach (var e in events)
            {
                e.Handle();
                DetectChanges();
            }
            events.Clear();
        }
        else
        {
            DetectChanges();
        }
        
    }

    private void DetectChanges()
    {
        if (_isDown)
        {
            if (_wasDown)
            {
                //[ when they are close enough
                if (_isMoving)
                {
                    //Debug.Log("moving...");
                    GenerateTouch(TouchPhase.Moved);
                }
                else
                {
                    GenerateTouch(TouchPhase.Stationary);
                }
            }
            else
            {
                GenerateTouch(TouchPhase.Began);
            }
        }
        else
        {
            if (_wasDown)
            {
                GenerateTouch(TouchPhase.Ended);
            }
        }
        _wasDown = _isDown;
    }

    public void GenerateTouch(TouchPhase touchPhase)
    {
        KZInput.GenerateTouch(new KZTouch(
                            _fingerId, _position, _position - _lastPosition,
                            Time.deltaTime, 0, touchPhase));
    }

    #region events

    private abstract class Event
    {
        protected KZMonkeyFinger _monkeyFinger;
        public Event(KZMonkeyFinger monkeyFinger)
        {
            _monkeyFinger = monkeyFinger;
        }
        public abstract void Handle();
    }

    private class JumpToEvent : Event
    {
        private Vector2 _destination;

        public JumpToEvent(
            KZMonkeyFinger monkeyFinger, Vector2 destination) :
            base(monkeyFinger)
        {
            _destination = destination;
        }

        public override void Handle()
        {
            _monkeyFinger.JumpToImpl(_destination);
        }
    }

    private class MoveToEvent : Event
    {
        private Vector2 _destination;
        private float _speed;
        
        public MoveToEvent(
            KZMonkeyFinger monkeyFinger, Vector2 destination, float speed) : 
            base(monkeyFinger) 
        {
            _destination = destination;
            _speed = speed;
        }

        public override void Handle()
        {
            _monkeyFinger.MoveToImpl(_destination, _speed);
        }
    }
    
    private class LiftUpEvent : Event
    {
        public LiftUpEvent(KZMonkeyFinger monkeyFinger) : base(monkeyFinger) {}
        public override void Handle()
        {
            _monkeyFinger.LiftUpImpl();
        }
    }
    private class PressDownEvent : Event
    {
        public PressDownEvent(KZMonkeyFinger monkeyFinger) : base(monkeyFinger) { }
        public override void Handle()
        {
            _monkeyFinger.PressDownImpl();
        }
    }
    #endregion

}
