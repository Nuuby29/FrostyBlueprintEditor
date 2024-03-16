﻿using System.Collections.Generic;
using BlueprintEditorPlugin.Editors.BlueprintEditor.Nodes;
using BlueprintEditorPlugin.Editors.BlueprintEditor.Nodes.Ports;
using BlueprintEditorPlugin.Models.Connections;
using BlueprintEditorPlugin.Models.Networking;
using BlueprintEditorPlugin.Models.Nodes.Ports;
using BlueprintEditorPlugin.Models.Status;
using Frosty.Core;

namespace BlueprintEditorPlugin.Editors.BlueprintEditor.Connections
{
    public abstract class EntityConnection : BaseConnection, INetworked
    {
        private static readonly List<(Realm, Realm)> ImplicitConnectionCombos = new List<(Realm, Realm)>
        {
            (Realm.ClientAndServer, Realm.Server),
            (Realm.ClientAndServer, Realm.Client),
            (Realm.NetworkedClientAndServer, Realm.Server),
            (Realm.NetworkedClientAndServer, Realm.NetworkedClient),
            (Realm.Server, Realm.NetworkedClient),
            (Realm.NetworkedClient, Realm.Client),
            (Realm.Client, Realm.NetworkedClient),
            (Realm.Client, Realm.ClientAndServer),
            (Realm.Server, Realm.ClientAndServer),
            (Realm.NetworkedClientAndServer, Realm.ClientAndServer),
            (Realm.Server, Realm.Client)
        };
        public virtual Realm Realm { get; set; }
        public abstract ConnectionType Type { get; }
        public object Object { get; set; }

        private bool _hasPlayer;
        public bool HasPlayer
        {
            get => _hasPlayer;
            set
            {
                _hasPlayer = value;
                NotifyPropertyChanged(nameof(HasPlayer));
            } 
        }

        private PropertyType _propType;
        public PropertyType PropType
        {
            get => _propType;
            set
            {
                _propType = value;
                ((dynamic)Object).Flags = PropertyFlagsHelper.GetAsFlags(Realm, PropType);
                NotifyPropertyChanged(nameof(PropType));
                UpdateStatus();
            }
        }

        public Realm ParseRealm(object obj)
        {
            // TODO: this is a fatty! Please make it smaller!
            switch ((string)obj)
            {
                case "Realm_Server":
                {
                    return Realm.Server;
                }
                case "Realm_Client":
                {
                    return Realm.Client;
                }
                case "Realm_ClientAndServer":
                {
                    return Realm.ClientAndServer;
                }
                case "EventConnectionTargetType_ClientAndServer":
                {
                    return Realm.ClientAndServer;
                }
                case "EventConnectionTargetType_Client":
                {
                    return Realm.Client;
                }
                case "EventConnectionTargetType_Server":
                {
                    return Realm.Server;
                }
                case "EventConnectionTargetType_NetworkedClient":
                {
                    return Realm.NetworkedClient;
                }
                case "EventConnectionTargetType_NetworkedClientAndServer":
                {
                    return Realm.NetworkedClientAndServer;
                }
                default:
                {
                    return Realm.Invalid;
                }
            }
        }

        public void DetermineRealm()
        {
            EntityPort source = (EntityPort)Source;
            EntityPort target = (EntityPort)Target;
            if (target.Realm != Realm.Any && target.Realm != Realm.Invalid)
            {
                Realm = target.Realm;
            }
            else if (source.Realm != Realm.Any && source.Realm != Realm.Invalid)
            {
                Realm = source.Realm;
            }
            // Fuck you
            else
            {
                source.DetermineRealm();
                target.DetermineRealm();
                
                if (target.Realm != Realm.Any && target.Realm != Realm.Invalid)
                {
                    Realm = target.Realm;
                }
            }
        }

        public override void UpdateStatus()
        {
            if (Realm == Realm.Invalid)
            {
                SetStatus(new EditorStatusArgs(EditorStatus.Broken, "Connection realm is invalid!"));
            }

            EntityPort source = Source as EntityPort;
            EntityPort target = Target as EntityPort;

            if (source == null || target == null)
            {
                SetStatus(new EditorStatusArgs(EditorStatus.Broken, "Connection source or target is not an EntityPort. Please use EntityPorts for Blueprints."));
                
                #if DEVELOPER___DEBUG
                App.Logger.LogError("HEY DUMBASS YOU'RE USING THE WRONG TYPES FOR CONNECTION {0} USE ENTITYPORTS INSTEAD GOD DAMMIT", ToString());
                #endif
            }
            
            if (source.Realm == Realm.Any && target.Realm == Realm.Any)
            {
                SetStatus(new EditorStatusArgs(EditorStatus.Flawed, $"Cannot implicitly determine the realms of this connection based on ports. Please manually set realms"));
                return;
            }

            if (source.Realm != target.Realm && !ImplicitConnectionCombos.Contains((source.Realm, target.Realm)))
            {
                SetStatus(new EditorStatusArgs(EditorStatus.Flawed, $"{source.Realm} to {target.Realm} is not a valid combination of realms"));
                return;
            }
        }

        #region Construction

        public EntityConnection(IPort source, IPort target, object obj) : base(source, target)
        {
            Object = obj;
            EntityPort entitySource = (EntityPort)source;
            EntityPort entityTarget = (EntityPort)target;
            UpdateStatus();
        }
        
        public EntityConnection(IPort source, IPort target) : base(source, target)
        {
        }

        protected EntityConnection()
        {
        }

        #endregion

        public override string ToString()
        {
            return $"Connection {Source.Node} ({Source.Name}) -> {Target.Node} ({Target.Name})";
        }
    }

    public enum ConnectionType
    {
        Event = 0,
        Link = 1,
        Property = 2
    }
}