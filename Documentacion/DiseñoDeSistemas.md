# 1. Introducción y alcance

Este documento constituye un **anexo técnico** complementario al **Documento de Diseño de Juego (GDD)**.

Mientras que el GDD describe **qué debe hacer cada sistema** desde la perspectiva del diseño y de la experiencia del jugador, este documento se enfoca en **cómo se implementan esos sistemas** desde el punto de vista del software.

En particular, se documentan:

- La arquitectura general del proyecto.
- Las clases e interfaces que componen cada sistema.
- Los patrones de diseño utilizados.
- Las decisiones técnicas adoptadas para obtener una implementación mantenible, modular y con bajo acoplamiento.

---

# 2. Comunicación entre sistemas: `IGameEventBus`

La mayoría de los sistemas del juego necesitan reaccionar a eventos generados por otros sistemas. Por ejemplo:

- El sistema de IA debe saber si la linterna está encendida.
- El sistema de progresión debe detectar cuándo se encontró una pista.
- El sistema de finales debe registrar si el jugador fue atrapado.

Permitir que cada sistema mantenga referencias directas a todos los demás genera un fuerte acoplamiento y suele derivar en el uso de **Managers estáticos** accesibles desde cualquier parte del proyecto, dificultando el mantenimiento y la escalabilidad.

Para evitar este problema, la comunicación se centraliza mediante la interfaz **`IGameEventBus`**, que combina dos patrones de diseño:

- **Observer:** cada sistema se suscribe únicamente a los eventos que necesita.
- **Mediator:** los sistemas no se conocen entre sí; todos interactúan exclusivamente con el bus de eventos.

Cada evento se representa mediante una clase sencilla que implementa `IGameEvent` y transporta únicamente la información necesaria.

## Interfaces

```csharp
public interface IGameEvent { }

public interface IGameEventBus
{
    void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent;

    void Subscribe<TEvent>(Action<TEvent> handler)
        where TEvent : IGameEvent;

    void Unsubscribe<TEvent>(Action<TEvent> handler)
        where TEvent : IGameEvent;
}
```

## Implementación

> La implementación concreta se inyecta desde `GameInstaller`, evitando el uso de un **Singleton** estático.

```csharp
public sealed class GameEventBus : IGameEventBus
{
    private readonly Dictionary<Type, Delegate> _handlers = new();

    public void Publish<TEvent>(TEvent gameEvent)
        where TEvent : IGameEvent
    {
        if (_handlers.TryGetValue(typeof(TEvent), out var d))
            ((Action<TEvent>)d)?.Invoke(gameEvent);
    }

    public void Subscribe<TEvent>(Action<TEvent> handler)
        where TEvent : IGameEvent
    {
        var t = typeof(TEvent);

        _handlers[t] = _handlers.TryGetValue(t, out var existing)
            ? Delegate.Combine(existing, handler)
            : handler;
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler)
        where TEvent : IGameEvent
    {
        var t = typeof(TEvent);

        if (_handlers.TryGetValue(t, out var existing))
            _handlers[t] = Delegate.Remove(existing, handler);
    }
}
```

---

# 3. Diseño de los sistemas

Cada sistema del proyecto se documenta siguiendo una estructura común:

- **Responsabilidad:** qué problema resuelve dentro del juego.
- **Clases e interfaces principales:** componentes que conforman el sistema.
- **Patrones de diseño:** soluciones arquitectónicas empleadas.
- **Esqueleto de implementación:** ejemplo simplificado en C#.
- **Relación con otros sistemas:** dependencias e integración con el resto de la arquitectura.

---

# 3.1 Sistema de exploración en primera persona *(Núcleo)*

## Responsabilidad

Controlar el movimiento del jugador y la cámara en primera persona, manteniendo desacoplados:

- El origen del **input**.
- La lógica de movimiento.
- La presentación visual.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IInputProvider` | Abstrae el origen del input (teclado, mouse, gamepad, etc.). |
| `PlayerInputProvider` | Implementación concreta utilizando el Input System de Unity. |
| `IMovementState` | Define la interfaz del patrón **State** para los estados de movimiento. |
| `WalkingState` | Implementa el comportamiento al caminar. |
| `CrouchingState` | Implementa el comportamiento al agacharse. |
| `PlayerMotor` | Contexto del patrón **State**; delega el movimiento al estado activo. |
| `FirstPersonCameraController` | Gestiona exclusivamente la rotación de la cámara. |

> **Nota:** `HidingState` se define en el sistema de escondites (apartado **3.9**) y reutiliza la misma arquitectura basada en estados.

---

### State

Permite representar los distintos estados de movimiento (`WalkingState`, `CrouchingState`, `HidingState`, etc.) sin llenar `PlayerMotor` de condicionales.

### Strategy

`IInputProvider` actúa como una estrategia de entrada, permitiendo cambiar el origen del input (teclado, gamepad o simulación para pruebas) sin modificar el sistema de movimiento.

---

## Esqueleto de implementación

```csharp
public interface IInputProvider
{
    Vector2 GetMoveAxis();
    Vector2 GetLookDelta();
    bool GetCrouchPressed();
}

