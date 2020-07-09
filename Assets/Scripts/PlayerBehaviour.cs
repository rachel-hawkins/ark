using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{


    public bool canMove;                //controls whether the player is static or not

    //horizontal variables
    public float speed = 5.0f;          //changed from within unity
    private float movement = 0f;
    Rigidbody2D rigid;

    //jump variables
    public float jumpSpeed = 5f;
    public float fallMultiplier = 2.5f;
    //double jump variables
    public int noOfJumps = 0;
    public int maxJump = 2;
    //wall jump variables
    public float distance = 1f;         //raycast distance
    public float wallJumpBoost = 30f;

    //grounded variables
    private bool grounded = false;
    public Transform groundCheck;       //checking whether its on the ground (feet)
    const float groundCheckRad = .2f;   //radius around the point
    public LayerMask groundLayer;       //anything we should treat as ground set in unity

    //respawn variables
    public Vector3 respawnPoint;

    //health variables
    public int health;
    public int MaxHealth = 10;
    public int damage = 5;

    //flip variables
    private bool facingRight = true;    //For determining which way the player is currently facing.

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        canMove = true;
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //check the grounding
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRad, groundLayer);

        //horizontal movement
        movement = Input.GetAxis("Horizontal");

        //simple left and right movement
        if (movement != 0 && !Input.GetButtonDown("Jump") && canMove) 
        {
            Move(movement);
        }
        //stops the velocity when controls are not pressed.
        else if (movement == 0 && canMove) 
        {
            Move(0);
        }
        
        //jumping
        if (Input.GetButtonDown("Jump") && canMove)
        {
            //raycast for wall jump
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right*transform.localScale.x,distance);
            if(hit.collider != null && noOfJumps == 1)
            {
                rigid.velocity = new Vector2(0, wallJumpBoost);
                noOfJumps++;
            }
            else
            {
                //simple jump
                Jump();
            }
            
        }
    }

    private void Move(float m)
    {
        rigid.velocity = new Vector2(m * speed, rigid.velocity.y);


        // If the input is moving the player right and the player is facing left...
        if (m > 0 && !facingRight)
        {
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (m < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Jump()
    {
        //reset number of jumps when hits ground
        if (grounded)
        {
            noOfJumps = 0;
        }
        //sets the jump
        if (grounded || noOfJumps < maxJump)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
            if (rigid.velocity.y < 0)
            {
                rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }

            noOfJumps++;
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Fall")
        {
            Respawn();
        }
        else if (collision.tag == "Checkpoint")
        {
            respawnPoint = collision.transform.position;
        }
        else if (collision.tag == "Spike")
        {   
            if (health-damage <= 0)
            {
                Respawn();
            }
            else
            {
                health -= damage;
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position+Vector3.right * transform.localScale.x*distance);
    }

    private void Respawn()
    {
        transform.position = respawnPoint;
        health = MaxHealth;
    }
}
