using System;

namespace Peg.Attributes
{
    /// <summary>
    /// Place this above a string in a MonoBehaviour to instruct the inspector
    /// to display a dropdown list with all class names derived from the named type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class InterfaceListAttribute : System.Attribute
    {
        public readonly Type InheritsFrom;
        public readonly string Label;
        public readonly string DefaultChoice;
        public readonly bool DefaultConstructorOnly;

        public InterfaceListAttribute(Type inheritsFrom, string label, string defaultLabel = "None")
        {
            InheritsFrom = inheritsFrom;
            Label = label;
            DefaultChoice = defaultLabel;
        }
    }
}