public interface IMovementState
{
    void Enter(PlayerMotor ctx);
    void Tick(PlayerMotor ctx, float deltaTime);
    void Exit(PlayerMotor ctx);
}

public sealed class PlayerMotor : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;

    private IInputProvider _input;
    private IMovementState _current;

    public void Construct(IInputProvider input)
        => _input = input;

    public void ChangeState(IMovementState next)
    {
        _current?.Exit(this);
        _current = next;
        _current.Enter(this);
    }

    private void Update()
        => _current?.Tick(this, Time.deltaTime);

    public CharacterController Controller => _controller;

    public IInputProvider Input => _input;
}

public sealed class WalkingState : IMovementState
{
    public void Enter(PlayerMotor ctx) { }

    public void Tick(PlayerMotor ctx, float dt)
    {
        var move = ctx.Input.GetMoveAxis();

        var dir = ctx.transform.TransformDirection(
            new Vector3(move.x, 0, move.y));

        ctx.Controller.Move(dir * dt * 4f);
    }

    public void Exit(PlayerMotor ctx) { }
}
```

---

## Relación con otros sistemas

El sistema publica la posición y el estado actual del jugador mediante **`IGameEventBus`**.

Esta información es utilizada por:

- **Sistema de progresión por zonas (3.11):** detecta el ingreso del jugador a nuevas áreas.
- **Sistema de amenaza (3.7):** ajusta el radio de detección del antagonista según el estado actual del jugador, reduciéndolo cuando se encuentra en `CrouchingState` o `HidingState`.

---

# 3.2 Sistema de interacción con objetos *(Núcleo)*

## Responsabilidad

Detectar e interactuar con los distintos objetos del escenario (puertas, cajones, llaves, notas, etc.) mediante una interfaz común, evitando que el sistema de detección dependa de implementaciones concretas.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IInteractable` | Define el comportamiento común de cualquier objeto interactuable. |
| `IBlockable` | Representa objetos que pueden bloquearse durante el juego. |
| `Door` | Implementa una puerta interactuable y bloqueable. |
| `Drawer` | Variante de objeto interactuable para cajones. |
| `PickableKey` | Objeto interactuable que representa una llave coleccionable. |
| `ReadableNote` | Permite leer una nota y publicar un evento de pista recolectada. |
| `InteractionDetector` | Detecta el objeto interactuable más cercano y ejecuta la interacción correspondiente. |

---

Todos los objetos interactuables implementan `IInteractable`, permitiendo que el detector trabaje únicamente con la abstracción, sin conocer los tipos concretos.

### Observer

`ReadableNote` no interactúa directamente con el sistema de pistas; simplemente publica un evento mediante `IGameEventBus`, desacoplando ambos sistemas.

---

## Esqueleto de implementación

```csharp
public interface IInteractable
{
    string Prompt { get; }

    void Interact(PlayerMotor player);
}

public interface IBlockable
{
    void Block();
}

public sealed class Door : MonoBehaviour, IInteractable, IBlockable
{
    [SerializeField] private bool _locked;

    public string Prompt =>
        _locked ? "Cerrada con llave" : "Abrir puerta";

    public void Interact(PlayerMotor player)
    {
        if (_locked)
            return;

        transform.Rotate(0, 90f, 0);
    }

    public void Block()
        => _locked = true;
}

public sealed class ReadableNote : MonoBehaviour, IInteractable
{
    [SerializeField] private Clue _clue;

    private IGameEventBus _bus;

    public void Construct(IGameEventBus bus)
        => _bus = bus;

    public string Prompt => "Leer nota";

    public void Interact(PlayerMotor player)
    {
        _bus.Publish(new ClueCollectedEvent(_clue));
    }
}

public sealed class InteractionDetector : MonoBehaviour
{
    [SerializeField] private float _range = 2f;

    private IInteractable _current;

    private void Update()
    {
        _current = FindClosestInteractable(); // Raycast (detalle omitido)

        if (_current != null && Input.GetKeyDown(KeyCode.E))
            _current.Interact(GetComponent<PlayerMotor>());
    }
}
```

