using UnityEngine;
using UnityEngine.EventSystems;

public class ClickParticle : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ParticleSystem clickParticle;

    private void Update()
    {
        OnMouseClickPartical();
    }

    public void OnMouseClickPartical()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            clickParticle.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2.0f);
            clickParticle.Play();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            clickParticle.Stop();
        }
#elif UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector3 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
                        clickParticle.transform.position = new Vector3(touchPosition.x, touchPosition.y, -2.0f);
                        clickParticle.Play();
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        clickParticle.Stop();
                    }
                }
            }
#endif
    }
}
