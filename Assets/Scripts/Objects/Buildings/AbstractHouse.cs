﻿using UnityEngine;

public class AbstractHouse : AbstractBuilding
{
    protected override void MakeNode()
    {
        // Move code below
        ObjectPool pool = GameObject.FindGameObjectWithTag("Manager").GetComponent<ObjectPool>();
        GameObject graph = GameObject.FindGameObjectWithTag("Graph");
        node = pool.GetPooledObject("ResidentialNode");

        node.transform.position = new Vector3(transform.position.x, Mathf.Floor(transform.position.y), transform.position.z);
        node.transform.parent = graph.transform;
        node.SetActive(true);
        graph.GetComponent<Graph>().AddNodes();
    }

    public int HousePopulation()
    {
        return node.GetComponent<ResidentialNode>().citizens.Count;
    }
}
