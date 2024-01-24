//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Nickk888/RTSCameraController/Scripts/Input Actions/RTSCC_InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @RTSCC_InputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @RTSCC_InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""RTSCC_InputActions"",
    ""maps"": [
        {
            ""name"": ""RTSCC"",
            ""id"": ""ee8158d0-5d12-4c5f-9e03-a3669fadc354"",
            ""actions"": [
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""581a8087-8aef-4a81-b6a7-ccc2bc2de612"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Button"",
                    ""id"": ""846589ca-c1f8-4540-9e53-d8fee47a2993"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Drag"",
                    ""type"": ""Button"",
                    ""id"": ""9b1943cc-b005-40b8-a699-bdfc1a1c215b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""fe70808e-4e32-4172-81cd-e1d9dc2ed60f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Mouse"",
                    ""type"": ""Value"",
                    ""id"": ""1445c101-34c9-4593-9dee-b6c5c1249adc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""Value"",
                    ""id"": ""67f53e59-dbca-42b3-b34f-3695be33a1dd"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""HeightUp"",
                    ""type"": ""Button"",
                    ""id"": ""790b6c85-d905-4070-874c-2674f9ab27b7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""HeightDown"",
                    ""type"": ""Button"",
                    ""id"": ""d586a954-fbbe-4214-9507-120e70752720"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""28fc3139-38d6-427e-9b9f-9dede674eb97"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0.01)"",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6d4b417-1106-491d-88b5-00e899ee8a6f"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38bcb516-ddf0-47f7-bb44-48bf6f86a95e"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""378cb335-6d57-4ec7-a0a9-5b7e6206a444"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drag"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""51f0ec3f-c513-4099-9c77-9b407c8ae7e0"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6fc0b76c-5751-42a1-a360-655b5eab77b1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3506f309-3115-4751-a95c-71b3de9caf66"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""490c9f00-4ee3-4612-a93c-7c3a5ba96263"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b9aa9642-9052-4af4-872e-f7d778df0c62"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""bf351461-c950-4a42-a73d-84e947accec2"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7bef1325-0c55-403e-a2f7-917b52366343"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.1,y=0.1)"",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""443a84f7-7d3b-4ab4-9bda-da3726b9926c"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8634c4f5-a486-4e73-abaf-c708a5aa8264"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57dda9e4-069f-444a-a2a4-971b19610fe4"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeightUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79fb2f14-0471-4f81-ad47-84bcce387890"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeightUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a329fcb4-6d2a-4f75-bbfa-1fcf7f443734"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeightDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e843c1bb-33d0-4a2c-8f45-8ce146eb77b8"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HeightDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // RTSCC
        m_RTSCC = asset.FindActionMap("RTSCC", throwIfNotFound: true);
        m_RTSCC_Zoom = m_RTSCC.FindAction("Zoom", throwIfNotFound: true);
        m_RTSCC_Rotate = m_RTSCC.FindAction("Rotate", throwIfNotFound: true);
        m_RTSCC_Drag = m_RTSCC.FindAction("Drag", throwIfNotFound: true);
        m_RTSCC_Move = m_RTSCC.FindAction("Move", throwIfNotFound: true);
        m_RTSCC_Mouse = m_RTSCC.FindAction("Mouse", throwIfNotFound: true);
        m_RTSCC_Position = m_RTSCC.FindAction("Position", throwIfNotFound: true);
        m_RTSCC_HeightUp = m_RTSCC.FindAction("HeightUp", throwIfNotFound: true);
        m_RTSCC_HeightDown = m_RTSCC.FindAction("HeightDown", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // RTSCC
    private readonly InputActionMap m_RTSCC;
    private IRTSCCActions m_RTSCCActionsCallbackInterface;
    private readonly InputAction m_RTSCC_Zoom;
    private readonly InputAction m_RTSCC_Rotate;
    private readonly InputAction m_RTSCC_Drag;
    private readonly InputAction m_RTSCC_Move;
    private readonly InputAction m_RTSCC_Mouse;
    private readonly InputAction m_RTSCC_Position;
    private readonly InputAction m_RTSCC_HeightUp;
    private readonly InputAction m_RTSCC_HeightDown;
    public struct RTSCCActions
    {
        private @RTSCC_InputActions m_Wrapper;
        public RTSCCActions(@RTSCC_InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Zoom => m_Wrapper.m_RTSCC_Zoom;
        public InputAction @Rotate => m_Wrapper.m_RTSCC_Rotate;
        public InputAction @Drag => m_Wrapper.m_RTSCC_Drag;
        public InputAction @Move => m_Wrapper.m_RTSCC_Move;
        public InputAction @Mouse => m_Wrapper.m_RTSCC_Mouse;
        public InputAction @Position => m_Wrapper.m_RTSCC_Position;
        public InputAction @HeightUp => m_Wrapper.m_RTSCC_HeightUp;
        public InputAction @HeightDown => m_Wrapper.m_RTSCC_HeightDown;
        public InputActionMap Get() { return m_Wrapper.m_RTSCC; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(RTSCCActions set) { return set.Get(); }
        public void SetCallbacks(IRTSCCActions instance)
        {
            if (m_Wrapper.m_RTSCCActionsCallbackInterface != null)
            {
                @Zoom.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnZoom;
                @Zoom.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnZoom;
                @Zoom.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnZoom;
                @Rotate.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnRotate;
                @Drag.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnDrag;
                @Drag.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnDrag;
                @Drag.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnDrag;
                @Move.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnMove;
                @Mouse.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnMouse;
                @Mouse.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnMouse;
                @Mouse.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnMouse;
                @Position.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnPosition;
                @Position.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnPosition;
                @Position.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnPosition;
                @HeightUp.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnHeightUp;
                @HeightUp.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnHeightUp;
                @HeightUp.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnHeightUp;
                @HeightDown.started -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnHeightDown;
                @HeightDown.performed -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnHeightDown;
                @HeightDown.canceled -= m_Wrapper.m_RTSCCActionsCallbackInterface.OnHeightDown;
            }
            m_Wrapper.m_RTSCCActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Drag.started += instance.OnDrag;
                @Drag.performed += instance.OnDrag;
                @Drag.canceled += instance.OnDrag;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
                @Position.started += instance.OnPosition;
                @Position.performed += instance.OnPosition;
                @Position.canceled += instance.OnPosition;
                @HeightUp.started += instance.OnHeightUp;
                @HeightUp.performed += instance.OnHeightUp;
                @HeightUp.canceled += instance.OnHeightUp;
                @HeightDown.started += instance.OnHeightDown;
                @HeightDown.performed += instance.OnHeightDown;
                @HeightDown.canceled += instance.OnHeightDown;
            }
        }
    }
    public RTSCCActions @RTSCC => new RTSCCActions(this);
    public interface IRTSCCActions
    {
        void OnZoom(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnDrag(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnMouse(InputAction.CallbackContext context);
        void OnPosition(InputAction.CallbackContext context);
        void OnHeightUp(InputAction.CallbackContext context);
        void OnHeightDown(InputAction.CallbackContext context);
    }
}
