# Game Design Document

Prototipo de GDD actualizado basado en las correcciones realizadas en el primer Parcial.

---

# High Concept

Juego de terror psicológico y supervivencia en primera persona donde un grupo de jóvenes, durante una exploración urbana, ingresa a una fábrica/edificio abandonado con antecedentes de asesinatos y desapariciones.

El protagonista queda separado de sus compañeros y debe sobrevivir mientras reconstruye la verdad del lugar, **sin posibilidad de defenderse mediante el combate**.

## Pilar de diseño

La ausencia de herramientas ofensivas es una decisión de diseño deliberada porque:

- Obliga al jugador a resolver cada situación de amenaza mediante:
  - La observación.
  - La evasión.
  - La gestión de recursos.
- Constituye el eje central sobre el que se desarrolla gran parte del análisis **MDA** presentado en las siguientes secciones.

---

# 1. Modelo MDA

Desde la perspectiva del diseñador, las **mecánicas** implementadas generan **dinámicas** de juego, y estas producen, en la experiencia del jugador, determinadas **estéticas** o respuestas emocionales.

Desde la perspectiva del jugador, el recorrido es inverso: la **estética** es lo primero que se percibe y, únicamente mediante la interacción con el juego, se descubren las dinámicas y mecánicas que la sustentan.

Para este proyecto, el punto de partida del diseño fueron las estéticas buscadas:

- Miedo.
- Tensión.
- Paranoia.
- Incertidumbre.

Todo ello enmarcado dentro de una experiencia de **exploración y supervivencia sin combate**.

A partir de esas estéticas se definieron las dinámicas necesarias para producirlas y, posteriormente, las mecánicas concretas que debían implementarse.

## 1.1 Mecánicas

Las mecánicas del proyecto se agrupan en ocho categorías funcionales. Ninguna fue seleccionada de forma aislada; cada una responde a una dinámica que necesitaba ser habilitada.

### Movimiento y percepción en primera persona

Incluye:

- Caminar.
- Esconderse.
- Uso de linterna con batería limitada.

Estas mecánicas definen qué puede ver y alcanzar el jugador en cada momento, constituyendo la base técnica sobre la que se construye la restricción de información característica del terror.

### Interacción con el entorno

Permite:

- Abrir puertas.
- Revisar cajones.
- Tomar llaves.
- Leer notas.
- Activar objetos.

Estas acciones habilitan el avance físico y narrativo, permitiendo que el jugador modifique progresivamente su relación con el espacio.

### Comunicación mediante walkie-talkie

Incluye:

- Recepción de mensajes.
- Interferencias.

Mantiene un vínculo narrativo con los compañeros ausentes, aunque de forma deliberadamente poco confiable.

### Recolección e investigación

Comprende:

- Notas.
- Grabaciones.
- Fotografías.
- Objetos personales.

Constituye la principal vía de acceso a la historia del edificio y de las víctimas anteriores.

### Inventario básico

Permite almacenar:

- Llaves.
- Documentos.
- Objetos clave.

Su función es sostener la recolección sin incorporar una gestión de recursos innecesaria para la experiencia buscada.

### Persecución, escape y ocultamiento sin combate

Es la mecánica más importante del proyecto, ya que establece que la única respuesta posible frente a la amenaza es evitarla.

### Comportamiento del antagonista

Incluye:

- Patrullaje.
- Detección.
- Persecución en zonas definidas.

Genera la amenaza que da sentido a las mecánicas de evasión.

### Fenómenos paranormales no hostiles

Comprenden:

- Apariciones.
- Alteraciones del entorno.

Funcionan como mecánica de ambientación y como medio para entregar narrativa fragmentada, diferenciándose de la amenaza física directa.

## 1.2 Dinámicas

Las dinámicas son los patrones de comportamiento que emergen cuando el jugador interactúa con las mecánicas del sistema. A diferencia de una mecánica, una dinámica no está programada directamente: surge como consecuencia de la interacción entre varias mecánicas.

### Gestión del riesgo y evasión activa (dinámica central del proyecto)

Esta dinámica nace de la combinación entre:

- La ausencia de combate.
- La batería limitada de la linterna.
- El sistema de detección del antagonista.

Como consecuencia, el jugador debe evaluar constantemente su nivel de exposición:

- Cuándo avanzar.
- Cuándo detenerse.
- Cuándo apagar la linterna.
- Cuándo esconderse.

### Exploración progresiva y construcción de un mapa mental

Surge de la interacción entre:

- Puertas y llaves.
- Progresión por zonas bloqueadas.

