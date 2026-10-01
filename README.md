# juego3dmandatory

# Entorno 3D Interactivo en Unity - Producto Académico 3 (PA3)
**Curso:** Desarrollo de Videojuegos  
**Docente:** Percy Maldonado Quispe (pmaldonado@continental.edu.pe)  
**Universidad:** Universidad Continental  
**Motor:** Unity 6 LTS (Versión 6000.3.24f1)  
**Render Pipeline:** Built-in Standard 3D / Universal PBR  

---

## 🚀 1. Resumen del Proyecto
El proyecto consiste en una **Estación Espacial y Laboratorio de Investigación Sci-Fi** totalmente interactivo y explorable en **Primera Persona (FPS)**. Fue diseñado y estructurado cumpliendo el 100% de la rúbrica del Producto Académico 3, aplicando técnicas de modelado con primitivas 3D, materiales PBR metálicos y emisivos (neón), iluminación atmosférica dinámica, físicas, colisionadores/triggers, efectos visuales de partículas y síntesis de audio procedimental.

---

## 🎮 2. Controles de Juego

| Acción | Tecla / Control | Descripción |
| :--- | :--- | :--- |
| **Moverse** | `W`, `A`, `S`, `D` | Movimiento en 4 direcciones relativas a la vista |
| **Correr (Sprint)** | `Shift Izquierdo` | Aumenta la velocidad de desplazamiento |
| **Saltar** | `Barra Espaciadora` | Salto físico con gravedad |
| **Mirar / Rotar** | `Movimiento del Mouse` | Control de cámara en 360° con inclinación vertical limitada |
| **Interactuar** | `E` | Abre y cierra compuertas de presión manualmente |
| **Reiniciar Nivel** | `R` | Reinicia la escena al ganar o en cualquier momento |
| **Bloquear Cursor** | `Click Izquierdo` | Bloquea y centra el cursor para inmersión |
| **Liberar Cursor** | `Esc` | Permite salir del modo de captura de cursor |

---

## 🛠️ 3. Cumplimiento de Requerimientos del PA3

| # | Requerimiento de la Rúbrica | Implementación en el Proyecto |
| :---: | :--- | :--- |
| **1** | Proyecto 3D en Unity | Creado con **Unity 6 (6000.3.24f1)** nativo, sin dependencias externas pesadas. |
| **2** | Entorno 3D utilizando primitivas | Estructura modular construida con cubos, cilindros, esferas y mallas biseladas. |
| **3** | Escenario mínimo: Piso, Paredes, Plataformas/Desniveles, 5 decorativos | • **Piso y Techo:** Sala principal con losas metálicas y terreno natural con piso de tierra y vegetación.<br>• **Paredes:** Paneles de titanio perimetrales con brecha de salida hacia el campo abierto exterior.<br>• **Desniveles y Terreno:** Rampa arquitectónica a plataforma elevada (+2.4m) y **Terreno 3D (Terrain)** con colinas y desniveles naturales tipo campo (hasta 4.5m).<br>• **Objetos Decorativos (6+):** Rack de servidores, Cajas apiladas, Barriles metálicos, Mesa de laboratorio, Rocas monolíticas, Arbustos alienígenas y Hongos. |
| **4** | Personaje 3D controlable | `Player_FPS` equipado con `CharacterController` y `PlayerController.cs` (caminar, correr, saltar). |
| **5** | Cámara configurada correctamente | `FirstPersonCamera` montada a 1.7m con mouselook suave, clamp vertical (-80° a 80°) y retícula central interactiva. |
| **6** | Materiales diferenciados | 10+ materiales PBR: Titanio (`Mat_Wall_Titanium`), Suelo (`Mat_Floor_Dark`), Plataforma (`Mat_Floor_Platform`), Cobre (`Mat_Pipes_Copper`), Oro metálico con emisión (`Mat_Coin_Gold`), Neón Cian, Naranja, Verde y Rojo (`_EMISSION`), Vidrio translúcido y Barandillas de seguridad. |
| **7** | Iluminación y atmósfera adecuada | Iluminación ambiental nocturna espacial, luz direccional estelar tenue, lámparas de neón cenitales con `PointLight` suave y luz pulsante de ambientación (`PulsingLight.cs`). |
| **8** | Objeto interactivo con Colisiones / Triggers | • **Compuerta de Presión (`SlidingDoor`):** Sensor de proximidad Trigger que abre la puerta automáticamente y botón manual `[E]`.<br>• **5 Monedas Doradas 3D Coleccionables (`EnergyCorePickup`):** Ítems dorados distribuidos en el mapa mediante Trigger `OnTriggerEnter`. Al conseguir las 5 monedas se gana el juego y se activa la pantalla de victoria. |
| **9** | Sistema de partículas o efecto visual | Efecto visual luminoso de neón, rotación continua y levitación armónica con luces dinámicas en las 5 Monedas Doradas coleccionables (`SimpleRotator.cs`). |
| **10** | Organización: GameObjects y Prefabs | Jerarquía organizada por categorías: `1. MANAGEMENT`, `2. ENVIRONMENT` (Architecture, Decorations, InteractiveObjects), `3. LIGHTING & FX`, `4. PLAYER`. Prefabs guardados en `Assets/_Project/Prefabs/`. |
| **11** | Escenario explorable y ejecutable | Escena `SciFi_Lab_PA3.unity` lista para reproducir pulsando **Play** en el editor de Unity. |

