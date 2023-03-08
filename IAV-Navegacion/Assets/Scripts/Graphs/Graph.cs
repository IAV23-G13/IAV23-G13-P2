/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEditor.MemoryProfiler;

namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using static UCM.IAV.Navegacion.Graph;
    using System.IO;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.MemoryProfiler;
    using UnityEngine.UIElements;
    using System.Linq;
    using JetBrains.Annotations;

    class VertexRecord
    {
        public Vertex vertex;
        public Connection connection;
        public float costSoFar;
        public float estimatedTotalCost;

        public static bool operator <(VertexRecord a, VertexRecord b)
        {
            return a.estimatedTotalCost < b.estimatedTotalCost;
        }

        public static bool operator >(VertexRecord a, VertexRecord b)
        {
            return a.estimatedTotalCost > b.estimatedTotalCost;
        }
    }

    struct Connection
    {
        public VertexRecord from;
        public VertexRecord to;

        public Connection(VertexRecord _to, VertexRecord _from)
        {
            from = _from;
            to = _to;
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

        public List<Vertex> GetPathAstar(GameObject start, GameObject goal, Heuristic heuristic = null)
        {
            var startRecord = new VertexRecord();
            startRecord.vertex = start.GetComponent<Vertex>();
            startRecord.connection = new Connection(startRecord, null);
            startRecord.costSoFar = 0;
            startRecord.estimatedTotalCost = heuristic(start.GetComponent<Vertex>(), goal.GetComponent<Vertex>());

            var open = new List<VertexRecord>();
            open.Add(startRecord);
            // open += startRecord

            var closed = new List<VertexRecord>();

            VertexRecord current = startRecord;

            while (open.Count > 0)
            {
                var next = open.Min();
                current.connection.to = next;
                current = next;

                if (current.vertex == goal)
                {
                    break;
                }

                var connections = this.GetNeighbours(current.vertex);

                foreach (var connection in connections)
                {

                    Vertex nextVertex = connection;
                    float costToNext = current.costSoFar + connection.cost;

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

                        nextVertexHeuristic = heuristic(nextVertexRecord.vertex, goal.GetComponent<Vertex>());
                    }


                    nextVertexRecord.costSoFar = costToNext;
                    nextVertexRecord.connection = new Connection(startRecord, null);
                    nextVertexRecord.estimatedTotalCost = costToNext + nextVertexHeuristic;

                    if (!open.Exists((a) => { return a.vertex == nextVertex; }))
                        open.Add(nextVertexRecord);
                }

                // open -= current
                // closed += current
                open.Remove(current);
                closed.Add(current);
            }

            if (current.vertex != goal)
                return null;
            else
            {
                var result = new List<Vertex>();

                var curr = startRecord;

                while (curr.vertex != null)
                {
                    result.Add(curr.vertex);

                    curr.vertex.gameObject.SetActive(false);

                    curr = curr.connection.to;
                }



                return result;
            }
            return new List<Vertex>();
        }

        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            // IMPLEMENTAR SUAVIZADO DE CAMINOS

            List<Vertex> outputPath = new List<Vertex>();

            return outputPath; 
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