El jugador construye gradualmente una representación mental del edificio, reforzando la sensación de progreso incluso cuando se reutilizan espacios.

### Investigación narrativa y reconstrucción del pasado

Nace de la combinación entre:

- Recolección de pistas.
- Fenómenos paranormales no hostiles.

El jugador reconstruye la historia de forma no lineal, impulsado tanto por la curiosidad como por el miedo.

### Comunicación poco confiable

Se genera mediante el sistema de walkie-talkie con:

- Interferencias.
- Señales falsas.

Como resultado, el jugador nunca puede asumir que la información recibida es completamente verdadera.

La incertidumbre se traslada así del plano físico:

> ¿Dónde está el asesino?

al plano narrativo:

> ¿Qué parte de lo que escucho es real?

### Alternancia entre calma y tensión (ritmo cíclico)

Se produce por la alternancia entre:

- Fases de exploración.
- Secuencias de persecución.

Este ciclo mantiene la tensión durante toda la experiencia sin saturar al jugador con amenaza constante.

### Toma de decisiones bajo presión

Aparece principalmente durante las persecuciones.

El jugador debe decidir rápidamente:

- Qué ruta seguir.
- Qué puerta utilizar.
- Dónde esconderse.

Todo ello con información incompleta sobre las consecuencias de cada decisión.

---

## 1.3 Estéticas

Las estéticas representan la respuesta emocional del jugador como consecuencia de las dinámicas descritas.

Cada una puede rastrearse hasta una dinámica concreta.

### Miedo y tensión

Son consecuencia principalmente de:

- La gestión del riesgo y evasión activa.
- La alternancia entre calma y tensión.

### Vulnerabilidad

Reemplaza correctamente a la antigua "dinámica de vulnerabilidad".

Se produce porque el jugador:

- No puede enfrentar al antagonista.
- Solo puede evitarlo.

### Paranoia e incertidumbre

Surgen gracias a:

- La comunicación poco confiable.
- Los fenómenos paranormales.

Ambos elementos hacen que el jugador dude constantemente de qué es real dentro del propio sistema del juego.

### Curiosidad y descubrimiento

Provienen de la dinámica de investigación narrativa.

Funcionan como contrapeso del miedo al ofrecer una motivación positiva para continuar explorando.

### Alivio momentáneo

Se experimenta cuando el jugador:

- Logra esconderse con éxito.
- Supera una secuencia de persecución.

Este alivio genera el contraste necesario para que la tensión posterior conserve su efectividad.

## 1.4 Síntesis: relación Mecánica → Dinámica → Estética

La siguiente tabla resume la relación entre las **mecánicas**, las **dinámicas** que emergen de ellas y las **estéticas** o emociones que experimenta el jugador.

| Mecánica | Dinámica | Estética |
|----------|----------|----------|
| Ausencia de combate, linterna con batería limitada y sistema de detección del antagonista | Gestión del riesgo y evasión activa | Vulnerabilidad, miedo |
| Puertas, llaves y progresión por zonas | Exploración progresiva | Curiosidad, sensación de avance |
| Recolección de pistas y apariciones paranormales | Investigación narrativa | Curiosidad, descubrimiento |
| Walkie-talkie con interferencias | Comunicación poco confiable | Paranoia, incertidumbre |
| Alternancia entre exploración y persecución | Ritmo cíclico de calma y tensión | Tensión sostenida, alivio momentáneo |
| Rutas, obstáculos y escondites durante las persecuciones | Toma de decisiones bajo presión | Miedo, adrenalina |

--- 

# 2. Flow y experiencia del jugador

El **estado de Flow**, es un estado de concentración plena en el que una persona se encuentra completamente inmersa en una actividad gracias al equilibrio entre el desafío que esta propone y sus propias habilidades.
En este proyecto, al tratarse de un juego **sin combate**, dicho equilibrio no depende de la precisión o los reflejos, sino de la capacidad del jugador para **observar, anticipar y gestionar el riesgo**. El diseño busca mantener ese estado de Flow durante toda la experiencia mediante los siguientes aspectos.

---

## 2.1 Equilibrio entre desafío y habilidad

El desafío principal es **perceptivo y decisional**, no mecánico. El jugador debe:

- Reconocer los patrones de patrulla del antagonista.
- Administrar la batería de la linterna.
- Elegir cuándo avanzar, esconderse o exponerse.

La progresión de la habilidad proviene del conocimiento acumulado del escenario y del comportamiento del enemigo, más que del perfeccionamiento de la destreza motriz.

En consecuencia, la dificultad puede ajustarse modificando variables de diseño como:

- Radio de detección del antagonista.
- Frecuencia de patrulla.
- Duración de la batería.

