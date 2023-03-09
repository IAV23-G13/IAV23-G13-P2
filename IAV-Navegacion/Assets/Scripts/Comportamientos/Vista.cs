using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

public class Vista : MonoBehaviour
{

    private Merodear mero;
    private Llegada lleg;
    [SerializeField]
    Transform playerTransform;
    RaycastHit sight = new RaycastHit();

    
    
    // Start is called before the first frame update
    void Awake()
    {
        mero = GetComponent<Merodear>();
        lleg = GetComponent<Llegada>();
        playerTransform = GameManager.instance.GetPlayer().transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, playerTransform.position - transform.position,out sight))
        {

            Debug.Log("Ray hit: " + sight.collider.gameObject.tag);
            if (sight.collider.gameObject.tag == "Player") //hay que comprobar tambien el angulo de vista
            {
                //asignar al jugador 
                lleg.objetivo = sight.collider.gameObject;
                //cambiar pesos para activar el seguimiento
            }
            else
            {
                //cambiar pesos para que merodee
            }
        }

    }
}
