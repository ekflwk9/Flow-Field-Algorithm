using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;

    private void Reset()
    {
        rigid = this.GetComponent<Rigidbody2D>();

        if(rigid == null)
        {
            rigid= this.AddComponent<Rigidbody2D>();
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) TestFlowFieldManager.Instance.UpdateGrid(this.transform.position);
    }
}
