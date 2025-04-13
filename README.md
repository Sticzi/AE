# One Room Horror – Puzzle Prototype (Unity)

A first-person horror puzzle experience focused on environmental storytelling and item-based puzzle solving. Created as a junior Unity developer recruitment task.

![Gameplay preview](https://github.com/user-attachments/assets/8f119dc5-8ecd-4283-9cae-9feb819b1c97)

## Overview
Atmospheric horror room with three core puzzle elements: swords, candles, and skulls. 
Solve multi-stage environmental puzzle by interacting with different objects.

## Controls
| Action                | Key           |
|-----------------------|---------------|
| Movement              | W/A/S/D       |
| Look                  | Mouse         |
| Interact/Toggle       | E             |
| Pick Up/Drop Item     | R             |
| Sprint                | Left Shift    |

## Key Features

### Core Systems
- **First-Person Controller**  
  `CharacterController`-based movement with Cinemachine camera management
  - Head bobbing with adjustable parameters
  - Physics-based jumping/gravity
  - Layer-based collision detection

- **Interaction Framework**
  - Modular interaction system 
  - Raycast Object detection with layer filtering
  - Dynamic object states (lit/extinguished, activated/deactivated)
  - New Input System integration

### Puzzle Mechanics
- **Object Validation**  
  Pedestal system requiring specific item types (swords/skulls)
  - Type matching verification
  - Position/rotation based animations;
  - Event-driven activation

- **Multi-Phase Puzzles**
  - Sword placement phase
  - Candle lighting puzzle
  - Finding the hidden skull

### Visual Feedback
- **UI System**  
  TextMeshPro-based hints with DoTween animations:
  - Contextual interaction prompts
  - Error messages for invalid actions
  - Fade in/out transitions

- **Environmental Effects**
  - Dynamic candle lighting (point lights + particle systems)
  - Object highlight shaders
  - Smooth door animations

- **Modular Audio Manager**
