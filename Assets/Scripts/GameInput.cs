using UnityEngine;
#if !ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Tiny wrapper so the game works with either the old Input Manager or the new Input System,
// whichever is active in Player Settings > Active Input Handling.
public static class GameInput
{
#if ENABLE_LEGACY_INPUT_MANAGER
    public static Vector2 MousePosition => Input.mousePosition;
    public static float ScrollDelta => Input.mouseScrollDelta.y;
    public static bool LeftClickDown => Input.GetMouseButtonDown(0);
    public static bool RightClickDown => Input.GetMouseButtonDown(1);

    public static Vector2 MoveAxis
    {
        get
        {
            Vector2 v = Vector2.zero;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) v.x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) v.x += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v.y -= 1f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v.y += 1f;
            return v;
        }
    }

    public static bool NumberKeyDown(int n)
    {
        if (n < 0 || n > 9) return false;
        return Input.GetKeyDown(KeyCode.Alpha0 + n) || Input.GetKeyDown(KeyCode.Keypad0 + n);
    }
#elif ENABLE_INPUT_SYSTEM
    public static Vector2 MousePosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    // New Input System reports ~120 per wheel notch on Windows; normalise to ~1 like the old system.
    public static float ScrollDelta => Mouse.current != null ? Mathf.Clamp(Mouse.current.scroll.ReadValue().y / 120f, -3f, 3f) : 0f;
    public static bool LeftClickDown => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    public static bool RightClickDown => Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;

    public static Vector2 MoveAxis
    {
        get
        {
            Vector2 v = Vector2.zero;
            Keyboard k = Keyboard.current;
            if (k == null) return v;
            if (k.aKey.isPressed || k.leftArrowKey.isPressed) v.x -= 1f;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) v.x += 1f;
            if (k.sKey.isPressed || k.downArrowKey.isPressed) v.y -= 1f;
            if (k.wKey.isPressed || k.upArrowKey.isPressed) v.y += 1f;
            return v;
        }
    }

    public static bool NumberKeyDown(int n)
    {
        Keyboard k = Keyboard.current;
        if (k == null || n < 0 || n > 9) return false;
        Key digit = n == 0 ? Key.Digit0 : Key.Digit1 + (n - 1);
        Key numpad = Key.Numpad0 + n;
        return k[digit].wasPressedThisFrame || k[numpad].wasPressedThisFrame;
    }
#else
    public static Vector2 MousePosition => Vector2.zero;
    public static float ScrollDelta => 0f;
    public static bool LeftClickDown => false;
    public static bool RightClickDown => false;
    public static Vector2 MoveAxis => Vector2.zero;
    public static bool NumberKeyDown(int n) => false;
#endif
}
