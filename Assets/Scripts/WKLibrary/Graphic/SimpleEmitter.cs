using UnityEngine;
using WK.Utils;

namespace WK { namespace Graphic {

public class SimpleEmitter : MonoBehaviour {
    public GameObject particlePrehab;
    public float      lifeTime         = 3.0f;
    public float      particleLifeTime = 3.0f;
    public int        numParticle      = 8;
    public float      velMax           = 200.0f;
    public float      velMin           = 50.0f;
    public float      velAlpha         = -1.0f;

	// Use this for initialization
	void Start () {
        Emit();
	}

	// Update is called once per frame
	void Update () {
        lifeTime -= Time.deltaTime;
        if( lifeTime < 0.0f )
        {
            Destroy( gameObject );
        }
	}

	public virtual void Emit () {
        for( int i = 0; i < numParticle; ++i )
        {
            GameObject particle = Instantiate( particlePrehab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            SimpleParticle particle_component = particle.AddComponent<SimpleParticle>();
            particle_component.vel = new Vector3(
                    WK.Utils.Utils.GetRandomSign() * Random.Range( velMin, velMax )
                    , WK.Utils.Utils.GetRandomSign() * Random.Range( velMin, velMax )
                    , 0.0f
                    );
            particle_component.lifeTime = particleLifeTime;
            particle_component.velAlpha = velAlpha;
            particle.transform.SetParent( transform );
            particle.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );
            particle.transform.localScale = new Vector3( 1.0f, 1.0f, 1.0f );
        }
    }
}

}}
