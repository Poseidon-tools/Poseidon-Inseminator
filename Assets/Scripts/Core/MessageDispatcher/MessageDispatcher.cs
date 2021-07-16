namespace Core.MessageDispatcher
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Utils.ScriptableObjectSingleton;
    
    using Core.MessageDispatcher.Interfaces;

    [CreateAssetMenu(menuName = "Core/MessageDispatcher")]
    public class MessageDispatcher : SingletonScriptableObject<MessageDispatcher>, IMessageDispatcher
    {
        #region Private Variables
        [ShowInInspector] private Dictionary<Type, List<Action<object>>> receivers = new Dictionary<Type, List<Action<object>>>();
        #endregion
        #region Public Methods
        public void RegisterReceiver(IMessageReceiver receiver)
        {
            Debug.Log("received!");
            foreach (var listenedType in receiver.ListenedTypes)
            {
                if (receivers.TryGetValue(listenedType, out var existingItem))
                {
                    existingItem.Add(receiver.OnMessageReceived);
                    continue;
                }
                receivers.Add(listenedType, new List<Action<object>>(){receiver.OnMessageReceived});
            }
        }
        public void UnregisterReceiver(IMessageReceiver receiver)
        {
            List<Type> entriesToRemove = new List<Type>();
            foreach (var listenedType in receiver.ListenedTypes)
            {
                if (receivers.TryGetValue(listenedType, out var existingItem))
                {
                    existingItem.Remove(receiver.OnMessageReceived);
                    if (existingItem.Count == 0)
                    {
                        entriesToRemove.Add(listenedType);
                    }
                }
            }
            // remove empty dictionary entries
            foreach (var entryToRemove in entriesToRemove)
            {
                receivers.Remove(entryToRemove);
            }
        }

        public void Send<T>(T message)
        {
            if (!receivers.TryGetValue(typeof(T), out var receiversList)) return;
            for (var index = 0; index < receiversList.Count; index++)
            {
                var receiver = receiversList[index];
                receiver.Invoke(message);
            }
        }
        
        #endregion
    }
}