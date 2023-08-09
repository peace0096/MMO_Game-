using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coPatrol;
    Coroutine _coSearch;
    Coroutine _coSkill;

    [SerializeField]
    Vector3Int _destCellPos;

    [SerializeField]
    GameObject _target;

    // 타겟 서칭 범위
    [SerializeField]
    float _searchRange = 10.0f;

    // 스킬 시전 범위
    [SerializeField]
    float _skillRange = 1.0f;

    [SerializeField]
    bool _rangedSkill = false;


    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;
            if (_coPatrol != null)
            {
                StopCoroutine("CoPatrol");
                _coPatrol = null;
            }
            if (_coSearch != null)
            {
                StartCoroutine("CoSearch");
                _coSearch = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.None;

        _speed = 3.0f;
        _rangedSkill = (Random.Range(0, 2) == 0 ? true : false);

        if (_rangedSkill)
            _skillRange = 10.0f;
        else
            _skillRange = 1.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coPatrol == null)
            _coPatrol = StartCoroutine("CoPatrol");

        if (_coSearch == null)
            _coSearch = StartCoroutine("CoSearch");
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if(_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            // 몬스터가 타겟을 바라보는 방향
            Vector3Int dir = destPos - CellPos;

            // 스킬 시전 범위 안에 있고, 일직선 상에 있을 경우
            if(dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = getDirFromVec(dir);
                State = CreatureState.Skill;

                // 만약 몬스터가 활을 쏠 수 있다면
                if(_rangedSkill)
                    _coSkill = StartCoroutine("CoStartShootArrow");
                else
                    _coSkill = StartCoroutine("CoStartPunch");
                return;
            }

        }

        // 길찾기
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        
        // 길을 못 찾았을 때
        if(path.Count < 2 || (_target != null && path.Count > 20))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }


        Vector3Int nextPos = path[1];   // 어차피 플레이어도 움직여서 다시 계산해야하므로, 임의의 값을 쓰겠다.
        Vector3Int moveCellDir = nextPos - CellPos;
        
        Dir = getDirFromVec(moveCellDir);


        //State = CreatureState.Moving;
        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        // TEMP
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);

    }

    // 몬스터 이동
    IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for(int i =0; i < 10; i++)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            // 해당 위치로 이동할 수 있고, 그 곳에 아무것도 없어야 함.
            if(Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;
        
    }

    IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null)
                continue;

            _target = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange)
                    return false;
                return true;
            });
        }
    }

    // 0.5초동안 상태가 바뀌지 않는다.
    IEnumerator CoStartPunch()
    {
        // 피격 판정. 자기 방향에 오브젝트가 존재하는지.
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            Debug.Log(go.name);
        }

        // 대기 시간
        // = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        _coSkill = null;

    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        ac.Dir = _lastDir;  // 플레이어가 바라보는 방향 = 화살을 쏘는 방향
        ac.CellPos = CellPos;

        // 대기시간
        //_rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        _coSkill = null;
    }
}
