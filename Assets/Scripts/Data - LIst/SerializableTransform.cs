using UnityEngine;


[System.Serializable]
public class SerializableTransform
{
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ;
    public float scaleX, scaleY, scaleZ;

    public SerializableTransform(Transform t)
    {
        posX = t.position.x;
        posY = t.position.y;
        posZ = t.position.z;

        rotX = t.eulerAngles.x;
        rotY = t.eulerAngles.y;
        rotZ = t.eulerAngles.z;

        scaleX = t.localScale.x;
        scaleY = t.localScale.y;
        scaleZ = t.localScale.z;
    }

    public void ApplyTo(Transform t)
    {
        t.position = new Vector3(posX, posY, posZ);
        t.eulerAngles = new Vector3(rotX, rotY, rotZ);
        t.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}