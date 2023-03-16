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
        var check = Physics.Raycast(transform.position, -transform.up, out var info, 10, ~(1 << gameObject.layer));

        if (check)
        {
            Vertex col = info.transform.GetComponent<Vertex>();

            if (col.isInPath)
            {
                player.ResetPath();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
    }
}
