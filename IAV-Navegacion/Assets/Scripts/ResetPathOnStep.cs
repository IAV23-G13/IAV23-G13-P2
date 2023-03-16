using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UCM.IAV.Navegacion;
using UCM.IAV.Movimiento;

public class ResetPathOnStep : MonoBehaviour
{

    SeguirCamino player;

    private void Start()
    {
        player = GameManager.instance.GetPlayer().GetComponent<SeguirCamino>();
    }

    private void Update()
    {
        //Checa a ver si hay algun gameobject con el componente vertex...
        var check = Physics.Raycast(transform.position, -transform.up, out var info, 10, ~(1 << gameObject.layer));
        if (check)
        {
            Vertex col = info.transform.GetComponent<Vertex>();


            //Si es parte del camino, le dice al player que resetee el camino
            if (col != null && col.isInPath)
            {
                player.ResetPath();
            }
        }
    }
}
