//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Core/_Scripts/PlayerInputActions.inputactions
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
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_SwitchTab = m_Gameplay.FindAction("SwitchTab", throwIfNotFound: true);
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
    public struct GameplayActions
    {
        private @PlayerInputActions m_Wrapper;
        public GameplayActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @SwitchTab => m_Wrapper.m_Gameplay_SwitchTab;
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
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @SwitchTab.started -= instance.OnSwitchTab;
            @SwitchTab.performed -= instance.OnSwitchTab;
            @SwitchTab.canceled -= instance.OnSwitchTab;
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
    }
}
