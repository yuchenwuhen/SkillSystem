using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = this.GetComponent<Animator>();
    }

    
    public void SetVariable(string name,float value)
    {
        m_animator.SetFloat(name, value);
    }

    public void SetVariable(string name, bool value)
    {
        m_animator.SetBool(name, value);
    }

    public void SetVariable(string name, int value)
    {
        m_animator.SetInteger(name, value);
    }

}
