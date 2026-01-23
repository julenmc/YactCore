# Intervals
## Introducción
Domain Service que se encarga de detectar intervalos de potencia en los que esta se mantiene constante. 

### Proceso
1. Búsqueda de intervalos convencionales (alta potencia, alta duración y medios).
2. Mergeo de intervalos.
3. Corrección de colisiones no mergeables.
4. Obtención de intervalos huérfanos.

### Definiciones
- Intervalo: cantidad de tiempo en el que la potencia se mantiene estable dentro de unos límites establecidos. Tipos de intervalos:
	- Intervalo de potencia alta: es aquel en el que la potencia media está en Z5 o superior y la duración es de al menos 30 segundos.
	- Intervalo de duración alta: es aquel en el que la duración es de al menos 20 minutos y la potencia es de al menos Z2.
	- Intervalo medio: es aquel que tiene una potencia media de al menos Z3 y una duración mínima de 4 minutos.
	- Intervalo de descanso: es aquel en el que la potencia media no llega a los requisitos mínimos de los intervalos anteriores. Se considerará descanso cuando la potencia se mantenga relativamente estable.
	- Intervalo irregular: es aquel que no mantiene la potencia estable, por lo que no se puede meter en ninguno de los grupos, pero tampoco se puede meter en el de descanso porque tiene altas potencias.
- Colisión: al realizar búsquedas en diferentes etapas, al finalizar la búsqueda de intervalos convencionales se pueden encontrar intervalos que se solapan en el tiempo, ya que esta búsqueda se hace en 3 etapas completamente independientes. A este solapamiento se le llama colisión.
- Mergeo: proceso en el que se juntan intervalos que colisionan. Es necesario que estos intervalos tengan potencias medias parecidas. Por ejemplo, un intervalo de alta potencia de 3 minutos a Z5 bajo con un intervalo medio de 5 minutos pero en alto Z4.

## Búsqueda de intervalos
Se utilizarán los siguientes datos para la búsqueda de intervalos:
- Tiempo.
- Potencia media.
- Coeficiente de desviación.
- Delta máximo-mínimo.
- Desviación respecto a la potencia media.

### Intervalos de alta potencia
Se trabaja con datos de los últimos 10 segundos. Requisitos:
- Tiempo mínimo: 30 segundos.
- Potencia media mínima: Z5 límite inferior.
- Comienzo de intervalo: coeficiente de desviación de 0.15 máximo.
- Desviaciones: solo hacia abajo. Comprobación de la desviación de media reciente respecto a la referencia (actual). Relaciones en función de la desviación de la potencia:
    > 50% => 1 segundo.
    > 25% => 5 segundos.
    > 10% => 15 segundos.
    else  => 30 segundos.
    Cuando hay una desviación se guarda el punto en el que comienza, cuando finaliza (si es que lo hace), se comprueba que el intervalo sigue llegando a la potencia mínima; en caso de que no lo haga, se da por finalizado el intervalo.

### Intervalos de alta duración
Se trabaja con los últimos 60 segundos. Requisitos:
- Tiempo mínimo: 20 minutos.
- Potencia media mínima: Z2 punto medio.
- Comienzo de intervalo: potencia máxima => Z5, delta máximo-mínimo.
- Desviaciones: comprobación de la desviación de media reciente respecto a la referencia (actual). Relaciones en función de la desviación de potencia:
    > 75% => descarte automático.
    > 50% => 30 segundos.
    > 25% => 2 minutos.
    else  => sigue el intervalo.

### Intervalos medios
Se trabaja con los últimos 30 segundos. Requisitos:
- Tiempo mínimo: 4 minutos.
- Potencia media mínima: Z3 punto medio, delta máximo-mínimo.
- Comienzo de intervalo: potencia máxima => Z5 (límite alto).
- Desviaciones: comprobación de la desviación de media reciente respecto a la referencia (actual). Relaciones en función de la desviación de potencia:
    > 75% => descarte automático.
    > 50% => 5 segundos.
    > 25% => 10 segundos.
    > 15% => 30 segundos.
    else  => sigue el intervalo.