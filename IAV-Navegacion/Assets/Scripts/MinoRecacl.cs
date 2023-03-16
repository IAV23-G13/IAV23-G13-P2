using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

public class MinoRecacl : MonoBehaviour
{

    float initialcost=1;

    private void OnTriggerEnter(Collider other)
    {
        Vertex possiblePath = other.gameObject.GetComponent<Vertex>();
        if (possiblePath!=null)
        {
            if (possiblePath.isInPath)
            {
                initialcost = possiblePath.cost;
                //si entra en el camino multiplicamos el coste para que elija otro camino
                possiblePath.cost *= 10;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Vertex possiblePath = other.gameObject.GetComponent<Vertex>();
        if (possiblePath != null)
        {
            if (possiblePath.isInPath)
            {
                //si sale del camino vuelve a ser el camino normal 
                possiblePath.cost=initialcost;

            }
        }
    }
}
