namespace PongGame.Input
{
    public interface IInputProvider
    {
        InputType GetInputType();
        float GetHorizontalInput();
        float GetTargetXPosition();
    }
    public enum InputType
    {
        Keyboard,
        Touch,
        AI
    }
}