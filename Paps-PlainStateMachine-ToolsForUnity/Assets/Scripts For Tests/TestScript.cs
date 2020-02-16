using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SomeNamespace
{
    public class TestScript : MonoBehaviour
    {
        public enum MyEnum
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(typeof(MyEnum).FullName);
        }
    }

}

