namespace CommonGames.Utilities.CGTK
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Utilities.Extensions;
	
    //using static System.Math;

	[JetBrains.Annotations.PublicAPIAttribute]
    public static partial class CGMath
    {
        //TODO: Comments & Summaries
        
        /// <summary>
        /// Used mainly in Monte Carlo algorithms to give an algorithm some exploration room.
        /// </summary>
        public static double UCT(float value, int visitedAmount) 
			=> value / (visitedAmount + Epsilon) + Sqrt(Log(visitedAmount + 1) / (visitedAmount + Epsilon)) + Epsilon;

        /// <summary>
        /// Clamps a double between min and max.
        /// </summary>
        public static double Clamp(double d, double min, double max)
        {
			if(d < min)
			{
				d = min;
			}
            else if(d > max)
			{
				d = max;
			}
            
            return d;
        }
        
	    //This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
		//to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
		//by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
		//the result of a dot product only has signed information when an angle is transitioning between more or less
		//than 90 degrees.
		public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal){
	 
			Vector3 perpVector;
			float dot;
	 
			//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
			perpVector = Vector3.Cross(normal, vectorA);
	 
			//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
			dot = Vector3.Dot(perpVector, vectorB);
	 
			return dot;
		}
	 
		public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
		{
			Vector3 perpVector;
			float angle;
	 
			//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
			perpVector = Vector3.Cross(normal, referenceVector);
	 
			//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
			angle = Vector3.Angle(referenceVector, otherVector);
			angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));
	 
			return angle;
		}
	 
		//Calculate the angle between a vector and a plane. The plane is made by a normal vector.
		//Output is in radians.
		public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
		{
			//calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
			float __dot = Vector3.Dot(vector, normal);
	 
			//this is in radians
			float __angle = (float)Acos(__dot);
	 
			return 1.570796326794897f - __angle; //90 degrees - angle
		}
	 
		//Calculate the dot product as an angle
		public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
		{
			//get the dot product
			double __dot = Vector3.Dot(vec1, vec2);
	 
			//Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
			if(__dot < -1.0f)
			{
				__dot = -1.0f;
			}
			
			if(__dot > 1.0f)
			{
				__dot =1.0f;
			}
	 
			//Calculate the angle. The output is in radians
			//This step can be skipped for optimization...
			double __angle = Acos(__dot);
	 
			return (float)__angle;
		}

		//Returns the forward vector of a quaternion
		public static Vector3 GetForwardVector(Quaternion q)
			=> q * Vector3.forward;

		//Returns the up vector of a quaternion
		public static Vector3 GetUpVector(Quaternion q)
			=> q * Vector3.up;

		//Returns the right vector of a quaternion
		public static Vector3 GetRightVector(Quaternion q)
			=> q * Vector3.right;
	}
}