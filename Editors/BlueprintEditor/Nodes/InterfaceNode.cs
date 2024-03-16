﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using BlueprintEditorPlugin.Editors.BlueprintEditor.Connections;
using BlueprintEditorPlugin.Editors.BlueprintEditor.Nodes.Ports;
using BlueprintEditorPlugin.Models.Networking;
using BlueprintEditorPlugin.Models.Nodes;
using BlueprintEditorPlugin.Models.Nodes.Ports;
using BlueprintEditorPlugin.Models.Status;
using FrostySdk;
using FrostySdk.Ebx;
using FrostySdk.IO;

namespace BlueprintEditorPlugin.Editors.BlueprintEditor.Nodes
{
    public class InterfaceNode : IObjectNode, ITransient, INetworked
    {
        private string _header;
        private Point _location;
        private bool _isSelected;

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                NotifyPropertyChanged(nameof(Header));
            }
        }

        public ObservableCollection<IPort> Inputs { get; } = new ObservableCollection<IPort>();
        public ObservableCollection<IPort> Outputs { get; } = new ObservableCollection<IPort>();

        public Point Location
        {
            get => _location;
            set
            {
                _location = value;
                NotifyPropertyChanged(nameof(Location));
            }
        }

        public Size Size { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        #region Object implementation

        public object Object { get; }
        
        public PointerRefType Type => PointerRefType.Internal;
        public AssetClassGuid InternalGuid { get; set; }
        
        public Guid FileGuid { get; }
        public Guid ClassGuid { get; }
        
        public EntityPort GetInput(string name)
        {
            if (Inputs.Count == 0)
                return null;
            
            return (EntityPort)Inputs[0];
        }

        public EntityPort GetOutput(string name)
        {
            if (Outputs.Count == 0)
                return null;
            
            return (EntityPort)Outputs[0];
        }

        public void AddInput(EntityInput input)
        {
            throw new NotImplementedException();
        }

        public void AddOutput(EntityOutput output)
        {
            throw new NotImplementedException();
        }

        public ConnectionType ConnectionType { get; set; }

        #endregion

        #region Property Changing

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Basic implementation

        public bool IsValid()
        {
            return true;
        }

        public void OnCreation()
        {
        }

        #endregion

        #region Status

        public EditorStatusArgs CurrentStatus { get; set; }
        public void CheckStatus()
        {
            NotifyPropertyChanged(nameof(CurrentStatus));
        }

        public void UpdateStatus()
        {
        }

        public void SetStatus(EditorStatusArgs args)
        {
            CurrentStatus = args;
            CheckStatus();
        }

        #endregion

        #region Complex implementation

        public void OnInputUpdated(IPort port)
        {
        }

        public void OnOutputUpdated(IPort port)
        {
        }

        public void AddPort(IPort port)
        {
            Inputs.Add(port);
        }

        #endregion

        #region Interface implementation

        public ITransient Load(NativeReader reader)
        {
            return null;
        }

        public ITransient Save(NativeWriter writer)
        {
            return null;
        }

        #endregion

        #region Networking implementation

        public Realm Realm { get; set; }
        public Realm ParseRealm(object obj)
        {
            return (Realm)obj;
        }

        public void DetermineRealm()
        {
            EntityPort port = (EntityPort)Inputs[0];
            if (port.Realm != Realm.Any)
            {
                Realm = port.Realm;
            }
            else
            {
                port.DetermineRealm();
                if (port.Realm != Realm.Any)
                {
                    Realm = port.Realm;
                }
            }
        }

        #endregion

        public InterfaceNode(object obj, string name, ConnectionType type, PortDirection direction, Realm realm = Realm.Any)
        {
            Object = obj;
            InternalGuid = ((dynamic)obj).GetInstanceGuid();
            int hash = int.Parse(name.Remove(0, 2), NumberStyles.AllowHexSpecifier);
            Header = Utils.GetString(hash);
            
            if (direction == PortDirection.In)
            {
                switch (type)
                {
                    case ConnectionType.Event:
                    {
                        EventInput input = new EventInput(Header, this)
                        {
                            Realm = realm
                        };
                        Inputs.Add(input);
                    } break;
                    case ConnectionType.Link:
                    {
                        LinkInput input = new LinkInput(Header, this)
                        {
                            Realm = realm
                        };
                        Inputs.Add(input);
                    } break;
                    case ConnectionType.Property:
                    {
                        PropertyInput input = new PropertyInput(Header, this)
                        {
                            Realm = realm,
                            IsInterface = true
                        };
                        Inputs.Add(input);
                    } break;
                }
            }
            else
            {
                switch (type)
                {
                    case ConnectionType.Event:
                    {
                        EventOutput input = new EventOutput(Header, this)
                        {
                            Realm = realm
                        };
                        Outputs.Add(input);
                    } break;
                    case ConnectionType.Link:
                    {
                        LinkOutput input = new LinkOutput(Header, this)
                        {
                            Realm = realm
                        };
                        Outputs.Add(input);
                    } break;
                    case ConnectionType.Property:
                    {
                        PropertyOutput input = new PropertyOutput(Header, this)
                        {
                            Realm = realm,
                            IsInterface = true
                        };
                        Outputs.Add(input);
                    } break;
                }
            }

            Realm = realm;
        }
    }
}