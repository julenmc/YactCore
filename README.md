# YACT

Framework que incluye diferentes herramientas para un "entrenador virtual" de ciclismo. 

> [!CAUTION]
> Estas herramientas están en proceso de desarrollo, por lo que no están listos y se pueden encontrar algunos errores.

La solución aplica Clean Architecture con DDD y CQRS. Está dividido en 4 capas actualmente, pero no se descarta dividir algunas capas de Dominio/Aplicación en módulos según sus funcionalidades.

## Principales casos de uso
1. **Leer un archivo de actividades .fit**. Esta lectura incluye la búsqueda de subidas e intervalos de potencia en la actividad. Guardará todos los datos de interés en la base de datos para su posterior consulta.
2. **Múltiples consultas disponibles**:
	1. Ciclista: Devuelve toda la información disponible sobre el ciclista, incluyendo una lista de las últimas actividades realizadas.
	2. Actividad: Devuelve toda la información disponible sobre una actividad, incluyendo una lista de subidas e intervalos.
	3. Subidas: Devuelve toda la información disponible sobre una subida, incluyendo los mejores tiempos de dicha subida. 