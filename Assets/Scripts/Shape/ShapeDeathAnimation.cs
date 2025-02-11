using UnityEngine;

public class ShapeDeathAnimation : MonoBehaviour {
    [SerializeField] private Transform shardsParent;
    [SerializeField] private Rigidbody2D[] shardsRBs;
    [SerializeField] private float burstForce, rotationForce, yDirectionMultipier, xDirectinOffset;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private float activeTime;


    public void Initialize(Transform shape){
        shardsParent.transform.position = shape.position;
        shardsParent.transform.rotation = shape.rotation;
        shardsParent.transform.localScale += shape.lossyScale - Vector3.one;
        ps.transform.position = shape.position;
    }

    public void Start(){
        for(int i=0; i<shardsRBs.Length; i++){
            Vector2 direction = (Vector2)(shardsRBs[i].transform.position - shardsParent.transform.position).normalized;
            direction = new Vector2(direction.x + xDirectinOffset, Mathf.Abs(direction.y) * yDirectionMultipier).normalized;

            shardsRBs[i].AddForce(direction * burstForce, ForceMode2D.Impulse);

            float randomTorque = Random.Range(-rotationForce, rotationForce);
            shardsRBs[i].AddTorque(randomTorque, ForceMode2D.Impulse);
        }
        //ps.Play();
        Destroy(gameObject, activeTime);
    }
}
