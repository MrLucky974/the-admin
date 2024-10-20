//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Core/PlayerInputActions.inputactions
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

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""038a7001-e1c7-4d1b-8813-6c75ac0b29a9"",
            ""actions"": [
                {
                    ""name"": ""SwitchTab"",
                    ""type"": ""Button"",
                    ""id"": ""b79b2d41-5ac1-4e85-a09c-d3effe7a5dc1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Accept"",
                    ""type"": ""Button"",
                    ""id"": ""8636b914-aee8-43ed-b708-300e1ec76982"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dismiss"",
                    ""type"": ""Button"",
                    ""id"": ""0f990df0-ccde-4de3-b935-abebde757f4c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""fd5575aa-b769-4302-bd95-469ac15f67d6"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Erase"",
                    ""type"": ""Button"",
                    ""id"": ""1444db1b-0a0a-4691-bcb3-ad2729f3dc9a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PreviousCommand"",
                    ""type"": ""Button"",
                    ""id"": ""88c0b765-7e1b-4bd7-9692-43701fb13128"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c4b8ef59-19af-409a-a243-92039adea895"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": """",
                    ""action"": ""SwitchTab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""37f8cf41-d6aa-49f9-a078-8239203e54ed"",
                    ""path"": ""<Keyboard>/f2"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": """",
                    ""action"": ""SwitchTab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c766ea2e-d6e3-4484-830e-7fddf822ea42"",
                    ""path"": ""<Keyboard>/f3"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": """",
                    ""action"": ""SwitchTab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f66b3a1a-3d44-4752-ba6c-deeb65bfda8d"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accept"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ba32568-640c-419c-924b-86580f061a1b"",
                    ""path"": ""<Keyboard>/n"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dismiss"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""UP/DOWN"",
                    ""id"": ""d0633b08-a198-472f-8016-891a82e4901e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""bbe138f2-b43f-41e1-96b4-7b5eede3e41d"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""28fc8d7e-aba6-4b2f-ad05-3b84c36e1735"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a9d4c163-0991-4e58-9d43-75ddbbe57cb9"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Erase"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ea5fea7-d6bf-410b-99bb-837b2c862b86"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PreviousCommand"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_SwitchTab = m_Gameplay.FindAction("SwitchTab", throwIfNotFound: true);
        m_Gameplay_Accept = m_Gameplay.FindAction("Accept", throwIfNotFound: true);
        m_Gameplay_Dismiss = m_Gameplay.FindAction("Dismiss", throwIfNotFound: true);
        m_Gameplay_Scroll = m_Gameplay.FindAction("Scroll", throwIfNotFound: true);
        m_Gameplay_Erase = m_Gameplay.FindAction("Erase", throwIfNotFound: true);
        m_Gameplay_PreviousCommand = m_Gameplay.FindAction("PreviousCommand", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_Gameplay_SwitchTab;
    private readonly InputAction m_Gameplay_Accept;
    private readonly InputAction m_Gameplay_Dismiss;
    private readonly InputAction m_Gameplay_Scroll;
    private readonly InputAction m_Gameplay_Erase;
    private readonly InputAction m_Gameplay_PreviousCommand;
    public struct GameplayActions
    {
        private @PlayerInputActions m_Wrapper;
        public GameplayActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @SwitchTab => m_Wrapper.m_Gameplay_SwitchTab;
        public InputAction @Accept => m_Wrapper.m_Gameplay_Accept;
        public InputAction @Dismiss => m_Wrapper.m_Gameplay_Dismiss;
        public InputAction @Scroll => m_Wrapper.m_Gameplay_Scroll;
        public InputAction @Erase => m_Wrapper.m_Gameplay_Erase;
        public InputAction @PreviousCommand => m_Wrapper.m_Gameplay_PreviousCommand;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @SwitchTab.started += instance.OnSwitchTab;
            @SwitchTab.performed += instance.OnSwitchTab;
            @SwitchTab.canceled += instance.OnSwitchTab;
            @Accept.started += instance.OnAccept;
            @Accept.performed += instance.OnAccept;
            @Accept.canceled += instance.OnAccept;
            @Dismiss.started += instance.OnDismiss;
            @Dismiss.performed += instance.OnDismiss;
            @Dismiss.canceled += instance.OnDismiss;
            @Scroll.started += instance.OnScroll;
            @Scroll.performed += instance.OnScroll;
            @Scroll.canceled += instance.OnScroll;
            @Erase.started += instance.OnErase;
            @Erase.performed += instance.OnErase;
            @Erase.canceled += instance.OnErase;
            @PreviousCommand.started += instance.OnPreviousCommand;
            @PreviousCommand.performed += instance.OnPreviousCommand;
            @PreviousCommand.canceled += instance.OnPreviousCommand;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @SwitchTab.started -= instance.OnSwitchTab;
            @SwitchTab.performed -= instance.OnSwitchTab;
            @SwitchTab.canceled -= instance.OnSwitchTab;
            @Accept.started -= instance.OnAccept;
            @Accept.performed -= instance.OnAccept;
            @Accept.canceled -= instance.OnAccept;
            @Dismiss.started -= instance.OnDismiss;
            @Dismiss.performed -= instance.OnDismiss;
            @Dismiss.canceled -= instance.OnDismiss;
            @Scroll.started -= instance.OnScroll;
            @Scroll.performed -= instance.OnScroll;
            @Scroll.canceled -= instance.OnScroll;
            @Erase.started -= instance.OnErase;
            @Erase.performed -= instance.OnErase;
            @Erase.canceled -= instance.OnErase;
            @PreviousCommand.started -= instance.OnPreviousCommand;
            @PreviousCommand.performed -= instance.OnPreviousCommand;
            @PreviousCommand.canceled -= instance.OnPreviousCommand;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnSwitchTab(InputAction.CallbackContext context);
        void OnAccept(InputAction.CallbackContext context);
        void OnDismiss(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnErase(InputAction.CallbackContext context);
        void OnPreviousCommand(InputAction.CallbackContext context);
    }
}
