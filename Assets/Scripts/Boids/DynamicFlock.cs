using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

///// DISCLAIMER: This is not mine

public class DynamicFlock : MonoBehaviour
{
    public float speed_;
    public Vector3Int size_;
    public GameObject[] prefabs_;
    public Transform area_;

    private GameManager manager_;
    private List<Boid> boids_;

    private Alignment alignment_;
    private Cohesion cohesion_;
    private Separation separation_;

    private void Awake()
    {
        alignment_ = new Alignment();
        cohesion_ = new Cohesion();
        separation_ = new Separation();

        boids_ = new List<Boid>();
    }

    private void Start()
    {
        StartCoroutine(spawnBoids());
    }

    private IEnumerator spawnBoids()
    {
        while(true)
        {
            for(int i = 0; i < speed_ / 100; ++i)
            {
                spawnBoid(new Vector3((area_.position.x - size_.x / 2) + UnityEngine.Random.value * size_.x, (area_.position.y - size_.y / 2) + UnityEngine.Random.value * size_.y, (area_.position.z - size_.z / 2) + UnityEngine.Random.value * size_.z));
            }
            yield return new WaitForSeconds(15 / (speed_ + 1));
        }
    }

    private void Update()
    {
        for(int i = boids_.Count - 1; i >= 0; --i)
        {
            Boid b = boids_[i];
            Vector3 pos = b.transform.position;

            if((size_.x > 0 && (pos.x <= area_.position.x - size_.x / 2 || pos.x >= area_.position.x + size_.x / 2)) ||
                    (size_.y > 0 && (pos.y <= area_.position.y - size_.y / 2 || pos.y >= area_.position.y + size_.y / 2)) ||
                    (size_.z > 0 && (pos.z <= area_.position.z - size_.z / 2 || pos.z >= area_.position.z + size_.z / 2)))
            {
                GameObject.Destroy(b.gameObject, 0.5f);
                boids_.RemoveAt(i);
                b = null;
            }
        }

        new Thread(() =>
        {
            for(int i = boids_.Count - 1; i >= 0; --i)
            {

                Boid b = boids_[i];
                Vector3 velocity = b.velocity;

                velocity += alignment_.getResult(boids_, i);
                velocity += cohesion_.getResult(boids_, i);
                velocity += separation_.getResult(boids_, i);

                velocity.Normalize();
                b.velocity = velocity;

                b.lookat = b.position + velocity;
            }
        }).Start();

        for(int i = boids_.Count - 1; i >= 0; --i)
        {
            boids_[i].transform.position += boids_[i].velocity * Time.deltaTime * speed_;
        }
    }

    private void spawnBoid(Vector3 position)
    {
        int index = (int)(UnityEngine.Random.value * prefabs_.Length);
        GameObject gobj = prefabs_[index];
        
        gobj = Instantiate(gobj, position, Quaternion.identity) as GameObject;
        gobj.transform.parent = area_;
        
        Vector3 moveTo = area_.position;
        Vector3 dir = moveTo - position;
        
        Boid b = gobj.AddComponent<Boid>();
        b.velocity = dir.normalized;

        boids_.Add(b);
    }
}
