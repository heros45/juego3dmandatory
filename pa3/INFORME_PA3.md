# PRODUCTO ACADÉMICO N° 3: DESARROLLO DE UN ENTORNO 3D INTERACTIVO EN UNITY

**Estudiante:** [Tu Nombre y Apellidos]  
**Docente:** Percy Maldonado Quispe – pmaldonado@continental.edu.pe  
**Asignatura:** Desarrollo de Videojuegos  
**Fecha:** [Fecha de Entrega]  
**Universidad Continental**  

---

## 1. INTRODUCCIÓN Y CONCEPTO DEL ESCENARIO
Para el presente producto académico se diseñó e implementó un **Puesto Avanzado de Investigación Científica Abandonado en un Planeta Alienígena (Abandoned Planetary Outpost)**. El entorno fue construido íntegramente en **Unity 6 LTS (6000.3.24f1)** aplicando los conceptos revisados en la Unidad 3:
* Construcción y ensamblaje de escenarios 3D mediante primitivas geométricas y estructuras modulares.
* Creación y configuración de un **Terreno 3D (Terrain)** con desniveles topográficos tipo campo (colinas y lomas de hasta 4.5m de elevación).
* Texturizado multicapa de terreno mediante **TerrainLayers** (tierra planetaria y vegetación colonizadora) con shader PBR optimizado para URP (`TerrainLit`).
* Integración de elementos de vegetación y formaciones rocosas (rocas, arbustos, hongos extraterrestres y árboles).
* Configuración de un sistema de iluminación atmosférica (luz direccional con sombras suaves, luces puntuales de emergencia y emisiones neón).
* Control de cámara en primera persona (FPS) y físicas fluidas con `CharacterController` para transitar entre interiores y el terreno exterior.
* Programación de interacciones basadas en colisiones (`Trigger`) y raycasting con teclado (`[E]`).
* Sistema interactivo de recolección de 5 monedas doradas coleccionables con condición de victoria.

---

## 2. ARQUITECTURA Y DISTRIBUCIÓN DEL ENTORNO 3D
El escenario combina arquitectura de ciencia ficción con un entorno montañoso agreste donde la vegetación y la tierra han comenzado a invadir la estación:
1. **Esclusa de Entrada (Airlock):** Zona interior sellada donde el jugador inicia la partida, con paredes de titanio, techo con luminaria cenital y panel de acceso.
2. **Piso Interior Sólido y Plano:** Toda la estación científica cuenta con pisos modulares de losas metálicas oscuras (`Mat_Floor_Dark`), 100% planos y con colisionadores rígidos (`BoxCollider`), garantizando estabilidad y una base arquitectónica pulcra.
3. **Compuerta Corrediza Automática:** Compuerta de seguridad que separa el airlock de las instalaciones, accionada por sensor de proximidad (`Trigger`) o manualmente con la tecla `[E]`.
4. **Estación Central de Laboratorio:** Espacio interior con mesas de investigación, servidores informáticos con luces parpadeantes y contenedores de suministros.
5. **Desnivel Arquitectónico (Plataforma y Rampa):** Rampa inclinada de 24° con barandillas amarillas de seguridad que conecta el suelo con un mirador elevado a +2.4 metros.
6. **Brecha Exterior hacia el Terreno Planetario:** Una apertura en la pared norte permite al jugador salir al exterior de la superficie planetaria.
7. **Terreno Montañoso con Desniveles Pronunciados:** Extensión de terreno (180x180 metros) con una imponente cordillera perimetral de montañas de hasta 22 metros y lomas onduladas de entre 5 y 12 metros de altura, con senderos naturales transitables.
8. **Vegetación y Rocas:** Rocas monolíticas, arbustos extraterrestres, hongos fosforescentes y árboles de coníferas distribuidos alrededor del puesto abandonado y en las laderas de las montañas.

---

