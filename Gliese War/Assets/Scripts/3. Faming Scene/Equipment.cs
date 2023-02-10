using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    
    public Item.ItemRank grade;
    public int attackPower;
    public int physicalStrength;

    public void equip_generate()
    {
        // Randomly determine the equipment grade
        int gradeIndex = Random.Range(0, 5);
        grade = (Item.ItemRank)gradeIndex;

        // Randomly determine the attack power and physical strength within specified ranges
        attackPower = Random.Range(1, 31);
        physicalStrength = Random.Range(1, 101);

        Debug.Log(this.ToString());
    }

    public float rotateSpeed;

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    public override string ToString()
    {
        return "Grade: " + grade + ", Attack Power: " + attackPower + ", Physical Strength: " + physicalStrength;
    }
}