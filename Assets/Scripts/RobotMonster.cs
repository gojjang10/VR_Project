using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum State { Idle, Walk, Attack, Dead, Size }
public class RobotMonster : MonoBehaviour
{
    [SerializeField] protected State curState = State.Idle;
    [SerializeField] protected BaseRobotMonsterState[] states = new BaseRobotMonsterState[(int)State.Size];
    [SerializeField] protected IdleState idleState;
    [SerializeField] protected WalkState walkState;
    [SerializeField] protected AttackState attackState;
    [SerializeField] protected DeadState deadState;

    // 플레이어
    [SerializeField] Transform player;
    // 탐지 범위
    [SerializeField] float detectionRange = 10f;
    // 걷는 시간
    //[SerializeField] float walkTime = 3f;
    // 걷는 속도
    [SerializeField] float walkSpeed = 3f;

    // 로봇몬스터의 리지드바디
    [SerializeField] Rigidbody rb;

    // 애니메이션을 위한 애니메이터
    [SerializeField] Animator animator;

    // idle 상태의 타이머
    [SerializeField] float idleTimer = 3f;

    // 2레벨 문 열기위한 이벤트
    [SerializeField] UnityEvent secondLevelClear;

    // 총 오브젝트 풀
    [SerializeField] ObjectPool bulletPool;
    [SerializeField] GameObject muzzlePoint;

    private Coroutine shotCoroutine;

    public int hp = 5;


    private void Awake()
    {
        // 스크립트를 적용시킨 오브젝트의 리지드바디 적용
        rb = GetComponent<Rigidbody>();

        // states에 상태들 저장
        states[(int)State.Idle] = idleState;
        states[(int)State.Walk] = walkState;
        states[(int)State.Attack] = attackState;
        states[(int)State.Dead] = deadState;

    }

    private void Update()
    {
        // 현재 상태의 Update함수 실행
        states[(int)curState].Update();
        Debug.Log($"현재상태{curState}");

        if (hp == 0 && curState != State.Dead)
        {
            ChangeState(State.Dead);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Robot"))
        {

            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            ChangeState(State.Idle);
            Debug.Log("벽에 부딪혀서 Idle 상태가 됩니다.");



        }
    }

    [System.Serializable]
    public class BaseRobotMonsterState
    {
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }

    [System.Serializable]
    protected class IdleState : BaseRobotMonsterState
    {
        [SerializeField] RobotMonster monster;

        private float timer;

        public override void Enter()
        {
            // idle 상태에 진입했을때 시간 초기화
            timer = monster.idleTimer;
            monster.animator.Play("Idle");

            monster.transform.rotation = Quaternion.Euler(0, monster.transform.rotation.eulerAngles.y, 0);
        }

        public override void Update()
        {
            // 타이머는 1초씩 줄어든다
            timer -= Time.deltaTime;
            //Debug.Log($"현재 타이머{timer}");

            // 몬스터가 플레이어를 찾았다면
            if (monster.IsPlayerDetected())
            {
                monster.ChangeState(State.Attack);
                return;
            }

            // 타이머가 0이 되었다면
            if (timer <= 0f)
            {
                monster.ChangeState(State.Walk);
            }
        }
    }
    [System.Serializable]
    protected class WalkState : BaseRobotMonsterState
    {
        [SerializeField] RobotMonster monster;

        // 랜덤으로 움직이기 위한 벡터
        private Vector3 walkDirection;

        public override void Enter()
        {
            // 반경을 지정해서 해당 반경 안의 랜덤 위치 선정
            Vector3 randomDirection = Random.insideUnitSphere * 3f;
            // y값은 고정
            randomDirection.y = 0;
            // 목적지 설정
            walkDirection = monster.transform.position + randomDirection;

            monster.animator.SetTrigger("Walk");
        }

        public override void Update()
        {
            // 플레이어를 감지했을때
            if (monster.IsPlayerDetected())
            {
                monster.ChangeState(State.Attack);
                return;
            }

            // 방향벡터로 만들기

            monster.transform.LookAt(walkDirection);
            Vector3 moveDirection = (walkDirection - monster.transform.position).normalized;
            monster.rb.velocity = moveDirection * monster.walkSpeed;

            // 몬스터의 위치와 설정해준 목적지의 사이 거리가 0.1f 아래로 내려간다면
            if (Vector3.Distance(monster.transform.position, walkDirection) < 0.1f)
            {
                monster.ChangeState(State.Idle);
            }
        }

    }
    [System.Serializable]
    protected class AttackState : BaseRobotMonsterState
    {
        [SerializeField] RobotMonster monster;

        public override void Enter()
        {
            monster.animator.SetBool("Attack", true);
        }

        public override void Update()
        {
            if (!monster.IsPlayerDetected())
            {
                monster.animator.SetBool("Attack", false);
                monster.ChangeState(State.Idle);
                return;
            }

            monster.AttackPlayer();
        }
    }

    [System.Serializable]
    protected class DeadState : BaseRobotMonsterState
    {
        [SerializeField] RobotMonster monster;

        public override void Enter()
        {
            monster.animator.SetBool("Dead", true);
            monster.StopCoroutine(monster.shotCoroutine);
            Destroy(monster.gameObject, 2.15f);
            GameManager.Instance.robots -= 1;
        }

        public override void Update()
        {
            if (GameManager.Instance.robots == 0)
            {
                monster.secondLevelClear?.Invoke();
            }
            else
            {
                return;
            }
        }

    }

    public void ChangeState(State nextState)
    {
        states[(int)curState].Exit();
        curState = nextState;
        states[(int)curState].Enter();
    }

    public bool IsPlayerDetected()
    {
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    public void AttackPlayer()
    {
        transform.LookAt(player.position);
        if(shotCoroutine == null)
        {
            shotCoroutine = StartCoroutine(Fire());
        }
        Debug.Log("플레이어를 공격합니다.");
    }

    public IEnumerator Fire()
    {
        while (true)
        {
            PooledObject instance = bulletPool.GetPool(muzzlePoint.transform.position, muzzlePoint.transform.rotation);

            if (instance.TryGetComponent(out Rigidbody rb))
            {
                SetForce(rb);
            }

            yield return new WaitForSeconds(1.4f);
        }
    }

    void SetForce(Rigidbody rb)
    {
        rb.velocity = muzzlePoint.transform.forward * 7;
    }

}
