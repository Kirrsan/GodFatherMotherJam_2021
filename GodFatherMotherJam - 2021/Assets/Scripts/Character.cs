using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("StaysOnScreen")]
    public float timeCharacterStaysOnScreenMin;
    public float timeCharacterStaysOnScreenMax;
    private float _stayOnScreenTimer = 0;
    private float _stayOnScreenTimerMax = 0;
    private bool _hasDeterminedTimerDuration = false;


    [Header("Patrol")]
    public PatrolPoint[] positions;
    public float speed = 1;
    public float distanceToBeOnPosition = 0.1f;
    private List<int> _positionsAvailable = new List<int>();
    private bool isStart = true;

    public float AmountToGoVertically = 2;
    public float timeToGoUpAndDown = 1;
    public AnimationCurve UpAndDownCurve;
    public Animator characterAnimator;

    public float ChanceToStop = 20f;
    public float timeToStopMin = 0.2f;
    public float timeToStopMax = 2f;
    private bool shouldStop = false;
    private bool isStopped = false;

    private float _UpAndDownTimer = 0;
    private float _ratio = 0;
    private float _ratioCurve = 0;
    private float _startYPos;
    private int _currentPositionIndex = 0;
    private Vector3 _movementDirection;
    private Vector3 _startMovementPos;

    // Start is called before the first frame update
    void Start()
    {
        _startYPos = transform.position.y;
        for (int i = 0; i < positions.Length; i++)
        {
            _positionsAvailable.Add(i);
        }
        NextPatrolStepToSelect();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        LerpUpAndDown();
        ReduceStayOnScreenTimer();
    }
    #region patrol
    private void Move()
    {
        if (isStopped) return;

        //calcul de la direction
        _movementDirection = positions[_currentPositionIndex].transform.position - _startMovementPos;

        //dirige le mouvement
        transform.position += _movementDirection.normalized * Time.deltaTime * speed;

        if(Vector3.Distance(positions[_currentPositionIndex].transform.position, transform.position) <= distanceToBeOnPosition)
        {
            if(shouldStop)
            {
                StartWaitCoroutine();
            }
            NextPatrolStepToSelect();
        }
    }

    private void NextPatrolStepToSelect()
    {
        _startMovementPos = transform.position;
        int random = Random.Range(0, _positionsAvailable.Count);
        if(!isStart)
        {
            //add at last position in list
            _positionsAvailable.Add(_currentPositionIndex);
        }
        else
        {
            isStart = false;
        }

        _currentPositionIndex = _positionsAvailable[random];

        //remove after assigning value to currentIndex to stop index from being 
        _positionsAvailable.RemoveAt(random);


        if (positions[_currentPositionIndex].canStopHere)
        {
            random = Random.Range(0, 101);
            if(random < ChanceToStop)
            {
                shouldStop = true;
            }
        }

    }
    private void StartWaitCoroutine()
    {
        StartCoroutine(WaitForTime());
        isStopped = true;
    }

    private IEnumerator WaitForTime()
    {
        float timeToStop = Random.Range(timeToStopMin, timeToStopMax);
        yield return new WaitForSeconds(timeToStop);
        isStopped = false;
    }

    private void LerpUpAndDown()
    {
        _UpAndDownTimer += Time.deltaTime * characterAnimator.speed;

        _ratio = _UpAndDownTimer / timeToGoUpAndDown;
        _ratioCurve = UpAndDownCurve.Evaluate(_ratio);


        if (_ratio <= 1)
        {
            float YPosition = Mathf.Lerp(_startYPos, _startYPos + AmountToGoVertically, _ratioCurve);
            Vector3 newPos = transform.position;
            newPos.y = YPosition;
            transform.position = newPos;
        }
        else
        {
            _UpAndDownTimer = 0;
        }
    }
    #endregion

    #region stayOnScreen
    private void ReduceStayOnScreenTimer()
    {
        if(!_hasDeterminedTimerDuration)
        {
            _stayOnScreenTimerMax = Random.Range(timeCharacterStaysOnScreenMin, timeCharacterStaysOnScreenMax);
            _hasDeterminedTimerDuration = true;
        }


        _stayOnScreenTimer += Time.deltaTime;
        if(_stayOnScreenTimer >= _stayOnScreenTimerMax)
        {
            //make character disappear or prep him for reutilisation
        }
    }
    #endregion
}
