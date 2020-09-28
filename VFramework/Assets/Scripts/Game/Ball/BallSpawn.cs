using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{

    [SerializeField]
    private Transform m_dirTransform;

    [SerializeField]
    private float m_strength = 5;

    [SerializeField]
    private float m_spawnTime = 2;

    private float m_tempTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_tempTime += Time.deltaTime;
        if (m_tempTime >= m_spawnTime)
        {
            var m_ball = Resources.Load("Sphere") as GameObject;
            var ball = Instantiate(m_ball,m_dirTransform.up,Quaternion.identity);
            ball.transform.position = m_dirTransform.position;
            ball.GetComponent<Rigidbody>().AddForce(-m_dirTransform.forward * m_strength);
            m_tempTime = 0;
        }
    }


}
