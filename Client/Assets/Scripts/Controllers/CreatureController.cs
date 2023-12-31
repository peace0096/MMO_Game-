using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    // 그리드 정보를 얻어올 수 없음.
    //public Grid _grid;
    [SerializeField]
    public float _speed = 5.0f;

    // 직접 정의했음. Define 파일 참고
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    [SerializeField]
    protected CreatureState _state = CreatureState.Idle;
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;

            // 상태가 바뀌면 애니메이션 업데이트
            UpdateAnimation();
        }
    }

    protected MoveDir _dir = MoveDir.Down;
    [SerializeField]
    protected MoveDir _lastDir = MoveDir.Down;

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }


    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;

            _dir = value;
            if(value != MoveDir.None)
            {
                _lastDir = value;
            }

            // 방향이 바뀌면 애니메이션 업데이트
            UpdateAnimation();
        }
    }

    public MoveDir getDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else if (dir.y < 0)
            return MoveDir.Down;
        else
            return MoveDir.None;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch(_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;

        }
        return cellPos;
    }

    protected virtual void UpdateAnimation()
    {
        if (_state == CreatureState.Idle)
        {
            switch(_lastDir)
            {
                case (MoveDir.Up):
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;

                case (MoveDir.Down):
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;

                case (MoveDir.Left):
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;

                case (MoveDir.Right):
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            
            }
            
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");   // 스케일을 바꾸면, 반대로 돈다.
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }

        // 스킬 사용
        else if (_state == CreatureState.Skill)
        {
            // 스킬은 최근에 바라본 방향으로 사용해야 함!
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_SIDE");   // 스케일을 바꾸면, 반대로 돈다.
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_SIDE");
                    _sprite.flipX = false;
                    break;
            }
            
        }
        else
        {

        }
    }

    void Start()
    {
        Init();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;


        }

    }

    // 움직임 체크. cellPos에 내가 이동할 좌표로 이동
    protected virtual void UpdateIdle()
    {
    }

    // 자연스럽게 움직이도록 처리함.
    protected virtual void UpdateMoving()
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);    // 이동 목적지
        Vector3 moveDir = destPos - transform.position;                             // 방향 벡터

        // 도착 여부 체크. moveDir.magnitude는 방향벡터의 크기 = 이동거리
        float dist = moveDir.magnitude;

        // 만약 한번에 이동할만큼 다 왔다면.
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }

    }

    protected virtual void MoveToNextPos()
    {
        if(_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }

        Vector3Int destPos = CellPos;

        switch (_dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        //State = CreatureState.Moving;
        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;

            }

        }
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged()
    {

    }

}
