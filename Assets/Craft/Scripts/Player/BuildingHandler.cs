using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    public static BuildingHandler Instance;

    [Header("Block")]
    public GameObject blockPrefabChoosed;
    public GameObject helpBlockView;
    public Camera Camera;

    [Header("Raycast Block")]
    private const float raycastDistance = 20.0f;
    private int layerMask = 1 << 6;
    public GameObject blockHit;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
    }

    public void BuildControll()
    {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.yellow);
            var position = Vector3.zero;

            if (hit.collider.transform.position.x - hit.point.x >= 0.5f)
                position = new Vector3(hit.collider.transform.position.x - 1.0f, hit.collider.transform.position.y, hit.collider.transform.position.z);
            else if (hit.collider.transform.position.x - hit.point.x <= -0.5f)
                position = new Vector3(hit.collider.transform.position.x + 1.0f, hit.collider.transform.position.y, hit.collider.transform.position.z);
            else if (hit.collider.transform.position.y - hit.point.y >= 0.5f)
                position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y - 1.0f, hit.collider.transform.position.z);
            else if (hit.collider.transform.position.y - hit.point.y <= -0.5f)
                position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1.0f, hit.collider.transform.position.z);
            else if (hit.collider.transform.position.z - hit.point.z >= 0.5f)
                position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z - 1.0f);
            else if (hit.collider.transform.position.z - hit.point.z <= -0.5f)
                position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z + 1.0f);

            Instantiate(blockPrefabChoosed, position, Quaternion.identity);

        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.white);
        }
    }

    public void DestroyControll()
    {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            Destroy(hit.collider.gameObject);
        }
    }


    public void HelpBuild()
    {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            if (!helpBlockView.activeSelf)
                helpBlockView.SetActive(true);

            helpBlockView.transform.position = hit.collider.transform.position;
            blockHit = hit.collider.gameObject;
        }
        else
        {
            blockHit = null;
            if (helpBlockView.activeSelf)
                 helpBlockView.SetActive(false);
        }
    }
}
