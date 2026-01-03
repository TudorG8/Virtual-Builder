# Virtual Builder
This is a virtual reality game about crafting items. Stranded on a deserted island, you must gather resources from around the world and craft your own items.

What sets this title apart from other crafting simulators is how items can be assembled in any way you want. 

Whether you want a long pickaxe, an axe with 4 sides or a double sword, players have a large variety of choices.

## Getting Started (Software Based)
The project is a Unity application using the Oculus framework.

### Prerequisites
In order to open the project, you require:

1. The Unity application ([Download](https://unity3d.com/get-unity/download)).

    The latest version is suggested, as the project was developed using Unity 2018.2.18
    
    Note: opening the project with Unity 2018.1 will most likely result in the assets getting broken.

2. An Oculus Headset

    You can still open the project without a headset, but moving around and interacting with items requires the [Oculus Headset](https://www.oculus.com/)

### Code

The code is based on the two main Oculus [Framework](https://developer.oculus.com/documentation/unity/latest/concepts/unity-sample-framework/)

The main assets used are:

1. [Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022)
2. [Oculus Sample Framework](https://assetstore.unity.com/packages/templates/tutorials/oculus-sample-framework-82503)

Integration is used for the avatar and movement and the sample framework is used for distance grabbing objects and looking at UI objects.

The Code introduced so far can be split into two categories:

1. Item Assembly

    Most of the code is related to assembling items. This is done by combining them under a new object. Grabbing said object involves rotating it with an offset related to what piece was picked up.

2. VR 

    This section is under the VR folder. It includes custom changes to the VR framework that are supposed to overwrite the default behaviour. 

    They involve similar code to the Distance Grabbing Module from the sample framework, due to the fact that some methods weren't virtual and inheriting from these classes was too much effort.

### Versioning Strategy
The main strategy used was having a develop branch and feature branches split based on sprints. 

Feature branches get merged into develop at the end of a cycle and develop back into master at the end of a release.

## Authors
Tudor Gheorghe

## References