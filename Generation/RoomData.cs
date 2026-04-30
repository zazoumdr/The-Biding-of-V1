using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheBindingOfV1.Generation
{
    public class RoomData : MonoBehaviour
    {
        [Header("Identity")]
        public string roomId;
        public RoomType roomType;

        [Header("Floors")]
        public int[] occupiedFloors;
        public int primaryFloor;

        [Header("Connections")]
        public Transform spawnPoint;
        public Transform entranceDoor;
        public List<Transform> exitDoors;

        [Header("Bounds")]
        public Vector3 dimensions;
    }
}
