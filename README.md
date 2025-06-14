# Mosaic

Mosaic facilitates the iteration, expansion, and re-use of gameplay mechanics allowing designers and developers to bring their vision to life. Use Mosaic to create gameplay actors, such as player characters, NPCs, Interactables, and other stateful entities out of fully modular components. 

So much of what game developers create gets thrown away due to the iterative nature of development. Mosaic’s modular approach allows designers to seamlessly remove, and add new gameplay elements to their actors without requiring unique code to tie the features together.

Mosaic is a framework that sits just above the game object model, and just below the character controller. By extending Mosaic, developers can create fully modular runtime features that are cross compatible across completely different actors and even projects. These features can include anything from movement, to attacks, to skill trees, and scripted sequences. 

## Features
Notice: Mosaic is undergoing significant structural revisions. Documentation may be out of date and will likely not reflect the final direction of the project. 

Systems that work with Mosaic can be broken down into two categories. External systems reference and interact with Mosaic through a single unified interface, allowing them to interact with actors built with Mosaic in an abstracted manner. Internal systems extend Mosaics Modifiers, Modifier Decorators, and DataTags, affording them all of the benefits of Mosaic. 

Mosaic shares many similarities with the component architecture and builds on the concepts to guarantee interoperability across actors. The Core can be thought of as the container object, DataTags can be thought of the container objects state, and Behaviors, Modifiers, and Modifier Decorators, can all be thought of as the components. In Game Programming Patterns, Nystrom outlines some of the major challenges with the component architecture, such as an inherent lack of encapsulation of data which can cause issues with code clarity and unnecessary memory usage. The benefit of Mosaic over the standard component architecture is that Mosaic solves all of these issues.

### DataTags
DataTags are essentially a dynamic type safe blackboard. This allows components to share data, without the risk of introducing bugs due to spelling errors, while still being able to add new data types as needed.

### Behaviors
One of the fundamental components of Mosaic are the behaviors. Each behavior must be fully modular, which requires a modular behavior selection algorithm. A utility system was chosen due to both its simplicity, as well as its ability to simulate any other behavior selection algorithm with relative ease.

### Modifiers
Not all actor behaviors are stateful, some are instantaneous, and some can persist for undetermined amounts of time while overlapping other stateful and non-stateful behaviors. Mosaic's solution to supporting this type of behavior was heavily inspired by For Honors modifiers. In For Honor Modifiers are used for everything from adding visual flair to a character, to applying status effects over time. Mosaic simplifies this solution down to its fundamentals, improving flexibility. Mosaic also includes a structure that allows for the dynamic decoration of any modifier. This enables a modular approach to reacting to and extending the modifiers functionality, and is an essential part of achieving full modularity and cross-compatibility.

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
