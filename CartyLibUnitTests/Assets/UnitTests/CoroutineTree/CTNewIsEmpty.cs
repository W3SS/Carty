﻿using UnityEngine;
using CartyLib.Internals;

[IntegrationTest.DynamicTest("CartyLibTests")]
public class CTNewIsEmpty : MonoBehaviour
{
    private CoroutineTree tree = new CoroutineTree();

    void Start()
    {
        tree.Start();
        IntegrationTest.Assert(tree.Empty == true);
        IntegrationTest.Pass(gameObject);
    }
}