---

## Relación con otros sistemas

El sistema de interacción actúa como punto de entrada para diversos sistemas del juego:

- **Sistema de pistas (3.5):** recibe los eventos publicados por `ReadableNote`.
- **Sistema de inventario (3.6):** registra la recolección de llaves y otros objetos mediante eventos generados por `PickableKey`.
- **Sistema de persecución (3.8):** utiliza la interfaz `IBlockable` para bloquear determinadas puertas durante una persecución.

---

# 3.3 Sistema de linterna *(Núcleo)*

## Responsabilidad

Administrar la linterna del jugador como una mecánica central de exploración y supervivencia.

El sistema controla el haz de luz, el consumo de batería como recurso limitado y comunica su estado al resto del juego mediante eventos, evitando que otros sistemas dependan de su implementación interna.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IBatterySource` | Abstrae cualquier fuente de energía para la linterna. |
| `Battery` | Implementación concreta de una batería consumible. |
| `Flashlight` | Controla el haz de luz y delega la gestión de energía a `IBatterySource`. |

---

### Observer

La linterna publica eventos (`BatteryChangedEvent` y `FlashlightToggledEvent`) para informar cambios de estado. Los demás sistemas reaccionan a dichos eventos sin consultar continuamente la linterna.

---

## Esqueleto de implementación

```csharp
public interface IBatterySource
{
    float Charge01 { get; }

    bool TryConsume(float amount);

    void Recharge(float amount);
}

public sealed class Battery : IBatterySource
{
    private float _charge = 1f;

    public float Charge01 => _charge;

    public bool TryConsume(float amount)
    {
        if (_charge <= 0f)
            return false;

        _charge = Mathf.Max(0f, _charge - amount);
        return true;
    }

    public void Recharge(float amount)
    {
        _charge = Mathf.Min(1f, _charge + amount);
    }
}

public sealed class Flashlight : MonoBehaviour
{
    [SerializeField] private Light _beam;

    private IBatterySource _battery;
    private IGameEventBus _bus;

    public bool IsOn { get; private set; }

    public void Construct(
        IBatterySource battery,
        IGameEventBus bus)
    {
        _battery = battery;
        _bus = bus;
    }

    public void Toggle()
    {
        IsOn = !IsOn;

        _beam.enabled = IsOn;

        _bus.Publish(new FlashlightToggledEvent(IsOn));
    }

    private void Update()
    {
        if (!IsOn)
            return;

        if (!_battery.TryConsume(0.01f * Time.deltaTime))
            Toggle(); // Se apaga automáticamente al agotarse la batería.

        _bus.Publish(new BatteryChangedEvent(_battery.Charge01));
    }
}
```

---

## Relación con otros sistemas

Los eventos generados por la linterna son utilizados por distintos componentes del juego:

- **Sistema de amenaza (3.7):** `FlashlightToggledEvent` modifica el radio de detección visual del antagonista cuando la linterna está encendida.
- **Interfaz de usuario:** `BatteryChangedEvent` actualiza el indicador visual del nivel de batería (fuera del alcance de este documento).

---

# 3.4 Sistema de walkie-talkie *(Apoyo narrativo)*

## Responsabilidad

Gestionar la recepción y reproducción de mensajes de radio, permitiendo distintos tipos de comunicación (mensajes guionados, interferencias y señales falsas) sin acoplar el sistema a implementaciones concretas.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IRadioMessage` | Define la información común de cualquier mensaje de radio. |
| `ScriptedMessage` | Representa un mensaje narrativo predefinido. |
| `InterferenceMessage` | Representa una transmisión con interferencias. |
| `FalseSignalMessage` | Simula una señal falsa o engañosa. |
| `RadioMessageFactory` | Crea la instancia adecuada de `IRadioMessage` según el tipo solicitado. |
| `WalkieTalkie` | Escucha eventos de activación, reproduce el mensaje y notifica su recepción. |

---

### Factory Method
La creación de mensajes se delega a `RadioMessageFactory`, desacoplando a `WalkieTalkie` de las distintas implementaciones de `IRadioMessage`.

### Observer
Los eventos que disparan una transmisión (`RadioTriggerEvent`) son publicados por otros sistemas a través de `IGameEventBus`, evitando referencias directas al walkie-talkie.

---

## Esqueleto de implementación