Este enfoque resulta coherente con protagonistas comunes que enfrentan una amenaza físicamente superior.

---

## 2.2 Progresión de la dificultad

La dificultad aumenta de forma gradual mediante la progresión por zonas del edificio.

Las primeras áreas funcionan como un espacio de aprendizaje donde el jugador se familiariza con las mecánicas básicas:

- Uso de la linterna.
- Apertura de puertas.
- Exploración e investigación.

En las zonas más profundas del edificio:

- El antagonista patrulla con mayor frecuencia.
- Las baterías de repuesto son más escasas.
- Las persecuciones son más extensas.
- Las rutas de escape resultan menos evidentes.

Esta curva progresiva evita tanto la frustración inicial como el aburrimiento en las etapas avanzadas.

---

## 2.3 Claridad de objetivos

Aunque la narrativa se apoya deliberadamente en el misterio, **los objetivos inmediatos siempre son claros**.

Algunos ejemplos son:

- Encontrar una llave específica.
- Llegar a una zona determinada.
- Responder al walkie-talkie.
- Investigar el origen de un sonido.

La incertidumbre debe recaer sobre la historia, **no sobre las acciones que el jugador debe realizar**. Mantener esta separación evita la desorientación y favorece la inmersión.

---

## 2.4 Retroalimentación constante

El juego proporciona información continua mediante distintos canales:

- **Visual:** nivel de batería, cambios en el entorno tras fenómenos paranormales.
- **Sonoro:** pasos, estática del walkie-talkie y proximidad del antagonista.
- **Narrativo:** notas, grabaciones y otros elementos que confirman el avance de la investigación.

Cada acción relevante produce una respuesta inmediata del sistema, reforzando la sensación de control incluso cuando el protagonista se encuentra en desventaja frente al antagonista.

---

## 2.5 Motivación para continuar jugando

La progresión del jugador no se basa en obtener armas o aumentar su poder, sino en la **motivación narrativa**.

Los principales motores del avance son:

- Encontrar a la hermana desaparecida.
- Descubrir el origen de los fenómenos paranormales.
- Desbloquear uno de los finales alternativos según las decisiones tomadas y las pistas encontradas.

En consecuencia, el progreso se mide por la comprensión de la historia y no por la evolución de las estadísticas del personaje, en línea con las convenciones del terror psicológico.

---

## 2.6 Síntesis: cómo estos elementos sostienen el Flow

El estado de Flow se mantiene gracias a la combinación de cinco principios de diseño:

- El desafío aumenta mediante la percepción y la toma de decisiones, no por la dificultad motriz.
- La progresión de la dificultad se distribuye gradualmente a través de las distintas zonas del edificio.
- Los objetivos inmediatos permanecen claros, aun cuando la narrativa sea ambigua.
- La retroalimentación es constante y utiliza múltiples canales sensoriales.
- La motivación surge del desarrollo de la historia y del descubrimiento, antes que del incremento del poder del personaje.

En conjunto, estos elementos buscan mantener al jugador inmerso en una experiencia continua de **terror, exploración y supervivencia**, evitando que la frustración o el aburrimiento interrumpan el estado de Flow.

---

# 3. Ciclo de juego, tutorial y exploración

## 3.1 Ciclo de juego

El ciclo de juego mantiene una estructura simple y repetitiva, donde cada etapa contribuye a reforzar la experiencia de **terror, exploración y supervivencia**.

1. **Explorar una zona del edificio.**  
   El jugador recorre el entorno observando detalles visuales y sonoros para obtener información. En ausencia de mapas o indicadores automáticos, la exploración constituye el principal medio para comprender el escenario, manteniendo una tensión constante al no revelar explícitamente las amenazas.

2. **Buscar pistas y objetos importantes.**  
   Se inspeccionan habitaciones, cajones y otros elementos del entorno para encontrar notas, llaves y objetos relevantes. Estas búsquedas impulsan tanto la progresión narrativa como el avance jugable, además de proporcionar información útil sobre el edificio y el comportamiento del antagonista.

3. **Utilizar la linterna y el walkie-talkie.**  
   Ambas herramientas representan las principales fuentes de información del jugador, pero implican riesgos. La linterna facilita la exploración a costa de aumentar la exposición, mientras que el walkie-talkie puede ofrecer ayuda o generar mayor incertidumbre mediante interferencias.

4. **Desbloquear una nueva área.**  
   Resolver obstáculos como puertas cerradas, mecanismos o llaves permite acceder a nuevas zonas del edificio. Cada apertura representa una recompensa tangible y refuerza la sensación de progreso.

