/* 
UVic CSC 305, 2019 Spring
Assignment 01
Name:
UVic ID:
This is skeleton code we provided.
Feel free to add any member variables or functions that you need.
Feel free to modify the pre-defined function header or constructor if you need.
Please fill your name and uvic id.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment01
{
    public class SphereGenerator
    {
        string name;
		float sRadius;
		Vector3 sCenter;
		

        public SphereGenerator()
        {
            //you can define the sphere center and radius in the constructor.
            this.name = "SphereGenerator";
			this.sRadius = 2;
			this.sCenter = new Vector3(0, 0, -3);
        }

        public Texture2D GenSphere(int width, int height)
        {
            /*
            implement ray-sphere intersection and render a sphere with ambient, diffuse and specular lighting.
            int width - width of the returned texture
            int height - height of the return texture
            return:
                Texture2D - Texture2D object which contains the rendered result
            */
			
			Texture2D sphereResult = new Texture2D(width, height);
			Vector3 origin = new Vector3(0, 0, 0);

			for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    float x = (float)(((2 * (i + 0.5)/ width) - 1) * (width/height));
					float y = (float)((2 * (j + 0.5) / height) - 1);
					Vector3 pov = new Vector3(x, y, -1);
					Vector3 rayDirection = pov - origin;
					rayDirection.Normalize();
					
					float t;
					Vector3 intersectNormal;
					
					bool doesIntersect = IntersectSphere(origin, rayDirection, sCenter, sRadius, out t, out intersectNormal);
					if(doesIntersect == true)
					{
						ColourSphere(i, j, t, intersectNormal, rayDirection, ref sphereResult);
					}
                }
            }
			
			return sphereResult;
            //throw new NotImplementedException();
        }
        private bool IntersectSphere(Vector3 origin,
                                        Vector3 direction,
                                        Vector3 sphereCenter,
                                        float sphereRadius,
                                        out float t,
                                        out Vector3 intersectNormal)
        {
            /*
            Vector3 origin - origin point of the ray
            Vector3 direction - the direction of the ray
            Vector3 sphereCenter - center of target sphere
            float sphereRadius - radius of target sphere
            out float t - distance the ray travelled to hit a point
            out Vector3 intersectNormal - normal of the hit point
            return:
                bool - indicating hit or not
            */
			
			t = 0;
			intersectNormal = new Vector3(0, 0, 0);
			
			Vector3 oc = sphereCenter - origin;
			float og = Vector3.Dot(oc, direction);
			
			if(og < 0)
			{
				return false;
			}
			float cg = Mathf.Sqrt(Mathf.Pow(oc.magnitude, 2) - Mathf.Pow(og, 2));
			if(sRadius <= cg)
			{
				return false;
			}
			
				float pg = Mathf.Sqrt(Mathf.Pow(sphereRadius, 2) - Mathf.Pow(cg, 2));
				t = og - pg;
				Vector3 p = Vector3.Scale(new Vector3(t,t,t), direction);
				p = origin + p;
				Vector3 pNormal = p - sCenter;
				pNormal.Normalize();
				intersectNormal = pNormal;
				return true;
			
            //throw new NotImplementedException();
        }
		
		private void ColourSphere(int i, int j, float t, Vector3 intersectNormal, Vector3 rayDirection, ref Texture2D sphereResult)
		{
			Color ambientK = new Color(0.2f, 0.4f, 0.9f);
			Color diffuseK = new Color(1f, 1f, 1f);
			float ambientIntensity = 0.9f;
			float diffuseIntensity = 0.5f;
			float specularIntensity = 0.15f;
			float specularN = 250;
			Color lightC = new Color(1f, 1f, 1f);
			Vector3 lightDir = new Vector3(30, 50, 10);
			lightDir.Normalize();
			
			float vecNL = Vector3.Dot(intersectNormal, lightDir);
			Vector3 specularR = ((2 * vecNL) * intersectNormal) - lightDir;
			float specularK = Vector3.Dot(-rayDirection, specularR);
			
			
			
			sphereResult.SetPixel(i, j, ( (ambientK * ambientIntensity) + ( diffuseIntensity * diffuseK * Mathf.Max(0, vecNL) ) + (lightC * (Mathf.Pow(specularK, specularN) * specularIntensity)) ));
		}
    }
}