```csharp
public interface IRadioMessage
{
    AudioClip Clip { get; }

    string Transcript { get; }

    bool IsReliable { get; }
}

public enum MessageType
{
    Scripted,
    Interference,
    FalseSignal
}

public static class RadioMessageFactory
{
    public static IRadioMessage Create(
        MessageType type,
        RadioMessageData data)
        => type switch
        {
            MessageType.Scripted => new ScriptedMessage(data),
            MessageType.Interference => new InterferenceMessage(data),
            MessageType.FalseSignal => new FalseSignalMessage(data),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
}

public sealed class WalkieTalkie : MonoBehaviour
{
    [SerializeField] private AudioSource _speaker;

    private IGameEventBus _bus;

    public void Construct(IGameEventBus bus)
    {
        _bus = bus;
        _bus.Subscribe<RadioTriggerEvent>(OnTrigger);
    }

    private void OnTrigger(RadioTriggerEvent e)
    {
        var msg = RadioMessageFactory.Create(e.Type, e.Data);

        _speaker.PlayOneShot(msg.Clip);

        _bus.Publish(new RadioMessageReceivedEvent(msg));
    }
}
```

---

## Relación con otros sistemas

El sistema de walkie-talkie se integra mediante eventos publicados en `IGameEventBus`:

- **Sistema de progresión por zonas (3.11):** puede publicar `RadioTriggerEvent` al ingresar a un nuevo sector del edificio.
- **Triggers narrativos:** también pueden generar eventos de radio de forma independiente.
- **Sistema de pistas (3.5):** puede observar `RadioMessageReceivedEvent` cuando un mensaje aporte información relevante para la investigación.

---

# 3.5 Sistema de pistas e investigación *(Apoyo narrativo)*

## Responsabilidad

Gestionar el almacenamiento de las pistas recolectadas durante la exploración (notas, grabaciones, fotografías, etc.), permitiendo reconstruir la historia de forma no lineal y manteniendo desacoplados el almacenamiento de los datos y su representación en la interfaz.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `Clue` | Estructura de datos que representa una pista. |
| `IClueRepository` | Define las operaciones para almacenar y consultar pistas. |
| `ClueJournal` | Implementación concreta del repositorio de pistas. |
| `ClueJournalView` | Interfaz de usuario que muestra las pistas disponibles y se actualiza mediante eventos. |

---

### Repository
El almacenamiento de pistas se encapsula detrás de `IClueRepository`, permitiendo cambiar la implementación sin afectar al resto del sistema.

### Observer
La interfaz (`ClueJournalView`) reacciona a los cambios mediante eventos publicados por el repositorio, evitando consultas constantes y reduciendo el acoplamiento.

---

## Esqueleto de implementación

```csharp
public readonly struct Clue
{
    public string Id { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }
}

public interface IClueRepository
{
    void Add(Clue clue);

    IReadOnlyList<Clue> GetAll();
}

public sealed class ClueJournal : IClueRepository
{
    private readonly List<Clue> _clues = new();

    private readonly IGameEventBus _bus;

    public ClueJournal(IGameEventBus bus)
    {
        _bus = bus;

        _bus.Subscribe<ClueCollectedEvent>(
            e => Add(e.Clue));
    }

    public void Add(Clue clue)
    {
        if (_clues.Any(c => c.Id == clue.Id))
            return;

        _clues.Add(clue);

        _bus.Publish(
            new ClueJournalUpdatedEvent(_clues.Count));
    }

    public IReadOnlyList<Clue> GetAll()
        => _clues;
}
```

---

## Relación con otros sistemas

El sistema de investigación se integra con el resto del juego mediante eventos:

- **Sistema de interacción con objetos (3.2):** recibe `ClueCollectedEvent`, publicado por `ReadableNote` al inspeccionar una nota.
- **Sistema de progresión por zonas (3.11):** consume `ClueJournalUpdatedEvent` para desbloquear nuevas áreas en función de la cantidad o del tipo de pistas descubiertas.

---

# 3.6 Sistema de inventario básico *(Núcleo)*

## Responsabilidad

Administrar el inventario del jugador almacenando llaves, documentos y otros objetos clave, ocultando la implementación interna de la colección y priorizando la simplicidad sobre sistemas complejos de gestión de recursos.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IInventoryItem` | Define la información básica de cualquier objeto almacenable. |
| `IUsable` | Representa objetos que pueden utilizarse por el jugador. |
| `KeyItem` | Implementación de un objeto tipo llave. |
| `DocumentItem` | Implementación de un documento o pista coleccionable. |
| `Inventory` | Gestiona la colección de objetos y permite recorrerla mediante iteradores. |

---

### Iterator

El inventario implementa `IEnumerable<IInventoryItem>`, permitiendo recorrer los elementos sin exponer la estructura interna de almacenamiento.

---

## Esqueleto de implementación

```csharp
public interface IInventoryItem
{
    string Name { get; }

