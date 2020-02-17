using Paps.StateMachines;
using UnityEngine;

namespace Tests
{
    [CreateAssetMenu(menuName = "StateAssets/Test State Assets")]
    public class TestStateAsset : ScriptableObject, IState
    {
        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}