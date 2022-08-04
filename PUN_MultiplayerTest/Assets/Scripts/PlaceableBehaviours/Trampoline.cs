using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : BasePlaceableBehaviours
{

    public float bouncePower = 5;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.rigidbody.velocity += GetBounceDirection(collision.rigidbody.velocity);
    }

    protected Vector2 GetBounceDirection(Vector2 otherVelocity)
    {
        switch(occupation.orientation)
        {
            case 0:
            case 2:
                return new Vector2(0, -otherVelocity.y * bouncePower);
            case 1:
            case 3:
                return new Vector2(otherVelocity.x * bouncePower,0);
            default: 
                return default;
        }
    }

}