## 3. SISTEMA DE MATERIALES E ILUMINACIÓN
Se crearon y aplicaron materiales PBR diferenciados según la función de cada elemento:
* `Mat_Floor_Dark` y `Mat_Floor_Platform`: Superficies transitables metálicas interiores con acabado semibrillante y piso plano.
* `Mat_Wall_Titanium`: Paneles de pared de alta resistencia con reflectividad controlada.
* `Mat_Railing_Yellow`: Barandillas de seguridad industrial para delimitar bordes y rampas.
* `Mat_Neon_Cyan` / `Mat_Neon_Orange` / `Mat_Neon_Green`: Materiales con emisión activa (`_EMISSION`) para señalización de pantallas e indicadores.
* `Mat_Coin_Gold`: Oro metálico reflectivo con brillo y halo de emisión para las monedas coleccionables.
* `TerrainLayers`: Capas PBR de tierra rocosa (`TerrainLayer_AlienDirt`) y vegetación verde (`TerrainLayer_AlienGrass`) aplicadas al terreno mediante el shader `TerrainLit` de URP.

**Iluminación y Atmósfera:**
* **Cielo Panorámico Espacial (`Skybox - AllSky Free`):** Se integró el material `AllSky_Space_AnotherPlanet` y se configuró la cámara principal en modo `Skybox`, mostrando una bóveda celeste con estrellas, planetas distantes y atmósfera cósmica que sustituye el fondo negro por una ambientación de ciencia ficción.
* **Luz Direccional (El Sol Planetario - `Directional Light`):** Fuente de luz primaria que actúa como el Sol natural del planeta. Orientada con inclinación de 50° en X y -35° en Y, emite luz cálida (intensidad 1.25) y proyecta sombras suaves dinámicas (`Soft Shadows`) en las colinas, rocas y árboles exteriores.
* **Luces Puntuales (Point Lights):** Lámparas artificiales cenitales ubicadas en el interior de la esclusa y pasillos, garantizando iluminación focalizada en recintos cerrados donde no penetra la luz solar.
* **Separación de Niveles de Suelo:** El terreno debajo del laboratorio se posicionó por debajo de las losas metálicas, garantizando un piso interior 100% plano, limpio y sin cruce de texturas (*Z-fighting*), con una suave rampa de transición en la salida norte.

---

## 4. SISTEMA DE CONTROL DE PERSONAJE Y CÁMARA (FPS)
* **Componente Base:** `CharacterController` para garantizar un desplazamiento suave tanto sobre el piso plano interior como sobre las laderas y desniveles del terreno montañoso sin atravesar la geometría.
* **Cámara en Primera Persona:** Ubicada a 1.7 metros de altura respecto al suelo, con limitación angular vertical de -80° a +80° para evitar inversiones indeseadas de perspectiva.
* **Interacción mediante Raycast:** Desde el centro de la cámara (`Physics.Raycast`) se evalúa en tiempo real si el jugador está mirando un objeto interactivo a menos de 3.5 metros, desplegando un aviso intuitivo en el HUD (`[E] Interactuar`).

---

## 5. MECÁNICAS INTERACTIVAS Y FÍSICAS (TRIGGERS / COLLIDERS)
1. **Compuerta de Seguridad (`SlidingDoor.cs`):** 
   * Detecta la llegada del jugador mediante un `BoxCollider` configurado como `IsTrigger` para apertura automática, o manualmente mediante raycast con la tecla `[E]`.
   * Al accionarse, desplaza el panel hacia arriba suavemente mediante interpolación matemática y cambia el indicador luminoso de rojo a verde con sonido de mecanismo.
2. **5 Monedas Doradas Coleccionables y Condición de Victoria (`EnergyCorePickup.cs`):**
   * Moneda 1: En la esclusa de entrada inicial.
   * Moneda 2: En la estación de investigación del laboratorio.
   * Moneda 3: En la sala central con servidores informáticos.
   * Moneda 4: En la plataforma elevada de observación (al final de la rampa interior).
   * Moneda 5: **En lo alto de la colina montañosa exterior**, obligando al jugador a navegar por los desniveles del terreno para alcanzarla.
   * Al recolectar las 5 monedas se dispara la fanfarria de victoria y la pantalla de **¡JUEGO COMPLETADO / HAS GANADO!**, permitiendo reiniciar con la tecla `[R]`.

