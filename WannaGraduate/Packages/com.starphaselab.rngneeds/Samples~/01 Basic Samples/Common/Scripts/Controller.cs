using UnityEngine;

namespace RNGNeeds.Samples
{
    public class Controller : MonoBehaviour
    {
        public UnitWidget unitWidget;
        public GameObject moveTargetIndicatorPrefab;
        public GameObject interactTargetIndicatorPrefab;
        private Unit selectedUnit;
        private Camera m_Camera;

        private void Awake()
        {
            m_Camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
                {
                    Unit unit = hit.collider.gameObject.GetComponent<Unit>();
                    if (unit)
                    {
                        if (selectedUnit != null) selectedUnit.ToggleSelection(false);

                        selectedUnit = unit;
                        unitWidget.SetUnit(unit);
                        selectedUnit.SelectCommand();
                    }
                    else
                    {
                        if (selectedUnit != null)
                        {
                            unitWidget.ClearWidget();
                            selectedUnit.ToggleSelection(false);
                            selectedUnit = null;
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity) && selectedUnit != null)
                {
                    Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
                    if (interactable)
                    {
                        Instantiate(interactTargetIndicatorPrefab, interactable.gameObject.transform.position, Quaternion.identity);
                        selectedUnit.InteractCommand(interactable);
                    }
                    else
                    {
                        selectedUnit.MoveCommand(hit.point);
                        Instantiate(moveTargetIndicatorPrefab, hit.point, Quaternion.identity);
                    }
                }
            }
        }
    }
}