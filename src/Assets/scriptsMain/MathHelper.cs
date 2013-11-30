using UnityEngine;
using System.Collections;
using Pathfinding;

public static class MathHelper {

    // Use this for initialization
    
    public static Vector3 truncateLength(Vector3 tVector, float maxLength)
    {
        float tLength = tVector.magnitude;
        Vector3 returnVector = tVector;
        if (tLength > maxLength)
        {
            //Debug.Log ("tLenght:" + tLength + " max:" + maxLength);
            returnVector.Normalize ();// = tVector - tVector*(tLength - maxLength);
            returnVector *= maxLength;
        }
        return returnVector;
    }
    
    public static IntRect getBoundsRect(Bounds b, GridGraph gg)
    {
        Vector3 min, max;
        gg.GetBoundsMinMax (b,gg.inverseMatrix,out min, out max);
        
        int minX = Mathf.RoundToInt (min.x-0.5F);
        int maxX = Mathf.RoundToInt (max.x-0.5F);
            
        int minZ = Mathf.RoundToInt (min.z-0.5F);
        int maxZ = Mathf.RoundToInt (max.z-0.5F);
            
        IntRect originalRect = new IntRect(minX,minZ,maxX,maxZ);
        
        IntRect gridRect = new IntRect(0,0,gg.width -1, gg.depth -1);
        IntRect clampedRect = IntRect.Intersection (originalRect, gridRect);
        
        return clampedRect;
    }

	public static bool isWithinRadius(Vector3 center, float radius, Vector3 position)
	{
		float distance =  Vector3.Distance(center, position);

		if(distance < radius) {
			//Debug.Log (distance);
			//Debug.Log ("pos: " + position + " center:" + center);
			return true;
		}
		return false;
	}
}