---

## 6. SISTEMAS DE EFECTOS VISUALES Y AMBIENTACIÓN
* **Efectos Visuales en Ítems Coleccionables:** Rotación continua y levitación armónica procedural con el script `SimpleRotator.cs`.
* **Iluminación Dinámica y Emisión:** Cada moneda emite luz puntual dorada (`PointLight`) y materiales con canal de emisión activo (`_EMISSION`).
* **Luz Pulsante Ambiental:** Oscilación armónica de intensidad luminosa controlada por `PulsingLight.cs`.

---

## 7. ORGANIZACIÓN DEL PROYECTO
El proyecto cumple una jerarquía estrictamente modular:
* `Assets/_Project/Scenes/`: Escena principal `SciFi_Lab_PA3.unity`.
* `Assets/_Project/Prefabs/`: Prefabs reutilizables (`SlidingDoor`, `LabConsole`, `LabWorkstation`).
* `Assets/_Project/Scripts/`: 8 scripts en C# con arquitectura limpia e interfaces (`IInteractable`).
* `Assets/_Project/Materials/`: Materiales PBR configurados (metálicos, neones, oro, titanio).

---

## 8. CUADRO COMPARATIVO: CUMPLIMIENTO DE LOS 11 REQUERIMIENTOS DEL PDF

A continuación se detalla la matriz de correspondencia entre los requerimientos solicitados en la rúbrica y las soluciones implementadas en el proyecto:

| N° | Requerimiento del PDF (PA3) | Implementación en el Proyecto | Estado |
|:---:|:---|:---|:---:|
| **1** | **Entorno 3D con Terreno y Desniveles** | `Terrain` de 180x180m con cordillera montañosa, desniveles ondulados naturales de 5 a 12m y rampa arquitectónica interior de 24°. | **CUMPLE (100%)** |
| **2** | **Estructuras y Primitivas Modulares** | Laboratorio de ciencia ficción con esclusa, sala principal, plataforma elevada a +2.4m, consolas de comando, servidores y contenedores. | **CUMPLE (100%)** |
| **3** | **Texturas y Materiales PBR** | Materiales PBR con metálico, suavidad y emisión (`Mat_Floor_Dark`, `Mat_Wall_Titanium`, `Mat_Coin_Gold`, `Mat_Neon_Cyan`) y capas `TerrainLayers` con shader URP `TerrainLit`. | **CUMPLE (100%)** |
| **4** | **Sistema de Iluminación Completo** | Sol Direccional (`Directional Light`) a 50° con sombras suaves (`Soft Shadows`), múltiples luces puntuales (`Point Light`) interiores y cielo espacial `AllSky Free`. | **CUMPLE (100%)** |
| **5** | **Cámara en Primera Persona (FPS)** | Vista FPS ubicada a 1.7m de altura con rotación vertical controlada por mouse (-80° a +80°) y retícula de apuntado central en pantalla. | **CUMPLE (100%)** |
| **6** | **Controlador de Personaje Fluido** | `CharacterController` con movimiento WASD (velocidad 5 m/s), salto con barra espaciadora (altura 1.3m), gravedad y física para subir rampas y lomas. | **CUMPLE (100%)** |
| **7** | **Interacciones con Físicas y Triggers** | Compuerta corrediza `PuertaCorrediza.cs` con `BoxCollider` (`IsTrigger`) para detección por proximidad y raycasting con teclado `[E]`. | **CUMPLE (100%)** |
| **8** | **Objetos Coleccionables con Lógica** | 5 monedas doradas distribuidas estratégicamente (4 en interior y 1 en la cima de la colina exterior) que se recolectan al contacto con `MonedaColeccionable.cs`. | **CUMPLE (100%)** |
| **9** | **Interfaz de Usuario (HUD)** | HUD dinámico en Canvas (`ControladorUI.cs`) con contador de monedas en tiempo real (0/5 a 5/5), indicador de interacción `[E]` y panel de victoria con reinicio (`[R]`). | **CUMPLE (100%)** |
| **10** | **Efectos Visuales y Sonoros** | Giro continuo y levitación de monedas (`RotadorSimple.cs`), luces pulsantes (`LuzPulsante.cs`), sonido de monedas, compuerta y fanfarria de victoria (`AudioJuego.cs`). | **CUMPLE (100%)** |
| **11** | **Estructura y Buenas Prácticas** | Arquitectura modular bajo carpeta `Assets/_Project/`, scripts y variables en español con nomenclatura estándar, código limpio y documentado. | **CUMPLE (100%)** |