    Sprite Icon { get; }
}

public interface IUsable
{
    void Use(PlayerMotor player);
}

public sealed class KeyItem : IInventoryItem
{
    public string Name { get; }

    public Sprite Icon { get; }
}

public sealed class Inventory : IEnumerable<IInventoryItem>
{
    private readonly List<IInventoryItem> _items = new();

    public void Add(IInventoryItem item)
        => _items.Add(item);

    public bool Remove(IInventoryItem item)
        => _items.Remove(item);

    public IEnumerator<IInventoryItem> GetEnumerator()
        => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
```

---

## Relación con otros sistemas

El inventario interactúa con otros sistemas mediante eventos y consultas:

- **Sistema de interacción con objetos (3.2):** incorpora nuevos objetos cuando el jugador recoge un ítem interactuable.
- **Sistema de progresión por zonas (3.11):** consulta la presencia de objetos específicos (por ejemplo, una llave) para determinar si una zona puede desbloquearse.

---

# 3.7 Sistema de amenaza / enemigo (IA) *(Núcleo)*

## Responsabilidad

Controlar el comportamiento del antagonista mediante una **máquina de estados**, garantizando transiciones claras entre los distintos comportamientos y evitando una IA basada en aleatoriedad. Las variaciones se producen de forma controlada dentro de cada estado para mantener una experiencia desafiante, pero justa.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IEnemyState` | Define la interfaz común para los estados del enemigo. |
| `PatrolState` | Estado de patrullaje. |
| `AlertState` | Estado de alerta ante posibles indicios del jugador. |
| `ChaseState` | Estado de persecución activa. |
| `SearchState` | Estado de búsqueda tras perder al jugador. |
| `ReturnState` | Regreso al recorrido de patrulla. |
| `IDetectionStrategy` | Abstrae el algoritmo de detección del jugador. |
| `VisionDetection` | Implementa la detección mediante visión. |
| `SoundDetection` | Implementa la detección mediante sonido. |
| `CompositeDetection` | Combina múltiples estrategias de detección. |
| `EnemyController` | Coordina la máquina de estados y las estrategias de percepción. |

---

### State
Cada comportamiento del enemigo se encapsula en una clase independiente, evitando un único `Update()` con múltiples condicionales y facilitando la incorporación de nuevos estados.

### Strategy
La detección del jugador se abstrae mediante `IDetectionStrategy`, permitiendo intercambiar o combinar algoritmos de visión y sonido sin modificar `EnemyController`.

---

## Esqueleto de implementación

```csharp
public interface IEnemyState
{
    void Enter(EnemyController ctx);

    void Tick(EnemyController ctx, float dt);

    void Exit(EnemyController ctx);
}

public interface IDetectionStrategy
{
    bool CanDetect(EnemyController ctx, PlayerMotor player);
}

public sealed class VisionDetection : IDetectionStrategy
{
    public bool CanDetect(
        EnemyController ctx,
        PlayerMotor player)
        => WithinConeAndUnobstructed(ctx, player);
        // Detalle omitido.
}

public sealed class EnemyController : MonoBehaviour
{
    private IEnemyState _state;
    private IDetectionStrategy _detection;
    private IGameEventBus _bus;

    public void Construct(
        IDetectionStrategy detection,
        IGameEventBus bus)
    {
        _detection = detection;
        _bus = bus;

        ChangeState(new PatrolState());
    }

    public void ChangeState(IEnemyState next)
    {
        _state?.Exit(this);

        _state = next;

        _state.Enter(this);

        _bus.Publish(
            new EnemyStateChangedEvent(next.GetType().Name));
    }

    public bool DetectsPlayer(PlayerMotor player)
        => _detection.CanDetect(this, player);

    private void Update()
        => _state?.Tick(this, Time.deltaTime);
}

public sealed class PatrolState : IEnemyState
{
    public void Enter(EnemyController ctx) { }

    public void Tick(EnemyController ctx, float dt)
    {
        if (ctx.DetectsPlayer(PlayerRef.Current))
            ctx.ChangeState(new ChaseState());
    }

    public void Exit(EnemyController ctx) { }
}

// AlertState, ChaseState, SearchState y ReturnState
// mantienen la misma estructura con lógica específica.
```
## Relación con otros sistemas

El inventario interactúa con otros sistemas mediante eventos y consultas:

- `EnemyStateChangedEvent` es el evento más consumido del proyecto: lo escuchan el sistema de persecución (3.8) para saber cuándo iniciar/terminar una secuencia, y potencialmente audio/UI. 
- La estrategia de detección consulta el estado publicado por `PlayerMotor (3.1)` y por `Flashlight (3.3)`.

---

# 3.8 Sistema de persecución y escape *(Núcleo)*

## Responsabilidad

Coordinar las secuencias de persecución, administrando su duración y determinando cuándo el jugador logra escapar o es capturado, sin depender de la implementación interna de la IA del antagonista.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IBlockable` | Interfaz reutilizada del sistema de interacción (3.2) para bloquear elementos del entorno durante una persecución. |
| `ChaseSequenceController` | Orquesta el ciclo de vida de una persecución a partir de los eventos publicados por la IA del enemigo. |

---

### Observer

`ChaseSequenceController` se suscribe a `EnemyStateChangedEvent` y responde únicamente cuando la IA cambia de estado, evitando consultar continuamente al enemigo.

### Reutilización de State

El sistema aprovecha la máquina de estados definida en el **Sistema de amenaza (3.7)** para identificar el inicio y el fin de una persecución, evitando duplicar lógica.

---

## Esqueleto de implementación

```csharp
public sealed class ChaseSequenceController : MonoBehaviour
{
    [SerializeField]
    private float _maxChaseDuration = 25f;

