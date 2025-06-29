using UnityEngine;

public class Target : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = -10;

            this.transform.position = mouse;
            TestFlowFieldManager.Instance.SetTarget(mouse);
        }
    }
}
