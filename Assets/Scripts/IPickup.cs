using UnityEngine;

interface IPickup
{
    public void PickUp(Transform hand);

    public void Drop();

    public void Use();
}