5. **Activar un evento paranormal o narrativo.**  
   Determinadas acciones o el ingreso a ciertas áreas desencadenan fenómenos paranormales o revelaciones de la historia. Estos eventos profundizan el terror psicológico y refuerzan la identidad del escenario como un lugar hostil y cargado de misterio.

6. **Superar una persecución o escapar.**  
   Cuando el antagonista detecta al jugador, la única alternativa es huir, esconderse o encontrar una ruta segura. Esta etapa concentra el mayor nivel de tensión y transmite la sensación de vulnerabilidad que caracteriza al proyecto.

7. **Retomar la investigación.**  
   Tras sobrevivir a la persecución, el jugador vuelve a explorar e investigar la siguiente zona, repitiendo el ciclo hasta reunir la información necesaria para escapar del edificio y alcanzar uno de los finales posibles.

---

## 3.2 Tutorial

El tutorial se integra de forma natural en la narrativa durante los primeros minutos de juego.

Mientras ingresa al edificio junto a sus compañeros, el jugador aprende progresivamente las mecánicas básicas mediante acciones como:

- Moverse por el escenario.
- Encender la linterna.
- Interactuar con puertas y objetos.
- Utilizar el walkie-talkie.

De esta forma, el aprendizaje ocurre dentro de la propia experiencia de exploración, evitando interrumpir la inmersión con pantallas o secuencias de instrucciones separadas.

---

## 3.3 Exploración

La exploración transcurre dentro de una **fábrica abandonada**, un escenario lo suficientemente amplio para ofrecer variedad, pero contenido para mantener un alcance técnico viable.

Entre las principales áreas se encuentran:

- Entrada principal.
- Pasillos.
- Sectores industriales.
- Sótano.
- Zonas elevadas o de difícil acceso.

Cada sector aporta nueva información sobre:

- Las víctimas anteriores.
- El antagonista.
- El destino del grupo de amigos.

Para reforzar la sensación de inquietud, algunas áreas pueden reutilizarse con ligeras modificaciones entre visitas, generando la impresión de un espacio cambiante o de un bucle psicológico. Esta decisión incrementa la atmósfera de terror sin requerir una cantidad excesiva de escenarios distintos, manteniendo el proyecto dentro de un alcance realista de producción.

--- 

# 4. Narrativa

La historia sigue a una joven de un pueblo olvidado cuya vida quedó marcada por una tragedia reciente: un grupo de exploradores urbanos desapareció dentro de una fábrica abandonada. Entre las víctimas se encontraba su hermana, cuyo cuerpo nunca fue encontrado.

Ante la falta de respuestas por parte de la policía, decide regresar al edificio junto a un grupo de amigos que busca grabar contenido paranormal para reactivar su canal. Sin embargo, durante la exploración ocurre un accidente: un montacargas falla y la protagonista queda aislada del resto del grupo, conservando únicamente un contacto intermitente mediante un walkie-talkie.

A partir de ese momento, la exploración da paso a la supervivencia. El jugador deberá recorrer la fábrica, reunir pistas y reconstruir los hechos mientras intenta mantenerse con vida.

Durante el recorrido encontrará:

- Documentos y objetos pertenecientes a las víctimas anteriores.
- Rastros de los sucesos ocurridos en el edificio.
- Manifestaciones paranormales que muestran fragmentos del pasado.

Estas apariciones no representan una amenaza directa, pero ofrecen información incompleta que incrementa la incertidumbre y la tensión.

La principal amenaza es una figura oscura que patrulla la fábrica. Cuando detecta al jugador, lo obliga a huir o esconderse, aunque su comportamiento resulta impredecible: en ocasiones parece anticipar sus movimientos o intervenir de formas que desafían la lógica.

Con el avance de la historia comienzan a surgir inconsistencias, como mensajes extraños en el walkie-talkie, cambios sutiles en el entorno y situaciones difíciles de explicar. Estos elementos buscan mantener al jugador en una duda constante sobre qué es real y qué forma parte de los fenómenos que envuelven al edificio.

El objetivo final es descubrir qué ocurrió con las víctimas, encontrar a la hermana de la protagonista, sobrevivir a la amenaza y escapar de la fábrica.

---

## 4.1 Escena de ejemplo

Tras sentirse repentinamente indispuesto, el protagonista es perseguido por la figura desconocida y consigue refugiarse en una habitación hasta que la amenaza desaparece.

Mientras inspecciona el lugar encuentra una pequeña llave. Al darse vuelta para continuar avanzando, el entorno cambia por completo: aparecen manchas de sangre, daños estructurales y evidentes señales de violencia que antes no estaban allí.