    private IGameEventBus _bus;

    private float _timer;

    private bool _active;

    public void Construct(IGameEventBus bus)
    {
        _bus = bus;

        _bus.Subscribe<EnemyStateChangedEvent>(OnEnemyState);
    }

    private void OnEnemyState(EnemyStateChangedEvent e)
    {
        _active = e.StateName == nameof(ChaseState);

        if (_active)
            _timer = 0f;
    }

    private void Update()
    {
        if (!_active)
            return;

        _timer += Time.deltaTime;

        if (_timer > _maxChaseDuration)
            _bus.Publish(new PlayerEscapedEvent());
    }

    public void OnPlayerCaught()
    {
        _bus.Publish(new PlayerCaughtEvent());
    }
}
```

---

## Relación con otros sistemas

El sistema de persecución se comunica con otros componentes mediante eventos y reutilización de interfaces:

- **Sistema de amenaza (3.7):** consume `EnemyStateChangedEvent` para iniciar o finalizar una persecución.
- **GameState:** recibe `PlayerCaughtEvent` y `PlayerEscapedEvent` para actualizar el estado general del juego.
- **Sistema de finales alternativos (3.12):** utiliza estos eventos para determinar el desenlace correspondiente.
- **Sistema de interacción con objetos (3.2):** puede invocar `Door.Block()` mediante la interfaz `IBlockable` para bloquear rutas de escape durante una persecución.

---

# 3.9 Sistema de escondite y zonas seguras *(Núcleo)*

## Responsabilidad

Permitir que el jugador se oculte del antagonista reutilizando la máquina de estados del sistema de movimiento (3.1), evitando la creación de un sistema paralelo de estados.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IHidingSpot` | Define un punto de ocultamiento interactuable. |
| `Locker` | Escondite tipo armario. |
| `UnderTable` | Escondite tipo mesa. |
| `HidingState` | Estado de movimiento del jugador cuando está oculto (reutiliza `IMovementState`). |

---

### reutilización del sistema existente

El sistema de escondite se integra directamente como una nueva implementación de `IMovementState`, reutilizando la arquitectura definida en el sistema de exploración (3.1).

Esto refuerza **extensibilidad sin duplicación de estructuras**.

---

## Esqueleto de implementación

```csharp
public interface IHidingSpot
{
    bool IsOccupied { get; }

    void Enter(PlayerMotor player);

    void Exit(PlayerMotor player);
}

public sealed class Locker : MonoBehaviour, IHidingSpot
{
    public bool IsOccupied { get; private set; }

    public void Enter(PlayerMotor player)
    {
        IsOccupied = true;

        player.ChangeState(new HidingState(this));
    }

    public void Exit(PlayerMotor player)
    {
        IsOccupied = false;

        player.ChangeState(new WalkingState());
    }
}

public sealed class HidingState : IMovementState
{
    private readonly IHidingSpot _spot;

    public HidingState(IHidingSpot spot)
        => _spot = spot;

    public void Enter(PlayerMotor ctx)
    {
        // Reduce visibilidad consultada por VisionDetection (3.7)
    }

    public void Tick(PlayerMotor ctx, float dt) { }

    public void Exit(PlayerMotor ctx) { }
}
```

---

## Relación con otros sistemas

- **Sistema de exploración en primera persona (3.1):** reutiliza `IMovementState`, integrando el escondite como un estado más del movimiento.
- **Sistema de amenaza / enemigo (3.7):** `VisionDetection` consulta el estado de visibilidad reducido por `HidingState` al ejecutar `CanDetect()`.
- **Sistema de interacción con objetos (3.2):** puede activar `IHidingSpot.Enter()` al interactuar con un objeto del entorno.

> La visibilidad reducida no se propaga mediante eventos, ya que representa un estado local y temporal del jugador, evaluado directamente durante la detección del enemigo.

---

# 3.10 Sistema de eventos paranormales *(Apoyo narrativo)*

## Responsabilidad

Gestionar la activación de eventos paranormales (cambios en el entorno, apariciones y anomalías sonoras) como acciones configurables y no hostiles, desacoplando completamente el trigger de zona del efecto concreto.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IParanormalEvent` | Define una acción paranormal ejecutable. |
| `EnvironmentChangeEvent` | Evento de modificación del entorno. |
| `ApparitionEvent` | Evento de aparición visual. |
| `SoundAnomalyEvent` | Evento de anomalía sonora. |
| `ParanormalEventTrigger` | Componente de zona que dispara un evento configurado al detectar al jugador. |

---

### Command

Cada `IParanormalEvent` encapsula una acción ejecutable como un objeto, permitiendo:

- Configuración desde el editor de Unity (`SerializeReference`)
- Posible encolado de eventos
- Registro para depuración o reproducción

### Observer

El sistema puede notificar al resto del juego mediante `IGameEventBus`, desacoplando completamente la ejecución del evento de sus efectos secundarios globales.

---

## Esqueleto de implementación

```csharp
public interface IParanormalEvent
{
    void Trigger(IGameEventBus bus);
}

public sealed class EnvironmentChangeEvent : IParanormalEvent
{
    public void Trigger(IGameEventBus bus)
    {
        ApplyEnvironmentChange(); // Detalle omitido.

        bus.Publish(
            new ParanormalTriggeredEvent(
                nameof(EnvironmentChangeEvent)));
    }
}

public sealed class ParanormalEventTrigger : MonoBehaviour
{
    [SerializeReference]
    private IParanormalEvent _event;

