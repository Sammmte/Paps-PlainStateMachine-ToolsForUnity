using UnityEditor;
using UnityEditor.Callbacks;

namespace Paps.PlainStateMachine_ToolsForUnity.Editor
{
    internal class PlainStateMachineAssetDoubleClickHandler
    {
        [OnOpenAsset(1)]
        public static bool OpenEditorWindow(int instanceID, int line)
        {
            PlainStateMachineBuilder builderAsset = EditorUtility.InstanceIDToObject(instanceID) as PlainStateMachineBuilder;

            if(builderAsset != null)
            {
                PlainStateMachineBuilderEditorWindow.OpenWindow(builderAsset);

                return true;
            }

            return false;
        }
    }
}