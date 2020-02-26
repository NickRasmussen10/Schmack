// GENERATED AUTOMATICALLY FROM 'Assets/Misc_/Inputs/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""cb948960-b45e-4ae7-8628-a862fefc950f"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""d483db94-5904-4b7a-abd8-92d5f31e842b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""90f31822-adc9-4421-8d13-e952a06cd1c6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""894fd227-cf75-48fb-abb5-0c66e0d80213"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Flow"",
                    ""type"": ""Button"",
                    ""id"": ""08439b99-1d16-4f7e-8c84-1aa095fe7767"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Draw"",
                    ""type"": ""Button"",
                    ""id"": ""f1d8ca88-ab5f-4d06-8180-3fe849b11407"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1820e916-e4c4-4b1f-b051-d1ac1589af04"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c408cbe-920a-49cb-afcc-e0d369e8853e"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db7784ad-ab28-41e4-a2d9-e8c7d745c033"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e226d543-fe98-42ba-9c3b-51edbf4761f8"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Flow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f3797e1-0b29-43ad-9d57-9a6df69d1b91"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Draw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Quit"",
            ""id"": ""a0569757-d18d-4378-8191-5116a586a7e2"",
            ""actions"": [
                {
                    ""name"": ""quit"",
                    ""type"": ""Button"",
                    ""id"": ""bd75073e-6da9-4362-aa8a-7a7f65047ac7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""888aaf3f-a9ac-4d9e-a2a1-ad052ec9d3e6"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Aim = m_Player.FindAction("Aim", throwIfNotFound: true);
        m_Player_Flow = m_Player.FindAction("Flow", throwIfNotFound: true);
        m_Player_Draw = m_Player.FindAction("Draw", throwIfNotFound: true);
        // Quit
        m_Quit = asset.FindActionMap("Quit", throwIfNotFound: true);
        m_Quit_quit = m_Quit.FindAction("quit", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Aim;
    private readonly InputAction m_Player_Flow;
    private readonly InputAction m_Player_Draw;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Aim => m_Wrapper.m_Player_Aim;
        public InputAction @Flow => m_Wrapper.m_Player_Flow;
        public InputAction @Draw => m_Wrapper.m_Player_Draw;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Aim.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                @Flow.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlow;
                @Flow.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlow;
                @Flow.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlow;
                @Draw.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDraw;
                @Draw.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDraw;
                @Draw.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDraw;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Flow.started += instance.OnFlow;
                @Flow.performed += instance.OnFlow;
                @Flow.canceled += instance.OnFlow;
                @Draw.started += instance.OnDraw;
                @Draw.performed += instance.OnDraw;
                @Draw.canceled += instance.OnDraw;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Quit
    private readonly InputActionMap m_Quit;
    private IQuitActions m_QuitActionsCallbackInterface;
    private readonly InputAction m_Quit_quit;
    public struct QuitActions
    {
        private @Controls m_Wrapper;
        public QuitActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @quit => m_Wrapper.m_Quit_quit;
        public InputActionMap Get() { return m_Wrapper.m_Quit; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(QuitActions set) { return set.Get(); }
        public void SetCallbacks(IQuitActions instance)
        {
            if (m_Wrapper.m_QuitActionsCallbackInterface != null)
            {
                @quit.started -= m_Wrapper.m_QuitActionsCallbackInterface.OnQuit;
                @quit.performed -= m_Wrapper.m_QuitActionsCallbackInterface.OnQuit;
                @quit.canceled -= m_Wrapper.m_QuitActionsCallbackInterface.OnQuit;
            }
            m_Wrapper.m_QuitActionsCallbackInterface = instance;
            if (instance != null)
            {
                @quit.started += instance.OnQuit;
                @quit.performed += instance.OnQuit;
                @quit.canceled += instance.OnQuit;
            }
        }
    }
    public QuitActions @Quit => new QuitActions(this);
    public interface IPlayerActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnFlow(InputAction.CallbackContext context);
        void OnDraw(InputAction.CallbackContext context);
    }
    public interface IQuitActions
    {
        void OnQuit(InputAction.CallbackContext context);
    }
}