---

## 9. ESTRUCTURA DEL PROYECTO EN UNITY
El proyecto sigue el estándar de la industria organizando todos los activos personalizados bajo la raíz `_Project`, con scripts y variables en español:
```text
Assets/
├── _Project/
│   ├── Scenes/
│   │   └── SciFi_Lab_PA3.unity        <-- Escena principal con todo el entorno
│   ├── Scripts/
│   │   ├── ControladorJugador.cs      <-- Movimiento WASD, salto, cámara y raycast
│   │   ├── IInteractuable.cs          <-- Interfaz para objetos que se accionan
│   │   ├── PuertaCorrediza.cs         <-- Compuerta corrediza automática/manual
│   │   ├── MonedaColeccionable.cs     <-- Lógica de recolección de las 5 monedas
│   │   ├── ControladorUI.cs           <-- Contador en pantalla y panel de victoria
│   │   ├── AudioJuego.cs              <-- Efectos de sonido generados por código
│   │   ├── RotadorSimple.cs           <-- Giro continuo y flotación de monedas
│   │   └── LuzPulsante.cs             <-- Luz pulsante ambiental
│   ├── Materials/
│   │   ├── Mat_Floor_Dark.mat         <-- Piso metálico plano del interior
│   │   ├── Mat_Wall_Titanium.mat      <-- Paredes de titanio
│   │   ├── Mat_Railing_Yellow.mat     <-- Barandillas de advertencia
│   │   ├── Mat_Coin_Gold.mat          <-- Oro reflectivo con emisión
│   │   └── Mat_Neon_Cyan.mat          <-- Emisión neón para terminales
│   └── Prefabs/
│       ├── SlidingDoor.prefab         <-- Puerta con colisionador y luz
│       ├── LabConsole.prefab          <-- Consola informática
│       └── LabWorkstation.prefab      <-- Estación de trabajo
└── AllSkyFree/                        <-- Paquete de cielo espacial AllSky
```

---

## 10. EXPLICACIÓN SENCILLA DE CADA SCRIPT EN C# (NIVEL ESTUDIANTE)

Todos los scripts cuentan con variables y métodos en español (`velocidadCaminar`, `monedasRecolectadas`, `estaAbierta`, `alturaSalto`, etc.) manteniendo las funciones nativas del motor en inglés (`Start`, `Update`, `OnTriggerEnter`, etc.) tal como se aprende en los tutoriales:

1. **`ControladorJugador.cs` (Control del Jugador y Cámara):**
   * *¿Qué hace?* Permite al jugador caminar con **W, A, S, D**, mirar a todos lados con el ratón y saltar con la **Barra Espaciadora**.
   * *Variables clave:* `velocidadCaminar`, `velocidadCorrer`, `alturaSalto`, `gravedad`, `sensibilidadMouse`, `distanciaInteraccion`.
   * *Detalle clave:* Utiliza el componente `CharacterController` para no atravesar paredes y subir pendientes con suavidad. También lanza un rayo invisible (`Raycast`) desde el centro de la pantalla para saber si estamos mirando la compuerta para interactuar con la tecla `[E]`.

2. **`IInteractuable.cs` (Contrato de Interacción):**
   * *¿Qué hace?* Es una **interfaz** (un molde de programación). Dice que todo objeto interactivo del juego debe tener una acción llamada `Interactuar()` y un texto `ObtenerTextoInteraccion()`. Gracias a esto, el jugador puede activar cualquier mecanismo sin importar el tipo de objeto.

