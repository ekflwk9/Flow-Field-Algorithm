using UnityEngine;

public class Target : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 10f;

            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;

            this.transform.position = worldPos;
            FlowFieldManager.Instance.SetTarget(worldPos);
        }
    }
}
