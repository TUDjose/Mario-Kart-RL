#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KartController: MonoBehaviour
{
   private Rigidbody? rb;
   
   [Header("Move")]
   public List<TrackCondition> trackSpeeds = new ()
   {
      new TrackCondition("Track", 40f, 3f),
      new TrackCondition("Grass", 15f, 2f),
      new TrackCondition("Speed", 100f, 20f)
   };
   public float realSpeed;
   [SerializeField] private float maxSpeed;
   [SerializeField] private float acceleration = 3f;
   [SerializeField] private float boostTime = 1f; 
   private float currentSpeed = 0;
   private bool inBoost;
   
   [Header("Steer")] 
   public float steerAmount;
   public float steerEffectiveness = 2f;
   public bool touchingGround;
   
   public string currTag;
   public List<float> inputs = new() {0f, 0f};

   [Header("Raycasts")]
   [SerializeField] private List<Transform> raycastsPoints;
   

   private void Start()
   {
      rb = GetComponent<Rigidbody>();
      Physics.IgnoreLayerCollision(2,2);
   }
   
   private void FixedUpdate()
   {
      DetermineTrackConditions();
      if (touchingGround)
      {
         Move();
      }
      Steer();
      GroundNormalRotation();
   }
   
   public void SetInputs(float v, float h)
   {
      inputs[0] = v;
      inputs[1] = h;
   }
   
   private void Move()
   {
      realSpeed = transform.InverseTransformDirection(rb.velocity).z;
   
      if (inputs[0] > 0)
      {
         currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * acceleration);
      }
      else if (inputs[0] < 0)
      {
         currentSpeed = Mathf.Lerp(currentSpeed, -maxSpeed / 2f, Time.deltaTime * 1f);
      }
      else
      {
         float decceleration = 1.5f;
         // if (Input.GetKey(KeyCode.Space)) decceleration = 5f;
         currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * decceleration);
      }
      
   
      Vector3 vel = transform.forward * currentSpeed;
      vel.y = rb.velocity.y;
      rb.velocity = vel;
   }
   
   private void Steer()
   {
      steerAmount = realSpeed switch
      {
         float n when n < 5 => inputs[1],
         float n when n > 45 => 45 / steerEffectiveness * inputs[1],
         float n when n > 30 && n < 46 => realSpeed / steerEffectiveness * inputs[1],
         float n when n < 30 => 35f * inputs[1],
         _ => steerAmount
      };

      Vector3 steerDirVect = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount,
         transform.eulerAngles.z);
      transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, steerDirVect, Time.deltaTime * 3f);
   }
   
   private void GroundNormalRotation()
   {
      RaycastHit hit;
      if (Physics.Raycast(transform.position, -Vector3.up, out hit, 0.75f))
      {
         transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 8f * Time.deltaTime);
         touchingGround = true;
      }
      else
      {
         touchingGround = false;
      }
   }
   
   private void DetermineTrackConditions()
   {
      List<string> tags = new();
      foreach (Transform point in raycastsPoints)
      {
         if (Physics.Raycast(point.position, Vector3.down, out RaycastHit hit, 1f,
                Physics.DefaultRaycastLayers,QueryTriggerInteraction.Collide))
         {
            tags.Add(hit.collider.gameObject.tag); 
         }
      }

      if (tags.All(s => s == "Track"))
      {
         currTag = "Track";
      }
      else if (tags.Any(s => s == "Speed"))
      {
         currTag = "Speed";
         if (!inBoost) StartCoroutine(SpeedBoost());
      }
      else if (tags.Any(s => s == "Grass") && tags.All(s => s != "Speed"))
      {
         currTag = "Grass";
      }

      if (!inBoost)
      {
         TrackCondition track = trackSpeeds.FirstOrDefault(s => s.condition == currTag) ?? TrackCondition.Default;
         maxSpeed = track.maxSpeed;
         acceleration = track.acceleration; 
      }
   }

   private IEnumerator SpeedBoost()
   {
      inBoost = true;
      TrackCondition track = trackSpeeds.FirstOrDefault(s => s.condition == currTag) ?? TrackCondition.Default;
      maxSpeed = track.maxSpeed;
      acceleration = track.acceleration;
      yield return new WaitForSeconds(boostTime);
      inBoost = false;
   }

   public void StopKart()
   {
      rb.isKinematic = true;
      inBoost = false;
      rb.rotation = Quaternion.Euler(0f, 0f, 0f);
      rb.isKinematic = false;
      currentSpeed = Mathf.Lerp(currentSpeed, 0, 1f);
      Vector3 vel = transform.forward * currentSpeed;
      vel.y = rb.velocity.y;
      rb.velocity = vel;
   }
}


[Serializable]
public class TrackCondition
{
   public string condition;
   public float maxSpeed;
   public float acceleration;

   public TrackCondition(string condition, float maxSpeed, float acceleration)
   {
      this.condition = condition;
      this.maxSpeed = maxSpeed;
      this.acceleration = acceleration;
   }

   public static readonly TrackCondition Default = new ("Track", 40f, 3f);
}