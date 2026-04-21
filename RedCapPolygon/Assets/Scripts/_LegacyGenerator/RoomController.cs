using UnityEngine;

namespace LegacyGenerator
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] private Transform entryPoint;
        [SerializeField] private Transform[] exitPoints;
        public Transform EntryPoint { get { return entryPoint; } }
        public Transform[] ExitPoints { get { return exitPoints; } }
    }
}