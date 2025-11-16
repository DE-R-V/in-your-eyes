# InYourEyes

In Your Eyes is a prototype and proof of concept application created to simulate various vision impairments using Extended Reality technologies. The project demonstrates how XR can be used as an educational and accessibility oriented tool to help users understand how different visual conditions affect perception in real environments.

This application is not intended to serve as a medical diagnostic tool. It is built strictly for education, demonstration and awareness purposes.

## Technical Overview

The prototype is built in Unity 6 with the Universal Render Pipeline (URP) and uses OpenXR as the primary XR backend. The project targets the Meta Quest platform and leverages passthrough features to blend virtual overlays with the real world.

### Key Components

### XR Framework
- **OpenXR** is used as the runtime for device compatibility and standardized input and rendering paths.
- **Meta Quest OpenXR features** were enabled to support passthrough, input profiles and device specific functionality.

### Rendering and Graphics
- **URP (Universal Render Pipeline)** provides the rendering foundation and allows the use of custom renderer features.
- **Custom shaders** are used to simulate various visual conditions such as blur, distortion, contrast changes and visual artifacts.
- **Passthrough layered rendering** is achieved by placing camera facing geometry (quads) in the scene and applying material effects to simulate vision filters.
- **Lens masking** is used to control how filters appear in stereoscopic rendering when needed.

### Architecture
- The application follows a modular structure where each vision condition is encapsulated as a separate visual effect module.
- Vision states can be modified dynamically to present different experiences.
- UI and experience flow are presented using world space interfaces optimized for XR readability, following Meta's and other more generic guidelines for these kind of experiences

### Data and Research Basis
- Vision conditions and their visual characteristics are informed by publicly available medical descriptions and credible educational resources.
- The goal is to present approximate visual experiences rather than medically precise simulations.

## Current Features

- Simulation of selected vision conditions through shader based image effects.
- Integration with Meta Quest passthrough to combine real world imagery with XR overlays.
- Basic experience flow that allows users to view and transition between different conditions.

## Limitations

This prototype is an early stage proof of concept and includes several limitations:

- Visual simulations are not medically validated.
- The system currently supports only a limited set of vision conditions.
- The passthrough based rendering pipeline uses workaround techniques due to API limitations.

## Possible Improvements

Improvements and exploration areas could include:

- Expansion of the vision condition library.
- Refined shader based models for more accurate perceptual effects.
- Improved UI and interaction design tailored for accessibility.
- Integration and use of PCA to access the camera feed for certain conditions.

## Disclaimer

This software is strictly for educational demonstration and awareness. It must not be used for medical analysis, diagnosis or treatment. All simulated effects are approximations.