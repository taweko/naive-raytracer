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
    public class CubeGenerator
    {
        int[] facesA;
		Vector3[] verticesA;
		int[] indicesA;
		string name;
        public CubeGenerator()
        {
            //you can define your cube vertices and indices in the constructor.
			facesA = new int[]{3,3,3,3,3,3,3,3,3,3,3,3};
			verticesA = new Vector3[]
			{
				new Vector3(-1, -1, -3),
				new Vector3(-1, 1, -3),
				new Vector3(1, 1, -3),
				new Vector3(1, -1, -3),
				new Vector3(-1, -1, -5),
				new Vector3(-1, 1, -5),
				new Vector3(1, 1, -5),
				new Vector3(1, -1, -5)
			};
			indicesA = new int[]{3, 0, 4, 4, 7, 3, 5, 6, 7, 7, 4, 5, 1, 5, 4, 4, 0, 1, 6, 2, 3, 3, 7, 6, 6, 5, 1, 1, 2, 6, 2, 1, 0, 0, 3, 2};
            name = "CubeGenerator";
			rotateCube();
        }

        public Texture2D GenBarycentricVis(int width, int height)
        {
            /*
            implement ray-triangle intersection and 
            visualize the barycentric coordinate on each of the triangles of a cube, 
            with Red, Green and Blue for each coordinate.
            int width - width of the returned texture
            int height - height of the return texture
            return:
                Texture2D - Texture2D object which contains the rendered result
            */
			Texture2D baryVisResult = new Texture2D(width, height);
			Vector3 origin = new Vector3(0, 0, 0);
			
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					float x = (float)(((2 * (i + 0.5)/ width) - 1) * (width/height));
					float y = (float)((2 * (j + 0.5) / height) - 1);
					Vector3 camView = new Vector3(x, y, -1);
					Vector3 rayDirection = camView - origin;
					rayDirection.Normalize();		
							
					float t;
					Vector3 barycentricCoordinate;
					
					Vector3 vA;
					Vector3 vB;
					Vector3 vC;	
					
					for(int k = 0; k < facesA.Length; k++)
					{
						vA = verticesA[indicesA[k*3]];
						vB = verticesA[indicesA[1 + (k*3)]];
						vC = verticesA[indicesA[2 + (k*3)]];
									
					
						bool doesIntersect = IntersectTriangle(origin, rayDirection, vA, vB, vC, out t, out barycentricCoordinate);
						if(doesIntersect == true)
						{
							Color rainbowC = new Color((barycentricCoordinate.x * 1 ), (barycentricCoordinate.y * 1), (barycentricCoordinate.z * 1));
							baryVisResult.SetPixel(i, j, rainbowC);
						}
					}
				}
			}
			return baryVisResult;
            //throw new NotImplementedException();
        }

        public Texture2D GenUVMapping(int width, int height, Texture2D inputTexture)
        {
            /*
            implement UV mapping with the calculated barycentric coordinate in the previous step, 
            and visualize a texture image on each face of the cube.
            (choose any texture you like)
            we have declared textureOnCube as a public variable,
            you can attach texture to it from Unity.
            you can define your cube vertices and indices in this function.
            int width - width of the returned texture
            int height - height of the return texture
            Texture2D inputTexture - the texture you need to sample from
            return:
                Texture2D - Texture2D object which contains the rendered result
            */
			
			Texture2D uvResult = new Texture2D(width, height);
			Vector3 origin = new Vector3(0, 0, 0);
			
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					float x = (float)(((2 * (i + 0.5)/ width) - 1) * (width/height));
					float y = (float)((2 * (j + 0.5) / height) - 1);
					Vector3 camView = new Vector3(x, y, -1);
					Vector3 rayDirection = camView - origin;
					rayDirection.Normalize();		
							
					float t;
					Vector3 barycentricCoordinate;
					
					Vector3 vA;
					Vector3 vB;
					Vector3 vC;	
					
					for(int k = 0; k < facesA.Length; k++)
					{
						vA = verticesA[indicesA[k*3]];
						vB = verticesA[indicesA[1 + (k*3)]];
						vC = verticesA[indicesA[2 + (k*3)]];
									
					
						bool doesIntersect = IntersectTriangle(origin, rayDirection, vA, vB, vC, out t, out barycentricCoordinate);
						if(doesIntersect == true)
						{	

							Vector2 uvA;
							Vector2 uvB;
							Vector2 uvC;
					
							if((k % 2) == 0)
							{
								uvA = new Vector2(1, 1);
								uvB = new Vector2(0, 1);
								uvC = new Vector2(0, 0);	
							}
							else
							{
								uvA = new Vector2(0, 0);
								uvB = new Vector2(1, 0);
								uvC = new Vector2(1, 1);	
							}

							Vector2 uvFunc = (barycentricCoordinate.x * uvA) + (barycentricCoordinate.y * uvB) + (barycentricCoordinate.z * uvC);
							int textureX = (int)(uvFunc.x * inputTexture.width);
							int textureY = (int)(uvFunc.y * inputTexture.height);
							uvResult.SetPixel(i, j, inputTexture.GetPixel(textureX, textureY));
						}
					}
				}
			}
			return uvResult;
            //throw new NotImplementedException();
        }

        private bool IntersectTriangle(Vector3 origin,
                                        Vector3 direction,
                                        Vector3 vA,
                                        Vector3 vB,
                                        Vector3 vC,
                                        out float t,
                                        out Vector3 barycentricCoordinate)
        {
            /*
            Vector3 origin - origin point of the ray
            Vector3 direction - the direction of the ray
            vA, vB, vC - 3 vertices of the target triangle
            out float t - distance the ray travelled to hit a point
            out Vector3 barycentricCoordinate - you should know what this is
            return:
                bool - indicating hit or not
            */
			t = 0;
			barycentricCoordinate = new Vector3(0, 0, 0);
			
			Vector3 planeNormal = Vector3.Cross((vB - vA),(vC - vA));
			//planeNormal.Normalize();
			
			if(Vector3.Dot(planeNormal, direction) == 0)
			{
				return false;
			}
			float planeDistance = Vector3.Dot(vA, planeNormal);
			t = (Vector3.Dot(planeNormal, (vA - origin))) / (Vector3.Dot(planeNormal, direction));
			if (t < 0)
			{
				return false;
			}
			
			Vector3 vP = origin + (t * direction);
			Vector3 outer1;
			Vector3 outer2;
			
			Vector3 vBC = vC - vB;
			vBC.Normalize();
			Vector3 vBP = vP - vB;
			Vector3 vQ = vB + (Vector3.Dot(vBP, vBC) * vBC);
			Vector3 vPQ = vQ - vP;
			float alphaC = vPQ.magnitude;
			
			Vector3 vBA = vA - vB;
			outer1 = Vector3.Cross(vBP, vBC);
			outer2 = Vector3.Cross(vBA, vBC);
			if (Mathf.Sign(Vector3.Dot(planeNormal, outer1)) != (Mathf.Sign(Vector3.Dot(outer2, planeNormal))))
			{
				return false;
			}
			
			Vector3 vCA = vA - vC;
			vCA.Normalize();
			Vector3 vCP = vP - vC;
			Vector3 vR = vC + (Vector3.Dot(vCP, vCA) * vCA);
			Vector3 vPR = vR - vP;
			float betaC = vPR.magnitude;
			
			Vector3 vCB = vB - vC;
			outer1 = Vector3.Cross(vCP, vCA);
			outer2 = Vector3.Cross(vCB, vCA);
			if (Mathf.Sign(Vector3.Dot(planeNormal, outer1)) != (Mathf.Sign(Vector3.Dot(outer2, planeNormal))))
			{
				return false;
			}
			
			Vector3 vAB = vB - vA;
			vAB.Normalize();
			Vector3 vAP = vP - vA;
			Vector3 vS = vA + (Vector3.Dot(vAP, vAB) * vAB);
			Vector3 vPS = vS - vP;
			float gammaC = vPS.magnitude;
			
			Vector3 vAC = vC - vA;
			outer1 = Vector3.Cross(vAP, vAB);
			outer2 = Vector3.Cross(vAC, vAB);
			if (Mathf.Sign(Vector3.Dot(planeNormal, outer1)) != (Mathf.Sign(Vector3.Dot(outer2, planeNormal))))
			{
				return false;
			}
			
			
			alphaC = Mathf.Abs(alphaC);
			betaC = Mathf.Abs(betaC);
			gammaC = Mathf.Abs(gammaC);
			
			
			float barySum = alphaC + betaC + gammaC;
			alphaC = alphaC / barySum;
			betaC = betaC / barySum;
			gammaC = gammaC / barySum;
			
			barycentricCoordinate.Set(alphaC, betaC, gammaC);
			return true;
            //throw new NotImplementedException();
        }
		
		private void rotateCube()
		{
			for(int v = 0; v < verticesA.Length; v++)
			{
				Vector3 rotationVect = new Vector3(0,45,45);
				Quaternion rotation = Quaternion.Euler(rotationVect);
				verticesA[v].z = verticesA[v].z + 4;
				verticesA[v] = rotation * verticesA[v]; //Quaternion.AngleAxis(30, (new Vector3(1,1,0))) * (verticesA[v]);
				verticesA[v].z = verticesA[v].z - 4;
			}
		}
    }
}