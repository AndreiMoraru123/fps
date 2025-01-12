
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable", menuName = "ScriptableObjects/ThrowableData")]
public class ThrowableData : ScriptableObject
{
    public ThrowableType throwableType;
    public float damageRadius;
    public float explosionForce;
    public float delay;
    public int damage;
}