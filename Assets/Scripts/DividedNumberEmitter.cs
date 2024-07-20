using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using WK.Utils;

public class DividedNumberEmitter : SimpleEmitter {
    private int number;//a number which will be written after emitting
    private float rotateY = 0.0f;
    private float velRotateY = 0.0f;
    public float birthSize = 300.0f;
    private Color color = Color.black;
    private Sprite sprite = null;

    /* const int cStateIdle    = 0; */
    /* const int cStateExplode = 1; */
    /* const int cStateRotate  = 2; */

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
        lifeTime -= Time.deltaTime;
        if( lifeTime < 0.0f )
        {
            Destroy( gameObject );
        }

        rotateY += velRotateY * Time.deltaTime;
        transform.localRotation = Quaternion.Euler( 0.0f, rotateY, 0.0f );
	}

	public void SetParticle ( int num_dvided, int num ) {
        numParticle = num_dvided;
        number = num;
    }

	public void SetColor( Color c ) {
        color = c;
    }

	public void SetSprite( Sprite s ) {
        sprite = s;
    }

	public override void Emit() {
        for( int i = 0; i < numParticle; ++i )
        {
            GameObject particle = Instantiate( particlePrehab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            particle.GetComponentInChildren<Image>().color = color;
            SimpleParticle particle_component = particle.AddComponent<SimpleParticle>();
            Vector3 vel = birthEmitPos( numParticle, i );
            particle_component.vel = vel * 10.0f;
            particle_component.damping = 0.8f;
            particle_component.lifeTime = particleLifeTime;
            particle_component.velAlpha = velAlpha;
            if( sprite != null )
            {
                particle_component.SetSprite( sprite );
            }

            float scale = Math.Max( 0.35f, 1.0f / (float)( Math.Sqrt( numParticle ) ) );
            particle.transform.SetParent( transform );
            particle.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );
            particle.transform.localScale    = new Vector3( scale, scale, scale );

            particle.GetComponentInChildren<Text>().text = number.ToString();
        }
    }

    private Vector3 birthEmitPos( int num_particle, int index )
    {
        if( num_particle <= 10 )
        {
            float angle = Mathf.PI * 2.0f / num_particle * index + Mathf.PI / 2.0f;
            return new Vector3( Mathf.Cos( angle ) * birthSize, Mathf.Sin( angle ) * birthSize, 0.0f );
        }
        else
        {
            float size = UnityEngine.Random.Range( birthSize / 2.0f, birthSize * 2.0f );
            float angle = Mathf.PI * 2.0f / num_particle * index + Mathf.PI / 2.0f;
            return new Vector3( Mathf.Cos( angle ) * size, Mathf.Sin( angle ) * size, 0.0f );
        }

    }

}
