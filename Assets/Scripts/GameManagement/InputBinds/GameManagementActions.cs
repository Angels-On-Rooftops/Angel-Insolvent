//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Scripts/GameManagement/InputBinds/GameManagementActions.inputactions
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

public partial class @GameManagementActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameManagementActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameManagementActions"",
    ""maps"": [
        {
            ""name"": ""Actions"",
            ""id"": ""ed287888-a672-493b-adcb-e465c679c88a"",
            ""actions"": [
                {
                    ""name"": ""SaveGame"",
                    ""type"": ""Button"",
                    ""id"": ""cd6992e2-71f7-4f62-a5ec-519e2f7df26f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LoadGame"",
                    ""type"": ""Button"",
                    ""id"": ""d814c52c-c177-4b7a-9031-0a740f4f55e1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b60f2b42-26ba-404a-b0c8-87f1f71c33c8"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SaveGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8fa97dd-6666-4dc2-989c-1f948f651ccd"",
                    ""path"": ""<Keyboard>/f6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LoadGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Actions
        m_Actions = asset.FindActionMap("Actions", throwIfNotFound: true);
        m_Actions_SaveGame = m_Actions.FindAction("SaveGame", throwIfNotFound: true);
        m_Actions_LoadGame = m_Actions.FindAction("LoadGame", throwIfNotFound: true);
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

    // Actions
    private readonly InputActionMap m_Actions;
    private List<IActionsActions> m_ActionsActionsCallbackInterfaces = new List<IActionsActions>();
    private readonly InputAction m_Actions_SaveGame;
    private readonly InputAction m_Actions_LoadGame;
    public struct ActionsActions
    {
        private @GameManagementActions m_Wrapper;
        public ActionsActions(@GameManagementActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @SaveGame => m_Wrapper.m_Actions_SaveGame;
        public InputAction @LoadGame => m_Wrapper.m_Actions_LoadGame;
        public InputActionMap Get() { return m_Wrapper.m_Actions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ActionsActions set) { return set.Get(); }
        public void AddCallbacks(IActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_ActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ActionsActionsCallbackInterfaces.Add(instance);
            @SaveGame.started += instance.OnSaveGame;
            @SaveGame.performed += instance.OnSaveGame;
            @SaveGame.canceled += instance.OnSaveGame;
            @LoadGame.started += instance.OnLoadGame;
            @LoadGame.performed += instance.OnLoadGame;
            @LoadGame.canceled += instance.OnLoadGame;
        }

        private void UnregisterCallbacks(IActionsActions instance)
        {
            @SaveGame.started -= instance.OnSaveGame;
            @SaveGame.performed -= instance.OnSaveGame;
            @SaveGame.canceled -= instance.OnSaveGame;
            @LoadGame.started -= instance.OnLoadGame;
            @LoadGame.performed -= instance.OnLoadGame;
            @LoadGame.canceled -= instance.OnLoadGame;
        }

        public void RemoveCallbacks(IActionsActions instance)
        {
            if (m_Wrapper.m_ActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_ActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ActionsActions @Actions => new ActionsActions(this);
    public interface IActionsActions
    {
        void OnSaveGame(InputAction.CallbackContext context);
        void OnLoadGame(InputAction.CallbackContext context);
    }
}
