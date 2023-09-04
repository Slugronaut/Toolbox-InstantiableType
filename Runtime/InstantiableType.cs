using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

namespace Peg
{
    /// <summary>
    /// A datatype that can be instantiated upon creation of it's container behaviour. All parameter
    /// info for the datatype is viewable in the inspector and serialized with the parent behaviour.
    /// </summary>
    [Serializable]
    public class InstantiableType : ISerializationCallbackReceiver
    {
        public readonly static Type[] SupportedTypes =
        {
            typeof(bool),
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(Color),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Rect),
            typeof(Bounds),
            typeof(Vector2Int),
            typeof(Vector3Int),
            typeof(RectInt),
            typeof(BoundsInt),
            typeof(Enum),
            typeof(Quaternion),
            typeof(UnityEngine.Object),
        };

        [SerializeField, HideInInspector]
        string _TypeName;
        [NonSerialized, HideInInspector]
        public object[] ParameterValues;
        [SerializeField, HideInInspector]
        List<UnityEngine.Object> UnityRefs;
        [SerializeField, HideInInspector]
        byte[] SerializedBytes;


        public ParameterInfo[] ConstructorParamList
        {
            get
            {
                Type type = TypeHelper.GetType(_TypeName);
                if (type == null)
                    return null;
                var ctor = TypeConstructorHelper.FindBestConstructor(type, SupportedTypes);
                if (ctor == null)
                    return null;

                return ctor.GetParameters();
            }
        }

        public string TypeName
        {
            get { return _TypeName; }
            set
            {
                //do this or resetting to same value will reset all parameters too
                if (value == _TypeName)
                    return;

                _TypeName = value;
                ParameterValues = null;
                if (string.IsNullOrEmpty(_TypeName))
                    return;

                Type type = TypeHelper.GetType(_TypeName);
                if (type == null)
                {
                    _TypeName = "None";
                    return;
                }

                var paramList = ConstructorParamList;
                ParameterValues = new object[paramList.Length];
                for (int i = 0; i < paramList.Length; i++)
                    ParameterValues[i] = TypeHelper.GetDefault(paramList[i].ParameterType);
            }
        }

        public Type Type => TypeHelper.GetType(_TypeName);


        public void OnAfterDeserialize()
        {
            ParameterValues = Sirenix.Serialization.SerializationUtility.DeserializeValue<object[]>(SerializedBytes, Sirenix.Serialization.DataFormat.Binary, UnityRefs);
        }

        public void OnBeforeSerialize()
        {
            SerializedBytes = Sirenix.Serialization.SerializationUtility.SerializeValue(ParameterValues, Sirenix.Serialization.DataFormat.Binary, out UnityRefs);
        }

        /// <summary>
        /// Creates an instance of the type and pushes all associated parameters to its constructor.
        /// </summary>
        /// <returns></returns>
        public object Instantiate()
        {
            Type t = TypeHelper.GetType(_TypeName);
            if (t == null) return null;
            return Activator.CreateInstance(t, ParameterValues);
        }
    }


}