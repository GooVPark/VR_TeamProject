using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class View : MonoBehaviour
{
    public bool isChild = false;
    public abstract void Initialize();

    public virtual void Hide() => gameObject.SetActive(false);
    public virtual void Show() => gameObject.SetActive(true);
}