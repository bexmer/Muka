using UnityEngine;


namespace Project.Interaction
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [SerializeField] string prompt = "E: Interact";
        public virtual string Prompt => prompt;
        public abstract void Interact(Project.Player.Interactor by);
    }
}