    private IGameEventBus _bus;

    public void Construct(IGameEventBus bus)
        => _bus = bus;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMotor>() != null)
            _event.Trigger(_bus);
    }
}
```

---

## Relación con otros sistemas

- **Sistema de pistas e investigación (3.5):** puede registrar eventos mediante `ParanormalTriggeredEvent` si la manifestación revela información relevante.
- **Sistema de audio/visual (fuera de alcance):** puede reaccionar a los eventos para generar efectos de atmósfera.
- **Sistema de progresión por zonas (3.11):** puede usar estos eventos como hitos narrativos o de desbloqueo contextual.

> Este sistema prioriza la separación entre *disparo* y *efecto*, permitiendo una gran variedad de eventos sin acoplar lógica al nivel de triggers.

---

# 3.11 Sistema de progresión por zonas *(Núcleo)*

## Responsabilidad

Controlar el desbloqueo progresivo de las áreas de la fábrica mediante un conjunto de condiciones combinables (ítems, pistas, flags narrativos), regulando el ritmo de exploración y progresión del jugador sin depender de lógica hardcodeada por zona.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IUnlockCondition` | Define la evaluación de una condición de desbloqueo. |
| `HasKeyCondition` | Condición basada en la posesión de un ítem. |
| `FlagCondition` | Condición basada en estado narrativo o flags del juego. |
| `CompositeCondition` | Combina múltiples condiciones con lógica AND/OR. |
| `ZoneManager` | Evalúa y desbloquea zonas en función del estado global del juego. |

---

### Composite / Specification Pattern

Las condiciones de desbloqueo se modelan como objetos componibles, permitiendo construir reglas complejas sin que `ZoneManager` conozca su lógica interna.

### Observer

El sistema no evalúa continuamente el estado del juego, sino que reacciona a eventos relevantes (pistas, ítems, etc.), reduciendo acoplamiento.

---

## Esqueleto de implementación

```csharp
public interface IUnlockCondition
{
    bool IsMet(GameState state);
}

public sealed class HasKeyCondition : IUnlockCondition
{
    private readonly string _keyId;

    public HasKeyCondition(string keyId)
        => _keyId = keyId;

    public bool IsMet(GameState state)
        => state.HasItem(_keyId);
}

public sealed class CompositeCondition : IUnlockCondition
{
    private readonly IUnlockCondition[] _conditions;
    private readonly bool _requireAll;

    public bool IsMet(GameState state)
        => _requireAll
            ? _conditions.All(c => c.IsMet(state))
            : _conditions.Any(c => c.IsMet(state));
}
```

