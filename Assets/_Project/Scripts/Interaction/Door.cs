using UnityEngine;
using Project.Player;


namespace Project.Interaction
{
    [RequireComponent(typeof(Animator))]
    public class Door : InteractableBase
    {
        public bool locked;
        public string requiredItemId = "Key_Ritual";
        public string promptLocked = "E: Usar llave";
        public string promptOpen = "E: Abrir";
        Animator _anim;
        bool _open;


        private void Awake() { _anim = GetComponent<Animator>(); }
        public override string Prompt => locked ? promptLocked : (_open ? "E: Cerrar" : promptOpen);


        public override void Interact(Interactor by)
        {
            if (locked)
            {
                var inv = by.GetComponent<Inventory>();
                if (inv && inv.Has(requiredItemId))
                {
                    inv.Consume(requiredItemId);
                    locked = false;
                }
                else return; // sigue bloqueada
            }


            _open = !_open;
            _anim?.SetBool("Open", _open);
        }
    }
}