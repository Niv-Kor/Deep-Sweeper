// GENERATED AUTOMATICALLY FROM 'Assets/Input/Player Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""7231610b-d206-4c4c-a2c4-7e480ad85376"",
            ""actions"": [
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""PassThrough"",
                    ""id"": ""04f2962e-e0a1-4c34-8b10-3323b31ba40f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Vertical"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fd5529d5-ab37-4914-abe8-3561f3b45810"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""60ee6ef5-0d97-477e-bac9-b6ade6022c74"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Primary Operation"",
                    ""type"": ""Button"",
                    ""id"": ""ec7d0841-28b3-4009-a372-296d83b20586"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Secondary Operation"",
                    ""type"": ""Button"",
                    ""id"": ""776c0f8e-2136-4d95-a3bf-a2987d27d68d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""be33665f-0c71-451a-a9e7-6eda74c6c906"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Primary Operation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7d8ca1cd-1c03-4e26-8667-425c04524de1"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c47f349b-85c3-4b6f-b5d8-318274d907af"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Secondary Operation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""6698fa1e-2924-4cda-a24a-98fc8ebba1d9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0dcf6ba6-6ef5-478c-9c1c-c744e9e30d3e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""73af5342-f69e-4d0b-b0d6-a48967223250"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""6271a985-62fd-4e1e-bbf0-a39f42185cde"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e6e949ec-1b63-4b17-90ef-7b473a70cebb"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7465a6d7-bb42-4442-bcaa-215698e6cea8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""273689a3-befc-4251-9995-3d936d5c0f3f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""10d9678b-a8e3-4d75-b720-d7c47d55a4b5"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""de466ea8-aedd-47f0-a056-36e0afd12fb0"",
            ""actions"": [
                {
                    ""name"": ""Cursor Display"",
                    ""type"": ""Button"",
                    ""id"": ""8592369f-761d-49e4-9185-244d3834e297"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cursor Hide"",
                    ""type"": ""Button"",
                    ""id"": ""5b1a5fab-6a44-4813-9e1f-7e50a5df0d88"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Commander Selection"",
                    ""type"": ""Button"",
                    ""id"": ""8358c66e-c45e-4339-8680-60dc0f8b59b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6de9d3d3-b557-4b55-9be0-f7fce41e2193"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cursor Display"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35891d3b-73a0-48a5-9b4e-33b6b308208a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cursor Hide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""22ac6c16-a079-49d2-a95e-f03f8be974ca"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Commander Selection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee36bfed-af4d-40c5-b4be-6f19140a3af0"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Commander Selection"",
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
        m_Player_Horizontal = m_Player.FindAction("Horizontal", throwIfNotFound: true);
        m_Player_Vertical = m_Player.FindAction("Vertical", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_PrimaryOperation = m_Player.FindAction("Primary Operation", throwIfNotFound: true);
        m_Player_SecondaryOperation = m_Player.FindAction("Secondary Operation", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_CursorDisplay = m_UI.FindAction("Cursor Display", throwIfNotFound: true);
        m_UI_CursorHide = m_UI.FindAction("Cursor Hide", throwIfNotFound: true);
        m_UI_CommanderSelection = m_UI.FindAction("Commander Selection", throwIfNotFound: true);
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
    private readonly InputAction m_Player_Horizontal;
    private readonly InputAction m_Player_Vertical;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_PrimaryOperation;
    private readonly InputAction m_Player_SecondaryOperation;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal => m_Wrapper.m_Player_Horizontal;
        public InputAction @Vertical => m_Wrapper.m_Player_Vertical;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @PrimaryOperation => m_Wrapper.m_Player_PrimaryOperation;
        public InputAction @SecondaryOperation => m_Wrapper.m_Player_SecondaryOperation;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Horizontal.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHorizontal;
                @Horizontal.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHorizontal;
                @Horizontal.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHorizontal;
                @Vertical.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVertical;
                @Vertical.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVertical;
                @Vertical.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVertical;
                @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @PrimaryOperation.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryOperation;
                @PrimaryOperation.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryOperation;
                @PrimaryOperation.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryOperation;
                @SecondaryOperation.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryOperation;
                @SecondaryOperation.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryOperation;
                @SecondaryOperation.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryOperation;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Horizontal.started += instance.OnHorizontal;
                @Horizontal.performed += instance.OnHorizontal;
                @Horizontal.canceled += instance.OnHorizontal;
                @Vertical.started += instance.OnVertical;
                @Vertical.performed += instance.OnVertical;
                @Vertical.canceled += instance.OnVertical;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @PrimaryOperation.started += instance.OnPrimaryOperation;
                @PrimaryOperation.performed += instance.OnPrimaryOperation;
                @PrimaryOperation.canceled += instance.OnPrimaryOperation;
                @SecondaryOperation.started += instance.OnSecondaryOperation;
                @SecondaryOperation.performed += instance.OnSecondaryOperation;
                @SecondaryOperation.canceled += instance.OnSecondaryOperation;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_CursorDisplay;
    private readonly InputAction m_UI_CursorHide;
    private readonly InputAction m_UI_CommanderSelection;
    public struct UIActions
    {
        private @PlayerControls m_Wrapper;
        public UIActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @CursorDisplay => m_Wrapper.m_UI_CursorDisplay;
        public InputAction @CursorHide => m_Wrapper.m_UI_CursorHide;
        public InputAction @CommanderSelection => m_Wrapper.m_UI_CommanderSelection;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @CursorDisplay.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCursorDisplay;
                @CursorDisplay.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCursorDisplay;
                @CursorDisplay.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCursorDisplay;
                @CursorHide.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCursorHide;
                @CursorHide.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCursorHide;
                @CursorHide.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCursorHide;
                @CommanderSelection.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCommanderSelection;
                @CommanderSelection.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCommanderSelection;
                @CommanderSelection.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCommanderSelection;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CursorDisplay.started += instance.OnCursorDisplay;
                @CursorDisplay.performed += instance.OnCursorDisplay;
                @CursorDisplay.canceled += instance.OnCursorDisplay;
                @CursorHide.started += instance.OnCursorHide;
                @CursorHide.performed += instance.OnCursorHide;
                @CursorHide.canceled += instance.OnCursorHide;
                @CommanderSelection.started += instance.OnCommanderSelection;
                @CommanderSelection.performed += instance.OnCommanderSelection;
                @CommanderSelection.canceled += instance.OnCommanderSelection;
            }
        }
    }
    public UIActions @UI => new UIActions(this);
    public interface IPlayerActions
    {
        void OnHorizontal(InputAction.CallbackContext context);
        void OnVertical(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnPrimaryOperation(InputAction.CallbackContext context);
        void OnSecondaryOperation(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnCursorDisplay(InputAction.CallbackContext context);
        void OnCursorHide(InputAction.CallbackContext context);
        void OnCommanderSelection(InputAction.CallbackContext context);
    }
}