```csharp
public sealed class ZoneManager : MonoBehaviour
{
    [SerializeField]
    private List<Zone> _zones;

    private IGameEventBus _bus;
    private GameState _state;

    public void Construct(
        IGameEventBus bus,
        GameState state)
    {
        _bus = bus;
        _state = state;

        _bus.Subscribe<ClueCollectedEvent>(_ => ReevaluateZones());
        _bus.Subscribe<ItemAcquiredEvent>(_ => ReevaluateZones());
    }

    private void ReevaluateZones()
    {
        foreach (var zone in _zones
            .Where(z => !z.Unlocked && z.Condition.IsMet(_state)))
        {
            zone.Unlock();

            _bus.Publish(
                new ZoneUnlockedEvent(zone.Id));
        }
    }
}
```

---

## Relación con otros sistemas

- **Sistema de pistas e investigación (3.5):** aporta `ClueCollectedEvent`, que puede habilitar nuevas áreas narrativas.
- **Sistema de inventario (3.6):** aporta `ItemAcquiredEvent`, utilizado para desbloqueos basados en llaves u objetos.
- **Sistema de walkie-talkie (3.4):** puede reaccionar a `ZoneUnlockedEvent` para disparar mensajes contextuales.
- **Sistema de eventos paranormales (3.10):** puede asociar fenómenos específicos a la entrada en nuevas zonas.

> Este sistema actúa como “regulador de ritmo” del juego, controlando la progresión estructural sin imponer una secuencia rígida al jugador.

---

# 3.12 Sistema de finales alternativos *(Apoyo narrativo / integrador)*

## Responsabilidad

Determinar el final correspondiente al llegar a la salida del edificio, evaluando un conjunto de condiciones sobre el estado acumulado de la partida. El sistema prioriza explícitamente los finales según el orden definido en el GDD.

---

## Clases e interfaces principales

| Componente | Responsabilidad |
|------------|-----------------|
| `IEndingCondition` | Define una condición evaluable para determinar un final. |
| `CaughtCondition` | Final por captura del jugador. |
| `CompanionRescuedCondition` | Final por escape con compañeros. |
| `SoloEscapeCondition` | Final por escape en solitario (condición por defecto). |
| `EndingEvaluator` | Evalúa la cadena de condiciones y selecciona el primer final válido. |

---

### Chain of Responsibility

Las condiciones de final se organizan en una cadena ordenada de evaluación. El sistema recorre la lista hasta encontrar la primera condición satisfecha.

El orden de la cadena no es arbitrario: representa directamente la **prioridad narrativa** definida en el GDD:

1. Ser atrapado
2. Escapar acompañado
3. Escapar solo (fallback)

---

## Esqueleto de implementación

```csharp
public interface IEndingCondition
{
    string EndingId { get; }

    bool IsSatisfied(GameState state);
}

public sealed class CaughtCondition : IEndingCondition
{
    public string EndingId => "Final3_Atrapado";

    public bool IsSatisfied(GameState state)
        => state.WasCaught;
}

public sealed class CompanionRescuedCondition : IEndingCondition
{
    public string EndingId => "Final2_Acompanado";

    public bool IsSatisfied(GameState state)
        => state.CompanionsRescued > 0;
}

public sealed class SoloEscapeCondition : IEndingCondition
{
    public string EndingId => "Final1_Solo";

    // Condición por defecto (fallback de la cadena)
    public bool IsSatisfied(GameState state)
        => true;
}

public sealed class EndingEvaluator
{
    // El orden del array define la prioridad narrativa
    private readonly IEndingCondition[] _chain =
    {
        new CaughtCondition(),
        new CompanionRescuedCondition(),
        new SoloEscapeCondition()
    };

    public string Evaluate(GameState state)
        => _chain.First(c => c.IsSatisfied(state)).EndingId;
}
```

---

## Relación con otros sistemas

El sistema de finales actúa como integrador del estado global de la partida:

- **Sistema de persecución y escape (3.8):** aporta `PlayerCaughtEvent`, que marca finales de captura.
- **Sistema de pistas e investigación (3.5):** contribuye indirectamente al estado narrativo acumulado.
- **Sistema de progresión por zonas (3.11):** influye en el acceso a rutas que determinan supervivencia y acompañamiento.

> Este sistema constituye el punto de convergencia del diseño: evalúa el resultado acumulado de todas las decisiones y sistemas del juego.