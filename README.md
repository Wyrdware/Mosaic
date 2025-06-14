# Mosaic

Mosaic facilitates the iteration, expansion, and re-use of gameplay mechanics allowing designers and developers to bring their vision to life. Use Mosaic to create gameplay actors, such as player characters, NPCs, Interactables, and other stateful entities out of fully modular components. 

So much of what game developers create gets thrown away due to the iterative nature of development. Mosaic’s modular approach allows designers to seamlessly remove, and add new gameplay elements to their actors without requiring unique code to tie the features together.

Mosaic shares many similarities in terms of structure with the component game object model. It diverges slightly in establishing a set of base components that are fully modular that can be extended to implement any sort of functionality, from movement, to actions, to entire progression systems. These components include Behaviors, Modifiers, and Modifier Decorators. These are fully decoupled from each other and only interact with DataTags and a target monobehaviour. DataTags are a blackboard-like system that is type safe and auto initializes all incoming data if it hasn’t been already. This allows for an ECS style structure when developing characters, and ensures there are no dependencies between modules.

## Features

- Seamlessly add and remove Mosaic gameplay elements from an existing actor.

- All Mosaic gameplay elements are cross-compatible, quickly test out gameplay elements in brand-new contexts, or create entirely new actors out of the elements from various actors you have already created!

- Utilize Mosaic to craft rewards for your players that matter. By utilizing Mosaic elements as rewards, developers can introduce fundamental shifts to the gameplay experience without the risk of complexity creep. 

- Each Mosaic element is fully decoupled from each other. Scale up your development by enabling a multitude of developers to work on various Mosaic elements at the same time.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing.
### Prerequisites

You will need a copy of Unity 2022.3 LTS or later downloaded. Very few version-specific features are used so it should be fine across a variety of versions.

### Installing

https://docs.unity3d.com/6000.1/Documentation/Manual/upm-ui-giturl.html

## Authors

Jared Goronkin

## License

No License
