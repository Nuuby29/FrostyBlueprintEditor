﻿using System.Collections.ObjectModel;
using System.Windows.Media;
using BlueprintEditor.Models.Connections;
using BlueprintEditor.Utils;
using FrostySdk.Ebx;

namespace BlueprintEditor.Models.Types.NodeTypes.Shared
{
    public class InterfaceDataNode : NodeBaseModel
    {
        public override string Name { get; set; } = "";
        public override string ObjectType { get; } = "EditorInterfaceNode"; //Not a real type

        public override ObservableCollection<InputViewModel> Inputs { get; set; } =
            new ObservableCollection<InputViewModel>()
            {
            };

        public override ObservableCollection<OutputViewModel> Outputs { get; set; } =
            new ObservableCollection<OutputViewModel>()
            {
            };
        
        public dynamic InterfaceItem { get; set; }

        /// <summary>
        /// Creates an interface data node from an item in an InterfaceDescriptorData
        /// </summary>
        /// <param name="interfaceItem">DataField, DynamicEvent, DynamicLink</param>
        /// <param name="isOut">The direction this node goes. If set to true, it is an output, false input.</param>
        /// <returns></returns>
        public static InterfaceDataNode CreateInterfaceDataNode(dynamic interfaceItem, bool isOut, AssetClassGuid guid)
        {
            switch (interfaceItem.GetType().Name)
            {
                default: //DataField
                    {
                        if (isOut)
                        {
                            var interfaceNode = new InterfaceDataNode()
                            {
                                Guid = guid,
                                HeaderColor =
                                    new SolidColorBrush(
                                        (Color)ColorConverter.ConvertFromString("#00FF21")), //Property connection color
                                InterfaceItem = interfaceItem,
                                Outputs = new ObservableCollection<OutputViewModel>()
                            {
                                new OutputViewModel() { Title = interfaceItem.Name, Type = ConnectionType.Property }
                            }
                            };
                            return interfaceNode;
                        }
                        else
                        {
                            var interfaceNode = new InterfaceDataNode()
                            {
                                Guid = guid,
                                HeaderColor =
                                    new SolidColorBrush(
                                        (Color)ColorConverter.ConvertFromString("#00FF21")), //Property connection color
                                InterfaceItem = interfaceItem,
                                Inputs = new ObservableCollection<InputViewModel>()
                            {
                                new InputViewModel() { Title = interfaceItem.Name, Type = ConnectionType.Property }
                            }
                            };
                            return interfaceNode;
                        }

                        break;
                    }
                case "DynamicEvent":
                    {
                        if (isOut)
                        {
                            var interfaceNode = new InterfaceDataNode()
                            {
                                Guid = guid,
                                HeaderColor =
                                    new SolidColorBrush(
                                        (Color)ColorConverter.ConvertFromString("#F8F8F8")), //Event connection color 5FD95F
                                InterfaceItem = interfaceItem,
                                Outputs = new ObservableCollection<OutputViewModel>()
                            {
                                new OutputViewModel() { Title = interfaceItem.Name, Type = ConnectionType.Event }
                            }
                            };
                            return interfaceNode;
                        }
                        else
                        {
                            var interfaceNode = new InterfaceDataNode()
                            {
                                Guid = guid,
                                HeaderColor =
                                    new SolidColorBrush(
                                        (Color)ColorConverter.ConvertFromString("#F8F8F8")), //Event connection color
                                InterfaceItem = interfaceItem,
                                Inputs = new ObservableCollection<InputViewModel>()
                            {
                                new InputViewModel() { Title = interfaceItem.Name, Type = ConnectionType.Event }
                            },
                            };
                            return interfaceNode;
                        }
                        break;
                    }
                case "DynamicLink":
                    {
                        if (isOut)
                        {
                            var interfaceNode = new InterfaceDataNode()
                            {
                                Guid = guid,
                                HeaderColor =
                                    new SolidColorBrush(
                                        (Color)ColorConverter.ConvertFromString("#6FA9CE")), //Link connection color
                                InterfaceItem = interfaceItem,
                                Outputs = new ObservableCollection<OutputViewModel>()
                            {
                                new OutputViewModel() { Title = interfaceItem.Name, Type = ConnectionType.Link }
                            }
                            };
                            return interfaceNode;
                        }
                        else
                        {
                            var interfaceNode = new InterfaceDataNode()
                            {
                                Guid = guid,
                                HeaderColor =
                                    new SolidColorBrush(
                                        (Color)ColorConverter.ConvertFromString("#6FA9CE")), //Link connection color
                                InterfaceItem = interfaceItem,
                                Inputs = new ObservableCollection<InputViewModel>()
                            {
                                new InputViewModel() { Title = interfaceItem.Name, Type = ConnectionType.Link }
                            }
                            };
                            return interfaceNode;
                        }
                        break;
                    }
            }
        }

        public override void OnModified()
        {
            if (Inputs.Count != 0)
            {
                Inputs[0].Title = InterfaceItem.Name.ToString();
            }
            else
            {
                Outputs[0].Title = InterfaceItem.Name.ToString();
            }

            //Really stupid way of updating connections
            //TODO: Create method in EbxBaseEditor for "refreshing" the ebx side of connections
            foreach (ConnectionViewModel connection in EditorUtils.CurrentEditor.GetConnections(this))
            {
                var source = connection.Source;
                var target = connection.Target;
                
                EditorUtils.CurrentEditor.Disconnect(connection);
                var newConnection = EditorUtils.CurrentEditor.Connect(source, target);
                EditorUtils.CurrentEditor.CreateConnectionObject(newConnection);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            
            if (obj.GetType() == GetType())
            {
                return ((InterfaceDataNode)obj).Name == Name && ((InterfaceDataNode)obj).Outputs == Outputs 
                                                             && ((InterfaceDataNode)obj).Inputs == Inputs;
            }
            else if (obj.GetType() == Object.GetType())
            {
                return ((dynamic)obj).Name == Object.Name;
            }

            return obj.GetType() == Object.GetType() && (bool)Object.Equals(obj);
        }
    }
}