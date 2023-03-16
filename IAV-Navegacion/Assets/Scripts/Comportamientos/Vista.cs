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

    float angvista; //para ver si te ve el minotauro

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
        if(Physics.Raycast(transform.position, playerTransform.position - transform.position,out sight)) //creamos una linea entre el jugador y el minotauro
        {

            angvista = Vector3.Angle(transform.forward, playerTransform.position - transform.position); //calculamos el angulo entre la direccion que lleva el minotauro y el raycast creado

            Debug.Log("Ray hit: " + sight.collider.gameObject.tag);
            if (sight.collider.gameObject.tag == "Player"&&angvista>-30&&angvista<30) //comprobamos que no haya nada entre player y el minotauro y ademas que esté en un angulo bajo de forma que pueda ver al jugador
            {
                if (!lleg.enabled) { 
                    //si lo ve que lo persiga
                    mero.enabled = false;
                    lleg.enabled = true;
                    lleg.objetivo = sight.collider.gameObject;

                }
            }
            else
            {
                if (lleg.enabled) { //para que solo lo haga 1 vez
                    //si no lo ve que siga merodeando
                    mero.enabled = true;
                    lleg.enabled = false;
                }
            }
        }

    }
}