3. **`PuertaCorrediza.cs` (Compuerta Corrediza):**
   * *¿Qué hace?* Abre y cierra la gran compuerta de metal suavemente.
   * *Variables clave:* `panelPuerta`, `velocidadDeslizamiento`, `desplazamientoApertura`, `estaAbierta`.
   * *Detalle clave:* Tiene dos modos: **automático** (cuando el jugador entra en su zona `Trigger`, la compuerta sube sola) y **manual** (presionando `[E]`). Además, cambia el color de su foco de luz de **Rojo (bloqueado)** a **Verde (abierto)** y reproduce un sonido mecánico.

4. **`MonedaColeccionable.cs` (Moneda Coleccionable):**
   * *¿Qué hace?* Detecta cuando el jugador toca una moneda mediante `OnTriggerEnter`.
   * *Variables clave:* `yaFueRecolectada`, `prefabEfectoAlRecoger`.
   * *Detalle clave:* Cuando entras en contacto con ella, avisa a `ControladorUI` para sumar +1 al contador, activa el sonido de recolección y hace desaparecer la moneda de la escena.

5. **`ControladorUI.cs` (Interfaz en Pantalla y Condición de Victoria):**
   * *¿Qué hace?* Dibuja los textos en pantalla: el contador `0/5`, la mirilla central y el mensaje de ayuda `[E]`.
   * *Variables clave:* `monedasRecolectadas`, `totalMonedas`, `juegoGanado`, `mensajeObjetivo`.
   * *Detalle clave:* Cuando el contador llega a `5/5`, muestra el gran letrero de victoria **¡JUEGO COMPLETADO!** y permite presionar la tecla `[R]` para reiniciar la partida.

6. **`AudioJuego.cs` (Efectos de Sonido):**
   * *¿Qué hace?* Genera sonidos agradables directamente desde el código matemático (ondas senoidales) para que el juego tenga audio sin necesidad de descargar archivos `.mp3` o `.wav` externos.
   * *Métodos clave:* `ReproducirSonidoMoneda()`, `ReproducirFanfarriaVictoria()`, `ReproducirSonidoPuerta()`.

7. **`RotadorSimple.cs` (Giro y Flotación):**
   * *¿Qué hace?* Hace que las monedas giren sobre su propio eje en 360 grados y se balanceen suavemente hacia arriba y hacia abajo, haciéndolas muy llamativas visualmente.
   * *Variables clave:* `velocidadRotacion`, `efectoFlotacion`, `velocidadFlotacion`, `alturaFlotacion`.

8. **`LuzPulsante.cs` (Luz Pulsante):**
   * *¿Qué hace?* Hace oscilar la intensidad de la luz con una función matemática armónica (`Mathf.Sin`), simulando lámparas de laboratorio en funcionamiento o alertas.
   * *Variables clave:* `intensidadMinima`, `intensidadMaxima`, `velocidadPulso`.
---

## 11. GUION MINUTO A MINUTO PARA EL VIDEO DEMOSTRATIVO (DURACIÓN: 3 A 5 MINUTOS)

### **Minuto 0:00 – 0:45 | Presentación y Organización del Proyecto**
* **Qué mostrar en pantalla:**
  * Ventana del Unity Editor mostrando la jerarquía y el explorador de archivos con la carpeta `Assets/_Project/`.
* **Qué decir (voz en off):**
  > *"Buenas tardes profesor y compañeros. Mi nombre es [Tu Nombre] y este es el Producto Académico N° 3 de Desarrollo de Videojuegos. Para este proyecto diseñé un Puesto Avanzado de Investigación en un Planeta Alienígena. Como se puede observar en la carpeta `_Project`, el proyecto está organizado de manera modular: tenemos carpetas separadas para Escenas, Prefabs, Materiales y Scripts de programación."*

