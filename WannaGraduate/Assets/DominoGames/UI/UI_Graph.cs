using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Graph : MonoBehaviour
{
    public void SetValue(float val)
    {
        transform.localScale = new Vector3(Mathf.Clamp01(val), 1f, 1f);
    }
}
