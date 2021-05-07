// GENERATED AUTOMATICALLY FROM 'Assets/Submarine/Ingame/Input/Player Controls.inputactions'

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
                    ""name"": ""Turbo"",
                    ""type"": ""PassThrough"",
                    ""id"": ""decc1732-e735-42e4-b517-203b4cf56d54"",
                    ""expectedControlType"": ""Axis"",
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
                },
                {
                    ""name"": ""Cursor Display"",
                    ""type"": ""Button"",
                    ""id"": ""504f1ab5-485b-48f8-b942-f56f0a2728b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cursor Hide"",
                    ""type"": ""Button"",
                    ""id"": ""f958ef77-cac4-449f-8f92-bc86b4dbee4a"",
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
                    ""name"": """",
                    ""id"": ""1479b758-0aac-4762-86f8-8468171ed542"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Turbo"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""c87d3784-7566-4c07-b494-d950da5c6e88"",
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
                    ""id"": ""5e24f649-713a-4acf-ae95-3c38ba6b8b2a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cursor Hide"",
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
        m_Player_Turbo = m_Player.FindAction("Turbo", throwIfNotFound: true);
        m_Player_PrimaryOperation = m_Player.FindAction("Primary Operation", throwIfNotFound: true);
        m_Player_SecondaryOperation = m_Player.FindAction("Secondary Operation", throwIfNotFound: true);
        m_Player_CursorDisplay = m_Player.FindAction("Cursor Display", throwIfNotFound: true);
        m_Player_CursorHide = m_Player.FindAction("Cursor Hide", throwIfNotFound: true);
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
    private readonly InputAction m_Player_Turbo;
    private readonly InputAction m_Player_PrimaryOperation;
    private readonly InputAction m_Player_SecondaryOperation;
    private readonly InputAction m_Player_CursorDisplay;
    private readonly InputAction m_Player_CursorHide;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal => m_Wrapper.m_Player_Horizontal;
        public InputAction @Vertical => m_Wrapper.m_Player_Vertical;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @Turbo => m_Wrapper.m_Player_Turbo;
        public InputAction @PrimaryOperation => m_Wrapper.m_Player_PrimaryOperation;
        public InputAction @SecondaryOperation => m_Wrapper.m_Player_SecondaryOperation;
        public InputAction @CursorDisplay => m_Wrapper.m_Player_CursorDisplay;
        public InputAction @CursorHide => m_Wrapper.m_Player_CursorHide;
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
                @Turbo.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurbo;
                @Turbo.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurbo;
                @Turbo.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurbo;
                @PrimaryOperation.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryOperation;
                @PrimaryOperation.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryOperation;
                @PrimaryOperation.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrimaryOperation;
                @SecondaryOperation.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryOperation;
                @SecondaryOperation.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryOperation;
                @SecondaryOperation.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSecondaryOperation;
                @CursorDisplay.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorDisplay;
                @CursorDisplay.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorDisplay;
                @CursorDisplay.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorDisplay;
                @CursorHide.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorHide;
                @CursorHide.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorHide;
                @CursorHide.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCursorHide;
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
                @Turbo.started += instance.OnTurbo;
                @Turbo.performed += instance.OnTurbo;
                @Turbo.canceled += instance.OnTurbo;
                @PrimaryOperation.started += instance.OnPrimaryOperation;
                @PrimaryOperation.performed += instance.OnPrimaryOperation;
                @PrimaryOperation.canceled += instance.OnPrimaryOperation;
                @SecondaryOperation.started += instance.OnSecondaryOperation;
                @SecondaryOperation.performed += instance.OnSecondaryOperation;
                @SecondaryOperation.canceled += instance.OnSecondaryOperation;
                @CursorDisplay.started += instance.OnCursorDisplay;
                @CursorDisplay.performed += instance.OnCursorDisplay;
                @CursorDisplay.canceled += instance.OnCursorDisplay;
                @CursorHide.started += instance.OnCursorHide;
                @CursorHide.performed += instance.OnCursorHide;
                @CursorHide.canceled += instance.OnCursorHide;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnHorizontal(InputAction.CallbackContext context);
        void OnVertical(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnTurbo(InputAction.CallbackContext context);
        void OnPrimaryOperation(InputAction.CallbackContext context);
        void OnSecondaryOperation(InputAction.CallbackContext context);
        void OnCursorDisplay(InputAction.CallbackContext context);
        void OnCursorHide(InputAction.CallbackContext context);
    }
}
