using UnityEngine;

public interface IPhysicsTrigger
{
    void OnTriggerEnter(Collider collider);

    void OnTriggerExit(Collider collider);

    bool enabled { get; set; }

    string Description { get; }
}
