# Toolbox-InstantiableType
A datatype that itself represents a datatype. It can be used in Unity MonoBehaviours to allow the inspector to provide a dropdown menu for all datatypes in the project.

Further, it provides controls for supplying parameter values needed to construct and instance of this datatype. All of this information is serialized and can be used at runtime to easily instantiate the selected class.

Currently, the only construct supported is the one with the most number of parameters. In the event multiple constructors have the same number of parameters, it's up to chance which one is chosen.

Dependencies:  
[com.postegames.typehelper](https://github.com/Slugronaut/Toolbox-TypeHelper)  
[com.postegames.editorguiextensions](https://github.com/Slugronaut/Toolbox-EditorGUIExtensions)  
