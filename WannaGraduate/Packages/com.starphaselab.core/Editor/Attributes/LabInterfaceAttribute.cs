using UnityEngine;

namespace StarphaseTools.Core.Editor
{
    public class LabInterfaceAttribute : PropertyAttribute
    {
        public System.Type interfaceType { get; private set; }
        
        public LabInterfaceAttribute(System.Type type)
        {
            interfaceType = type;
        }
    }
}