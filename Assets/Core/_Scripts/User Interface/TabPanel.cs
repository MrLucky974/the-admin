using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPanel : MonoBehaviour
{
    public void Toggle(bool selected)
    {
        gameObject.SetActive(selected);
    }
}
