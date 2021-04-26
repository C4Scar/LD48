using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private float rayTimer;
    public bool raycastLock;
    public GameObject hoveredObject;
    public GameObject previousHovered;
    // Start is called before the first frame update
    void Start()
    {
        //raycastLock = true;
        previousHovered = null;
    }
    // Update is called once per frame
    void Update()
    {
        if(GameMaster.instance.gameStartedBool)
        {
            rayTimer += Time.deltaTime;
            if (rayTimer >= 0.1)
            {
                rayTimer = 0;
                hoveredObject = CastRay();
                if (hoveredObject != null)
                {
                    if(previousHovered == null)
                    {
                        previousHovered = hoveredObject;
                        previousHovered.GetComponent<Territory>().selectionIndicator.SetActive(true);
                        SelectionIndicator.instance.UpdateHoverUI(previousHovered);
                    }
                    else
                    {
                        if (previousHovered.TryGetComponent<Territory>(out Territory previousTerritory))
                        {
                            previousTerritory.selectionIndicator.SetActive(false);
                        }
                        previousHovered = hoveredObject;
                        previousHovered.GetComponent<Territory>().selectionIndicator.SetActive(true);
                        SelectionIndicator.instance.UpdateHoverUI(previousHovered);
                    }
                }
            }
            if (GameMaster.instance.globalInteractionFlag)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    LeftClick(hoveredObject);
                }
                if (Input.GetMouseButtonUp(1))
                {
                    RightClick(hoveredObject);
                }
            }
        }
    }
    private GameObject CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.collider.gameObject;
            }
            else
            {
                Debug.Log(hit.collider.name);
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    public void LeftClick(GameObject hoveredObject)
    {
        Debug.Log("Left click");
        if(hoveredObject != null)
        {
            if (hoveredObject.TryGetComponent<Territory>(out Territory terrHovered))
            {
                terrHovered.FundRebels(GameMaster.instance.players[0]);
                //terrHovered.checkers1.color = GameMaster.instance.players[0].playerMaterial.color;
                //terrHovered.checkers1.gameObject.SetActive(true);
            }
        }
    }
    public void RightClick(GameObject hoveredObject)
    {
        Debug.Log("Right click");
        if (hoveredObject != null)
        {
            if (hoveredObject.TryGetComponent<Territory>(out Territory terrHovered))
            {
                terrHovered.Attack(GameMaster.instance.players[0]);
                //terrHovered.checkers1.color = GameMaster.instance.players[0].playerMaterial.color;
                //terrHovered.checkers1.gameObject.SetActive(true);
            }
        }
    }
    public void UnlockRaycast()
    {
        raycastLock = false;
    }
}
