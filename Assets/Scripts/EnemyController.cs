using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public float range;
    public Transform player;
    public float moveSpeed;
    public float limit = 2;
    Rigidbody rb;
    public Animator animator;
    public int health = 100;
    public bool isDead;
    public Texture monkey;
    public GameObject bodyAsset;
    Renderer m_Renderer;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isDead = false;
        m_Renderer = bodyAsset.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            animator.SetBool("dead", true);
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.useGravity = false;
            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = false; 
            }
            m_Renderer.material.mainTexture  = monkey;
        }
        else
        {
            if (Vector3.Distance(player.position, transform.position) <= range && Vector3.Distance(player.position, transform.position) > limit)
            {
                Vector3 direction = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(direction);
                transform.LookAt(player);
                animator.SetBool("fwd", true);
                animator.ResetTrigger("attack");
                moveSpeed = 15;
            }
            else if (Vector3.Distance(player.position, transform.position) <= limit)
            {
                animator.SetTrigger("attack");
                moveSpeed = 0;
                player.GetComponent<PlayerControls>().TakeDamage(20);
            }
            else
            {
                animator.SetBool("fwd", false);
                animator.ResetTrigger("attack");
                moveSpeed = 15;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
            isDead = true;
    }

}
