/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/

namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using static UCM.IAV.Navegacion.Graph;
    using System.IO;
    
    
    
    using System.Linq;
    using JetBrains.Annotations;
    using System;
    using System.Transactions;

    //Seguramente podiamos haber utilizado la clase Node, pero no la vi hasta que ya estaba todo hecho
    class VertexRecord : IComparable<VertexRecord>
    {
        public Vertex vertex;
        public VertexRecord previous;
        public float costSoFar;
        public float estimatedTotalCost;

        public int CompareTo(VertexRecord other)
        {
            return estimatedTotalCost.CompareTo(other.estimatedTotalCost);
        }

        public static bool operator <(VertexRecord a, VertexRecord b)
        {
            return a.estimatedTotalCost < b.estimatedTotalCost;
        }

        public static bool operator >(VertexRecord a, VertexRecord b)
        {
            return a.estimatedTotalCost > b.estimatedTotalCost;
        }
    }

    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbourVertex;
        protected List<List<float>> costs;
        protected bool[,] mapVertices;
        protected float[,] costsVertices;
        protected int numCols, numRows;

        // this is for informed search like A*
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;

        float smoothRayFatness = 0.13f;


        public virtual void Start()
        {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual void UpdateVertexCost(Vector3 position, float costMultipliyer) { }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }

        public virtual GameObject GetRandomPos()
        {
            return null;
        }

        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new Vertex[0];
            return neighbourVertex[v.id].ToArray();
        }

        public virtual float[] GetNeighboursCosts(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new float[0];

            Vertex[] neighs = neighbourVertex[v.id].ToArray();
            float[] costsV = new float[neighs.Length];
            for (int neighbour = 0; neighbour < neighs.Length; neighbour++) {
                int j = (int)Mathf.Floor(neighs[neighbour].id / numCols);
                int i = (int)Mathf.Floor(neighs[neighbour].id % numCols);
                costsV[neighbour] = costsVertices[j, i];
            }

            return costsV;
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO BFS
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO DFS
            return new List<Vertex>();
        }

        //Basicamente el pseudocodigo de millington traducido a C#
        public List<Vertex> GetPathAstar(GameObject start, GameObject goal, Heuristic heuristic = null)
        {
            var startRecord = new VertexRecord();
            startRecord.vertex = this.GetNearestVertex(start.transform.position);
            startRecord.previous = null;
            startRecord.costSoFar = 0;
            startRecord.estimatedTotalCost = heuristic(this.GetNearestVertex(start.transform.position), this.GetNearestVertex(goal.transform.position));

            var open = new List<VertexRecord>();
            open.Add(startRecord);
            // open += startRecord

            var closed = new List<VertexRecord>();

            VertexRecord current = startRecord;

            while (open.Count > 0)
            {

                current = open.Min();

                if (current.vertex == this.GetNearestVertex(goal.transform.position))
                {
                    break;
                }

                var connections = this.GetNeighbours(current.vertex);

                foreach (var neighbour in connections)
                {

                    Vertex nextVertex = neighbour;
                    float costToNext = current.costSoFar + neighbour.cost;

                    VertexRecord nextVertexRecord;
                    float nextVertexHeuristic;

                    if (closed.Exists((a) => { return a.vertex == nextVertex; }))
                    {
                        nextVertexRecord = closed.Find((a) => { return a.vertex == nextVertex; });

                        if (nextVertexRecord.costSoFar <= costToNext)
                            continue;

                        closed.Remove(nextVertexRecord);
                        // closed -= nextVertexRecord

                        nextVertexHeuristic = nextVertexRecord.estimatedTotalCost - nextVertexRecord.costSoFar;
                    }
                    else if (open.Exists((a) => { return a.vertex == nextVertex; }))
                    {
                        nextVertexRecord = open.Find((a) => { return a.vertex == nextVertex; });

                        if (nextVertexRecord.costSoFar <= costToNext)
                            continue;

                        nextVertexHeuristic = nextVertexRecord.vertex.cost - nextVertexRecord.costSoFar;
                    }
                    else
                    {
                        nextVertexRecord = new VertexRecord();
                        nextVertexRecord.vertex = nextVertex;

                        nextVertexHeuristic = heuristic(nextVertexRecord.vertex, this.GetNearestVertex(goal.transform.position));
                        nextVertexRecord.previous = current;
                    }


                    nextVertexRecord.costSoFar = costToNext;
                    nextVertexRecord.estimatedTotalCost = costToNext + nextVertexHeuristic;

                    if (!open.Exists((a) => { return a.vertex == nextVertex; }))
                        open.Add(nextVertexRecord);
                }

                open.Remove(current);
                closed.Add(current);
            }

            if (current.vertex.id != this.GetNearestVertex(goal.transform.position).id)
                return null;
            else
            {
                var result = new List<Vertex>();

                var curr = current;

                while (curr.vertex.id != this.GetNearestVertex(start.transform.position).id)
                {
                    result.Add(curr.vertex);
                    curr.vertex.isInPath = true;

                    Debug.Log(curr.vertex.id);

                    curr = curr.previous;
                }

                //Ponemos el primer vertice directamente
                result.Add(this.GetNearestVertex(start.transform.position));
                // result.Reverse();
                return result;
            }
        }

        public void ResetVertexPath(List<Vertex> path)
        {
            foreach (var ver in path)
            {
                ver.isInPath = false;
            }
        }

        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            List<Vertex> smoothedPath = new List<Vertex>();
            int patIndex = 0;
            smoothedPath.Add(inputPath[0]);
            smoothedPath.Add(inputPath[1]);

            while (patIndex < inputPath.Count)
            {
                if (this.IsLineOfSight(smoothedPath[smoothedPath.Count - 2], inputPath[patIndex]))
                {
                    smoothedPath[smoothedPath.Count - 1] = inputPath[patIndex];
                }
                else
                {
                    smoothedPath.Add(inputPath[patIndex - 1]);
                }
                patIndex++;
            }

            smoothedPath.Add(inputPath[inputPath.Count - 1]);
            return smoothedPath;
        }

        public bool IsLineOfSight(Vertex v1, Vertex v2)
        {
            Vector3 dir = v2.transform.position - v1.transform.position;
            float distance = dir.magnitude;
            dir.Normalize();

            Vector3 origin1 = v1.transform.position + Vector3.up * 0.5f + Vector3.forward * smoothRayFatness + Vector3.right * -smoothRayFatness;
            Vector3 origin2 = v1.transform.position + Vector3.up * 0.5f + Vector3.forward * -smoothRayFatness + Vector3.right * smoothRayFatness;
            Vector3 origin3 = v1.transform.position + Vector3.up * 0.5f + Vector3.forward * -smoothRayFatness + Vector3.right * -smoothRayFatness;
            Vector3 origin4 = v1.transform.position + Vector3.up * 0.5f + Vector3.forward * smoothRayFatness + Vector3.right * smoothRayFatness;

            Vector3 dir1 = (v2.transform.position + Vector3.forward * smoothRayFatness + Vector3.right * -smoothRayFatness) - origin1;
            Vector3 dir2 = (v2.transform.position + Vector3.forward * -smoothRayFatness + Vector3.right * smoothRayFatness) - origin2;
            Vector3 dir3 = (v2.transform.position + Vector3.forward * -smoothRayFatness + Vector3.right * -smoothRayFatness) - origin3;
            Vector3 dir4 = (v2.transform.position + Vector3.forward * smoothRayFatness + Vector3.right * smoothRayFatness) - origin4;

            return (!Physics.Raycast(origin1, dir1, distance) && 
                    !Physics.Raycast(origin2, dir2, distance) && 
                    !Physics.Raycast(origin3, dir3, distance) &&
                    !Physics.Raycast(origin4, dir4, distance));
        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();

            if (dstId < 0 || dstId >= vertices.Count) 
                return path;

            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

    }
}