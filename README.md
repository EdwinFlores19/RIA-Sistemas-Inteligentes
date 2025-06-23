RIA: Reactive Intelligent Agent 🧠🎮
Simulador lúdico que enseña los principios del aprendizaje supervisado a través de un agente reactivo programado en Unity.
📚 Descripción
RIA es un serious game educativo diseñado para introducir de manera interactiva conceptos fundamentales de la inteligencia artificial, específicamente el paradigma de aprendizaje supervisado. A través de un simpático agente reactivo, el jugador configura datasets que determinan el comportamiento del personaje frente a diversos estímulos del entorno. El objetivo es aprender mientras se juega, fomentando el pensamiento lógico, la comprensión de datos y la toma de decisiones algorítmica.
🎯 Objetivos del Proyecto
- Enseñar los fundamentos de input, output y datasets en IA.
- Promover habilidades como resolución de problemas y razonamiento lógico.
- Incentivar el interés por las tecnologías inteligentes desde edades tempranas.
- Aplicar principios de diseño centrado en el usuario (UCD) y mecánicas de juego efectivas.
- Construir un sistema fácilmente escalable a otros conceptos de IA.
🕹️ Mecánica de Juego
El jugador define reglas condicionales dentro de un dataset visual compuesto por:
- Forma del objeto
- Color del objeto
- Reacción del agente: comer, saltar, evadir, ignorar, etc.
Con base en este dataset, el agente RIA interpreta su entorno y decide cómo actuar ante cada ítem que encuentra, evaluando similitudes y aplicando lógicas condicionales.
🧩 Arquitectura del Proyecto
El sistema está organizado modularmente bajo los principios de la programación orientada a objetos. Principales componentes:
1. Capa de Presentación (UI)
- MainMenu: Navegación del juego.
- InterfaceManager: Controla botones, misiones y elementos visuales.
- DataSelector, ShapeSelector_UI, ColorSelector_UI, ReactionSelector_UI: Permiten configurar cada fila del dataset con atributos visuales.
2. Capa Lógica del Juego
- LevelManager: Orquestador principal del ciclo de vida de un nivel.
- MissionManager: Supervisa condiciones de éxito o fallo.
- RIA: Agente que explora, percibe y reacciona en el mundo.
3. Capa de Datos
- DataSet, DataRow, ItemShapeAndColor: Representación lógica del conjunto de reglas definidas por el usuario.
4. Capa de Servicios
- SoundPlayer, MonoSoundPlayer: Reproducción de efectos de sonido y música de fondo.
🧠 Lógica de Decisión del Agente
El comportamiento de RIA se basa en un sistema de priorización por similitud con los datos del dataset:
- Alta coincidencia: forma y color exactos → máxima prioridad.
- Coincidencia parcial: forma o color nulo → prioridad menor.
- Reacciones múltiples: se elige la más frecuente o se selecciona aleatoriamente si hay empate.
🛠️ Tecnologías Utilizadas
- Unity (2D): Desarrollo del simulador y control de física/animaciones.
- Visual Studio: Codificación del comportamiento en C#.
- PowerPoint: Diseño inicial de interfaces visuales y prototipos.
🧪 Pruebas y Validación
El juego ha sido probado con usuarios reales (principalmente niños) para validar:
- Usabilidad
- Curva de aprendizaje
- Claridad de la interfaz
- Efectividad educativa
Retroalimentaciones clave incluyeron sugerencias sobre la necesidad de tutoriales, indicaciones en pantalla, opciones de configuración de sonido y mejoras visuales, muchas de las cuales se integraron en una segunda versión del prototipo.
🚧 Roadmap y Próximos Pasos
- [ ] Integrar sistema de pistas y tutoriales in-game.
- [ ] Mejorar accesibilidad de íconos e instrucciones.
- [ ] Incorporar feedback auditivo adaptativo según el desempeño.
- [ ] Escalar el prototipo a otros tópicos de IA (clustering, aprendizaje por refuerzo, etc.).
🙌 Créditos
Proyecto desarrollado por estudiantes de Ingeniería de Sistemas de la Universidad San Ignacio de Loyola:
