using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditingDynamicSkills : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public DynamicSkillsDisplayPanel DynamicSkillsPanel;
    public DynamicSkillCell MovingCell;
    private int firstCellIndex;
    private bool active = false; //Возможно переработать

    public void SetEditMode(bool value)
    {
        active = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (active)
        {
            Vector3 position = eventData.position;
            position.z = 90;
            position = Camera.main.ScreenToWorldPoint(position);
            firstCellIndex = findCloserCellIndex(position);
            MovingCell.SetSkillImage(DynamicSkillsPanel.Cells[firstCellIndex].SkillImage.sprite);
            MovingCell.gameObject.SetActive(true);
            MovingCell.transform.position = position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (active)
        {
            Vector3 position = eventData.position;
            position.z = 90;
            position = Camera.main.ScreenToWorldPoint(position);
            MovingCell.transform.position = position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (active)
        {
            Vector3 position = eventData.position;
            position.z = 90;
            position = Camera.main.ScreenToWorldPoint(position);
            int secondCellIndex = findCloserCellIndex(position);
            DynamicSkillsPanel.ChangeCells(firstCellIndex, secondCellIndex);
            MovingCell.gameObject.SetActive(false);
        }
    }

    private int findCloserCellIndex(Vector3 position)
    {
        int closerCellIndex = 0;
        float closerDistance = Vector3.Distance(position, DynamicSkillsPanel.Cells[0].transform.position);
        for(int i=0; i < DynamicSkillsPanel.Cells.Count; i++)
        {
            float distance = Vector3.Distance(position, DynamicSkillsPanel.Cells[i].transform.position);
            if (distance < closerDistance)
            {
                closerCellIndex = i;
                closerDistance = distance;
            }
        }
        return closerCellIndex;
    }
}