---

## 📁 4. Estructura del Proyecto

```text
Assets/_Project/
├── Editor/
│   └── SciFiSceneBuilder.cs       # Herramienta de automatización y generación de escena
├── Materials/
│   ├── Mat_Floor_Dark.mat         # Suelo metálico de la base
│   ├── Mat_Wall_Titanium.mat      # Paneles de pared
│   ├── Mat_Coin_Gold.mat          # Oro metálico brillante para las monedas
│   ├── Mat_Neon_Cyan.mat          # Neón cian para pantallas e iluminación
│   ├── Mat_Neon_Green.mat         # Neón verde de compuerta abierta
│   ├── Mat_Neon_Red.mat           # Neón rojo de compuerta cerrada
│   └── ...                        # Otros materiales PBR
├── Prefabs/
│   ├── SlidingDoor.prefab         # Compuerta automática corrediza
│   ├── LabConsole.prefab          # Terminal decorativa de control
│   └── LabWorkstation.prefab      # Estación de trabajo de laboratorio
├── Scenes/
│   └── SciFi_Lab_PA3.unity        # Escena principal interactiva
└── Scripts/
    ├── IInteractable.cs           # Interfaz común de interacción ([E])
    ├── PlayerController.cs        # Controlador FPS (WASD, salto, correr, raycast)
    ├── SlidingDoor.cs             # Lógica de apertura suave y triggers de puerta
    ├── EnergyCorePickup.cs        # Recolección de monedas con triggers y audio
    ├── SimpleRotator.cs           # Rotación continua y levitación suave
    ├── PulsingLight.cs            # Oscilación armónica de intensidad luminosa
    ├── ProceduralAudio.cs         # Generador de efectos de sonido (moneda, puerta, victoria)
    └── UIManager.cs               # HUD con retícula, contador [X/5] y pantalla de ¡VICTORIA!
```

---

## 🎬 5. Guía para el Video Demostrativo (3 a 5 min)
Al grabar tu video para la entrega, sigue este orden sugerido para cubrir todos los puntos de la rúbrica:

1. **Introducción (30 seg):**
   - Presentación personal y del curso.
   - Mostrar la versión de Unity: **Unity 6 LTS (6000.3.24f1)**.
   - Mostrar la jerarquía ordenada (`1. MANAGEMENT`, `2. ENVIRONMENT`, `3. LIGHTING & FX`, `4. PLAYER`) y la carpeta de **Prefabs** en el Project Explorer.
2. **Navegación e Iluminación (1 min):**
   - Pulsar **Play**.
   - Mostrar el movimiento fluido con WASD, el salto con la Barra Espaciadora y la aceleración con Shift.
   - Mostrar la cámara mouselook fluida y la retícula central (Crosshair).
   - Apuntar hacia los materiales diferenciados (metal, neón, titanio, monedas doradas) y la atmósfera de iluminación espacial.
3. **Interacciones con Triggers y Recolección de Monedas (2 min):**
   - Acercarse a la compuerta de presión: evidenciar cómo el **Trigger de proximidad** abre la puerta suavemente (o pulsar `[E]`) y la luz cambia de rojo a verde.
   - Recorrer el escenario recolectando las **5 Monedas Doradas**:
     1. Moneda 1: En la esclusa / habitación inicial.
     2. Moneda 2: Sobre la mesa de laboratorio.
     3. Moneda 3: En la sala central.
     4. Moneda 4: En la zona de almacén con cajas y barriles.
     5. Moneda 5: Subiendo la rampa hacia la plataforma elevada con barandillas.
   - Mostrar cómo al tocar cada moneda con el personaje (`OnTriggerEnter`), se emite el sonido de moneda y el contador se actualiza en el HUD.
   - Al recolectar la 5ta moneda, evidenciar la fanfarria de victoria y la pantalla de **¡HAS GANADO! / JUEGO COMPLETADO**, mostrando cómo reiniciar con la tecla `[R]`.
4. **Cierre (30 seg):**
   - Conclusión rápida sobre el cumplimiento de los requerimientos y despedida.
