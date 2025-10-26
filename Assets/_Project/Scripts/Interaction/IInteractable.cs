namespace Project.Interaction
{
    public interface IInteractable
    {
        string Prompt { get; }
        void Interact(Project.Player.Interactor by);
    }
}