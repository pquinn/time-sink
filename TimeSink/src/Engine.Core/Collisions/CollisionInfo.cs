using System;
using Microsoft.Xna.Framework;


// Structure that stores the results of the PolygonCollision function
public struct CollisionInfo 
{
    // Are the polygons currently intersecting?
    public bool Intersect { get; set; }

    // The translation to apply to the first polygon to push the polygons apart.
    public Vector2 MinimumTranslationVector;

    public static CollisionInfo NoCollision = new CollisionInfo();
}