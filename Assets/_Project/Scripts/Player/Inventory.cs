using System.Collections.Generic;
using UnityEngine;


namespace Project.Player
{
    public class Inventory : MonoBehaviour
    {
        HashSet<string> _items = new HashSet<string>();
        public bool Has(string id) => _items.Contains(id);
        public void Add(string id) => _items.Add(id);
        public bool Consume(string id) => _items.Remove(id);
    }
}