// Gère le comportement des voitures
// Avance de base, mais lorsqu'il detecte une voiture devant ou derrière
// et s'immobilise.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voiture : MonoBehaviour
{
    // variables
    public float lowRandom = 0.02f;
    public float highRandom = 0.1f;
    private bool isRolling = true;
    public float boost = 1;
    private float coolDown = 0f;
    public float speed = 1;
    public int coolDownPeriodInSeconds = 8;
    public Renderer rend;
    public Collider colliderEnable;
    Random rnd = new Random();
    Rigidbody rbVoiture;
    public bool crazyPhysics = true;
    public bool capsuleCol = false;
    public Vector3 endPoint;
    public float currentSpeed = 0f;

    void Awake()
    {
        // generates random colors for cars.
        setColorCars();
        // // enables crazy physics
        if (crazyPhysics)
        {
            if (!capsuleCol)
                GetComponent<BoxCollider>().isTrigger = false;
            if (capsuleCol)
                GetComponent<CapsuleCollider>().isTrigger = false;
        }
        if (!crazyPhysics)
        {
            if (!capsuleCol)
                GetComponent<BoxCollider>().isTrigger = true;
            if (capsuleCol)
                GetComponent<CapsuleCollider>().isTrigger = true;
        }
    }

    // PROPULSE L'AUTO
    void Update()
    {
        //newSpeedControl();
        checkIfRolling();
        // boost = 1;
        //callRandomStop();
        //callRandomBoost();
    }    

    // =================================================== DÉTECTIONS DES COLLISIONS
    // si ca touche d'autres autos
    void OnTriggerEnter(Collider other)
    {
        
        // le devant touche le derrière d'une auto
        if (other.gameObject.tag == "back")
        {
            freezeConstraint();
            isNotRollingMethod();
            newStoppingControl();
        }

        // si l<auto se fait entrer dedans
        if (other.gameObject.tag == "front")
        {
            unfreezeConstraint();
            coolDown = 0;
            isRollingMethod();
            //boost = 10;
            //newSpeedControl();
        }

        // elimine duplicata
        // does it work?
        if (other.gameObject.tag == "carDeath")
        {
            rend = GetComponent<Renderer>();
            rend.enabled = false;
        }  

        // detruit la voiture
        if (other.gameObject.tag == "destructionWall")
        {
            Destroy(gameObject);
        }
    }

    // si ca reste en contact
    void OnTriggerStay(Collider other)
    {
    }

    // si ca quitte
    void OnTriggerExit(Collider other)
    {
        // le devant ne touche plus le derrière d'une auto
        isRollingMethod();
        //newSpeedControl();
    }

    // ============================================== METHODS

    void setColorCars()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void checkIfRolling()
    {
        
        // check is car is either rolling or not.
        if (isRolling)
        {
            Debug.Log("isRolling");
            isRollingMethod();
            //newSpeedControl();  
        }
        if (!isRolling)
        {
            isNotRollingMethod();
            //newStoppingControl();
        }
    }
    void isRollingMethod()
    {
        // check pour le cool down, si c'est ok
        // refait mouvoir l'auto
        if (coolDown <= Time.time)
        {       
            unfreezeConstraint();
            changeSpeedOfCarRolling();
            isRolling = true;
        }
    }

    void isNotRollingMethod()
    {
        // cool down
        if (coolDown <= Time.time)
        {
            coolDown = Time.time + coolDownPeriodInSeconds;
        }
        freezeConstraint();        
        isRolling = false;
    }

    void freezeConstraint()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        float rndBoost = Random.Range(0.6f, 1.6f);
        boost = rndBoost;
        changeSpeedOfCarAfterStopping();
    }

    void unfreezeConstraint()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        float rndBoost = Random.Range(0.6f, 1.6f);
        boost = rndBoost;
    }

    void completeStop()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;        
    }

    void changeSpeedOfCarRolling()
    {
        float fluctuation = Random.Range(lowRandom, highRandom);      
        transform.Translate(Vector3.right * fluctuation * boost); 
    }

    void changeSpeedOfCarAfterStopping()
    {
        transform.Translate(Vector3.right * 0.05f);
    }

    void callRandomStop()
    {
        // car have a .05% chance of stopping
        float rndStopping = Random.Range(0f, 100f);
        if (rndStopping > 99.95f)
        {
            //isNotRollingMethod();
            newStoppingControl(); 
            Debug.Log("random stop has been called");   
        }
    }

    void callRandomBoost()
    {
        // car have a .2% chance of starting again
        float rndStopping = Random.Range(0f, 100f);
        if (rndStopping < 0.2f)
        {
        //     unfreezeConstraint();
        //     coolDown = 0;
        //     isRollingMethod();
            newSpeedControl();
            Debug.Log("random boost has been called");
        }
    }

    void newSpeedControl()
    {
        // check pour le cool down, si c'est ok
        // refait mouvoir l'auto
        if (coolDown <= Time.time)
        {
            float t = 0.0f;
            float tempPosition = (Mathf.Lerp(lowRandom, highRandom, t));
            currentSpeed = transform.position.x * tempPosition + boost;
            transform.Translate(Vector3.right * tempPosition);

            
            // .. and increase the t interpolater
            t += 0.5f * Time.deltaTime;

            // now check if the interpolator has reached 1.0
            // and swap maximum and minimum so game object moves
            // in the opposite direction.
            if (t > 1.0f)
            {
                float temp = highRandom;
                highRandom = lowRandom;
                lowRandom = temp;
                t = 0.0f;
            }
        }
    }

    void newStoppingControl()
    {
        // cool down
        if (coolDown <= Time.time)
        {
            coolDown = Time.time + coolDownPeriodInSeconds;
        }
        // slowdown to 0
        float t = 1f;        
        float tempPosition = (Mathf.Lerp(currentSpeed, 0.05f, t));
        t -= 0.02f;
    }
}
