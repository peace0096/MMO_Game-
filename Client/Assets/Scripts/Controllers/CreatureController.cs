using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    // �׸��� ������ ���� �� ����.
    //public Grid _grid;
    public float _speed = 5.0f;

    // ���� ��������. Define ���� ����
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    CreatureState _state = CreatureState.Idle;
    public CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;

            // ���°� �ٲ�� �ִϸ��̼� ������Ʈ
            UpdateAnimation();
        }
    }

    MoveDir _dir = MoveDir.Down;
    MoveDir _lastDir = MoveDir.Down;

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

            // ������ �ٲ�� �ִϸ��̼� ������Ʈ
            UpdateAnimation();
        }
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
                    _animator.Play("WALK_RIGHT");   // �������� �ٲٸ�, �ݴ�� ����.
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            // TODO 
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
        UpdatePosition();
        UpdateIsMoving();
    }

    // �ڿ������� �����̵��� ó����.
    void UpdatePosition()
    {
        if (State != CreatureState.Moving)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);    // �̵� ������
        Vector3 moveDir = destPos - transform.position;                             // ���� ����

        // ���� ���� üũ. moveDir.magnitude�� ���⺤���� ũ�� = �̵��Ÿ�
        float dist = moveDir.magnitude;

        // ���� �ѹ��� �̵��Ҹ�ŭ �� �Դٸ�.
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            _state = CreatureState.Idle;

            // Ű���忡�� ���� �� ���¶��..
            if(_dir == MoveDir.None)
                UpdateAnimation();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving;
        }

    }

    // ������ üũ. cellPos�� ���� �̵��� ��ǥ�� �̵�
    void UpdateIsMoving()
    {
        if (State == CreatureState.Idle && _dir != MoveDir.None)
        {
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

            State = CreatureState.Moving;
            if (Managers.Map.CanGo(destPos))
            {
                if(Managers.Object.Find(destPos) == null)
                {
                    CellPos = destPos;
                    
                }
 
            }
        }
    }
}
