using UnityEngine;

public class TapInfo : MonoBehaviour
{
    [SerializeField] private GameObject description;

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ToggleDescription();
                }
            }
        }
    }

    void ToggleDescription()
    {
        if (description != null)
        {
            description.SetActive(!description.activeSelf); 
        }
    }
}
