using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class Move
	{
		public Move ()
		{
			
		}
		
		public float calculateVelocity(float maxVelocity, float currentVelocity, float distanceToTarget, float acceleration, bool lastNode, float angle)
		{
			if((currentVelocity / acceleration + 1 >= distanceToTarget * 2.5 
				&& currentVelocity > acceleration - currentVelocity 
				&& lastNode)
				|| (angle > 70
			    && currentVelocity > maxVelocity / 4
			    )) {
			    //Debug.Log ("got here:" + angle + " vel: " + currentVelocity + " distance: " + distanceToTarget);
				return currentVelocity - (acceleration + angle / 15);	
			}
			if(currentVelocity == maxVelocity) {
				return currentVelocity;
			}
			if(currentVelocity == 0 && distanceToTarget > acceleration * 2) {
			    return acceleration;	
			}
			if(currentVelocity / acceleration + 1 <= distanceToTarget + (acceleration * 3) && currentVelocity < maxVelocity) {
			    return currentVelocity + (acceleration /5);	
			}
			
			return currentVelocity;
		}
		
		public float getViewDeltaAngle(Vector3 target, MonoBehaviour unit)
		{
			Vector3 direction = (target - unit.transform.position).normalized;
			return Vector3.Angle (direction, unit.transform.forward);
		}
		
		//deprecated
		public void executeMove(Vector3 target, float velocity, MonoBehaviour unit, float turnRate, float center)
		{

		    Vector3 targetDir = target;
			targetDir.y += center;
			//Debug.Log ("unit pos: " + unit.transform.position + " target: " +  targetDir.y);
			Quaternion  rot = Quaternion.LookRotation(targetDir - unit.transform.position);
	        float rotateVelocity = Mathf.Min (3.0f * Time.deltaTime, 1); 
		    unit.transform.rotation = Quaternion.Lerp(unit.transform.rotation, rot, rotateVelocity);

			unit.transform.Translate(Vector3.forward * Time.deltaTime * velocity);

		}
		
		protected Vector3 seekBehavior(Vector3 target, Unit unit, bool lastNode)
		{
			Vector3 desiredVel = (target - unit.transform.position).normalized;
			//Vector3 desiredVel = truncateLength (targetDir - unit.transform.position, velocity) * Time.deltaTime;
			//Vector3 currentVel =  unit.transform.forward  * 25 * Time.deltaTime;

			if (Vector3.Distance (target, unit.transform.position) <= unit.slowingRadius 
				&& (lastNode 
				|| Vector3.Distance(unit.transform.position, unit.path[unit.path.Length -1]) < unit.slowingRadius / 1.8f)) {
				desiredVel*= unit.maxVelocity * Vector3.Distance (target, unit.transform.position) / unit.slowingRadius;
				//desired.scaleBy(MAX_VELOCITY * distance/slowingRadius);
			} else {
				desiredVel *= (unit.maxVelocity);
			}
			
			Vector3 steering = desiredVel * Time.deltaTime - unit.currentVel;
			steering = MathHelper.truncateLength (steering, unit.maxForce);
			steering *= (1 / unit.mass);
			
			return steering;
		}
		
		protected Vector3 obstacleAvoidance(Unit unit, Vector3 steering)
		{
			float dynamicLength =  unit.maxCollisionDistance * (unit.currentVel.magnitude * 20);
		    Vector3 futurePos = unit.transform.position + unit.currentVel.normalized * dynamicLength;
			List<Vector3> futurePositions = new List<Vector3>();
			
			for(int i = 1; i < 15; i++) {
				futurePositions.Add (unit.transform.position + unit.currentVel.normalized * (dynamicLength / i));
			}
			
			Vector3 avoidance = Vector3.zero;
			for(int i = 0; i < unit.Obstacles.Count; i++) {
				for(int z = 0; z < futurePositions.Count; z++) {
					if(unit.Obstacles[i].collider.bounds.Contains (futurePositions[z])) {
						Vector3 center = unit.Obstacles[i].collider.bounds.center;
						avoidance = futurePositions[z] - unit.Obstacles[i].collider.bounds.center;
						avoidance = avoidance.normalized * Time.deltaTime;
						//avoidance *=  unit.maxAvoidForce;
						//Debug.Log ("got here");
					}
				}
			}
			return avoidance * 4.0f;
		}
		
		public static Vector3 parallelComponent (Vector3 source,Vector3 unitBasis)
        {
            float projection = Vector3.Dot(source, unitBasis);
            return unitBasis * projection;
        }

        // return component of vector perpendicular to a unit basis vector
        // (IMPORTANT NOTE: assumes "basis" has unit magnitude (length==1))

        public static Vector3 perpendicularComponent (Vector3 source, Vector3 unitBasis)
        {
            return source - parallelComponent(source,unitBasis);
        }
		
		public void seek(Vector3 target, float velocity, Unit unit, float turnRate, float center, bool lastNode)
		{
			Vector3 targetDir = target;
			targetDir.y += center;
			
	        float rotateVelocity = Mathf.Min (3.0f * Time.deltaTime, 1); 
		    
			Vector3 steering = seekBehavior (targetDir, unit, lastNode);

			steering += obstacleAvoidance (unit, steering);

			steering.y = 0;
			unit.currentVel = unit.currentVel + steering;
			unit.currentVel = MathHelper.truncateLength (unit.currentVel, unit.maxVelocity);

			
			Quaternion  rot = Quaternion.LookRotation(unit.transform.position + unit.currentVel * 150 - unit.transform.position);
			unit.transform.rotation = Quaternion.Lerp(unit.transform.rotation, rot, rotateVelocity * 1.5f);
			unit.transform.position = unit.transform.position + unit.currentVel ;
			//}
		}
	}
}