La escena pone en duda todo lo ocurrido durante la persecución y, pocos metros más adelante, el jugador descubre el cuerpo sin vida de uno de sus compañeros.

---

## 4.2 Objetivos narrativos

- Descubrir qué ocurrió con el grupo de amigos.
- Reconstruir la historia de las víctimas anteriores.
- Sobrevivir a la entidad que habita la fábrica.
- Encontrar una salida y escapar del edificio.

---

## 4.3 Posibles finales

El desenlace dependerá del progreso del jugador y de las decisiones tomadas durante la exploración.

- **Final 1:** la protagonista logra escapar sola.
- **Final 2:** la protagonista encuentra a uno de sus amigos y ambos consiguen huir.
- **Final 3:** la protagonista es capturada por la entidad y se convierte en una nueva víctima del edificio.

---

# 5. Desarrollo: sistemas de juego

A continuación se describen los principales sistemas previstos para el desarrollo del proyecto, diferenciando aquellos que forman parte del **núcleo jugable** de los que cumplen un **rol de apoyo narrativo**.

---

## 5.1 Sistemas núcleo

Estos sistemas conforman la base del gameplay y son indispensables para que el ciclo de juego funcione correctamente.

### Exploración en primera persona

Permite recorrer la fábrica, interactuar con el entorno y progresar por distintas zonas. Requiere una navegación consistente, colisiones sólidas y referencias visuales que orienten al jugador sin recurrir a mapas o waypoints.

### Interacción con objetos

Gestiona acciones como abrir puertas, revisar cajones, recoger llaves, leer documentos y activar elementos del escenario. Debe mantener el estado de cada objeto e integrarse con el sistema de investigación.

### Linterna

Es una herramienta central tanto para la exploración como para la supervivencia. Limita la visión, consume batería progresivamente y puede revelar la posición del jugador al antagonista, convirtiendo cada uso en una decisión estratégica.

### Inventario básico

Permite almacenar llaves, documentos y objetos necesarios para progresar. Se prioriza la simplicidad y la claridad sobre sistemas complejos de gestión.

### Sistema de amenaza (IA del antagonista)

El enemigo debe operar mediante una máquina de estados que contemple:

- Patrulla.
- Detección.
- Persecución.
- Búsqueda.
- Retorno a la patrulla.

Su comportamiento puede presentar variaciones controladas para mantener la incertidumbre sin resultar injusto para el jugador.

### Persecución y escape

Diseñado para generar momentos de alta tensión mediante rutas de escape reconocibles, obstáculos previsibles y escondites. Se busca evitar que el error implique siempre un *game over* inmediato.

### Escondites y zonas seguras

Permite ocultarse en armarios, muebles o zonas oscuras. El sistema de detección debe considerar tanto la visibilidad como el ruido generado por el jugador.

### Progresión por zonas

Controla el desbloqueo gradual de la fábrica mediante llaves, eventos narrativos u obstáculos, regulando el ritmo del juego y su curva de dificultad.

---

## 5.2 Sistemas de apoyo narrativo

Estos sistemas enriquecen la atmósfera y el desarrollo de la historia.

### Walkie-talkie

Introduce comunicación con el grupo mediante mensajes, interferencias y señales falsas, reforzando la incertidumbre narrativa.

### Sistema de pistas e investigación

Centraliza notas, grabaciones, fotografías y objetos relevantes en un diario o códice consultable, permitiendo reconstruir la historia de forma no lineal.

### Eventos paranormales

Incluyen cambios en el entorno, sonidos y apariciones que aportan información sobre el pasado sin representar una amenaza directa.

### Finales alternativos

Un sistema de variables registra decisiones, pistas descubiertas y posibles rescates para determinar cuál de los tres desenlaces obtiene el jugador.

---

## 5.3 Priorización de producción

Para el desarrollo inicial del proyecto se propone la siguiente estrategia:

### Primera etapa (MVP)

Implementar los sistemas esenciales para validar el ciclo principal de juego:

- Exploración.
- Interacción.
- Linterna.
- Inventario.
- IA del antagonista.
- Persecución.
- Escondites.
- Progresión por zonas.

### Segunda etapa

Una vez validado el prototipo jugable, incorporar de forma incremental los sistemas orientados a reforzar la narrativa y la atmósfera:

- Walkie-talkie.
- Sistema de investigación.
- Eventos paranormales.
- Finales alternativos.

Esta planificación permite obtener un prototipo funcional en los primeros sprints y ampliar posteriormente la experiencia sin comprometer el alcance del proyecto.