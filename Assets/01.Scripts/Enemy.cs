using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Header("이동 속도")] private float moveSpeed = 1;
    [SerializeField] private Rigidbody2D rigid;

    private float timer;
    private float moveTimer;
    private Vector2 direction;

    private void Reset()
    {
        rigid = this.GetComponent<Rigidbody2D>();

        if(rigid == null)
        {
            rigid= this.AddComponent<Rigidbody2D>();
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Awake()
    {
        moveTimer = 1 / moveSpeed;
    }

    private void Update()
    {
        timer += Time.smoothDeltaTime;

        if (timer > moveTimer)
        {
            timer = 0;

            var grid = FlowFieldManager.Instance.GetGrid(this.transform.position);
            this.transform.position = grid.position;
            direction = grid.direction;
        }
    }

    private void FixedUpdate()
    {
        rigid.linearVelocity = direction * moveSpeed;
    }
}
