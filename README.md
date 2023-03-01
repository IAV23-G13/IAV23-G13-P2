# Práctica 2 (IAV) - Grupo 13

## Autores
- Javier Vaillegas Montelongo ([Yavi123](https://github.com/Yavi123))
- Gonzalo Fernández Moreno ([GonzaloFdezMoreno](https://github.com/GonzaloFdezMoreno))
- Enrique Juan Gamboa ([ivo_hr](https://github.com/ivo-hr))

## Propuesta
Esta práctica consiste en realizar un prototipo de IA dentro de un pequeño entorno virtual con obstáculos y una entidad jugador, que será **el flautista**. En este entorno se encuentran, de manera *dinámica* **un perro** y un grupo de **ratas**.

La idea principal de la práctica es conseguir los siguientes comportamientos:

 1. **El perro** deberá de *seguir al flautista* de manera que prediga el movimiento del flautista, y quedarse a una distancia de él.
 2. **El perro** ha de huir si *hay tres o más ratas cerca suya*.
 3. **Las ratas** han de moverse erráticamente cuando *el flautista no toca la flauta*
 4. **Las ratas**, al escuchar la flauta, tienen que *seguir al flautista*, y *ordenarse entre ellas en formación*. Tienen que tener separación entre ellas y quedarse a una distancia del flautista.


## Punto de partida
Se parte de un proyecto base de Unity proporcionado por el profesor aquí:
https://github.com/Narratech/IAV-P1.

En resumen, partimos de un proyecto de Unity en el que se nos proporciona una **escena principal** con un **flautista** (el jugador), una **rata** y un **perro**. 

En cuanto a código, se nos ha proporcionado varios scripts generales: *Agente, ComportamientoAgente, Dirección* y *GestorJuego*. A un nivel más específico, tenemos scripts para el perro  (*Persecución*, *Huir*), las ratas (*Merodear*), para ambos (*Llegada*) y para el jugador (*ControlJugador, TocarFlauta*). La mayoría de estos últimos scripts **vienen sin hacer**. 
También se incluyen en el proyecto *prefabs* para el jugador, el perro y las ratas.

Con todo lo anterior, sin cambiar, añadir, quitar ni editar nada, **tenemos un mundo** en el que existen **obstáculos**, una **rata**, un **perro** y un **flautista** (el jugador). El jugador puede *moverse* (WASD/flechas), *tocar la flauta* (espacio), *cambiar la cámara* (N), *activar y desactivar obstáculos* (T), *añadir ratas* (O), *quitar ratas* (P), *cambiar los FPS* (F) y *recargar la escena* (R), indicado al jugador a través de  **una UI** en la que nos muestra una leyenda de los controles, un contador de ratas y otro contador de FPS.


## Diseño de la solución

La manera en la que vamos a afrontar esta práctica es la siguiente:

 - Completaremos **el script de seguimiento** que se le aplicará a las ratas, de manera que puedan *seguir al objetivo*.
 
 - También completaremos **el script** que lé de a las ratas un **movimiento errático** para cuando no estén bajo el control del flautista.
 
 - **Crearemos un script de seguimiento** sólo para el perro, pero que **heredará** del *script de seguimiento de las ratas* completado anteriormente.
 
 - Finalmente completaremos **el script de huida** del perro para que *se aleje de las ratas* que tenga en su proximidad. Éste código también heredará del *script de seguimiento de las ratas*.

 - También vamos a crear **un script de detección**, que se aplicará tanto al perro como a las ratas, para poder detectar otros objetos alrededor suya y seguir un comportamiento u otro en función de ello.

Pseudocódigo del algoritmo de seguimiento:
```
class Follow:
    character: Kinematic
    target: Kinematic

    maxAcc: float
    maxSp: float

    # Distancia mínima entre la entidad y el objetivo
    minDist: float

    # Distancia a la que la entidad va parando
    slowDist: float

    # Tiempo en llegar a la velocidad pedida
    timToVel: float

    function gatAngl() -> AnglOut:
        result = new AnglOut()

        # Dirección hacia el objetivo
        dir = target.position - character.position
        dist = dir.length()

        # Si ya llegamos, paramos
        if dist < minDist:
            return null

        # Si no estamos a distancia de frenar, vamos a tope
        if dist > slowDist:
            targetSp = maxSp
        # si no, a calcular
        else:
            targetSp = maxSp * dist / slowDist

         targetVel = dir
        targetVel.normalize()
        targetVel *= targetSp

        # Aceleración para llegar a la velocidad que queremos
        result.linear = (targetVel - character.velocity)/timeToVel

        # Si la aceleración es demasiada
        if result.linear.length() > maxAcc:
            result.linear.normalize()
            result.linear *= maxAcc

        result.angular = 0
        return result
```

Pseudocódigo de la detección de ratas:

    function AreRatsNearby(int maxNumOfRats) -> bool 
	    nearbyObjects = Physics.SphereCast(...)
	    numOfRats = 0 
	    foreach object in nearbyObjects: 
		    if object is a rat: 
			    numOfRats++ 
			    if numOfRats >= maxNumOfRats: 
				    return true 
			    return false



Diagrama de la máquina de estado del perro

![Máquina de estados del perro](https://cdn.discordapp.com/attachments/1072955659827556384/1072964346335989841/image.png)
Diagrama de la máquina de estado de las ratas
![Máquina de estado de las ratas](https://cdn.discordapp.com/attachments/1072955659827556384/1072965194038390824/image.png)


## Pruebas y métricas

- [Vídeo con la batería de pruebas](https://youtu.be/AGn-hzSmkSk)

## Ampliaciones

TBD

## Producción

Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores.

| Estado  |  Tarea  |  Fecha  |  
|:-:|:--|:-:|
| ✔ | Diseño: Primer borrador | 08/02/2023 |
| ✔ | Característica A: Mundo virtual | 09/02/2023 |
| ✔ | Característica B: Perro sigue al flautista| 18/02/2023 |
| ✔ | Característica C: Perro huye de ratas| 18/02/2023 |
| ✔ | Característica D: Ratas merodeadoras| 22/02/2023 |
| ✔ | Característica E: Ratas hipnotizadas| 19/02/2023 |
|   | ... | |
|  | OPCIONAL |  |
|  | Generador pseudoaleatorio | --/--/---- |
|  | Varios flautistas/generadores de ratas| --/--/---- |
|  | Curarle la ceguera al perro| --/--/---- |
|  | Perro y ratas evitan obstáculos| --/--/---- |

## Referencias

Los recursos de terceros utilizados son de uso público.

- *AI for Games*, Ian Millington.
- [Kaykit Medieval Builder Pack](https://kaylousberg.itch.io/kaykit-medieval-builder-pack)
- [Kaykit Dungeon](https://kaylousberg.itch.io/kaykit-dungeon)
- [Kaykit Animations](https://kaylousberg.itch.io/kaykit-animations)
