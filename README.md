
# Práctica 2 (IAV) - Grupo 13

## Autores
- Javier Vaillegas Montelongo ([Yavi123](https://github.com/Yavi123))
- Gonzalo Fernández Moreno ([GonzaloFdezMoreno](https://github.com/GonzaloFdezMoreno))
- Enrique Juan Gamboa ([ivo_hr](https://github.com/ivo-hr))

## Propuesta
Esta práctica consiste en realizar una IA dentro de un laberinto que tratará de encontrar la ruta más corta a la salida. Contaremos con una entidad jugador, que será **Teseo**. En este laberinto también se encuentra de manera *dinámica* **el Minotauro**, el cual la IA tendrá que evitar a la hora de encontrar la ruta hacia la salida.

La idea principal de la práctica es conseguir los siguientes comportamientos:

 1. **Teseo** deberá de *llegar a la casilla destino* de manera que recorra la menor distancia posible mientras *evita al minotauro*, haciendo uso del algoritmo A* para navegar por el laberinto.
 2. **El Minotauro** debe *merodear por el laberinto*, "buscando" a Teseo.
 3. **El Minotauro** ha de *perseguir a Teseo* si lo ve o si él se le acerca mucho.


## Punto de partida
Se parte de un proyecto base de Unity proporcionado por el profesor aquí:
https://github.com/Narratech/IAV-Navegacion.

En resumen, partimos de un proyecto de Unity en el que se nos proporciona, en una escena, un **menú principal** en el que podremos seleccionar el *tamaño del laberinto* y el *número de minotauros* en él. Una vez seleccionados los parámetros que queremos y le demos al botón de *Comenzar*, aparecerá un **laberinto del tamaño seleccionado**, y en él el **número de minotauros seleccionado** y a **Teseo**. Teseo se puede mover con WASD, podemos cambiar los FPS con F, hacer zoom con la rueda del ratón, reiniciar con R y cambiar la heurística utilizada con C.

En cuanto a código, se nos ha proporcionado varios scripts generales: *Agente, ComportamientoAgente, Dirección*, *MinoManager* y *GestorJuego*. A un nivel más específico, tenemos scripts para el jugador  (*ControlJugador* y *SeguirCamino*) y para los Minotauros (*Merodear*, *MinoCollision*, *MinoEvader* y *Slow*). La mayoría de estos últimos scripts, a pesar de venir con algo de código, **hay que completarlos**. 
También se incluyen en el proyecto *prefabs* para el jugador, los minotauros y los elementos del laberinto.

A continuación mostramos una tabla con la descripción de las clases más importantes que nos han sido proporcionados:
| **Clase**|**Script origen**|**Descripción**|
|:--:|:--:|:--|
|Graph|*Graph.cs*|Proporciona la arquitectura necesaria para poder crear y leer grafos. También ofrece métodos para generar caminos usando heurísticas diferentes.
|GraphGrid (hereda de Graph)|*GraphGrid.cs*|Genera el nivel y proporciona un grafo asociado a él. Utiliza los costes que tienen cada *tile* utilizado para generarlo para siempre ofrecer una solución.
|Node|*Node.cs*|Pequeña clase que ofrece nodos con valores a *Graph*. También proporciona métodos para compararlos.
|TheseusGraph|*TheseusGraph.cs*|Clase que genera un camino a través del nivel para Teseo, usando la clase *Graph*. Ofrece métodos para encontrar el camino con heurísticas diferentes y para dibujar el camino que va a tomar, junto a otras utilidades más pequeñas.
|Vertex|*Vertex.cs*|Similar a *Node*, proporciona los vértices para utilizarse en los grafos de *Graph*. También ofrece métodos para comparar dichos vértices.


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

- [Vídeo con la batería de pruebas](https://www.youtube.com/watch?v=e9LektRme1c)

## Ampliaciones

TBD

## Producción

Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores.

| Estado  |  Tarea  |  Fecha  |  
|:-:|:--|:-:|
|  | Diseño: Primer borrador | 02/03/2023 |
|  | Característica A: Jugador controlado con el clic izquierdo | --/--/---- |
|  | Característica B: Lógica de los minotauros | --/--/---- |
|  | Característica C: Camino más corto con A* + heurística variable | --/--/---- |
|  | Característica D: Suavizado del camino | --/--/---- |
|  | Característica E: Navegación del avatar | --/--/---- |
||||
| **-----** | **OPCIONAL** | **-----** |
|  | Generación procedural | --/--/---- |
|  | Camino de patrulla de los mintauros | --/--/---- |
|  | Baldosas de distinto coste| --/--/---- |
|  | Múltiples salidas (Dijkstra)| --/--/---- |
|  | Cambio de color del hilo + recalculación| --/--/---- |
|  | Corte del hilo| --/--/---- |

## Referencias

Los recursos de terceros utilizados son de uso público.

- *AI for Games*, Ian Millington.
- [Kaykit Medieval Builder Pack](https://kaylousberg.itch.io/kaykit-medieval-builder-pack)
- [Kaykit Dungeon](https://kaylousberg.itch.io/kaykit-dungeon)
- [Kaykit Animations](https://kaylousberg.itch.io/kaykit-animations)
