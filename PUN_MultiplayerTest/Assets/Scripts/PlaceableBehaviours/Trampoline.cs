using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : BasePlaceableBehaviours
{

    public float bouncePower = 5;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.rigidbody.velocity = GetBounceDirection(collision.relativeVelocity);
    }

    protected Vector2 GetBounceDirection(Vector2 otherVelocity)
    {
        switch(occupation.orientation)
        {
            case 0:
            case 2:
                return new Vector2(otherVelocity.x, -otherVelocity.y * bouncePower);
            case 1:
            case 3:
                return new Vector2(-otherVelocity.x * bouncePower, otherVelocity.y);
            default: 
                return default;
        }
    }

}
