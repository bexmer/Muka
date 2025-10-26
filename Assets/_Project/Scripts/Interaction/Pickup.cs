using UnityEngine;
using Project.Player;


namespace Project.Interaction
{
    public class Pickup : InteractableBase
    {
        public string itemId = "Key_Ritual";
        public AudioSource sfx;


        public override void Interact(Interactor by)
        {
            var inv = by.GetComponent<Inventory>();
            if (!inv) return;
            inv.Add(itemId);
            if (sfx) sfx.Play();
            Destroy(gameObject);
        }
    }
}