### **Minuto 0:45 – 1:30 | Inicio del Juego, Controles y Primera Moneda**
* **Qué mostrar en pantalla:**
  * Presionar el botón **Play**. El jugador aparece dentro de la esclusa cerrada (`Airlock`). Mover el mouse a los lados, arriba y abajo, y caminar con WASD.
* **Qué decir (voz en off):**
  > *"Al pulsar Play, el jugador inicia en la esclusa de la estación. Tenemos un sistema de cámara en primera persona con control del mouse y limitación angular para una vista cómoda. El movimiento utiliza `CharacterController` con teclas W, A, S, D y salto con la barra espaciadora. Frente a nosotros vemos la primera moneda dorada, que cuenta con rotación continua, levitación y luz propia. Al tocarla, el `Trigger` detecta la colisión, emite un sonido y el HUD actualiza el marcador a 1 de 5."*

### **Minuto 1:30 – 2:30 | Compuerta Interactiva, Laboratorio Interior y Materiales PBR**
* **Qué mostrar en pantalla:**
  * Acercarse a la compuerta metálica. Mostrar cómo se enciende el aviso `[E] Interactuar` o cómo al acercarse el Trigger abre la compuerta hacia arriba y la luz cambia de rojo a verde.
  * Caminar por el interior: mostrar el piso metálico plano, las consolas de investigación, la rampa inclinada y recoger las monedas 2, 3 y 4.
* **Qué decir (voz en off):**
  > *"Nos acercamos a la compuerta de seguridad. Esta puerta implementa la interfaz `IInteractable` y colisionadores de tipo Trigger: al aproximarnos o presionar la tecla E, la puerta se desliza hacia arriba, la luz cambia de rojo a verde y suena el mecanismo de apertura. Al ingresar, observamos el laboratorio interior: los pisos son completamente planos con materiales metálicos reflectivos, iluminación artificial puntual en el techo y terminales neón. Subimos por la rampa inclinada de seguridad para recolectar las monedas del interior, alcanzando 4 de 5."*

### **Minuto 2:30 – 3:45 | Salida al Terreno Exterior, Sol Planetario y Vegetación**
* **Qué mostrar en pantalla:**
  * Salir por la abertura norte hacia el exterior. Mostrar el contraste entre el piso plano interior y el terreno exterior con desniveles, montañas, árboles, rocas firmes en el suelo y el cielo estrellado `AllSky`.
  * Subir caminando por la loma natural hacia la última moneda.
* **Qué decir (voz en off):**
  > *"Ahora salimos al exterior a través de la brecha del laboratorio. Aquí apreciamos el sistema de Terreno 3D (Terrain) con desniveles topográficos y montañas en el horizonte. En el cielo tenemos configurado el Skybox espacial de AllSky con estrellas y planetas distantes, iluminado por una luz direccional que representa el Sol del planeta con sombras suaves. Toda la vegetación, como árboles y rocas, se asienta de forma natural sobre el terreno. El jugador sube con facilidad por las laderas gracias a la física de colisión."*

### **Minuto 3:45 – 4:30 | Condición de Victoria y Conclusión**
* **Qué mostrar en pantalla:**
  * Llegar a la cima de la colina exterior y recolectar la 5ta y última moneda. Se activa la pantalla verde de victoria: **¡JUEGO COMPLETADO / HAS GANADO!**. Mostrar la tecla `[R]` para reiniciar.
* **Qué decir (voz en off):**
  > *"En la cima de esta colina encontramos la quinta y última moneda. Al recolectarla, se activa la condición de victoria: suena la fanfarria de triunfo y la interfaz despliega el mensaje de juego completado con éxito, dándonos la opción de reiniciar con la tecla R. Con esto se cumplen satisfactoriamente todos los requerimientos de la rúbrica: terreno con desniveles, shaders y materiales PBR, iluminación dual interior-exterior, controlador FPS, colisionadores Trigger, recolección interactiva e interfaz de usuario. Muchas gracias por su atención."*

---

## 12. ENLACES FINALES
* **Repositorio de GitHub:** [Pegar enlace de tu repositorio de GitHub]
* **Video Demostrativo:** [Pegar enlace de YouTube o Google Drive]

