using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : CreatureController
{
    // ��ų�� ����ϴ� ����, ���°� ������ �ʵ��� ��.
    Coroutine _coSkill;
    bool _rangeSkill = false;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateAnimation()
    {
        // ������ ���� ��
        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
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
        // ������ ��
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
        // ��ų ���
        else if (_state == CreatureState.Skill)
        {
            // ��ų�� �ֱٿ� �ٶ� �������� ����ؾ� ��!
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_SIDE");   // �������� �ٲٸ�, �ݴ�� ����.
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_SIDE");
                    _sprite.flipX = false;
                    break;
            }

        }
        else
        {

        }
    }

    protected override void UpdateController()
    {
        switch(State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
        
        base.UpdateController();
    }
    
    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    protected override void UpdateIdle()
    {
        if(Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        // ��ų ��� ��Ʈ�ѷ�
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            //_coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }


    // ���⸸ Ű����� ���ϰ�, ���Ŀ� �������� �̵��� �ϵ��� �Ұ���.
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

    // 0.5�ʵ��� ���°� �ٲ��� �ʴ´�.
    IEnumerator CoStartPunch()
    {
        // �ǰ� ����. �ڱ� ���⿡ ������Ʈ�� �����ϴ���.
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if(go != null)
        {
            Debug.Log(go.name);
        }

        // ��� �ð�
        _rangeSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;

    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = _lastDir;  // �÷��̾ �ٶ󺸴� ���� = ȭ���� ��� ����
        ac.CellPos = CellPos;

        // ���ð�
        _rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hit !");
    }

}
