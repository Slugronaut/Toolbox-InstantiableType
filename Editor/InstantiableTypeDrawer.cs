using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Peg.Attributes;

namespace Peg.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InstantiableTypeDrawer : OdinAttributeDrawer<InterfaceListAttribute, InstantiableType>
    {
        static readonly int IndentLevel = 2;
        readonly List<string> Names = new();
        readonly Dictionary<string, int> Indexer = new();
        static Dictionary<Type, Type[]> Lookup = new Dictionary<Type, Type[]>(20);
        bool DisplayMethodParams = true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //We need to convert our string property to an index
            //that is used by the dropdown list and then back again.
            //Not the fastest, or the cleverest, but it gets the job done.
            string selected = ValueEntry.SmartValue.TypeName;
            Names.Clear();
            Indexer.Clear();
            int i = 1;
            Names.Add(Attribute.DefaultChoice);
            Indexer.Add(Attribute.DefaultChoice, 0);

            //we use a static cached lookup here to *significantly* speed up the process and imporve editor response
            if (!Lookup.TryGetValue(Attribute.InheritsFrom, out Type[] types))
            {
                if (Attribute.DefaultConstructorOnly)
                    types = TypeHelper.FindInterfaceImplementationsWithDefaultConstructors(Attribute.InheritsFrom);
                else types = TypeHelper.FindInterfaceImplementations(Attribute.InheritsFrom);

                //filter types further to only allow certain kinds of parameters in the constructors
                types = TypeConstructorHelper.GetTypesWithValidConstructors(types, InstantiableType.SupportedTypes).OrderBy(t => t.FullName).ToArray();
                Lookup[Attribute.InheritsFrom] = types;
            }

            //Lookup.Clear(); //for debugging only, remove in final version or this will be slow as shit!!!

            foreach (Type t in types)
            {
                Names.Add(t.FullName);
                Indexer.Add(t.FullName, i);
                i++;
            }
            if (string.IsNullOrEmpty(selected)) selected = Attribute.DefaultChoice;
            
            //TODO: We need a guard against removed types here!!

            int selectedIndex = EditorGUILayout.Popup(label, Indexer[selected], Names.Select(x => x.Replace('.', '/')).ToArray());
            var newSelected = Names[selectedIndex].Replace('/', '.');
            var selectedType = TypeHelper.GetType(newSelected);
            if (selectedType != null)
            {
                ValueEntry.SmartValue.TypeName = newSelected;
                EditorGUI.indentLevel += IndentLevel;
                DrawConstructorParams(ValueEntry.SmartValue);
                EditorGUI.indentLevel -= IndentLevel;
            }
            //if the type is not found, be sure to null this out or the whole inspector will break and everything will die
            else ValueEntry.SmartValue.TypeName = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        void DrawConstructorParams(InstantiableType inst)
        {
            ParameterInfo[] paramList = inst.ConstructorParamList;
            if (paramList == null || paramList.Length == 0)
                return;

            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            DisplayMethodParams = SirenixEditorGUI.Foldout(DisplayMethodParams, "Constructor Parameters");
            SirenixEditorGUI.EndBoxHeader();
            if (SirenixEditorGUI.BeginFadeGroup(this, DisplayMethodParams))
            {
                //draw editor controls for each param of the contructor
                for (int i = 0; i < paramList.Length; i++)
                    inst.ParameterValues[i] = EditorGUIExtensions.DoFieldLayout($"{paramList[i].Name} <{paramList[i].ParameterType.Name}>", paramList[i].ParameterType, inst.ParameterValues[i]);

            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();

        }
    }
}