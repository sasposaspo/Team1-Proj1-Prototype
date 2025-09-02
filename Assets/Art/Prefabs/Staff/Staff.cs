using UnityEngine;

public class Staff : PickupBase
{
    // Fields

    private int currentElement = 0; // 1 is Ice, 1 is Fire, 2 is Electric

    // Methods

    public override void Use()
    {
        switch (currentElement)
        {
            case 0:
                UseIce();
                break;
            case 1:
                UseFire();
                break;
            case 2:
                UseElectric();
                break;
            default:
                break;
        }
    }

    public void SwitchElement()
    {
        currentElement++;
        currentElement = currentElement % 3;
    }

    public void UseIce()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Transform other = hitInfo.transform;

            if (other.TryGetComponent(out IFreezable otherFreeze))
            {
                otherFreeze.Freeze();
                return;
            }

            if (other.TryGetComponent(out Freezable otherFreezable))
            {
                otherFreezable.Freeze();
                return;
            }
        }
    }

    public void UseFire()
    {
        Debug.Log("Used Fire");
    }

    public void UseElectric()
    {
        Debug.Log("Used Electric");
    }
}
