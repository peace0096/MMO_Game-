using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : CreatureController
{
    // ��ų�� ����ϴ� ����, ���°� ������ �ʵ��� ��.
    Coroutine _coSkill;



    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        switch(State)
        {
            case CreatureState.Idle:
                GetDirInput();
                GetIdleInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
        
        base.UpdateController();
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

    void GetIdleInput()
    {
        // ��ų ��� ��Ʈ�ѷ�
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            _coSkill = StartCoroutine("CoStartPunch");
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
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;

    }

}
