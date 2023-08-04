using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : MonoBehaviour
{
    // 그리드 정보를 얻어올 수 없음.
    //public Grid _grid;
    public float _speed = 5.0f;

    // 직접 정의했음. Define 파일 참고
    MoveDir _dir = MoveDir.None;
    Vector3Int _cellPos = Vector3Int.zero;
    bool _isMoving = false;
    Animator _animator;

    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;

            switch(value)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");   // 스케일을 바꾸면, 반대로 돈다.
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.None:
                    if(_dir == MoveDir.Up)
                    {
                        _animator.Play("IDLE_BACK");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }
                    else if (_dir == MoveDir.Down)
                    {
                        _animator.Play("IDLE_FRONT");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }
                    else if (_dir == MoveDir.Left)
                    {
                        _animator.Play("IDLE_RIGHT");
                        transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    }
                    else if (_dir == MoveDir.Right)
                    {
                        _animator.Play("IDLE_RIGHT");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }
                    else
                    {
                        _animator.Play("IDLE_RIGHT");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }

                    break;
            }

            _dir = value;
        }
    }
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        GetDirInput();
        UpdatePosition();
        UpdateIsMoving();
    }

    // 방향만 키보드로 정하고, 추후에 서버에서 이동을 하도록 할것임.
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += Vector3.up * Time.deltaTime * _speed;
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //transform.position += Vector3.down * Time.deltaTime * _speed;
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //transform.position += Vector3.left * Time.deltaTime * _speed;
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //transform.position += Vector3.right * Time.deltaTime * _speed;
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    // 자연스럽게 움직이도록 처리함.
    void UpdatePosition()
    {
        if (_isMoving == false)
            return;
        
        Vector3 destPos = Managers.Map.CurrentGrid
            
            
            
       .CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);    // 이동 목적지
        Vector3 moveDir = destPos - transform.position;                             // 방향 벡터

        // 도착 여부 체크. moveDir.magnitude는 방향벡터의 크기 = 이동거리
        float dist = moveDir.magnitude;

        // 만약 한번에 이동할만큼 다 왔다면.
        if(dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime;
            _isMoving = true;
        }

    }

    // 움직임 체크. cellPos에 내가 이동할 좌표로 이동
    void UpdateIsMoving()
    {
        if (_isMoving == false && _dir != MoveDir.None)
        {
            Vector3Int destPos = _cellPos;

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

            if (Managers.Map.CanGo(destPos))
            {
                _cellPos = destPos;
                _isMoving = true;
            }
        }
    }
}
